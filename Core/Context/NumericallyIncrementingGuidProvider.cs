using System;

namespace Sfa.Core.Context
{
    /// <summary>
    /// Creates new Guids based on a numeric seed that increments with each call.
    /// </summary>
    public class NumericallyIncrementingGuidProvider : IGuidProvider
    {
        #region Fields

        private int _seed;
        private int _callsMade;

        #endregion


        #region Main Api

        /// <summary>
        /// Sets the seed method and resets the calls made.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        /// <returns>This instance.</returns>
        public NumericallyIncrementingGuidProvider SetSeedValue(int seed)
        {
            _seed = seed;
            _callsMade = 0;
            return this;
        }

        #endregion


        #region IGuidProvider Implementation

        /// <summary>
        /// Returns a new Guid based on the seed and the amount of times the method has been called.
        /// </summary>
        /// <returns>A new Guid.</returns>
        public Guid NewGuid()
        {
            var newGuid = new Guid($"00000000-0000-0000-0000-{(_seed + _callsMade).ToString("000000000000")}");
            _callsMade++;
            return newGuid;
        }

        #endregion
    }
}