using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core
{
    [TestClass]
    public class EnumAttributeMapTests : BaseTest
    {
        #region Test Classes

        public class Simple1Attribute : Attribute
        {
            public int MyInt { get; set; }
        }

        public class ChildSimple1Attribute : Simple1Attribute
        {
        }

        public class Simple2Attribute : Attribute
        {
        }

        public enum SimpleEnum
        {
            Default = 0
        }

        #endregion


        #region Constructors

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullEnumValue()
        {
            // Act
            new EnumAttributeMap(null, null);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullAttributes()
        {
            // Act
            new EnumAttributeMap(SimpleEnum.Default, null);
        }

        [TestMethod, TestCategory("Unit")]
        public void Constructor()
        {
            // Act
            var actual = new EnumAttributeMap(SimpleEnum.Default, new[] { new Simple1Attribute() });

            // Assert
            actual.Attributes.ShouldHaveSameValueAs(new[] {new Simple1Attribute()});
            actual.EnumValue.ShouldHaveSameValueAs(SimpleEnum.Default);
        }

        #endregion


        #region GetFirstAttribute
        
        [TestMethod, TestCategory("Unit")]
        public void GetFirstAttribute_NoMatchingAttributes()
        {
            // Arrange
            var componentUnderTest = new EnumAttributeMap(SimpleEnum.Default, new[] { new Simple1Attribute() });

            // Act
            var actual = componentUnderTest.GetFirstAttribute<ChildSimple1Attribute>();

            // Assert
            actual.ShouldHaveSameValueAs(null);
        }
        
        [TestMethod, TestCategory("Unit")]
        public void GetFirstAttribute_MatchingAttributes()
        {
            // Arrange
            var componentUnderTest = new EnumAttributeMap(SimpleEnum.Default, new[] { new Simple1Attribute {MyInt = 1}, new Simple1Attribute { MyInt = 2 } });

            // Act
            var actual = componentUnderTest.GetFirstAttribute<Simple1Attribute>();

            // Assert
            actual.ShouldHaveSameValueAs(new Simple1Attribute { MyInt = 1 });
        }

        #endregion


        #region GetFirstAttribute
        
        [TestMethod, TestCategory("Unit")]
        public void GetAttributes_MatchingAttributes()
        {
            // Arrange
            var componentUnderTest = new EnumAttributeMap(SimpleEnum.Default, new Attribute[] { new Simple1Attribute { MyInt = 1 }, new Simple1Attribute { MyInt = 2 }, new ChildSimple1Attribute(), new Simple2Attribute()  });

            // Act
            var actual = componentUnderTest.GetAttributes<Simple1Attribute>();

            // Assert
            actual.ShouldHaveSameValuesAs(new Attribute[] { new Simple1Attribute { MyInt = 1 }, new Simple1Attribute { MyInt = 2 }, new ChildSimple1Attribute() });
        }

        #endregion
    }
}