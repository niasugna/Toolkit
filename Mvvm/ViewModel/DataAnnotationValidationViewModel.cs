using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Pollux.Mvvm;

namespace Pollux.ViewModel
{
    public class DataAnnotationViewModel : Pollux.Mvvm.BindableBase, System.ComponentModel.INotifyDataErrorInfo
    {
        protected override bool SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            //測試
            ValidateProperty(propertyName, value);


            return base.SetProperty<T>(ref storage, value, propertyName);
        }
        protected void ValidateProperty(string propertyName, object value)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            this.ValidateProperty(new ValidationContext(this, null, null) { MemberName = propertyName }, value);
        }
        protected void ValidateProperty<TProp>(Expression<Func<TProp>> propExpression, object value)
        {
            string propertyName = ((MemberExpression)propExpression.Body).Member.Name;

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            this.ValidateProperty(new ValidationContext(this, null, null) { MemberName = propertyName }, value);
        }
        protected virtual void ValidateProperty(ValidationContext validationContext, object value)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException("validationContext");
            }
            var propertyAttributes = validationContext.ObjectInstance.GetType()
                                              .GetProperty(validationContext.MemberName).GetCustomAttributes<ValidationAttribute>();//.GetRuntimeProperties()
            //.Where(c => c.GetCustomAttributes(typeof(ValidationAttribute)).Any());
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateValue(value, validationContext, validationResults, propertyAttributes);
            //bool isValid = Validator.TryValidateProperty(value, validationContext, validationResults);

            if (validationResults.Any())
            {
             this.errorsContainer.SetErrors(validationContext.MemberName, validationResults);
                //OnPropertyChanged(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Item[{0}]", validationContext.MemberName));
            }
            else
            {
                this.errorsContainer.ClearErrors(validationContext.MemberName);
            }
        }
        protected virtual void SetErrors<TProp>(Expression<Func<TProp>> propExpression, IEnumerable<ValidationResult> errors)
        {
            errorsContainer.SetErrors(propExpression, errors);
        }
        private ErrorsContainer<ValidationResult> errorsContainer = null;

        public DataAnnotationViewModel()
        {
            if (errorsContainer == null)
                errorsContainer = new ErrorsContainer<ValidationResult>(propertyName => RaiseErrorsChanged(propertyName));

        }

        //[JsonIgnore]
        public ErrorsContainer<ValidationResult> ErrorsContainer
        {
            get
            {
                if (errorsContainer == null)
                    return new ErrorsContainer<ValidationResult>(propertyName => RaiseErrorsChanged(propertyName));

                else
                    return errorsContainer;
            }
        }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return this.errorsContainer.GetErrors(propertyName);
        }
        protected void RaiseErrorsChanged(string propertyName)
        {
            var handler = this.ErrorsChanged;
            if (handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }
        //[JsonIgnore]
        public bool HasErrors
        {
            get { return this.errorsContainer.HasErrors; }
        }
    }

    public class DataAnnotationViewModel<T> : DataAnnotationViewModel
    {
    }
}
