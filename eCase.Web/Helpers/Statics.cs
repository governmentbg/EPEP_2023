using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace eCase.Web.Helpers
{
    public class Statics
    {
        #region Public

        public static int MaxUserItemsPerPage
        {
            get
            {
                return GetAppConfigValue<int>("eCase.Web:MaxUserItemsPerPage");
            }
        }

        public static int MaxCaseItemsPerPage
        {
            get
            {
                return GetAppConfigValue<int>("eCase.Web:MaxCaseItemsPerPage");
            }
        }

        public static int MaxCaseItems
        {
            get
            {
                return GetAppConfigValue<int>("eCase.Web:MaxCaseItems");
            }
        }

        public static int SmallPageSize
        {
            get
            {
                return GetAppConfigValue<int>("eCase.Web:SmallPageSize");
            }
        }

        public static int MaxNomItems
        {
            get
            {
                return GetAppConfigValue<int>("eCase.Web:MaxNomItems");
            }
        }

        public static int MaxSummonItemsPerPage
        {
            get
            {
                return GetAppConfigValue<int>("eCase.Web:MaxSummonItemsPerPage");
            }
        }

        public static bool IsSummonsPortal
        {
            get
            {
                string isSummonsPortal = GetAppConfigValue<string>("eCase.Web:IsSummonsPortal");

                if (!String.IsNullOrWhiteSpace(isSummonsPortal) && isSummonsPortal.ToLower().Equals("true"))
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region Private

        private static ConcurrentDictionary<string, object> _valueCache = new ConcurrentDictionary<string, object>();
        private static object _syncRoot = new object();

        private static T GetAppConfigValue<T>(string appConfigKey)
        {
            if (!_valueCache.ContainsKey(appConfigKey))
            {
                lock (_syncRoot)
                {
                    if (!_valueCache.ContainsKey(appConfigKey))
                    {
                        string appConfigValue = System.Configuration.ConfigurationManager.AppSettings[appConfigKey];

                        T configValue = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(appConfigValue);

                        _valueCache.TryAdd(appConfigKey, configValue);
                    }
                }
            }

            return (T)_valueCache[appConfigKey];
        }

        #endregion
    }
}
