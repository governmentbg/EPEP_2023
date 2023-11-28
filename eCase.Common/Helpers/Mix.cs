using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace eCase.Common.Helpers
{
    /// <summary>
    /// Клас, съдържащ помощни методи
    /// </summary>
    public static class Mix
    {
        /// <summary>
        /// Връща поток от данни в битов масив
        /// </summary>
        /// <param name="stream">поток от данни</param>
        /// <returns></returns>
        public static byte[] GetAllStreamBytes(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[1024];

                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Добавя/редактира запис от хештаблица
        /// </summary>
        /// <typeparam name="TKey">тип на ключа</typeparam>
        /// <typeparam name="TVal">тип на стойността</typeparam>
        /// <param name="dictionary">обект от тип хештаблица</param>
        /// <param name="key">ключ</param>
        /// <param name="val">стойност</param>
        public static void AddOrUpdateDictionaryItem<TKey, TVal>(IDictionary<TKey, TVal> dictionary, TKey key, TVal val)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = val;
            }
            else
            {
                dictionary.Add(key, val);
            }
        }

        /// <summary>
        /// Добавя списък от записи към хештаблица
        /// </summary>
        /// <typeparam name="TKey">тип на ключа</typeparam>
        /// <typeparam name="TVal">тип на стойността</typeparam>
        /// <param name="dictionary">обект от тип хештаблица</param>
        /// <param name="kvps">списък записи</param>
        public static void AddDictionaryItems<TKey, TVal>(IDictionary<TKey, TVal> dictionary, IEnumerable<KeyValuePair<TKey, TVal>> kvps)
        {
            foreach (var kvp in kvps)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Връща частична страница в текст
        /// </summary>
        /// <param name="context">контекст на контролера</param>
        /// <param name="partialViewName">име на частична страница</param>
        /// <param name="masterName">име на мастър страница</param>
        /// <param name="viewData">хештаблица с данни</param>
        /// <param name="tempData">хештаблица с временни данни</param>
        /// <returns></returns>
        public static string RenderPartialToString(ControllerContext context, string partialViewName, string masterName, ViewDataDictionary viewData, TempDataDictionary tempData)
        {
            ViewEngineResult result = ViewEngines.Engines.FindView(context, partialViewName, masterName);

            if (result.View != null)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                using (System.IO.StringWriter sw = new System.IO.StringWriter(sb))
                {
                    using (System.Web.UI.HtmlTextWriter output = new System.Web.UI.HtmlTextWriter(sw))
                    {
                        ViewContext viewContext = new ViewContext(context, result.View, viewData, tempData, output);
                        result.View.Render(viewContext, output);
                    }
                }

                return sb.ToString();
            }

            return String.Empty;
        }

        public static string RemoveXmlElement(string xml, string signatureXPath, IDictionary<string, string> signatureXPathNamespaces)
        {
            try
            {
                XDocument document = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                
                XmlNamespaceManager nss = new XmlNamespaceManager(document.CreateNavigator().NameTable);
                foreach (var ns in signatureXPathNamespaces)
                    nss.AddNamespace(ns.Key, ns.Value);

                XElement node = document.XPathSelectElement(signatureXPath, nss);
                foreach (var element in node.Elements())
                    element.Remove();

                xml = document.Declaration.ToString() + document.ToString(SaveOptions.DisableFormatting);
            }
            catch { }

            return xml;
        }

        public static string ToXsdDurationByDays(this System.String str)
        {
            return string.Format("P0Y0M{0}DT0H0M0S", str);
        }

        public static string GetHostUrl(object lang = null)
        {
            string port = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
            if (port == null || port == "80" || port == "443")
                port = "";
            else
                port = ":" + port;

            string protocol = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"];
            if (protocol == null || protocol == "0")
                protocol = "http://";
            else
                protocol = "https://";

            string sOut = protocol + System.Web.HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + port + System.Web.HttpContext.Current.Request.ApplicationPath;

            if (sOut.EndsWith("/"))
                sOut = sOut.Substring(0, sOut.Length - 1);

            if (lang != null)
                sOut += "/" + lang;

            return sOut;
        }

        public static string GetRouteLang(object routeDataLang)
        {
            string lang = string.Empty;
            
            if (routeDataLang != null)
            {
                lang = routeDataLang as string;// +"/";
            }

            return lang;
        }
    }
}
