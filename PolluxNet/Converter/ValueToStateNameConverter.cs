using System;
using System.Globalization;
using System.Windows.Data;

namespace Pollux.Converters
{
    //<!--  Binds the view VisualState to the View CurrentPage property  -->
    //<i:Interaction.Behaviors>
    //    <b:BindVisualStateBehavior StateName="{Binding CurrentPage, Converter={StaticResource ValueToStateNameConverter}}" />
    //</i:Interaction.Behaviors>
    public class ValueToStateNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "Page" + value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
