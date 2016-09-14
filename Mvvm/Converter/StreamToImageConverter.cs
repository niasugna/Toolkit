using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Pollux.Converters
{
    public sealed class StreamToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                BitmapImage img = new BitmapImage();
                System.IO.MemoryStream stream = null;
                img.BeginInit();
                if (value == null)
                {
                    stream = new System.IO.MemoryStream();
                }
                else
                    stream = (System.IO.MemoryStream)value;
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                img.StreamSource = stream;
                img.EndInit();
                return img;
                //return new BitmapImage(new Uri((string)filter));
            }
            catch
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
