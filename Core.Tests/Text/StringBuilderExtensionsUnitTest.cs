using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Text
{
    [TestClass]
    public class StringBuilderExtensionsUnitTest : BaseTest
    {
        #region AppendHtmlLine

        [TestMethod, TestCategory("Unit")]
        public void AppendHtmlLine_WithHTMLInStringBuilder()
        {
            // Arrange
            var componentUnderTest = new StringBuilder("<p>Test Text Test Text</p>");
            var expected = $"<p>Test Text Test Text</p>{Environment.NewLine}<br />{Environment.NewLine}";

            // Act
            componentUnderTest.AppendHtmlLine();

            // Assert
            componentUnderTest.ToString().ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void AppendHtmlLine_WithEmptyStringBuilder()
        {
            // Arrange
            var componentUnderTest = new StringBuilder();
            var expected = $"{Environment.NewLine}<br />{Environment.NewLine}";

            // Act
            componentUnderTest.AppendHtmlLine();

            // Assert
            componentUnderTest.ToString().ShouldHaveSameValueAs(expected);
        }

        #endregion


        #region AppendCsvData

        [TestMethod, TestCategory("Unit")]
        public void AppendCsvData()
        {
            // Arrange
            var componentUnderTest = new StringBuilder();
            var headers = new[] {"h1", "h2", "h3"};
            var data = new[]
            {
                new[] {"1", "2"},
                new[] {"a", "b", "c"}
            };
            var expected = $"h1,h2,h3{Environment.NewLine}1,2{Environment.NewLine}a,b,c{Environment.NewLine}";

            // Act
            var actual = componentUnderTest.AppendCsvData(headers, data);

            // Assert
            actual.ToString().ShouldHaveSameValueAs(expected);
        }

        #endregion
    }
}

