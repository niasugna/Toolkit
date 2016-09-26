using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pollux.Async
{
    //https://blogs.msdn.microsoft.com/pfxteam/2011/01/15/asynclazyt/
    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<T> valueFactory) :
            base(() => Task.Factory.StartNew(valueFactory)) { }
        public AsyncLazy(Func<Task<T>> taskFactory) :
            base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap()) { }

        public System.Runtime.CompilerServices.TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
    }

    public static class AsyncHelper
    {
        

        ////only 1 thread can access
        //private static readonly SemaphoreSlim _sl = new SemaphoreSlim(initialCount: 1);

        //public async Task TestSeraphoneAsync<T>(this Task<T> task)
        //{
        //    await _sl.WaitAsync();

        //    try
        //    {
        //        await task;
        //        // Do async work here

        //    }
        //    finally
        //    {
        //        // Release the thread
        //        _sl.Release();
        //    }
        //}
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                        s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            return await task;
        }
        public static async Task<T> WithCancellation1<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => tcs.TrySetResult(true)))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            return await task;
        }


        //http://blogs.msdn.com/b/pfxteam/archive/2011/11/10/10235834.aspx
        //此方法較簡單 但執行太多可能有效能問題
        public static async Task<T> TimeoutAfter2<T>(this Task<T> task, int millisecondsTimeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout)))
                return await task;
            else
                throw new TimeoutException();
        }
        public static Task<Result> TimeoutAfter<Result>(this Task<Result> task, int millisecondsTimeout)
        {
            // Short-circuit #1: infinite timeout or task already completed
            if (task.IsCompleted || (millisecondsTimeout == Timeout.Infinite))
            {
                Console.WriteLine("task.IsCompleted");
                // Either the task has already completed or timeout will never occur.
                // No proxy necessary.
                return task;
            }
            // tcs.Task will be returned as a proxy to the caller
            var tcs = new TaskCompletionSource<Result>();

            // Short-circuit #2: zero timeout
            if (millisecondsTimeout == 0)
            {
                // Console.WriteLine("millisecondsTimeout == 0");
                // We've already timed out.
                tcs.SetException(new TimeoutException());
                return tcs.Task;
            }

            // Set up a timer to complete after the specified timeout period
            var timer = new Timer(state => tcs.TrySetException(new TimeoutException()), null, millisecondsTimeout, Timeout.Infinite);

            // Wire up the logic for what happens when source task completes
            task.ContinueWith(antecedent =>
                {
                    timer.Dispose();
                    MarshalTaskResults(antecedent, tcs); 
                }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            return tcs.Task;
        }
        
        internal static void MarshalTaskResults<TResult>(Task source, TaskCompletionSource<TResult> proxy)
        {
            switch (source.Status)
            {
                case TaskStatus.Faulted:
                    proxy.TrySetException(source.Exception);
                    break;
                case TaskStatus.Canceled:
                    proxy.TrySetCanceled();
                    break;
                case TaskStatus.RanToCompletion:
                    Task<TResult> castedSource = source as Task<TResult>;
                    proxy.TrySetResult(
                        castedSource == null ? default(TResult) : // source is a Task
                            castedSource.Result); // source is a Task<TResult>
                    break;
            }
        }
    }
}
