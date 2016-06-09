using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Reflection
{
    [TestClass]
    public class CustomAttributeProviderExtensionsTests : BaseTest
    {
        #region Test Classes

        public class SimpleAttribute : Attribute
        {
            public int MyValue { get; set; }
        }

        public class ChildAttribute : SimpleAttribute
        {
        }

        public class OtherAttribute : Attribute
        {
        }

        public class SimplePoco
        {
            [Child]
            public virtual string MyStringWithAttribute { get; set; }

            public virtual string MyStringWithoutAttribute { get; set; }

            [Child]
            [Simple]
            public virtual string MyStringWithMultipleAttributes { get; set; }

            [Child]
            public virtual void MyMethod()
            {
            }

            [Child(MyValue = 1)]
            [Simple(MyValue = 2)]
            public virtual void MyMethodWithMultipleAttributes()
            {
            }
        }

        public class ChildPoco : SimplePoco
        {
            public override string MyStringWithAttribute { get; set; }

            public override string MyStringWithoutAttribute { get; set; }

            [Child]
            public override string MyStringWithMultipleAttributes { get; set; }
            
            public override void MyMethod()
            {
            }

            [Simple(MyValue = 3)]
            public override void MyMethodWithMultipleAttributes()
            {
            }
        }

        #endregion


        #region HasAttribute

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasAttribute_InstanceNull()
        {
            // Act
            CustomAttributeProviderExtensions.HasAttribute<Attribute>(null);
        }

        [TestMethod, TestCategory("Unit")]
        public void HasAttribute_False_Property_InheritenceNotOnProperties()
        {
            // Arrange
            var provider = typeof(ChildPoco).GetProperty(nameof(ChildPoco.MyStringWithAttribute));

            // Act
            var actual = CustomAttributeProviderExtensions.HasAttribute<SimpleAttribute>(provider);

            // Assert
            actual.ShouldHaveSameValueAs(false);
        }

        [TestMethod, TestCategory("Unit")]
        public void HasAttribute_False_Property()
        {
            // Arrange
            var provider = typeof(ChildPoco).GetProperty(nameof(ChildPoco.MyStringWithoutAttribute));

            // Act
            var actual = CustomAttributeProviderExtensions.HasAttribute<SimpleAttribute>(provider);

            // Assert
            actual.ShouldHaveSameValueAs(false);
        }

        [TestMethod, TestCategory("Unit")]
        public void HasAttribute_False_NoInherit_Property()
        {
            // Arrange
            var provider = typeof(ChildPoco).GetProperty(nameof(ChildPoco.MyStringWithAttribute));

            // Act
            var actual = CustomAttributeProviderExtensions.HasAttribute<SimpleAttribute>(provider, false);

            // Assert
            actual.ShouldHaveSameValueAs(false);
        }

        #endregion


        #region GetAttributes

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetAttributes_InstanceNull_Property()
        {
            // Act
            CustomAttributeProviderExtensions.GetAttributes<Attribute>(null);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetAttributes_Property()
        {
            // Arrange
            var provider = typeof(ChildPoco).GetProperty(nameof(ChildPoco.MyStringWithMultipleAttributes));
            var expected = new[] { new ChildAttribute() };

            // Act
            var actual = CustomAttributeProviderExtensions.GetAttributes<SimpleAttribute>(provider);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetAttributes_NoMatchingTypes_Property()
        {
            // Arrange
            var provider = typeof(ChildPoco).GetProperty(nameof(ChildPoco.MyStringWithMultipleAttributes));
            var expected = new OtherAttribute[] {  };

            // Act
            var actual = CustomAttributeProviderExtensions.GetAttributes<OtherAttribute>(provider);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetAttributes_NoInherit_Method()
        {
            // Arrange
            var provider = typeof(ChildPoco).GetMethod(nameof(ChildPoco.MyMethod));
            var expected = new SimpleAttribute[] {  };

            // Act
            var actual = CustomAttributeProviderExtensions.GetAttributes<SimpleAttribute>(provider, false);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetAttributes_Inherit_Method()
        {
            // Arrange
            var provider = typeof(ChildPoco).GetMethod(nameof(ChildPoco.MyMethod));
            var expected = new SimpleAttribute[] { new ChildAttribute() };

            // Act
            var actual = CustomAttributeProviderExtensions.GetAttributes<SimpleAttribute>(provider);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region GetFirstAttribute

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetAttribute_InstanceNull_Property()
        {
            // Act
            CustomAttributeProviderExtensions.GetAttribute<Attribute>(null);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetAttribute_Property()
        {
            // Arrange
            var provider = typeof(ChildPoco).GetProperty(nameof(ChildPoco.MyStringWithMultipleAttributes));
            var expected = new ChildAttribute();

            // Act
            var actual = CustomAttributeProviderExtensions.GetAttribute<SimpleAttribute>(provider);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetAttribute_NoMatchingTypes_Property()
        {
            // Arrange
            var provider = typeof(ChildPoco).GetProperty(nameof(ChildPoco.MyStringWithMultipleAttributes));

            // Act
            CustomAttributeProviderExtensions.GetAttribute<OtherAttribute>(provider);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetAttribute_Inherit_Method_IndexSet()
        {
            // Arrange
            var provider = typeof(ChildPoco).GetMethod(nameof(ChildPoco.MyMethodWithMultipleAttributes));
            var expected = new ChildAttribute()
            {
                MyValue = 1
            };

            // Act
            var actual = CustomAttributeProviderExtensions.GetAttribute<SimpleAttribute>(provider, index:1);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetAttribute_Inherit_Method_IndexSetButOfOufRange()
        {
            // Arrange
            var provider = typeof(ChildPoco).GetMethod(nameof(ChildPoco.MyMethodWithMultipleAttributes));

            // Act
            CustomAttributeProviderExtensions.GetAttribute<SimpleAttribute>(provider, index: 99);
        }

        #endregion
    }
}