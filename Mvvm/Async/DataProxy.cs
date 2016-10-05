using System;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Mvvm
{
    public class NotifyTask<TResult> : INotifyPropertyChanged where TResult : new()
    {
        public NotifyTask(Func<TResult> defaultValueFactory,Func<Task<TResult>> taskFactory)
        {
            DefaultValueFactory = defaultValueFactory;

            Result = DefaultValueFactory();
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Result"));

            TaskFactory = taskFactory;
            
            Task = TaskFactory();

            if (!Task.IsCompleted)
            {
                var _ = WatchTaskAsync(Task);
            }
        }
        public bool Refresh()
        {
            if (Task.IsCompleted == false)
                return false;

            Result = DefaultValueFactory();
            
            Task = TaskFactory();

            if (!Task.IsCompleted)
            {
                var _ = WatchTaskAsync(Task);
            }
            return true;
        }
        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
            }



            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs("Status"));
            propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                propertyChanged(this, new PropertyChangedEventArgs("InnerException"));
                propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                if (Task.Status == TaskStatus.RanToCompletion)
                    Result = Task.Result;

                propertyChanged(this, new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                propertyChanged(this, new PropertyChangedEventArgs("Result"));
            }
        }
        public Func<Task<TResult>> TaskFactory { get; private set; }
        public Func<TResult> DefaultValueFactory { get; private set; }
        public Task<TResult> Task { get; private set; }
        public TResult Result { get; set; }
        public TaskStatus Status { get { return Task.Status; } }
        public bool IsCompleted { get { return Task.IsCompleted; } }
        public bool IsNotCompleted { get { return !Task.IsCompleted; } }
        public bool IsSuccessfullyCompleted
        {
            get
            {
                return Task.Status ==
                    TaskStatus.RanToCompletion;
            }
        }
        public bool IsCanceled { get { return Task.IsCanceled; } }
        public bool IsFaulted { get { return Task.IsFaulted; } }
        public AggregateException Exception { get { return Task.Exception; } }
        public Exception InnerException
        {
            get
            {
                return (Exception == null) ?
                    null : Exception.InnerException;
            }
        }
        public string ErrorMessage
        {
            get
            {
                return (InnerException == null) ?
                    null : InnerException.Message;
            }
        }

        public System.Runtime.CompilerServices.TaskAwaiter<TResult> GetAwaiter() { return Task.GetAwaiter(); }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
