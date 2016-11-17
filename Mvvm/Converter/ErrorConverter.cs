using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;


 //      <Style x:Key="textBoxInError" TargetType="{x:Type TextBox}">
 //           <Style.Triggers>
 //               <Trigger Property="Validation.HasError" Value="true">
 //                   <Setter Property="ToolTip" Value="{Binding RelativeSource={RelativeSource Self}, Path=(Validation.Errors), Converter={StaticResource errorConverter}}"/>
 //               </Trigger>
 //           </Style.Triggers>
 //       </Style>

namespace Pollux.Converter
{
    public class ErrorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList<ValidationError> errors = value as IList<ValidationError>;

            if (errors == null || errors.Count == 0)
                return string.Empty;

            Exception exception = errors[0].Exception;
            if (exception != null)
            {
                if (exception is TargetInvocationException)
                {
                    // It's an exception in the the model's Property setter. Get the inner exception
                    exception = exception.InnerException;
                }

                return exception.Message;
            }

            return errors[0].ErrorContent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}