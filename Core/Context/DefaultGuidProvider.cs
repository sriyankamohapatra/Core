using System;

namespace Sfa.Core.Context
{
    /// <summary>
    /// Default implementation for providing guids.
    /// </summary>
    public class DefaultGuidProvider : IGuidProvider
    {
        /// <summary>
        /// Returns a new Guid.
        /// </summary>
        /// <returns>A new Guid.</returns>
        public Guid NewGuid()
        {
            return Guid.NewGuid();
        }
    }
}