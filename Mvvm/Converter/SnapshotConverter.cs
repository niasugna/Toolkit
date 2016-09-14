using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Pollux.Converters
{
    //public sealed class ImageConverter : IValueConverter, IMultiValueConverter
    //{
    //    public object Convert(object date, Type targetType,object parameter, CultureInfo culture)
    //    {
    //        try
    //        {

    //            var option = date as ChartOption;
    //            if (option == null)
    //            {
    //                return DependencyProperty.UnsetValue;
    //            }
    //            string chartType = parameter as string;
    //            string url = string.Format(@"..\..\Resources\{0}\{1}{2}(M).png", option.Presentation, option.Unit, chartType);
    //            if (chartType == "Column" && option.IsColumnChartEnabled)
    //            {
    //                return new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
    //            }
    //            if (chartType == "Pie" && option.IsPieChartEnabled)
    //            {
    //                return new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
    //            }
    //            if (chartType == "Line" && option.IsLineChartEnabled)
    //            {
    //                return new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
    //            }

    //            return DependencyProperty.UnsetValue;
    //            //return new BitmapImage(new Uri(@"pack://application:,,,/Resources/Access.pngl", UriKind.RelativeOrAbsolute));
    //        }
    //        catch (Exception)
    //        {
    //            return new BitmapImage();
    //        }
    //    }

    //    public object ConvertBack(object date, Type targetType,
    //                              object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        try
    //        {
    //            var option = values[0] as ChartOption;
    //            var single = ((Boolean)(values[1]) == true) ? "S" : "M:";
    //            if (option == null)
    //            {
    //                return DependencyProperty.UnsetValue;
    //            }
    //            string chartType = parameter as string;
    //            string url = string.Format(@"..\..\Resources\{0}\By{1}{2}({3}).png", option.Presentation, option.Unit, chartType, single);
    //            if (chartType == "Column" && option.IsColumnChartEnabled)
    //            {
    //                return new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
    //            }
    //            if (chartType == "Pie" && option.IsPieChartEnabled)
    //            {
    //                return new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
    //            }
    //            if (chartType == "Line" && option.IsLineChartEnabled)
    //            {
    //                return new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
    //            }

    //            return DependencyProperty.UnsetValue;
    //        }
    //        catch (Exception)
    //        {
    //            return new BitmapImage();
    //        }
    //    }

    //    public object[] ConvertBack(object date, Type[] targetTypes, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
