using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pollux.Helper
{
    public static class TaskEx
    {
        public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))

                // task is completed or cancel is completed
                if (task != await Task.WhenAny(task, tcs.Task))
                {
                    // task is not completed
                    //Trace.WriteLine("cancel!!");
                    // throw new OperationCanceledException(cancellationToken);
                }
            await task;
        }
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            return await task;
        }
        public static Task StartSTATask(Action action)
        {
            TaskCompletionSource<object> source = new TaskCompletionSource<object>();
            Thread thread = new Thread(() =>
            {
                try
                {
                    action();
                    source.SetResult(null);
                }
                catch (Exception ex)
                {
                    source.SetException(ex);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return source.Task;
        }
        public static Task<T> StartSTATask<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();
            Thread thread = new Thread(() =>
            {
                try
                {
                    tcs.SetResult(func());
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }

        //public static async Task<T> TimeoutAfter<T>(this Task<T> task, int millisecondsTimeout) 
        //{
        //    if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout)))
        //        return await task;
        //    else
        //        throw new TimeoutException();
        //}
        //http://blogs.msdn.com/b/pfxteam/archive/2011/11/10/10235834.aspx
        internal struct VoidTypeStruct { }  // See Footnote #1
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
                //                Console.WriteLine("millisecondsTimeout == 0");
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
