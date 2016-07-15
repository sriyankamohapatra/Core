using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Diagnostics
{
    [TestClass]
    public class DebugTextWriterTests : BaseTest
    {
        #region Write

        [TestMethod, TestCategory("Unit")]
        public void Write_String()
        {
            // Arrange
            var componentUnderTest = new DebugTextWriter();
            var output = string.Empty;

            var stubListener = new System.Diagnostics.Fakes.StubConsoleTraceListener
            {
                WriteString = s =>
                {
                    Console.WriteLine("method called");
                    output = s;
                }
            };
            
            Debug.Listeners.Add(stubListener);

            // Act
            componentUnderTest.Write("test");

            // Assert
            output.ShouldHaveSameValueAs("test");
        }

        [TestMethod, TestCategory("Unit")]
        public void Write_Bufferred()
        {
            // Arrange
            var componentUnderTest = new DebugTextWriter();
            var output = string.Empty;

            var buffer = "test".ToCharArray();

            var stubListener = new System.Diagnostics.Fakes.StubConsoleTraceListener
            {
                WriteString = s => { output = s; }
            };

            Debug.Listeners.Add(stubListener);

            // Act
            componentUnderTest.Write(buffer, 1, 2);

            // Assert
            output.ShouldHaveSameValueAs("es");
        }

        #endregion


        #region Encoding

        [TestMethod, TestCategory("Unit")]
        public void Encoding()
        {
            // Arrange
            var componentUnderTest = new DebugTextWriter();

            // Act
            var actual = componentUnderTest.Encoding;

            // Assert
            actual.ShouldHaveSameValueAs(System.Text.Encoding.Default);
        }

        #endregion
    }
}