using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Logging
{
    [TestClass]
    public class LoggingConfigurationSettingsTests : BaseTest
    {
        #region Life Cycle

        protected override IEnumerable<Assembly> AssembliesWithTypesToPerformFieldValueEqualityOn
        {
            get { yield return typeof(LoggingConfigurationSettings).Assembly; }
        }

        #endregion


        #region Constructors

        [TestMethod, TestCategory("Unit")]
        public void DefaultConstructor()
        {
            // Act
            var actual = new LoggingConfigurationSettings();

            // Assert
            Assert.IsNotNull(actual.Settings);
            Assert.AreEqual(0, actual.Settings.Count);
        }

        #endregion


        #region Read From Config File


        [TestMethod, TestCategory("Unit")]
        public void FromConfigFile()
        {
            // Act
            var actual = (LoggingConfigurationSettings)ConfigurationManager.GetSection("loggingOk");

            // Assert
            Assert.IsNotNull(actual.Settings);
            Assert.AreEqual(2, actual.Settings.Count);
            Assert.AreEqual("cat1", actual.Settings[0].Category);
            Assert.AreEqual(LoggingLevel.Debug, actual.Settings[0].Level);
            Assert.AreEqual("cat2", actual.Settings[1].Category);
            Assert.AreEqual(LoggingLevel.Warn, actual.Settings[1].Level);
        }

        #endregion
    }
}