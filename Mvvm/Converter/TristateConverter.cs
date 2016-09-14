using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Pollux.Converters
{
    //public class TristateConverter : IValueConverter
    //{
    //    public object Convert(object date, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (date is CollectionViewGroup)
    //        {
    //            var g = date as CollectionViewGroup;

    //            if (g.Items.Where(i => i is JsonDeviceInfo).Count() > 0)
    //            {
    //                var selected = g.Items.OfType<JsonDeviceInfo>().Where(i => i.IsChecked == true).Count();
    //                if (selected == g.Items.Count)
    //                    return true;
    //                else if (selected != 0)
    //                    return null;
    //                else
    //                    return false;
    //            }
    //            if (g.Items.Where(i => i is CollectionViewGroup).Count() > 0)
    //            {
    //                var all = g.Items.Where(i => i is CollectionViewGroup).Cast<CollectionViewGroup>().SelectMany(a => a.Items).Cast<JsonDeviceInfo>();
    //                var selected = all.Where(i => i.IsChecked == true).Count();
    //                if (selected == all.Count())
    //                    return true;
    //                else if (selected != 0)
    //                    return null;
    //                else
    //                    return false;
    //            }

    //        }

    //        return false;
    //        return DependencyProperty.UnsetValue;
    //    }

    //    public object ConvertBack(object date, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (date is CollectionViewGroup)
    //            return null;
    //        return true;
    //    }
    //}
}
