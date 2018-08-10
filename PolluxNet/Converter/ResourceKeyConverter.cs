using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Pollux.Converters
{

    //load specified resource by key
    public class ResourceKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var o = value as string;
            if (o == null)
                return value;

            try
            {
                if (Application.Current.Resources.Contains(value))
                    return Application.Current.FindResource(value);
                else
                    return o;
            }
            catch (ResourceReferenceKeyNotFoundException)
            {
                return o;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
