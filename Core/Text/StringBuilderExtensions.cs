using System.Collections.Generic;
using System.Text;

namespace Sfa.Core.Text
{
    /// <summary>
    /// extensions based on StringBuilder.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// HtmlBreak will added an html break tag <br />
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuilder AppendHtmlLine(this StringBuilder value)
        {
            return value.Append("<br />");
        }

        /// <summary>
        /// Appends the passed headers and data to the string builder, formatted for output as a csv file.
        /// </summary>
        /// <param name="csvHeaders">The headers that appear on the first line of the csv file.</param>
        /// <param name="csvData">The data for the csv file.  Each nested list represents a line in the csv file.</param>
        /// <param name="value">The string builder being extended.</param>
        /// <returns></returns>
        public static StringBuilder AppendCsvData(this StringBuilder value, List<string> csvHeaders, List<List<string>> csvData)
        {
            value.AppendLine(string.Join(",", csvHeaders));
            foreach (var dataLine in csvData)
            {
                value.AppendLine(string.Join(",", dataLine));
            }
            return value;
        }
    }
}
