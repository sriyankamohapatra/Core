using System;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Context
{
    [TestClass]
    public class DefaultGuidProviderTests : BaseTest
    {
        #region NewGuid

        [TestMethod, TestCategory("Unit")]
        public void NewGuid()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var componentUnderTest = new DefaultGuidProvider();
                var expected = Guid.Parse("00000000-0000-0000-0000-000000001234");
                System.Fakes.ShimGuid.NewGuid = () => expected;

                // Act
                var actual = componentUnderTest.NewGuid();

                // Assert
                actual.ShouldHaveSameValueAs(expected);
            }
        }

        #endregion
    }
}