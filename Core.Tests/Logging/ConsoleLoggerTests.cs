using System;
using System.IO;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Diagnostics;
using Sfa.Core.Testing;

namespace Sfa.Core.Logging
{
    [TestClass]
    public class ConsoleLoggerTests : BaseTest
    {
        #region LogMessage

        [TestMethod, TestCategory("Unit")]
        public void LogMessage()
        {
            // Arrange
            var componentUnderTest = new ConsoleLogger();
            componentUnderTest.SetLayout((level, category, message) => $"MyLevel:{level} MyCategtory:{category} MyMessage:{message}");

            using (var writer = new StringWriter())
            {
                Console.SetOut(writer);

                // Act
                componentUnderTest.Log(LoggingLevel.Info, "cat1", () => "test");

                // Assert
                writer.ToString().ShouldHaveSameValueAs("MyLevel:Info MyCategtory:cat1 MyMessage:test" + Environment.NewLine);
            }
        }

        #endregion
    }
}