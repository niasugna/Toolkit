using Mvvm;
using Pollux.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.ViewModel
{
    public abstract class BusyViewModelBase : BindableBase
    {
        private string _VisualState;
        public string VisualState
        {
            get { return _VisualState; }
            set
            {
                _VisualState = value;
                OnPropertyChanged<string>(() => this.VisualState);
            }
        }
        

        public bool IsBusy { get; private set; }

        public bool IsLoaded { get; private set; }

        public bool IsFaulted { get; set; }

        public NotifyTask<bool> RemoteDataLoader { get; set; }
        
        public string ErrorMessage { get; set; }

        public void Load()
        {
            if (RemoteDataLoader == null)
            {
                RemoteDataLoader = new NotifyTask<bool>(() => false, () => LoadAsync());
            }
            else
            {
                IsLoaded = RemoteDataLoader.Refresh();
                OnPropertyChanged(() => IsLoaded);
            }
        }
        public virtual async Task<bool> LoadAsync()
        {
            try
            {
                IsBusy = true;
                OnPropertyChanged(() => IsBusy);

                await RefreshAsync();

                return true;
            }
            catch (Exception e)
            {
                var query = new StackTrace(e, true).GetFrames()         // get the frames
                              .Select(frame => new
                              {                   
                                  // get the info
                                  FileName = frame.GetFileName(),
                                  LineNumber = frame.GetFileLineNumber(),
                                  ColumnNumber = frame.GetFileColumnNumber(),
                                  Method = frame.GetMethod(),
                                  Class = frame.GetMethod().DeclaringType,
                              });
                Trace.WriteLine("Exception  : " + query.First().Class);

                IsFaulted = true;
                OnPropertyChanged(() => IsFaulted);

                ErrorMessage = e.Message;
                OnPropertyChanged(() => ErrorMessage);

                SetVisualState("Exception");

                return false;
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(() => IsBusy);
            }
        }
        public virtual async Task RefreshAsync()
        {
            await Task.Delay(0);
            return;
        }

        public BusyViewModelBase()
        {
            
        }

        public virtual void HandleVisualState(string state)
        {
        }
        public void SetVisualState(string state)
        {
            HandleVisualState(state);

            VisualState = state;

            System.Diagnostics.Trace.WriteLine("SetVisualState = " +state);
            OnPropertyChanged(() => VisualState);
        }
        public bool IsInDesignMode
        {
            get
            {
                return System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());
            }
        }
        public object CreateView(object viewModel)
        {
            var modelType = viewModel.GetType();
            var viewClassName = modelType.Name.Replace("ViewModel", "View");
            var viewType = modelType.Assembly.GetTypes().Where(t => t.IsClass && t.Name == viewClassName).Single();
            return Activator.CreateInstance(viewType);
        }
    }
}
