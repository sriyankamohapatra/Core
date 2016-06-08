using System;
using System.IO;

namespace Sfa.Core.IO
{
    /// <summary>
    /// Extends the standard <see cref="Stream"/> class with more functions.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Read the full stream into a byte array.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The full byte array that the stream represents.</returns>
        public static byte[] ToByteArray(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var buffer = new byte[16 * 1024];
            using (var memoryStream = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, read);
                }
                return memoryStream.ToArray();
            }
        }
    }
}