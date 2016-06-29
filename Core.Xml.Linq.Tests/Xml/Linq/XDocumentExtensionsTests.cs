using System;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Xml.Linq
{
    [TestClass]
    public class XDocumentExtensionsTests : BaseTest
    {
        #region Fields

        // 123 elements for performance testing of lower casing.
        private string _largeTextXml = @"<testLargeObject>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
  <Element Attribute=""blah"">Value</Element>
</testLargeObject>";

        #endregion


        #region LowerCaseAllElementNames

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LowerCaseAllElementNames_NullPassed()
        {
            // Act
            XDocumentExtensions.LowerCaseAllElementNames(null);
        }

        [TestMethod, TestCategory("Unit")]
        public void XmlLowerCaseNameExt_Success()
        {
            // Arrange
            var rawXml = "<xml><StartDate xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" BoB=\"tim\"></StartDate></xml>";
            var expected = "<xml><startdate xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" BoB=\"tim\"></startdate></xml>";

            // Act
            var doc = XDocument.Parse(rawXml);
            doc.LowerCaseAllElementNames();

            // Assert
            Assert.AreEqual(expected, doc.ToString(SaveOptions.DisableFormatting));
        }

        #endregion


        #region LowerCaseAllAttributeNames

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LowerCaseAllAttributeNames_NullPassed()
        {
            // Act
            XDocumentExtensions.LowerCaseAllAttributeNames(null);
        }

        [TestMethod, TestCategory("Unit")]
        public void LowerCaseAllAttributeNames()
        {
            // Arrange
            var rawXml = "<xml><startdate xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" BoB=\"tim\"></startdate></xml>";
            var expected = "<xml><startdate xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" bob=\"tim\"></startdate></xml>";

            // Act
            var doc = XDocument.Parse(rawXml);
            doc.LowerCaseAllAttributeNames();

            // Assert
            Assert.AreEqual(expected, doc.ToString(SaveOptions.DisableFormatting));
        }

        #endregion


        #region LowerCaseAllElementAndAttributeNames

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LowerCaseAllElementAndAttributeNames_NullPassed()
        {
            // Act
            XDocumentExtensions.LowerCaseAllElementAndAttributeNames(null);
        }

        [TestMethod, TestCategory("Unit")]
        public void LowerCaseAllElementAndAttributeNames()
        {
            // Arrange
            var rawXml = "<xml><StartDate xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" BoB=\"Tim\">SomeContent</StartDate></xml>";
            var expected = "<xml><startdate xsi:nil=\"true\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" bob=\"Tim\">SomeContent</startdate></xml>";

            // Act
            var doc = XDocument.Parse(rawXml);
            doc.LowerCaseAllElementAndAttributeNames();

            // Assert
            Assert.AreEqual(expected, doc.ToString(SaveOptions.DisableFormatting));
        }

        #endregion
        

        #region LowerCasePerformance

        [TestMethod, TestCategory("Unit")]
        public void LowerCaseAllElementAndAttributeNames_Performance()
        {
            // Arrange
            var doc = XDocument.Parse(_largeTextXml);

            // Act
            var stopWatch = Stopwatch.StartNew();
            doc.LowerCaseAllElementAndAttributeNames();
            stopWatch.Stop();

            // Assert
            Assert.IsTrue(stopWatch.ElapsedMilliseconds <= 10, "Code took longer than 10 milliseconds.");
        }

        #endregion


        #region CopyWithoutNamespaces

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyWithoutNamespaces_NullPassed()
        {
            // Act
            XDocumentExtensions.CopyWithoutNamespaces(null);
        }

        [TestMethod, TestCategory("Unit")]
        public void CopyWithoutNamespaces()
        {
            // Arrange
            var xml = "<xml xmlns:xs=\"www.madeup.com\"><xs:parent><xs:child attr1=\"some value\"/></xs:parent></xml>";
            var expected = "<xml><parent><child attr1=\"some value\"></child></parent></xml>";
            var doc = XDocument.Parse(xml);

            // Act
            var actual = doc.CopyWithoutNamespaces().ToString(SaveOptions.DisableFormatting);

            // Assert
            actual.ShouldHaveSameValueAs(expected);
        }

        #endregion
    }
}
