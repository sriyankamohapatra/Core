using System;

namespace Sfa.Core.Context
{
    /// <summary>
    /// Defines an interface for a type that provides Guids.
    /// </summary>
    public interface IGuidProvider
    {
        /// <summary>
        /// Returns a new Guid.
        /// </summary>
        /// <returns>A new Guid.</returns>
        Guid NewGuid();
    }
}