using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Reflection
{
    [TestClass]
    public class MemberInfoExtensionsTests : BaseTest
    {
        #region Test Classes

        public class SimpleAttribute : Attribute
        {
            
        }


        public class ChildSimpleAttribute : SimpleAttribute
        {

        }

        public class SimplePoco
        {
            [Simple]
            public string MyString { get; set; }

            [ChildSimple]
            public string MyOtherString { get; set; }

            [ChildSimple] public string MyOtherStringField;

            public int MyInt { get; set; }

            [ChildSimple]
            public virtual void MyMethod()
            {
            }
        }

        public class ChildPoco : SimplePoco
        {
            public override void MyMethod()
            {
            }
        }

        #endregion


        #region IsDefined

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsDefined_MemberInfoNull()
        {
            // Act
            ((MemberInfo) null).IsDefined<Attribute>();
        }

        [TestMethod, TestCategory("Unit")]
        public void IsDefined_True()
        {
            // Arrange
            var componentUnderTest = typeof(SimplePoco).GetProperty(nameof(SimplePoco.MyString));

            // Act
            var actual = componentUnderTest.IsDefined<SimpleAttribute>();

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }

        [TestMethod, TestCategory("Unit")]
        public void IsDefined_True_InheritedSetTrue_Property()
        {
            // Arrange
            var componentUnderTest = typeof(SimplePoco).GetProperty(nameof(SimplePoco.MyOtherString));

            // Act
            var actual = componentUnderTest.IsDefined<SimpleAttribute>();

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }

        [TestMethod, TestCategory("Unit")]
        public void IsDefined_True_InheritedSetFalse_Property()
        {
            // Arrange
            var componentUnderTest = typeof(SimplePoco).GetProperty(nameof(SimplePoco.MyOtherString));
            
            // Act
            var actual = componentUnderTest.IsDefined<SimpleAttribute>(false);

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }

        [TestMethod, TestCategory("Unit")]
        public void IsDefined_True_InheritedSetTrue_Field()
        {
            // Arrange
            var componentUnderTest = typeof(SimplePoco).GetField(nameof(SimplePoco.MyOtherStringField));

            // Act
            var actual = componentUnderTest.IsDefined<SimpleAttribute>();

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }

        [TestMethod, TestCategory("Unit")]
        public void IsDefined_True_InheritedSetFalse_Field()
        {
            // Arrange
            var componentUnderTest = typeof(SimplePoco).GetField(nameof(SimplePoco.MyOtherStringField));

            // Act
            var actual = componentUnderTest.IsDefined<SimpleAttribute>(false);

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }

        [TestMethod, TestCategory("Unit")]
        public void IsDefined_True_InheritedSetTrue_Method()
        {
            // Arrange
            var componentUnderTest = typeof(ChildPoco).GetMethod(nameof(ChildPoco.MyMethod));

            // Act
            var actual = componentUnderTest.IsDefined<SimpleAttribute>();

            // Assert
            actual.ShouldHaveSameValueAs(true);
        }

        [TestMethod, TestCategory("Unit")]
        public void IsDefined_False_InheritedSetFalse_Method()
        {
            // Arrange
            var componentUnderTest = typeof(ChildPoco).GetMethod(nameof(ChildPoco.MyMethod));

            // Act
            var actual = componentUnderTest.IsDefined<SimpleAttribute>(false);

            // Assert
            actual.ShouldHaveSameValueAs(false);
        }

        #endregion
    }
}