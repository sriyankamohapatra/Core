using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Reflection
{
    [TestClass]
    public class TypeExtensionsTests : BaseTest
    {
        #region Test Classes

        public class SimpleAttribute : Attribute
        {
        }

        public class SimplePoco
        {
            private string _myString;
            [SimpleAttribute]
            private int _myInt;
            private DateTime? _myNullableDateTime;

            public string MyString
            {
                get { return _myString; }
                set { _myString = value; }
            }

            public int Myint
            {
                get { return _myInt; }
                set { _myInt = value; }
            }

            public DateTime? MyDateTime
            {
                get { return _myNullableDateTime; }
                set { _myNullableDateTime = value; }
            }
        }

        public class SimpleChildPoco : SimplePoco
        {
            private string _myOtherString;

            public string MyOtherString
            {
                get { return _myOtherString; }
                set { _myOtherString = value; }
            }
        }

        #endregion


        #region GetAllFields

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetAllFields_MissingType()
        {
            // Act
            ((Type) null).GetAllFields();
        }

        [TestMethod, TestCategory("Unit")]
        public void GetAllFields()
        {
            // Arrange
            var componentUnderTest = typeof(SimpleChildPoco);
            var simplePocoType = typeof(SimplePoco);
            var bindingFlags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic;
            var expected = new List<FieldInfo>
            {
                componentUnderTest.GetField("_myOtherString", bindingFlags),
                simplePocoType.GetField("_myString", bindingFlags),
                simplePocoType.GetField("_myInt", bindingFlags),
                simplePocoType.GetField("_myNullableDateTime", bindingFlags)
            };

            // Act
            var actual = componentUnderTest.GetAllFields();

            // Assert
            actual.ShouldHaveSameValuesAs(expected);
        }

        #endregion


        #region GetAllFieldsWithoutAttribute

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetAllFieldsWithoutAttribute_MissingType()
        {
            // Act
            ((Type)null).GetAllFieldsWithoutAttribute<SimpleAttribute>();
        }

        [TestMethod, TestCategory("Unit")]
        public void GetAllFieldsWithoutAttribute()
        {
            // Arrange
            var componentUnderTest = typeof(SimpleChildPoco);
            var simplePocoType = typeof(SimplePoco);
            var bindingFlags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic;
            var expected = new List<FieldInfo>
            {
                componentUnderTest.GetField("_myOtherString", bindingFlags),
                simplePocoType.GetField("_myString", bindingFlags),
                simplePocoType.GetField("_myNullableDateTime", bindingFlags)
            };

            // Act
            var actual = componentUnderTest.GetAllFieldsWithoutAttribute<SimpleAttribute>();

            // Assert
            actual.ShouldHaveSameValuesAs(expected);
        }

        #endregion
    }
}