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
    public class CallMethodConverter : IValueConverter
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

            var dcType = vm.GetType();

            var method = dcType.GetMethod(MethodName);

            return method.Invoke(vm, new object[] { value });

            //return DependencyProperty.UnsetValue;
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
