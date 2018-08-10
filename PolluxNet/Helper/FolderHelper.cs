using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.Helper
{
    public class FolderHelper
    {
        public static string ApplicationPath
        {
            get
            {
                //Application.StartupPath
                //System.IO.Path.GetDirectoryName(
                //System.Reflection.Assembly.GetExecutingAssembly().Location)
                //AppDomain.CurrentDomain.BaseDirectory
                //System.IO.Directory.GetCurrentDirectory()
                //Environment.CurrentDirectory
                //System.IO.Path.GetDirectoryName(
                //System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
                //System.IO.Path.GetDirectory(Application.ExecutablePath)

                string fullpath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                //UriBuilder uri = new UriBuilder(codeBase);
                //string path = Uri.UnescapeDataString(uri.Path);
                return System.IO.Path.GetDirectoryName(fullpath);
            }
        }

        public string GetFolderPath(System.Environment.SpecialFolder folder)
        {
            return Environment.GetFolderPath(folder);
        }
        public string GetTempFileName(string extension)
        {
            return Path.ChangeExtension(Path.GetTempFileName(), extension);
        }
    }
}
