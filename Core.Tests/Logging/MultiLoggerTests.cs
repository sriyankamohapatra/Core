using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Logging
{
    [TestClass]
    public class MultiLoggerTests : BaseTest
    {
        #region Life Cycle

        protected override IEnumerable<Assembly> AssembliesWithTypesToPerformFieldValueEqualityOn
        {
            get { yield return typeof(MultiLoggerTests).Assembly; }
        }

        #endregion


        #region Test Classes

        public class SetCategory
        {
            public LoggingLevel LoggingLevel { get; set; }
            public string Category { get; set; }
        }

        public class SetLog : SetCategory
        {
            public string Message { get; set; }
        }

        public class SetException : SetCategory
        {
            public Exception Exception { get; set; }
        }

        #endregion


        #region SetCategoryLogging

        [TestMethod, TestCategory("Unit")]
        public void SetCategoryLogging()
        {
            // Arrange
            var logger = new Fakes.StubILogger();
            var componentUnderTest = new MultiLogger(logger);
            SetCategory setCategory = null;

            logger.SetCategoryLoggingStringLoggingLevel = (category, level) => setCategory = new SetCategory {Category = category, LoggingLevel = level};

            var expected = new SetCategory {Category = "cat", LoggingLevel = LoggingLevel.Info};

            // Act
            componentUnderTest.SetCategoryLogging("cat", LoggingLevel.Info);

            setCategory.ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region ShouldLog


        [TestMethod, TestCategory("Unit")]
        public void ShouldLog_True()
        {
            // Arrange
            var logger1 = new Fakes.StubILogger();
            var logger2 = new Fakes.StubILogger();
            var logger3 = new Fakes.StubILogger();
            var componentUnderTest = new MultiLogger(logger1, logger2, logger3);

            logger1.ShouldLogLoggingLevelString = (level, category) => false;
            logger2.ShouldLogLoggingLevelString = (level, category) => true;
            logger3.ShouldLogLoggingLevelString = (level, category) => false;
            
            // Act
            var actual = componentUnderTest.ShouldLog(LoggingLevel.Info, "cat");

            actual.ShouldHaveSameValueAs(true);
        }


        [TestMethod, TestCategory("Unit")]
        public void ShouldLog_False()
        {
            // Arrange
            var logger1 = new Fakes.StubILogger();
            var logger2 = new Fakes.StubILogger();
            var logger3 = new Fakes.StubILogger();
            var componentUnderTest = new MultiLogger(logger1, logger2, logger3);

            logger1.ShouldLogLoggingLevelString = (level, category) => false;
            logger2.ShouldLogLoggingLevelString = (level, category) => false;
            logger3.ShouldLogLoggingLevelString = (level, category) => false;

            // Act
            var actual = componentUnderTest.ShouldLog(LoggingLevel.Info, "cat");

            actual.ShouldHaveSameValueAs(false);
        }

        #endregion


        #region Log
        
        [TestMethod, TestCategory("Unit")]
        public void Log()
        {
            // Arrange
            var logger = new Fakes.StubILogger();
            var componentUnderTest = new MultiLogger(logger);
            SetLog setLog = null;

            logger.ShouldLogLoggingLevelString = (level, category) => true;
            logger.LogLoggingLevelStringFuncOfStringObjectArray = (level, category, message, args) => setLog = new SetLog
            {
                LoggingLevel = level,
                Category = category,
                Message = string.Format(message(), args)
            };

            var expected = new SetLog
            {
                LoggingLevel = LoggingLevel.Info,
                Category = "cat",
                Message = "test arg1"
            };

            // Act
            componentUnderTest.Log(LoggingLevel.Info, "cat", () => "test {0}", "arg1");

            setLog.ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region LogException

        [TestMethod, TestCategory("Unit")]
        public void LogException()
        {
            // Arrange
            var logger = new Fakes.StubILogger();
            var componentUnderTest = new MultiLogger(logger);
            var exception = new Exception("test");
            SetException setException = null;

            logger.ShouldLogLoggingLevelString = (level, category) => true;
            logger.LogExceptionStringException = (category, ex) => setException = new SetException
            {
                Category = category,
                Exception = ex
            };

            var expected = new SetException
            {
                Category = "cat",
                Exception = exception
            };

            // Act
            componentUnderTest.LogException("cat", exception);

            setException.ShouldHaveSameValueAs(expected);
        }

        #endregion
    }
}