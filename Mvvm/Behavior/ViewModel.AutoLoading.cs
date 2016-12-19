using Pollux.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pollux.Behavior
{
    public class ViewModel
    {
        static ViewModel()
        {
        }



        public static bool GetAutoLoading(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoLoadingProperty);
        }

        public static void SetAutoLoading(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoLoadingProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoLoadingProperty =
            DependencyProperty.RegisterAttached("ViewModel", typeof(bool), typeof(ViewModel), new PropertyMetadata(false,
                (s, e) =>
                {
                    var vm = (s as FrameworkElement).DataContext as BusyViewModelBase;
                    if (vm !=null)
                    {
                        vm.Load();
                    }
                    else
                        throw new NotSupportedException(string.Format("DataContext of {0} is null.",s));
                }));

        
    }
}
