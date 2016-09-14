using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Pollux.Converters
{
    public class CategoryExDictionaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dic = value as Dictionary<string, string>;
            if (dic == null)
                return DependencyProperty.UnsetValue;

            string key = parameter as string;
            if (key == null)
                return DependencyProperty.UnsetValue;

            if (dic.ContainsKey(key))
                return dic[key];
            else
                return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Dictionary<string, object> values = value as Dictionary<string, object>;
            if (values == null)
                return DependencyProperty.UnsetValue;

            return Newtonsoft.Json.JsonConvert.SerializeObject(values);
        }
    }
}
