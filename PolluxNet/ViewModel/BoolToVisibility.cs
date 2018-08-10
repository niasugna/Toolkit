using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.ViewModel
{
    //BoolToVisiblity
    public class BoolVisibility : Pollux.Mvvm.BindableBase
    {
        public static implicit operator bool(BoolVisibility v1) { return v1.Value; }
        public static implicit operator BoolVisibility(bool v1) { return new BoolVisibility { Value = v1 }; }

        private bool _value = true;
        public bool Value
        {
            get { return _value; }
            set
            {
                SetProperty<bool>(ref _value, value);

                OnPropertyChanged(() => this.Visibility);
            }
        }
        public System.Windows.Visibility Visibility
        {
            get
            {
                if (_value == true)
                    return System.Windows.Visibility.Visible;
                else
                    return System.Windows.Visibility.Hidden;

            }
        }
    }
}
