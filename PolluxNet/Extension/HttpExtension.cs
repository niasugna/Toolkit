﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.Extension
{
    public static partial class Extensions
    {
        ///<configuration>
        ///<system.net>
        ///<settings>
        ///<httpWebRequest useUnsafeHeaderParsing="true" />
        ///</settings>
        ///</system.net>
        ///</configuration>
        public static bool EnableUnsafeHeaderParsing()
        {

            //METHOD 1
            var httpWebRequestElement = new HttpWebRequestElement();
            httpWebRequestElement.UseUnsafeHeaderParsing = true;

            //METHOD 2
            //Get the assembly that contains the internal class
            Assembly aNetAssembly = Assembly.GetAssembly(typeof(SettingsSection));
            if (aNetAssembly != null)
            {
                //Use the assembly in order to get the internal type for the internal class
                Type aSettingsType = aNetAssembly.GetType("System.Net.Configuration.SettingsSectionInternal");
                if (aSettingsType != null)
                {
                    //Use the internal static property to get an instance of the internal settings class.
                    //If the static instance isn't created allready the property will create it for us.
                    object anInstance = aSettingsType.InvokeMember("Section", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic, null, null,new object[] { });
                    if (anInstance != null)
                    {
                        //Locate the private bool field that tells the framework is unsafe header parsing should be allowed or not
                        FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (aUseUnsafeHeaderParsing != null)
                        {
                            aUseUnsafeHeaderParsing.SetValue(anInstance, true);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static HttpClient Authorization(this HttpClient httpClient, string user, string password)
        {
            try
            {
                httpClient.MaxResponseContentBufferSize = 256000;
                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(String.Format("{0}:{1}", user, password))));
                return httpClient;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static HttpClient Referrer(this HttpClient httpClient, string referrer)
        {
            //http://stackoverflow.com/questions/13903470/using-httpclient-class-with-winrt
            //string URI = "http://www.indianrail.gov.in/cgi_bin/inet_pnrstat_cgi.cgi";
            //string MessageInfo = Uri.EscapeUriString("lccp_pnrno1=8561180604&amp;submitpnr=Get Status");
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, URI);
            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            //request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8", 0.7));
            //request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-us", 0.5));
            //request.Content = new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(MessageInfo)));
            //request.Content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //request.Headers.Host = "www.indianrail.gov.in";
            //request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            httpClient.DefaultRequestHeaders.Referrer = new Uri(referrer);

            return httpClient;
        }
    }
}
