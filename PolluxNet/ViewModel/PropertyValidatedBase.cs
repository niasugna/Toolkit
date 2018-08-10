using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.ViewModel
{
    //http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifydataerrorinfo%28v=vs.95%29.aspx

    public abstract class PropertyValidatedBase : BusyViewModelBase, INotifyDataErrorInfo
    {
        public virtual bool Validate(Func<bool> predicate, string errorMessage, [CallerMemberName] string propertyName = null)
        {
            var ret = predicate();
            if (ret)
                RemoveError(propertyName, errorMessage);
            else
                AddError(propertyName, errorMessage, false);
            return ret;
        }

        #region INotifyDataErrorInfo

        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors
        {
            get
            {
                return errors.Any();
            }
        }

        public IEnumerable GetErrors([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName) || !errors.ContainsKey(propertyName)) return null;
            return errors[propertyName];
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        // Adds the specified error to the errors collection if it is not 
        // already present, inserting it in the first position if isWarning is 
        // false. Raises the ErrorsChanged event if the collection changes. 
        public void AddError(string propertyName, string error, bool isWarning)
        {
            if (!errors.ContainsKey(propertyName))
                errors[propertyName] = new List<string>();

            if (!errors[propertyName].Contains(error))
            {
                if (isWarning) errors[propertyName].Add(error);
                else errors[propertyName].Insert(0, error);
                RaiseErrorsChanged(propertyName);
            }
        }

        // Removes the specified error from the errors collection if it is
        // present. Raises the ErrorsChanged event if the collection changes.
        public void RemoveError(string propertyName, string error)
        {
            if (errors.ContainsKey(propertyName) &&
                errors[propertyName].Contains(error))
            {
                errors[propertyName].Remove(error);
                if (errors[propertyName].Count == 0) errors.Remove(propertyName);
                RaiseErrorsChanged(propertyName);
            }
        }

        #endregion
    }
}
