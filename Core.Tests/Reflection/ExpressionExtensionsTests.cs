using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Reflection
{
    [TestClass]
    public class ExpressionExtensionsTests : BaseTest
    {
        #region Test Classes

        public class SimplePoco
        {
            public string MyStringField;
            public int _myInt;
            public DateTime dateTime;
            public byte m_byte;
            public object myObject;

            public string MyStringProperty { get; set; }

            public int MyInt
            {
                get { return _myInt; }
                set { _myInt = value; }
            }

            public DateTime DateTime
            {
                get { return dateTime; }
                set { dateTime = value; }
            }

            public byte Byte
            {
                get { return m_byte; }
                set { m_byte = value; }
            }

            public object Object
            {
                get { return myObject; }
                set { myObject = value; }
            }
        }

        #endregion


        #region EnsureProperty

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureProperty_NoExpressionProvided()
        {
            // Act
            ExpressionExtensions.EnsureProperty((Expression<Func<string, string>>)null);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureProperty_NotMemberExpression()
        {
            // Act
            ExpressionExtensions.EnsureProperty<string, string>(s => s);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void EnsureProperty_NotPropertyInfo()
        {
            // Act
            ExpressionExtensions.EnsureProperty<SimplePoco, string>(s => s.MyStringField);
        }

        [TestMethod, TestCategory("Unit")]
        public void EnsureProperty()
        {
            // Act
            var actual = ExpressionExtensions.EnsureProperty<SimplePoco, string>(s => s.MyStringProperty);

            // Assert
            Assert.IsNotNull(actual); // Not really sure how to test this?
        }

        #endregion


        #region GetPropertyName


        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPropertyName_NoExpressionProvided()
        {
            // Act
            ExpressionExtensions.GetPropertyName((Expression<Func<string, string>>)null);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPropertyName_NotMemberExpression()
        {
            // Act
            ExpressionExtensions.GetPropertyName<string, string>(s => s);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPropertyName_NotPropertyInfo()
        {
            // Act
            ExpressionExtensions.GetPropertyName<SimplePoco, string>(s => s.MyStringField);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetPropertyName()
        {
            // Assert
            var expected = nameof(SimplePoco.MyStringProperty);

            // Act
            var actual = ExpressionExtensions.GetPropertyName<SimplePoco, string>(s => s.MyStringProperty);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region GetProperty


        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetProperty_NoExpressionProvided()
        {
            // Act
            ExpressionExtensions.GetProperty((Expression<Func<string, string>>)null);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetProperty_NotMemberExpression()
        {
            // Act
            ExpressionExtensions.GetProperty<string, string>(s => s);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetProperty_NotPropertyInfo()
        {
            // Act
            ExpressionExtensions.GetProperty<SimplePoco, string>(s => s.MyStringField);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetProperty()
        {
            // Assert
            var expected = typeof(SimplePoco).GetProperty(nameof(SimplePoco.MyStringProperty));

            // Act
            var actual = ExpressionExtensions.GetProperty<SimplePoco, string>(s => s.MyStringProperty);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region GetPropertyBackingField
        
        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPropertyBackingField_NoExpressionProvided()
        {
            // Act
            ExpressionExtensions.GetPropertyBackingField((Expression<Func<string, string>>)null);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPropertyBackingField_NotMemberExpression()
        {
            // Act
            ExpressionExtensions.GetPropertyBackingField<string, string>(s => s);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPropertyBackingField_NotPropertyInfo()
        {
            // Act
            ExpressionExtensions.GetPropertyBackingField<SimplePoco, string>(s => s.MyStringField);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetPropertyBackingField_LowercasedUnderscore()
        {
            // Assert
            var expected = typeof(SimplePoco).GetField(nameof(SimplePoco._myInt));

            // Act
            var actual = ExpressionExtensions.GetPropertyBackingField<SimplePoco, int>(s => s.MyInt);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetPropertyBackingField_Lowercased()
        {
            // Assert
            var expected = typeof(SimplePoco).GetField(nameof(SimplePoco.dateTime));

            // Act
            var actual = ExpressionExtensions.GetPropertyBackingField<SimplePoco, DateTime>(s => s.DateTime);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetPropertyBackingField_LowercasedUnderscoreLeadingM()
        {
            // Assert
            var expected = typeof(SimplePoco).GetField(nameof(SimplePoco.m_byte));

            // Act
            var actual = ExpressionExtensions.GetPropertyBackingField<SimplePoco, byte>(s => s.Byte);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetPropertyBackingField_AutoBackingField()
        {
            // Assert
            var expected = typeof(SimplePoco).GetField("<MyStringProperty>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

            // Act
            var actual = ExpressionExtensions.GetPropertyBackingField<SimplePoco, string>(s => s.MyStringProperty);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPropertyBackingField_UnexpectedNamingConventionUsedOrMissingBackingField()
        {
            // Act
            ExpressionExtensions.GetPropertyBackingField<SimplePoco, object>(s => s.Object);
        }

        #endregion
    }
}