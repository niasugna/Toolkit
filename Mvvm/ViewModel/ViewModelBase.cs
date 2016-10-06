using Mvvm;
using Pollux.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.ViewModel
{
    public abstract class BusyViewModelBase : BindableBase
    {
        public string VisualState { get; set; }

        public bool IsBusy { get; set; }

        public bool IsFaulted { get; set; }
        public string ErrorMessage { get; set; }
        public Lazy<NotifyTask<bool>> IsLoaded { get; set; }

        public virtual async Task<bool> Initialize()
        {
            try
            {
                IsBusy = true;
                OnPropertyChanged(() => IsBusy);

                await Refresh();

                return true;
            }
            catch (Exception e)
            {
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
        public virtual async Task Refresh()
        {
            await Task.Delay(0);
            return;
        }

        public BusyViewModelBase()
        {
            IsLoaded = new Lazy<NotifyTask<bool>>(() => new NotifyTask<bool>(() => false, () => Initialize()));
        }
        public virtual void VisualStateLogic(string state)
        {
        }
        public void SetVisualState(string state)
        {
            VisualStateLogic(state);

            VisualState = state;
            OnPropertyChanged(() => VisualState);
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
