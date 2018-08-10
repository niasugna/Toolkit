using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Pollux.Controls.Wizard
{
    public class TabIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TabItem tabItem = value as TabItem;
            var fromContainer = ItemsControl.ItemsControlFromItemContainer(tabItem).ItemContainerGenerator;

            var items = fromContainer.Items.Cast<TabItem>().Where(x => x.Visibility == Visibility.Visible).ToList();
            var count = items.Count();

            var index = items.IndexOf(tabItem);
            if (parameter == null)
            {
                return index + 1;
            }
            else
            {
                if (index == 0)
                    return string.Equals("First", parameter);    
                else if (count - 1 == index)
                    return string.Equals("Last", parameter);    
                else
                    return false;

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class IsProgressedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var tab = values[0] as TabItem;
            var itemControl =ItemsControl.ItemsControlFromItemContainer(tab);
            int index = itemControl.ItemContainerGenerator.IndexFromContainer(tab);

            bool checkNextItem = System.Convert.ToBoolean(parameter.ToString());
            if (checkNextItem)
            {
                if ((int)values[1] == itemControl.Items.Count - 1)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

            if (index < (int)values[1])
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
