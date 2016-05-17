using System;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Sfa.Core.Logging
{
    /// <summary>
    /// Implementation of a logger using the Enterprise Library Application blocks.
    /// </summary>
    public class EnterpriseLibraryLogger : ILogger
    {
        #region ILogger implementation

        /// <summary>
        /// Used to log a message. Logs if Logger.IsLoggingEnabled and Logger.ShouldLog.
        /// </summary>
        /// <param name="level">The level of logging to be performed.</param>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="createMessage">Deferred function that creates the message only if the implementing system decides to actually log the message using <see cref="args"/>.</param>
        /// <param name="args">Any arguments for the message.</param>
        public void Log(LoggingLevel level, string category, Func<string> createMessage, params object[] args)
        {
            if (!Logger.IsLoggingEnabled())
            {
                return;
            }

            var entry = NewLogEntry(level);

            if (Logger.ShouldLog(entry))
            {
                entry.Message = string.Format(createMessage(), args);
                Logger.Write(entry);
            }
        }


        /// <summary>
        /// Determines if this logger should log at the level for the supplied category.
        /// </summary>
        /// <param name="level">The level to log at.</param>
        /// <param name="category">The category of the log.</param>
        /// <returns><c>true</c> if...</returns>
        public bool ShouldLog(LoggingLevel level, string category)
        {
            return Logger.IsLoggingEnabled() && Logger.ShouldLog(NewLogEntry(level));
        }

        /// <summary>
        /// Used to log the details of an exception.
        /// </summary>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="exception">The exception to log.</param>
        public void LogException(string category, Exception exception)
        {
            Log(LoggingLevel.Error, category, exception.ToString);
        }

        /// <summary>
        /// Sets that logging level of a category.
        /// </summary>
        /// <param name="category">The category to set the logging of.</param>
        /// <param name="level">The level to log at.</param>
        public void SetCategoryLogging(string category, LoggingLevel level)
        {
        }

        /// <summary>
        /// Logs a message directly without checking ShouldLog.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="level">The LoggingLevel of the message</param>
        /// <param name="category">The category of the message.</param>
        public void LogMessage(LoggingLevel level, string category, string message)
        {
            LogEntry entry = NewLogEntry(level);
            entry.Message = message;
            Logger.Write(entry);
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Create a new log entry object.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private LogEntry NewLogEntry(LoggingLevel level)
        {
            var entry = new LogEntry();

            switch (level)
            {
                case LoggingLevel.None:
                    break;
                case LoggingLevel.Debug:
                    entry.Severity = TraceEventType.Verbose;
                    break;
                case LoggingLevel.Info:
                    entry.Severity = TraceEventType.Information;
                    break;
                case LoggingLevel.Warn:
                    entry.Severity = TraceEventType.Warning;
                    break;
                case LoggingLevel.Error:
                    entry.Severity = TraceEventType.Error;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level));
            }
            return entry;
        }

        #endregion
    }
}