using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;
using Sfa.Core.Text;

namespace Sfa.Core.Text
{
    [TestClass]
    public class StringBuilderExtensionsUnitTest : BaseTest
    {
        #region StringExtension AppendHtmlLine

        [TestMethod, TestCategory("Unit")]
        public void AppendHtmlLine_WithHTMLInStringBuilder()
        {
            // Arrange
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("<p>Test Text Test Text</p>");
            const string expected = "<p>Test Text Test Text</p><br />";

            //Act
            stringBuilder.AppendHtmlLine();

            //Assert
            stringBuilder.ToString().ShouldHaveSameValueAs(expected);
        }

        [TestMethod, TestCategory("Unit")]
        public void AppendHtmlLine_WithEmptyStringBuilder()
        {
            // Arrange
            var stringBuilder = new StringBuilder();
            const string expected = "<br />";

            //Act
            stringBuilder.AppendHtmlLine();

            //Assert
            stringBuilder.ToString().ShouldHaveSameValueAs(expected);
        }

        #endregion
    }
}

