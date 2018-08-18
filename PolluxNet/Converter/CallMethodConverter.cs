using Mvvm;
using Pollux.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Pollux.Converter
{
    //[ValueConversion(typeof(object), typeof(string))]
    public class CallMethodConverter : IValueConverter,IMultiValueConverter
    {
        public DependencyObject View { get; set; }

        public string MethodName { get; set; }

        public CallMethodConverter(DependencyObject view, string methodName)
        {
            View = view;
            MethodName = methodName;
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var vm = View.GetValue(FrameworkElement.DataContextProperty) ?? View.GetValue(FrameworkContentElement.DataContextProperty);

            if (vm == null)
            {
                return DependencyProperty.UnsetValue;
                throw new ArgumentNullException("vm", string.Format("DataContext is not found on View."));
            }

            var dcType = vm.GetType();

            var method = dcType.GetMethod(MethodName);
            if (method == null)
            {
                return DependencyProperty.UnsetValue;
                throw new ArgumentNullException(MethodName, string.Format("{0} is not found on {1}.", MethodName, dcType));
            }

            return method.Invoke(vm, new object[] { value });

            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        //static Type[] GetParameterTypes(object eventInfo)
        //{
        //    var invokeMethod = eventInfo.EventHandlerType.GetMethod("Invoke");
        //    return invokeMethod.GetParameters().Select(p => p.ParameterType).ToArray();
        //}

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var vm = View.GetValue(FrameworkElement.DataContextProperty) ?? View.GetValue(FrameworkContentElement.DataContextProperty);

            if (vm == null)
            {
                return DependencyProperty.UnsetValue;
                throw new ArgumentNullException("vm", string.Format("DataContext is not found on View."));
            }

            var dcType = vm.GetType();

            var method = dcType.GetMethod(MethodName);
            if (method == null)
            {
                return DependencyProperty.UnsetValue;
                throw new ArgumentNullException(MethodName, string.Format("{0} is not found on {1}.", MethodName, dcType));
            }

            return method.Invoke(vm, values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CallAsyncMethodConverter : IValueConverter
    {
        public DependencyObject View { get; set; }
        public string MethodName { get; set; }
        public CallAsyncMethodConverter(DependencyObject view,string methodName)
        {
            View=view;
            MethodName = methodName;
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var vm = View.GetValue(FrameworkElement.DataContextProperty) ?? View.GetValue(FrameworkContentElement.DataContextProperty);
            
            if(vm==null)                            
                return DependencyProperty.UnsetValue;

            var dcType = vm.GetType();

            var method = dcType.GetMethod(MethodName);

            var task = Task.Run(async () =>
            {
                return await (dynamic)method.Invoke(vm, new object[] { value });
            });
            return new NotifyTaskCompletion<dynamic>(task);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
