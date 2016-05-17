using System;
using System.IO;

namespace Sfa.Core.Context
{
    /// <summary>
    /// Defines a context that has access to the network and other common resources.
    /// </summary>
    public interface INetworkContext
    {
        /// <summary>
        /// Gets the current date time.
        /// </summary>
        DateTime CurrentDateTime { get; }

        /// <summary>
        /// Gets the current utc date time.
        /// </summary>
        DateTime CurrentUtcDateTime { get; }

        /// <summary>
        /// Gets the current date.
        /// </summary>
        DateTime CurrentDate { get; }
        
        /// <summary>
        /// Gets a new GUID.
        /// </summary>
        Guid NewGuid { get; }

        /// <summary>
        /// Converts the stream into its byte array.
        /// </summary>
        /// <param name="stream">The stream to convert.</param>
        /// <returns>The byte array representation of the contents of the stream.</returns>
        byte[] ToBytes(Stream stream);
    }
}