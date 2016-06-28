using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Logging
{
    [TestClass]
    public class BaseLoggerTests : BaseTest
    {
        #region LifeCycle

        protected override IEnumerable<Assembly> AssembliesWithTypesToPerformFieldValueEqualityOn
        {
            get { yield return typeof(BaseLoggerTests).Assembly; }
        }

        #endregion


        #region Test Classes

        public class LoggedMessage
        {
            public LoggingLevel Level { get; set; }

            public string Message { get; set; }

            public string Category { get; set; }
        }

        public class MyLogger : BaseLogger
        {
            public MyLogger()
            {
                Messages = new List<LoggedMessage>();
            }

            public MyLogger(string configSectionName)
                : base(configSectionName)
            {
                Messages = new List<LoggedMessage>();
            }

            public IList<LoggedMessage> Messages { get; set; }

            protected override void LogMessage(LoggingLevel level, string category, string message)
            {
                Messages.Add(new LoggedMessage
                {
                    Level = level,
                    Category = category,
                    Message = message
                });
            }
        }

        #endregion

        #region Constructors

        [TestMethod, TestCategory("Unit")]
        public void DefaultConstructor_SectionInConfig()
        {
            // Act
            var componentUnderTest = new MyLogger();

            // Assert
            Assert.IsTrue(componentUnderTest.ShouldLog(LoggingLevel.Debug, "cat1"));
            Assert.IsTrue(componentUnderTest.ShouldLog(LoggingLevel.Warn, "cat2"));
        }

        [TestMethod, TestCategory("Unit")]
        public void Constructor_SectionInConfig()
        {
            // Act
            var componentUnderTest = new MyLogger("loggingOk");

            // Assert
            Assert.IsTrue(componentUnderTest.ShouldLog(LoggingLevel.Debug, "cat1"));
            Assert.IsTrue(componentUnderTest.ShouldLog(LoggingLevel.Warn, "cat2"));
        }

        [TestMethod, TestCategory("Unit")]
        public void Constructor_NoSectionInConfig()
        {
            // Act
            var componentUnderTest = new MyLogger("badSectionName");

            // Assert
            Assert.IsFalse(componentUnderTest.ShouldLog(LoggingLevel.Debug, "cat1"));
            Assert.IsFalse(componentUnderTest.ShouldLog(LoggingLevel.Warn, "cat2"));
        }

        #endregion


        #region ShouldLog


        [TestMethod, TestCategory("Unit")]
        public void ShouldLog()
        {
            // Act
            var componentUnderTest = new MyLogger("loggingOk");

            // Assert

            // Prove that the level and below is respected
            Assert.IsTrue(componentUnderTest.ShouldLog(LoggingLevel.Debug, "cat1"));
            Assert.IsTrue(componentUnderTest.ShouldLog(LoggingLevel.Info, "cat1"));
            Assert.IsFalse(componentUnderTest.ShouldLog(LoggingLevel.Debug, "cat2"));
            Assert.IsTrue(componentUnderTest.ShouldLog(LoggingLevel.Warn, "cat2"));
        }


        [TestMethod, TestCategory("Unit")]
        public void ShouldLog_AllLoggingCategoriesSet()
        {
            // Act
            var componentUnderTest = new MyLogger("loggingLevels2");

            // Assert
            // Prove that the level and below is respected
            Assert.IsFalse(componentUnderTest.ShouldLog(LoggingLevel.Debug, "sdafsa"));
            Assert.IsTrue(componentUnderTest.ShouldLog(LoggingLevel.Info, "catsdfsda1"));
        }


        #endregion


        #region Log

        [TestMethod, TestCategory("Unit")]
        public void Log_ShouldNotLog()
        {
            // Arrange
            var componentUnderTest = new MyLogger("loggingLevels");

            // Act
            componentUnderTest.Log(LoggingLevel.Debug, "", () => "");

            // Assert
            componentUnderTest.Messages.ShouldHaveSameValueAs(new List<LoggedMessage>());
        }

        [TestMethod, TestCategory("Unit")]
        public void Log_ShouldLog()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var componentUnderTest = new MyLogger("loggingLevels");
                var expected = new DateTime(2000, 1, 2, 1, 2, 3);
                System.Fakes.ShimDateTime.NowGet = () => expected;

                // Act
                componentUnderTest.Log(LoggingLevel.Info, "cat1", () => "My Message {0}", "arg1");

                // Assert
                componentUnderTest.Messages.ShouldHaveSameValueAs(new[]
                {
                    new LoggedMessage
                    {
                        Category = "cat1",
                        Level = LoggingLevel.Info,
                        Message = "When:02/01/2000 01:02:03 Level:Info Category:cat1 Message:My Message arg1"
                    }
                });
            }
        }

        #endregion


        #region LogException

        [TestMethod, TestCategory("Unit")]
        public void LogException_ShouldNotLog()
        {
            // Arrange
            var componentUnderTest = new MyLogger("loggingEmpty");
            var exception = new Exception("Test");

            // Act
            componentUnderTest.LogException( "", exception);

            // Assert
            componentUnderTest.Messages.ShouldHaveSameValueAs(new List<LoggedMessage>());
        }

        [TestMethod, TestCategory("Unit")]
        public void LogException_ShouldLog()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var componentUnderTest = new MyLogger("loggingLevels");
                var expected = new DateTime(2000, 1, 2, 1, 2, 3);
                System.Fakes.ShimDateTime.NowGet = () => expected;
                var exception = new Exception("Test");
                
                // Act
                componentUnderTest.LogException("cat1", exception);

                // Assert
                componentUnderTest.Messages.ShouldHaveSameValueAs(new[]
                {
                    new LoggedMessage
                    {
                        Category = "cat1",
                        Level = LoggingLevel.Error,
                        Message = "When:02/01/2000 01:02:03 Level:Error Category:cat1 Message:" + exception
                    }
                });
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void LogException_ShouldLog_InnerException()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var componentUnderTest = new MyLogger("loggingLevels");
                var expected = new DateTime(2000, 1, 2, 1, 2, 3);
                System.Fakes.ShimDateTime.NowGet = () => expected;
                var exception = new Exception("Test");
                var aggregateException = new AggregateException(exception);

                // Act
                componentUnderTest.LogException("cat1", aggregateException);

                // Assert
                componentUnderTest.Messages.ShouldHaveSameValueAs(new[]
                {
                    new LoggedMessage
                    {
                        Category = "cat1",
                        Level = LoggingLevel.Error,
                        Message = "When:02/01/2000 01:02:03 Level:Error Category:cat1 Message:" + exception
                    }
                });
            }
        }

        #endregion


        #region SetLayout


        [TestMethod, TestCategory("Unit")]
        public void SetLayout()
        {
            // Arrange
            var componentUnderTest = new MyLogger("loggingLevels");

            // Act
            componentUnderTest.SetLayout((level, category, message) => $"MyLevel:{level} MyCategtory:{category} MyMessage:{message}");
            componentUnderTest.Log(LoggingLevel.Info, "cat1", () => "My Message {0}", "arg1");

            // Assert
            componentUnderTest.Messages.ShouldHaveSameValueAs(new[]
            {
                new LoggedMessage
                {
                    Category = "cat1",
                    Level = LoggingLevel.Info,
                    Message = "MyLevel:Info MyCategtory:cat1 MyMessage:My Message arg1"
                }
            });
        }

        #endregion
    }
}