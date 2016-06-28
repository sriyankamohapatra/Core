using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Logging
{
    [TestClass]
    public class TraceLoggerTests : BaseTest
    {
        #region LogMessage

        [TestMethod, TestCategory("Unit")]
        public void LogMessage()
        {
            // Arrange
            var componentUnderTest = new TraceLogger();
            componentUnderTest.SetLayout((level, category, message) => $"MyLevel:{level} MyCategtory:{category} MyMessage:{message}");

            using (var writer = new StringWriter())
            {
                Console.SetOut(writer);
                Trace.Listeners.Add(new ConsoleTraceListener());

                // Act
                componentUnderTest.Log(LoggingLevel.Info, "cat1", () => "test");

                // Assert
                writer.ToString().ShouldHaveSameValueAs("MyLevel:Info MyCategtory:cat1 MyMessage:test" + Environment.NewLine);
            }
        }

        #endregion
    }
}