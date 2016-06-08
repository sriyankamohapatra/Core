using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Reflection
{
    [TestClass]
    public class ObjectExtensionsTests : BaseTest
    {
        #region Test Classes

        public class SimplePoco
        {
            public string MyPublicField = "";
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
    }
}