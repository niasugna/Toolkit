using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Pollux.Converters
{
    
    //<RadioButton.IsChecked>
    //    <MultiBinding Converter="{x:Static conv:Converters.EnumCheckedConverter}" Mode="OneWay" NotifyOnSourceUpdated="True" UpdateSourceTrigger="PropertyChanged">
    //        <Binding Path="ChartInterval" RelativeSource="{RelativeSource Mode=FindAncestor, AncestorType=UserControl}" />
    //        <Binding Path="." />
    //    </MultiBinding>
    //</RadioButton.IsChecked>
    public class EnumCheckedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return !values.Contains(null) && values[0].ToString().Equals(values[1].ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
