using System;

namespace Sfa.Core.Context
{
    /// <summary>
    /// Defines a class that provides the default network implementation but allows the user to adjust the current time for the application context.
    /// </summary>
    public class AdjustableNetworkContext : DefaultNetworkContext
    {
        #region Fields

        private readonly Func<DateTime, DateTime> _defaultDateTimeAdjuster;
        private readonly IGuidProvider _guidProvider;
        private Func<DateTime, DateTime> _additionalDateTimeAdjuster = o => o;

        #endregion


        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="defaultDateTimeAdjuster">A function that alters all default times for the life span of the context.</param>
        /// <param name="guidProvider">The Guid provider implementation.</param>
        public AdjustableNetworkContext(Func<DateTime, DateTime> defaultDateTimeAdjuster, IGuidProvider guidProvider)
        {
            _defaultDateTimeAdjuster = defaultDateTimeAdjuster;
            _guidProvider = guidProvider;
        }

        #endregion


        #region Main Api

        /// <summary>
        /// Sets the function that will adjust all times once applied above the default adjuster.
        /// </summary>
        /// <param name="additionalDateTimeAdjuster"></param>
        public void SetAdditionalDateTimeAdjuster(Func<DateTime, DateTime> additionalDateTimeAdjuster)
        {
            _additionalDateTimeAdjuster = additionalDateTimeAdjuster;
        }

        /// <summary>
        /// Removes the additional adjusters.
        /// </summary>
        public void ClearAdditionalDateTimeAdjuster()
        {
            _additionalDateTimeAdjuster = o => o;
        }

        #endregion


        #region Overrides
        
        /// <summary>
        /// Get current datetime.
        /// </summary>
        /// <value>The current date time.</value>
        public override DateTime CurrentDateTime => _additionalDateTimeAdjuster(_defaultDateTimeAdjuster(base.CurrentDateTime));


        /// <summary>
        /// Get current date.
        /// </summary>
        /// <value>The current date.</value>
        public override DateTime CurrentDate => _additionalDateTimeAdjuster(_defaultDateTimeAdjuster(base.CurrentDate));

        /// <summary>
        /// Gets a new GUID.
        /// </summary>
        public override Guid NewGuid => _guidProvider.NewGuid();

        #endregion
    }
}