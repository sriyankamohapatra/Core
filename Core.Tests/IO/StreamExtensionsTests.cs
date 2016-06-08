using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.IO
{
    [TestClass]
    public class StreamExtensionsTests : BaseTest
    {
        #region ToByteArray

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToByteArray_NullStream()
        {
            // Act
            ((Stream) null).ToByteArray();
        }

        [TestMethod, TestCategory("Unit")]
        public void ToByteArray()
        {
            // Arrange
            var expected = Encoding.Default.GetBytes("test");
            using(var stream = GenerateStreamFromString("test"))
            {
                // Act
                var actual = stream.ToByteArray();

                // Assert
                actual.ShouldHaveSameValueAs(expected);
            }
        }

        public Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        #endregion
    }
}