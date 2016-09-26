using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace Pollux.Helper
{
    public static class SystemInfo
    {
        static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        static string GetFingerprint(string productName)
        {
            return GetSha256Hash(productName + ":" + GetMachineIdentifier());
        }

        static string GetSha256Hash(string input)
        {
            try
            {
                using (var sha256 = SHA256.Create())
                {
                    var bytes = Encoding.UTF8.GetBytes(input);
                    var hash = sha256.ComputeHash(bytes);

                    return string.Join("", hash.Select(b => b.ToString("x2", System.Globalization.CultureInfo.InvariantCulture)));
                }
            }
            catch (Exception e)
            {
                log.Error(e,"IMPOSSIBLE! Generating Sha256 hash caused an exception.");
                return null;
            }
        }
        public static string GetMachineIdentifier()
        {
            try
            {
                // adapted from http://stackoverflow.com/a/1561067
                var fastedValidNetworkInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .Where( nic=>nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    //.Where(nic=>
                    //    nic
                    //    .GetIPProperties()
                    //    .UnicastAddresses.FirstOrDefault(ip=>ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) != null)
                    .OrderBy(nic => nic.Speed)
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                    .Select(nic => nic.GetPhysicalAddress().ToString());
                    //.FirstOrDefault();//address => address.Length > 12);
                string mac = GetMACAddress();
        return fastedValidNetworkInterface.First() ?? GetMachineNameSafe();
            }
            catch (Exception e)
            {
                log.Info(e,"Could not retrieve MAC address. Fallback to using machine name.");
                return GetMachineNameSafe();
            }
        }
        public static string GetMACAddress()
        {
            //Select MACAddress,PNPDeviceID FROM Win32_NetworkAdapter WHERE MACAddress IS NOT NULL
            ManagementObjectSearcher searcher = 
                new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration where IPEnabled=true");
            IEnumerable<ManagementObject> objects = searcher.Get().Cast<ManagementObject>();
            string mac = (from o in objects orderby o["IPConnectionMetric"] select o["MACAddress"].ToString()).FirstOrDefault();
            return mac;
        }
        public static string GetMachineNameSafe()
        {
            try
            {
                return Dns.GetHostName();
            }
            catch (Exception e)
            {
                log.Info(e,"Failed to retrieve host name using `DNS.GetHostName`.");
                try
                {
                    return Environment.MachineName;
                }
                catch (Exception ex)
                {
                    log.Info(ex,"Failed to retrieve host name using `Environment.MachineName`.");
                    return "(unknown)";
                }
            }
        }
        public static float AvailableMemory
        {
            get
            {
                using (PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes", true))
                {
                    return ramCounter.NextValue();
                }
            }
        }
    }
    public static class LinqEx
    {
        //Merge 2 Dictionarys
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dictA, IDictionary<TKey, TValue> dictB) where TValue : class
        {
            return dictA.Keys.Union(dictB.Keys).ToDictionary(k => k, k => dictA[k] ?? dictB[k]);
        }
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IEnumerable<Dictionary<TKey, TValue>> dicts, Func<IGrouping<TKey, TValue>, TValue> resolveDuplicates)
        {
            if (resolveDuplicates == null)
                resolveDuplicates = new Func<IGrouping<TKey, TValue>, TValue>(group => group.First());

            return dicts.SelectMany<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>(dict => dict)
                        .ToLookup(pair => pair.Key, pair => pair.Value)
                        .ToDictionary(group => group.Key, group => resolveDuplicates(group));
        }
    }

    public static class JsonHelper
    {
        public static string SerializeObject(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        #region DataContractJsonSerializer
        //DataContractJsonSerializer
        public static T FromJson<T>(string json) where T: class
        {
            var ser = new DataContractJsonSerializer(typeof(T));

            T obj = null;

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                obj = ser.ReadObject(stream) as T;
            }

            return obj;
        }
        //DataContractJsonSerializer
        public static string ToJson<T>(this object obj)
        {
            using (var stream = new MemoryStream())
            {
                var ser = new DataContractJsonSerializer(typeof(T));
                ser.WriteObject(stream, obj);
                stream.Position = 0;
                return new StreamReader(stream).ReadToEnd();
            }
        }
        #endregion
    }
    public static class CloneHelper
    {
        public static List<T> Clone<T>(this List<T> oldList)
        {
            var array = new T[oldList.Count];
            Array.Copy(oldList.ToArray(), array, oldList.Count);
            return array.ToList();

        }
    }
    
    public static class XmlHelper
    {
        public static string Serialize<T>(this object obj)
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(typeof(T));
            var settings = new XmlWriterSettings();
            settings.Indent = true;
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

        public static XElement ToXElement<T>(this object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(streamWriter, obj);
                    return XElement.Parse(Encoding.UTF8.GetString(memoryStream.ToArray()));
                }
            }
        }

        public static T FromXElement<T>(this XElement xElement)
        {
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xElement.ToString())))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(memoryStream);
            }
        }
        public static T Deserialize<T>(this String xmlText)
        {
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xmlText)))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(memoryStream);
            }
        }
    }
    public static class VisualTreeHelperEx
    {
        public static T Clone<T>(T source)
        {
            string xaml = System.Windows.Markup.XamlWriter.Save(source);
            StringReader stringReader = new StringReader(xaml);
            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringReader);
            T t = (T)System.Windows.Markup.XamlReader.Load(xmlReader);
            return t;
        }
        public static void ToXamlFile<T>(this DependencyObject depencyObject, string filename) where T : DependencyObject
        {
            string xaml = System.Windows.Markup.XamlWriter.Save(depencyObject);
            using (FileStream filestream = File.Create(filename))
            {
                using (StreamWriter streamwriter = new StreamWriter(filestream))
                {
                    streamwriter.Write(xaml);
                }
            }
        }
        public static T FromXamlFile<T>(string filename) where T : DependencyObject
        {
            FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read);
            T dependencyObject = System.Windows.Markup.XamlReader.Load(fs) as T;
            fs.Close();
            return dependencyObject;
        }
        public static DependencyObject FindVisualTreeRoot(this DependencyObject initial)
        {
            DependencyObject current = initial;
            DependencyObject result = initial;

            while (current != null)
            {
                result = current;
                if (current is Visual || current is Visual3D)
                {
                    current = VisualTreeHelper.GetParent(current);
                }
                else
                {
                    // If we're in Logical Land then we must walk 
                    // up the logical tree until we find a 
                    // Visual/Visual3D to get us back to Visual Land.
                    current = LogicalTreeHelper.GetParent(current);
                }
            }

            return result;
        }
        public static T FindVisualParent<T>(this DependencyObject obj) where T : DependencyObject
        {
            while (obj != null && !(obj is T))
                obj = System.Windows.Media.VisualTreeHelper.GetParent(obj);

            return (T)obj;
        }
        public static T FindVisualChild<T>(this DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static object FindViewModel(this DependencyObject associatedObject)
        {
            var all = associatedObject.GetSelfAndAncestors();

            return all.Cast<FrameworkElement>().Where(x => x.DataContext != null).Select(x=>x.DataContext).FirstOrDefault();
        }
    }

    public static class ImageHelper
    {
        public static Bitmap Resize(Bitmap bitmap, int newWidth, int newHeight)
        {
            var resizedBitmap = new Bitmap(newWidth, newHeight);
            var g = Graphics.FromImage(resizedBitmap);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(bitmap, 0, 0, newWidth, newHeight);
            g.Dispose();
            return resizedBitmap;
        }
        public static BitmapImage ToBitmapImage(this byte[] imageBytes)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(imageBytes);
            bitmapImage.EndInit();
            //BitmapImage EndInit完不能再變更

            return bitmapImage;
        }
        //img1.Source =new BitmapImage(new Uri(@"image file path", UriKind.RelativeOrAbsolute));
        public static BitmapSource ToBitmapSource(this System.Drawing.Image image)
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            //save the image to memStream as a png
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            //gets a decoder from this stream
            System.Windows.Media.Imaging.PngBitmapDecoder decoder = new PngBitmapDecoder(
                memoryStream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.Default);

            return decoder.Frames[0];

        }


        public static BitmapSource ConvertGDI_To_WPF(System.Drawing.Bitmap bitmap)
        {
            BitmapSource bms = null;
            if (bitmap != null)
            {
                //Memory Leak 
                IntPtr h_bm = bitmap.GetHbitmap();
                bms = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(h_bm, IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            return bms;
        }
    }
}
