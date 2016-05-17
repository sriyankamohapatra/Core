using System;
using System.Configuration;
using System.IO;

namespace Sfa.Core.Environment
{
    /// <summary>
    /// Utilities to help configure the Current app domains DataDictionary.
    /// </summary>
    public class DataDictionaryHelper
    {
        /// <summary>
        /// Sets the DataDictionaries location relative to the currently executing assembly and the path defines by the app setting.
        /// </summary>
        /// <param name="appSettingKey">The name of the app setting that contains the relative path.</param>
        public static void SetDataDictionaryLocationRelativeToAppSettings(string appSettingKey)
        {
            var baseLocation = AppDomain.CurrentDomain.BaseDirectory;
            var dataDictionaryPath = Path.Combine(baseLocation, ConfigurationManager.AppSettings["dataDictionaryRelativePath"]);
            dataDictionaryPath = Path.GetDirectoryName(dataDictionaryPath);
            dataDictionaryPath = Path.GetFullPath(dataDictionaryPath);
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDictionaryPath);
        } 
    }
}