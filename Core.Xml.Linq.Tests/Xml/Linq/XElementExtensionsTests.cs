using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Xml.Linq
{
    [TestClass]
    public class XElementExtensionsTests : BaseTest
    {
        #region CopyWithoutNamespaces

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyWithoutNamespaces_NullPassed()
        {
            // Act
            XElementExtensions.CopyWithoutNamespaces(null);
        }

        [TestMethod, TestCategory("Unit")]
        public void CopyWithoutNamespaces()
        {
            // Arrange
            var xml = "<xml xmlns:xs=\"www.madeup.com\"><xs:parent><xs:child attr1=\"some value\"/></xs:parent></xml>";
            var expected = "<xml><parent><child attr1=\"some value\"></child></parent></xml>";
            var doc = XDocument.Parse(xml);

            // Act
            var actual = doc.Root.CopyWithoutNamespaces().ToString(SaveOptions.DisableFormatting);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region LowerCaseAllElementNames

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LowerCaseAllElementNames_NullPassed()
        {
            // Act
            XElementExtensions.LowerCaseAllElementNames(null);
        }

        [TestMethod, TestCategory("Unit")]
        public void LowerCaseAllElementNames()
        {
            // Arrange
            var xml = "<xMl xmlns:xs=\"www.madeup.com\"><xs:parent><xs:CHILD aTTr1=\"some value\"/></xs:parent></xMl>";
            var expected = "<xml xmlns:xs=\"www.madeup.com\"><xs:parent><xs:child aTTr1=\"some value\" /></xs:parent></xml>";
            var doc = XDocument.Parse(xml);

            // Act
            var actual = doc.Root.LowerCaseAllElementNames().ToString(SaveOptions.DisableFormatting);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region LowerCaseAllAttributeNames

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LowerCaseAllAttributeNames_NullPassed()
        {
            // Act
            XElementExtensions.LowerCaseAllAttributeNames(null);
        }

        [TestMethod, TestCategory("Unit")]
        public void LowerCaseAllAttributeNames()
        {
            // Arrange
            var xml = "<xMl xmlns:xs=\"www.madeup.com\"><xs:parent><xs:CHILD aTTr1=\"some value\"/></xs:parent></xMl>";
            var expected = "<xMl xmlns:xs=\"www.madeup.com\"><xs:parent><xs:CHILD attr1=\"some value\" /></xs:parent></xMl>";
            var doc = XDocument.Parse(xml);

            // Act
            var actual = doc.Root.LowerCaseAllAttributeNames().ToString(SaveOptions.DisableFormatting);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        #endregion
    }
}