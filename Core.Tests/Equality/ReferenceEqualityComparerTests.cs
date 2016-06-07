using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Equality
{
    [TestClass]
    public class ReferenceEqualityComparerTests : BaseTest
    {
        #region Test Classes

        public class SimplePoco
        {
        }

        public class SimpleHashCodePoco
        {
            public override int GetHashCode()
            {
                return 1;
            }
        }

        #endregion

        #region Equals

        [TestMethod, TestCategory("Unit")]
        public void Equals_True()
        {
            // Arrange
            var componentUnderTest = new ReferenceEqualityComparer<object>();
            var simplePoco = new SimplePoco();

            // Act
            var actual = componentUnderTest.Equals(simplePoco, simplePoco);

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_False()
        {
            // Arrange
            var componentUnderTest = new ReferenceEqualityComparer<object>();

            // Act
            var actual = componentUnderTest.Equals(new SimplePoco(), new SimplePoco());

            // Assert
            actual.ShouldHaveSameValueAs(false);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_False_Null_Rhs()
        {
            // Arrange
            var componentUnderTest = new ReferenceEqualityComparer<object>();

            // Act
            var actual = componentUnderTest.Equals(new SimplePoco(), null);

            // Assert
            actual.ShouldHaveSameValueAs(false);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_False_Null_Lhs()
        {
            // Arrange
            var componentUnderTest = new ReferenceEqualityComparer<object>();

            // Act
            var actual = componentUnderTest.Equals(null, new SimplePoco());

            // Assert
            actual.ShouldHaveSameValueAs(false);
        }

        [TestMethod, TestCategory("Unit")]
        public void Equals_True_Null()
        {
            // Arrange
            var componentUnderTest = new ReferenceEqualityComparer<object>();

            // Act
            var actual = componentUnderTest.Equals(null, null);

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }

        #endregion


        #region GetHashCode


        [TestMethod, TestCategory("Unit")]
        public void GetHashCodeTest()
        {
            // Arrange
            var componentUnderTest = new ReferenceEqualityComparer<object>();

            // Act
            var actual = componentUnderTest.GetHashCode(new SimpleHashCodePoco());

            // Assert
            actual.ShouldHaveSameValueAs(1);
        }


        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetHashCode_ThrowException()
        {
            // Arrange
            var componentUnderTest = new ReferenceEqualityComparer<object>();

            // Act
            componentUnderTest.GetHashCode(null);
        }

        #endregion
    }
}