using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Logging;
using Sfa.Core.Logging.Fakes;
using Sfa.Core.Testing;

namespace Sfa.Core.Diagnostics
{
    [TestClass]
    public class HealthCheckTests : BaseTest
    {
        #region Life Cycle

        protected override IEnumerable<ILogger> LogggersToUse
        {
            get
            {
                foreach (var logger in base.LogggersToUse)
                {
                    yield return logger;
                }
                yield return StubLogger;
            }
        }

        protected StubILogger StubLogger = new StubILogger();

        protected override IEnumerable<Assembly> AssembliesWithTypesToPerformFieldValueEqualityOn
        {
            get
            {
                foreach (var assembly in base.AssembliesWithTypesToPerformFieldValueEqualityOn)
                {
                    yield return assembly;
                }
                yield return typeof(HealthCheckTests).Assembly;
            }
        }

        #endregion


        #region Test Classes

        public class LoggedData
        {
            public LoggingLevel LoggingLevel { get; set; }
            public string Category { get; set; }
            public string Message { get; set; }
            public object[] Args { get; set; }
        }

        public class LoggedException
        {
            public Exception Exception { get; set; }
            public string Category { get; set; }
        }

        #endregion


        #region RunTest

        [TestMethod, TestCategory("Unit")]
        public void RunTest_PassedTest()
        {
            // Arrange
            var componentUnderTest = new HealthCheck();
            var loggedData = new List<LoggedData>();

            StubLogger.ShouldLogLoggingLevelString = (level, s) => true;

            StubLogger.LogLoggingLevelStringFuncOfStringObjectArray = (level, category, messageFormat, args) =>
            {
                loggedData.Add(new LoggedData
                {
                    LoggingLevel = level,
                    Category = category,
                    Message = messageFormat(),
                    Args = args
                });
            };

            // Act
            componentUnderTest.RunTest(() => { }, "simple");

            // Assert
            loggedData.ShouldHaveSameValueAs(new List<LoggedData>
            {
                new LoggedData
                {
                    LoggingLevel = LoggingLevel.Info,
                    Category = CoreLoggingCategory.HealthCheck,
                    Message = "simple passed",
                    Args = new object[0]
                }
            });

            componentUnderTest.AllPassed.ShouldHaveSameValueAs(true);
        }

        [TestMethod, TestCategory("Unit")]
        public void RunTest_FailedTest()
        {
            // Arrange
            var componentUnderTest = new HealthCheck();
            var loggedData = new List<LoggedData>();
            var loggedExceptions = new List<LoggedException>();
            var referenceException = new NullReferenceException();

            StubLogger.ShouldLogLoggingLevelString = (level, s) => true;

            StubLogger.LogLoggingLevelStringFuncOfStringObjectArray = (level, category, messageFormat, args) =>
            {
                loggedData.Add(new LoggedData
                {
                    LoggingLevel = level,
                    Category = category,
                    Message = messageFormat(),
                    Args = args
                });
            };

            StubLogger.LogExceptionStringException = (category, exception) =>
            {
                loggedExceptions.Add(new LoggedException
                {
                    Exception = exception,
                    Category = category
                });
            };

            // Act
            componentUnderTest.RunTest(() => { throw referenceException; }, "simple");

            // Assert
            loggedData.ShouldHaveSameValueAs(new List<LoggedData>
            {
                new LoggedData
                {
                    LoggingLevel = LoggingLevel.Info,
                    Category = CoreLoggingCategory.HealthCheck,
                    Message = "simple failed",
                    Args = new object[0]
                }
            });
            loggedExceptions.ShouldHaveSameValueAs(new List<LoggedException>
            {
                new LoggedException
                {
                    Category = CoreLoggingCategory.HealthCheck,
                    Exception = referenceException
                }
            });

            componentUnderTest.AllPassed.ShouldHaveSameValueAs(false);
        }

        #endregion


        #region AllPassed
        

        [TestMethod, TestCategory("Unit")]
        public void AllPassed_PassingTest()
        {
            // Arrange
            var componentUnderTest = new HealthCheck();
            componentUnderTest.RunTest(() => { }, "test1");
            componentUnderTest.RunTest(() => { }, "test2");

            // Act
            var actual = componentUnderTest.AllPassed;

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }


        [TestMethod, TestCategory("Unit")]
        public void AllPassed_FailedTest()
        {
            // Arrange
            var componentUnderTest = new HealthCheck();
            componentUnderTest.RunTest(() => {  }, "test1");
            componentUnderTest.RunTest(() => { throw new Exception(); }, "test2");

            // Act
            var actual = componentUnderTest.AllPassed;

            // Assert
            actual.ShouldHaveSameValueAs(false);
        }

        #endregion
    }
}