using System;
using System.IO;

namespace Sfa.Core.Context
{
    /// <summary>
    /// Default implementation of <see cref="INetworkContext"/>.
    /// </summary>
    [Serializable]
    public class DefaultNetworkContext : INetworkContext
    {
        #region INetworkContext Api

        /// <summary>
        /// Get current datetime.
        /// </summary>
        /// <value>The current date time.</value>
        public virtual DateTime CurrentDateTime => DateTime.Now;

        /// <summary>
        /// Gets the current utc date time.
        /// </summary>
        public virtual DateTime CurrentUtcDateTime => DateTime.UtcNow;

        /// <summary>
        /// Get current date.
        /// </summary>
        /// <value>The current date.</value>
        public virtual DateTime CurrentDate => DateTime.Today;

        /// <summary>
        /// Gets a new GUID.
        /// </summary>
        public virtual Guid NewGuid => Guid.NewGuid();

        /// <summary>
        /// Converts the stream into its byte array.
        /// </summary>
        /// <param name="stream">The stream to convert.</param>
        /// <returns>The byte array representation of the contents of the stream.</returns>
        public byte[] ToBytes(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        #endregion
    }
}