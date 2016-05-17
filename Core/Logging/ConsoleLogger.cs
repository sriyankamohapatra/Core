using System;
using Sfa.Core.Context;

namespace Sfa.Core.Logging
{
    /// <summary>
    /// Console logger implementation
    /// </summary>
    public class ConsoleLogger : BaseLogger
    {
        #region BaseLogger implementation


        /// <summary>
        /// Logs a message directly without checking ShouldLog.
        /// </summary>
        /// <param name="message">Message to be written to the log.</param>
        /// <param name="level">The level of logging to be performed.</param>
        /// <param name="category">The category that this message belongs to.</param>
        protected override void LogMessage(LoggingLevel level, string category, string message)
        {
            Console.WriteLine(message);
        }

        #endregion
    }
}