using System;
using System.IO;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Context
{
    [TestClass]
    public class DefaultNetworkContextTests : BaseTest
    {
        #region CurrentDateTime

        [TestMethod]
        public void CurrentDateTime()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var componentUnderTest = new DefaultNetworkContext();
                var expected = new DateTime(2000, 1, 2, 3, 4, 5);
                System.Fakes.ShimDateTime.NowGet = () => expected;
                
                // Act
                var actual = componentUnderTest.CurrentDateTime;

                // Assert
                actual.ShouldHaveSameValueAs(expected);
            }
        }

        #endregion


        #region CurrentDate

        [TestMethod]
        public void CurrentDate()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var componentUnderTest = new DefaultNetworkContext();
                var expected = new DateTime(2000, 1, 2);
                System.Fakes.ShimDateTime.TodayGet = () => expected;

                // Act
                var actual = componentUnderTest.CurrentDate;

                // Assert
                actual.ShouldHaveSameValueAs(expected);
            }
        }

        #endregion


        #region CurrentUtcDateTime

        [TestMethod]
        public void CurrentUtcDateTime()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var componentUnderTest = new DefaultNetworkContext();
                var expected = new DateTime(2000, 1, 2);
                System.Fakes.ShimDateTime.UtcNowGet = () => expected;

                // Act
                var actual = componentUnderTest.CurrentUtcDateTime;

                // Assert
                actual.ShouldHaveSameValueAs(expected);
            }
        }

        #endregion


        #region NewGuid

        [TestMethod]
        public void NewGuid()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var componentUnderTest = new DefaultNetworkContext();
                var expected = Guid.Parse("00000000-0000-0000-0000-000000001234");
                System.Fakes.ShimGuid.NewGuid = () => expected;

                // Act
                var actual = componentUnderTest.NewGuid;

                // Assert
                actual.ShouldHaveSameValueAs(expected);
            }
        }

        #endregion


        #region ToBytes

        [TestMethod]
        public void ToBytes()
        {
            // Arrange
            var componentUnderTest = new DefaultNetworkContext();
            var original = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
            var expected = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
            
            using (var stream = new MemoryStream(original))
            {
                // Act
                var actual = componentUnderTest.ToBytes(stream);

                // Assert
                actual.ShouldHaveSameValueAs(expected);
            }
        }

        #endregion
    }
}