using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Pollux.Helper;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace Pollux.Converters
{
    public class XmlConverter<T> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string xml = value as string;
            if (xml == null)
                return DependencyProperty.UnsetValue;

            return xml.Deserialize<T>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            T obj = (T)value;
            if (obj == null)
                return DependencyProperty.UnsetValue;

            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(typeof(T));
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            {
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    serializer.Serialize(writer, obj, emptyNamepsaces);
                    return stream.ToString();
                }
            }
        }
    }
}
