using System.Configuration;

namespace Sfa.Core.Logging
{
    /// <summary>
    /// Defines the config file settings for the logging levels.
    /// </summary>
    public class LoggingConfigurationSettings : ConfigurationSection
    {
        [ConfigurationProperty("settings")]
        public LogSettingCollection Settings
        {
            get { return (LogSettingCollection)this["settings"]; }
            set { this["settings"] = value; }
        }
    }

    /// <summary>
    /// Defines an individual settings for a category definition.
    /// </summary>
    public class LogSetting : ConfigurationElement
    {
        [ConfigurationProperty("category", DefaultValue = "Audit", IsKey = true)]
        public string Category
        {
            get { return (string)this["category"]; }
            set { this["category"] = value; }
        }

        [ConfigurationProperty("level", DefaultValue = "None")]
        public LoggingLevel Level
        {
            get { return (LoggingLevel)this["level"]; }
            set { this["level"] = value; }
        }
    }

    /// <summary>
    /// Defines a collection of <see cref="LogSetting"/>s.
    /// </summary>
    public class LogSettingCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LogSetting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LogSetting)element).Category;
        }

        protected override string ElementName => "logSetting";

        protected override bool IsElementName(string elementName)
        {
            return !string.IsNullOrEmpty(elementName) && elementName == "logSetting";
        }

        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

        public LogSetting this[int index] => BaseGet(index) as LogSetting;

        public new LogSetting this[string key] => BaseGet(key) as LogSetting;
    }
}
