using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core
{
    [TestClass]
    public class EnumTranslatorTests : BaseTest
    {
        #region Test Classes

        public enum TestEnum
        {
            Value0 = 0,
            Value1 = 1,
            Value2 = 2,
            Value4 = 4
        }

        #endregion


        #region Translate

        [TestMethod, TestCategory("Unit")]
        public void Translate_Matches()
        {
            // Arrange
            var expected = TestEnum.Value0;

            // Act
            var actual = EnumTranslator.Translate(0, TestEnum.Value1);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void Translate_Null_UseDefault()
        {
            // Arrange
            var expected = TestEnum.Value1;

            // Act
            var actual = EnumTranslator.Translate(null, TestEnum.Value1);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void Translate_NotTranslated_UseDefault()
        {
            // Arrange
            var expected = TestEnum.Value1;

            // Act
            var actual = EnumTranslator.Translate(99, TestEnum.Value1);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        #endregion
    }
}