using System;

namespace Sfa.Core.Logging
{
    /// <summary>
    /// Defines a logging interface.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level">The level of logging to be performed.</param>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="createMessage">Deferred function that created the message only if the implementing system decides to actually log the message.</param>
        /// <param name="args">Any arguments for the message.</param>
        void Log(LoggingLevel level, string category, Func<string> createMessage, params object[] args);
        
        /// <summary>
        /// Logs the details of an exception.
        /// </summary>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="exception">The exception to log.</param>
        void LogException(string category, Exception exception);

        /// <summary>
        /// Sets that logging level of a category.
        /// </summary>
        /// <param name="category">The category to set the logging of.</param>
        /// <param name="level">The level to log at.</param>
        void SetCategoryLogging(string category, LoggingLevel level);

        /// <summary>
        /// Returns true if the message should be written to the log.
        /// </summary>
        /// <returns>True if should be written to the log.</returns>
        bool ShouldLog(LoggingLevel level, string category);
    }
}