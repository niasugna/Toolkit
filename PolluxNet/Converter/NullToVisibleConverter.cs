using System;
using System.Windows;
using System.Windows.Data;

namespace Pollux.Converters
{
    /// <summary>
    /// Value converter that translates null to <see cref="Visibility.Visible"/> and the opposite to <see cref="Visibility.Collapsed"/> 
    /// </summary>
    public sealed class NullToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value == null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value is Visibility && (Visibility)value == Visibility.Visible) ? false : true;
        }
    }

    /// <summary>
    /// Value converter that translates NOT null to <see cref="Visibility.Visible"/> and the opposite to <see cref="Visibility.Collapsed"/> 
    /// </summary>
    public sealed class NotNullToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value != null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value is Visibility && (Visibility)value == Visibility.Visible) ? true : false;
        }
    }
}