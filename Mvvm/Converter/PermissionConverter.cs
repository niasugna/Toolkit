using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Pollux.Converters
{
    public class PermissionConverter : IValueConverter
    {
        public PermissionConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            var o = (int)value;

            if (o == 1)
            {
                return "Admin";
            }
            if (o == 2)
            {
                return "Power User";
            }
            if (o == 3)
            {
                return "User";
            }

            return DependencyProperty.UnsetValue;
            //try
            //{
            //    if (Application.Current.Resources.Contains(filter))
            //        return Application.Current.FindResource(filter);
            //    else
            //        return o;
            //}
            //catch (ResourceReferenceKeyNotFoundException)
            //{
            //    return o;
            //}
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
