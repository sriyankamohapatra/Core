using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Reflection
{
    [TestClass]
    public class ObjectExtensionsTests : BaseTest
    {
        #region Test Classes

        public class SimpleAttribute : Attribute
        {
        }

        public class SimplePoco
        {
            public string MyPublicField = "";

            [SimpleAttribute]
            public int MyIntField = 2;

            private DateTime? MyNullableDateTimeField;

            public SimplePoco(DateTime? dateTime = null)
            {
                MyNullableDateTimeField = dateTime;
            }
        }

        public class RecursivePoco
        {
            public string MyString { get; set; }

            public RecursivePoco Child { get; set; }
        }

        #endregion


        #region Life Cycle

        protected override IEnumerable<Assembly> AssembliesWithTypesToPerformFieldValueEqualityOn
        {
            get { yield return typeof(ObjectExtensionsTests).Assembly; }
        }

        #endregion


        #region GetPropertyInfo

        [TestMethod, TestCategory("Unit")]
        public void GetPropertyInfo()
        {
            // Arrange
            var expected = typeof(string).GetProperty("Length");

            // Act
            var actual = "".GetPropertyInfo(s => s.Length);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPropertyInfo_NoExpressionProvided()
        {
            // Act
            "".GetPropertyInfo((Expression<Func<string, string>>)null);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPropertyInfo_NotMemberExpression()
        {
            // Act
            "".GetPropertyInfo(s => s);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPropertyInfo_NotPropertyInfo()
        {
            // Act
            new SimplePoco().GetPropertyInfo(s => s.MyPublicField);
        }

        #endregion


        #region SetPropertyValue

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetPropertyValue_NoExpressionProvided()
        {
            // Act
            "".SetPropertyValue((Expression<Func<string, string>>)null, "");
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void SetPropertyValue_NotMemberExpression()
        {
            // Act
            "".SetPropertyValue(s => s, "");
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void SetPropertyValue_NotAProperty()
        {
            // Act
            new SimplePoco().SetPropertyValue(s => s.MyPublicField, "");
        }

        [TestMethod, TestCategory("Unit")]
        public void SetPropertyValue_TopLevel_Null()
        {
            // Arrange
            var componentUnderTest = new RecursivePoco
            {
                MyString = "x"
            };

            var expected = new RecursivePoco
            {
                MyString = null
            };

            // Act
            componentUnderTest.SetPropertyValue(o => o.MyString, null);

            // Assert
            componentUnderTest.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void SetPropertyValue_TopLevel_WithValue()
        {
            // Arrange
            var componentUnderTest = new RecursivePoco
            {
                MyString = "x"
            };

            var expected = new RecursivePoco
            {
                MyString = "a"
            };

            // Act
            componentUnderTest.SetPropertyValue(o => o.MyString, "a");

            // Assert
            componentUnderTest.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void SetPropertyValue_Nested_WithValue()
        {
            // Arrange
            var componentUnderTest = new RecursivePoco
            {
                MyString = "x",
                Child = new RecursivePoco
                {
                    Child = new RecursivePoco
                    {
                        Child = new RecursivePoco
                        {
                            MyString = "original"
                        }
                    }
                }
            };

            var expected = new RecursivePoco
            {
                MyString = "x",
                Child = new RecursivePoco
                {
                    Child = new RecursivePoco
                    {
                        Child = new RecursivePoco
                        {
                            MyString = "changed"
                        }
                    }
                }
            };

            // Act
            componentUnderTest.SetPropertyValue(o => o.Child.Child.Child.MyString, "changed");

            // Assert
            componentUnderTest.ShouldHaveSameValueAs(expected);
        }


        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetPropertyValue_Nested_NullInChain()
        {
            // Arrange
            var componentUnderTest = new RecursivePoco
            {
                MyString = "x",
                Child = new RecursivePoco
                {
                    Child = new RecursivePoco()
                }
            };

            // Act
            componentUnderTest.SetPropertyValue(o => o.Child.Child.Child.MyString, "changed");
        }

        #endregion


        #region GetAllFieldsWithoutAttribute
        
        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetAllFieldsWithoutAttribute_NullInstance()
        {
            // Act
            ((SimplePoco) null).GetAllFieldsWithoutAttribute<SimpleAttribute>();
        }

        [TestMethod, TestCategory("Unit")]
        public void GetAllFieldsWithoutAttribute()
        {
            // Arrange
            var type = typeof(SimplePoco);
            var expected = new List<FieldInfo>
            {
                type.GetField("MyPublicField"),
                type.GetField("MyNullableDateTimeField", BindingFlags.Instance | BindingFlags.NonPublic)
            };

            // Act
            var actual = new SimplePoco().GetAllFieldsWithoutAttribute<SimpleAttribute>();

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region GetPrivateField



        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPrivateField_NullType()
        {
            // Act
            ObjectExtensions.GetPrivateField(null, null);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPrivateField_NullFieldName()
        {
            // Act
            typeof(object).GetPrivateField(null);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPrivateField_FieldNameNotMatched()
        {
            // Act
            typeof(object).GetPrivateField("sdfsdf");
        }

        [TestMethod, TestCategory("Unit")]
        public void GetPrivateField()
        {
            // Act
            var actual = typeof(SimplePoco).GetPrivateField("MyNullableDateTimeField");

            // Assert
            Assert.IsNotNull(actual);
        }


        #endregion


        #region GetPrivateFieldValue



        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPrivateFieldValue_NullType()
        {
            // Act
            ObjectExtensions.GetPrivateFieldValue(null, null);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPrivateFieldValue_NullFieldName()
        {
            // Act
            typeof(object).GetPrivateFieldValue(null);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPrivateFieldValue_FieldNameNotMatched()
        {
            // Act
            typeof(object).GetPrivateFieldValue("sdfsdf");
        }

        [TestMethod, TestCategory("Unit")]
        public void GetPrivateFieldValue()
        {
            // Arrange
            var componentUnderTest = new SimplePoco(new DateTime(2000, 1, 1));

            // Act
            var actual = componentUnderTest.GetPrivateFieldValue("MyNullableDateTimeField");

            // Assert
            actual.ShouldHaveSameValueAs(new DateTime(2000, 1, 1));
        }


        #endregion


        #region SetPrivateFieldValue



        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetPrivateFieldValue_NullType()
        {
            // Act
            ObjectExtensions.SetPrivateFieldValue<SimplePoco>(null, null, null);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetPrivateFieldValue_NullFieldName()
        {
            // Act
            typeof(object).SetPrivateFieldValue(null, null);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void SetPrivateFieldValue_FieldNameNotMatched()
        {
            // Act
            typeof(object).SetPrivateFieldValue("sdfsdf", null);
        }

        [TestMethod, TestCategory("Unit")]
        public void SetPrivateFieldValue()
        {
            // Arrange
            var componentUnderTest = new SimplePoco();

            var expected = new SimplePoco(new DateTime(2000, 1, 1));

            // Act
            var actual = componentUnderTest.SetPrivateFieldValue("MyNullableDateTimeField", new DateTime(2000, 1, 1));

            // Assert
            componentUnderTest.ShouldHaveSameValueAs(expected);
            actual.ShouldHaveSameValueAs(expected);
        }


        #endregion
    }
}