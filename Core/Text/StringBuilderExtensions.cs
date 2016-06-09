using System.Collections.Generic;
using System.Text;

namespace Sfa.Core.Text
{
    /// <summary>
    /// Extensions available for <see cref="StringBuilder"/>.
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
            return value.AppendLine().AppendLine("<br />");
        }

        /// <summary>
        /// Appends the passed headers and data to the <see cref="StringBuilder"/> instance, formatted for output as a csv file.
        /// </summary>
        /// <param name="csvHeaders">The headers that appear on the first line of the csv file.</param>
        /// <param name="csvData">The data for the csv file. Each nested list represents a line in the csv file.</param>
        /// <param name="builder">The string builder being extended.</param>
        /// <returns>The builder instance.</returns>
        public static StringBuilder AppendCsvData(this StringBuilder builder, IEnumerable<string> csvHeaders, IEnumerable<IEnumerable<string>> csvData)
        {
            builder.AppendLine(string.Join(",", csvHeaders));
            foreach (var dataLine in csvData)
            {
                builder.AppendLine(string.Join(",", dataLine));
            }
            return builder;
        }
    }
}
