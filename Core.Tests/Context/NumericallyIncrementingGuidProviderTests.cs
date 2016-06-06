using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Context
{
    [TestClass]
    public class NumericallyIncrementingGuidProviderTests : BaseTest
    {
        #region NewGuid

        [TestMethod, TestCategory("Unit")]
        public void NewGuid_DefaultSeed()
        {
            // Arrange
            var componentUnderTest = new NumericallyIncrementingGuidProvider();

            // Act / Assert
            componentUnderTest.NewGuid().ShouldHaveSameValueAs(Guid.Parse("00000000-0000-0000-0000-000000000000"));
            componentUnderTest.NewGuid().ShouldHaveSameValueAs(Guid.Parse("00000000-0000-0000-0000-000000000001"));
            componentUnderTest.NewGuid().ShouldHaveSameValueAs(Guid.Parse("00000000-0000-0000-0000-000000000002"));
        }

        [TestMethod, TestCategory("Unit")]
        public void NewGuid_UsingSeed()
        {
            // Arrange
            var componentUnderTest = new NumericallyIncrementingGuidProvider();

            // Act
            componentUnderTest.SetSeedValue(1000);

            // Assert
            componentUnderTest.NewGuid().ShouldHaveSameValueAs(Guid.Parse("00000000-0000-0000-0000-000000001000"));
            componentUnderTest.NewGuid().ShouldHaveSameValueAs(Guid.Parse("00000000-0000-0000-0000-000000001001"));
            componentUnderTest.NewGuid().ShouldHaveSameValueAs(Guid.Parse("00000000-0000-0000-0000-000000001002"));
        }

        #endregion
    }
}