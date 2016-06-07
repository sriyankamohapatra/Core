using System.Diagnostics;
using System.IO;
using System.Text;

namespace Sfa.Core.Diagnostics
{
    /// <summary>
    /// Writes buffered text out using the debug diagnostics utility.
    /// </summary>
    public class DebugTextWriter : TextWriter
    {
        /// <summary>
        /// Overrides the Encoding to be the default for the system.
        /// </summary>
        public override Encoding Encoding => Encoding.Default;

        /// <summary>
        /// Writes a sub-array of characters.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">The character position in the buffer at which to start retrieving data. </param>
        /// <param name="count">The number of characters to write.</param>
        public override void Write(char[] buffer, int index, int count)
        {
            Debug.Write(new string(buffer, index, count));
        }

        /// <summary>
        /// Writes a string.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public override void Write(string value)
        {
            Debug.Write(value);
        }
    }
}