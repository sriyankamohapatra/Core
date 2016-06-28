using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core
{
    [TestClass]
    public class EnumAttributeHelperTests : BaseTest
    {
        #region Test Classes

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        public class SimpleAttribute : Attribute
        {
            public int MyInt { get; set; }

            public string MyString { get; set; }
        }

        public enum TestEnum
        {
            [Simple(MyInt = 1, MyString = "a")]
            [Simple(MyInt = 2, MyString = "b")]
            Value0 = 0,

            Value1 = 1,

            [Simple(MyInt = 3, MyString = "c")]
            Value2 = 2,

            [Simple(MyInt = 4, MyString = "d")]
            [Simple(MyInt = 5, MyString = null)]
            Value3 = 3
        }

        public struct SimpleStruct
        {
        }

        #endregion


        #region GetEnumValueAttribute

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetEnumValueAttribute_NullValue()
        {
            // Act
            EnumAttributeHelper.GetEnumValueAttribute<SimpleAttribute>(null);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetEnumValueAttribute()
        {
            // Arrange
            var expected = new SimpleAttribute {MyInt = 1, MyString = "a"};

            // Act
            var actual = EnumAttributeHelper.GetEnumValueAttribute<SimpleAttribute>(TestEnum.Value0);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetEnumValueAttribute_NoAttribute()
        {
            // Act
            var actual = EnumAttributeHelper.GetEnumValueAttribute<SimpleAttribute>(TestEnum.Value1);

            // Assert
            actual.ShouldHaveSameValueAs(null);
        }

        #endregion


        #region GetEnumFromPropertyValue


        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetEnumFromPropertyValue_NullPropertyExpression()
        {
            // Act
            EnumAttributeHelper.GetEnumFromPropertyValue<TestEnum, SimpleAttribute, int>(1, null);
        }


        [TestMethod, TestCategory("Unit")]
        public void GetEnumFromPropertyValue_ValueGiven()
        {
            // Arrange
            var expected = TestEnum.Value2;

            // Act
            var actual = EnumAttributeHelper.GetEnumFromPropertyValue<TestEnum, SimpleAttribute, int>(3, o => o.MyInt);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }


        [TestMethod, TestCategory("Unit")]
        public void GetEnumFromPropertyValue_NullValueGiven()
        {
            // Arrange
            var expected = TestEnum.Value3;

            // Act
            var actual = EnumAttributeHelper.GetEnumFromPropertyValue<TestEnum, SimpleAttribute, string>(null, o => o.MyString);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region GetPropertyValue


        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPropertyValue_NotAnEnum()
        {
            // Act
            EnumAttributeHelper.GetPropertyValue<SimpleStruct, SimpleAttribute>(new SimpleStruct(), null);
        }


        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPropertyValue_NullPropertyExpression()
        {
            // Act
            EnumAttributeHelper.GetPropertyValue<TestEnum, SimpleAttribute>(TestEnum.Value3, null);
        }


        [TestMethod, TestCategory("Unit")]
        public void GetPropertyValue()
        {
            // Act
            var actual = EnumAttributeHelper.GetPropertyValue<TestEnum, SimpleAttribute>(TestEnum.Value3, attribute => attribute.MyString);

            // Assert
            actual.ShouldHaveSameValueAs("d");
        }


        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(NotImplementedException))]
        public void GetPropertyValue_NoAttributeDefined()
        {
            // Act
            EnumAttributeHelper.GetPropertyValue<TestEnum, SimpleAttribute>(TestEnum.Value1, attribute => attribute.MyString);
        }


        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPropertyValue_String_NotAnEnum()
        {
            // Act
            EnumAttributeHelper.GetPropertyValue<SimpleStruct, SimpleAttribute, string>(new SimpleStruct(), null);
        }


        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPropertyValue_String_NullPropertyExpression()
        {
            // Act
            EnumAttributeHelper.GetPropertyValue<TestEnum, SimpleAttribute, string>(TestEnum.Value3, null);
        }


        [TestMethod, TestCategory("Unit")]
        public void GetPropertyValue_String()
        {
            // Act
            var actual = EnumAttributeHelper.GetPropertyValue<TestEnum, SimpleAttribute, string>(TestEnum.Value3, attribute => attribute.MyString);

            // Assert
            actual.ShouldHaveSameValueAs("d");
        }


        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(NotImplementedException))]
        public void GetPropertyValue_String_NoAttributeDefined()
        {
            // Act
            EnumAttributeHelper.GetPropertyValue<TestEnum, SimpleAttribute, string>(TestEnum.Value1, attribute => attribute.MyString);
        }

        #endregion
    }
}