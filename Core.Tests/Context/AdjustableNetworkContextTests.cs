using System;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Context
{
    [TestClass]
    public class AdjustableNetworkContextTests : BaseTest
    {
        #region CurrentDateTime

        [TestMethod, TestCategory("Unit")]
        public void CurrentDateTime_DefaultDateTime()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var expected = new DateTime(2000, 1, 2, 3, 4, 5);
                var componentUnderTest = new AdjustableNetworkContext(o => o.AddHours(1), null);
                System.Fakes.ShimDateTime.NowGet = () => expected;

                // Act
                var actual = componentUnderTest.CurrentDateTime;

                // Assert
                actual.ShouldHaveSameValueAs(new DateTime(2000, 1, 2, 4, 4, 5));
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void CurrentDateTime_SetAdditionalDateTimeAdjuster()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var expected = new DateTime(2000, 1, 2, 3, 4, 5);
                var componentUnderTest = new AdjustableNetworkContext(o => o.AddHours(1), null);
                System.Fakes.ShimDateTime.NowGet = () => expected;

                // Act
                componentUnderTest.SetAdditionalDateTimeAdjuster(o => o.AddMinutes(2));
                var actual = componentUnderTest.CurrentDateTime;

                // Assert
                actual.ShouldHaveSameValueAs(new DateTime(2000, 1, 2, 4, 6, 5));
            }
        }

        #endregion


        #region CurrentDate

        [TestMethod, TestCategory("Unit")]
        public void CurrentDate_DefaultDateTime()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var expected = new DateTime(2000, 1, 2);
                var componentUnderTest = new AdjustableNetworkContext(o => o.AddDays(1), null);
                System.Fakes.ShimDateTime.NowGet = () => expected;

                // Act
                var actual = componentUnderTest.CurrentDate;

                // Assert
                actual.ShouldHaveSameValueAs(new DateTime(2000, 1, 3));
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void CurrentDate_SetAdditionalDateTimeAdjuster()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var expected = new DateTime(2000, 1, 2);
                var componentUnderTest = new AdjustableNetworkContext(o => o.AddDays(1), null);
                System.Fakes.ShimDateTime.NowGet = () => expected;

                // Act
                componentUnderTest.SetAdditionalDateTimeAdjuster(o => o.AddMonths(3));
                var actual = componentUnderTest.CurrentDate;

                // Assert
                actual.ShouldHaveSameValueAs(new DateTime(2000, 4, 3));
            }
        }

        #endregion


        #region NewGuid

        [TestMethod, TestCategory("Unit")]
        public void NewGuid()
        {
            // Arrange
            var stubGuidProvider = new Fakes.StubIGuidProvider
            {
                NewGuid = () => Guid.Parse("00000000-0000-0000-0000-000000001234")
            };
            var componentUnderTest = new AdjustableNetworkContext(o => o, stubGuidProvider);

            // Act
            var actual = componentUnderTest.NewGuid;

            // Assert
            actual.ShouldHaveSameValueAs(Guid.Parse("00000000-0000-0000-0000-000000001234"));
        }

        #endregion
    }
}