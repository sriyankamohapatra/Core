using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfa.Core.Logging
{
    public class BaseLoggerConfigurationSettings : ConfigurationSection
    {
        [ConfigurationProperty("settings")]
        public LogSettingCollection Settings
        {
            get
            {
                return (LogSettingCollection)this["settings"];
            }
            set
            { this["settings"] = value; }
        }
    }

    public class LogSetting : ConfigurationElement
    {

        [ConfigurationProperty("category", DefaultValue = "Audit", IsKey = true)]
        public string category
        {
            get { return (string)this["category"]; }
            set { this["category"] = value; }
        }

        [ConfigurationProperty("level", DefaultValue = "None")]
        public LoggingLevel level
        {
            get { return (LoggingLevel)this["level"]; }
            set { this["level"] = value; }
        }
    }

    public class LogSettingCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LogSetting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as LogSetting).category;
        }

        protected override string ElementName
        {
            get
            {
                return "logSetting";
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return !String.IsNullOrEmpty(elementName) && elementName == "logSetting";
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        public LogSetting this[int index]
        {
            get
            {
                return base.BaseGet(index) as LogSetting;
            }
        }

        public new LogSetting this[string key]
        {
            get
            {
                return base.BaseGet(key) as LogSetting;
            }
        }
    }
}
