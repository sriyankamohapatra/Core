using System;
using System.Linq;

namespace Sfa.Core.Logging
{
    /// <summary>
    /// Represents a class that contains other loggers and ensures that they all have the opportunity to log 
    /// </summary>
    public class MultiLogger : ILogger
    {
        #region Fields
        
        private readonly ILogger[] _loggers;
        
        #endregion


        #region Constructors
        

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public MultiLogger(params ILogger[] loggers)
        {
            _loggers = loggers;
        }


        #endregion


        #region ILogger Impl


        /// <summary>
        /// Logs a message. If none of the consumers require the message to be logged then createMessage is
        /// not run.
        /// </summary>
        /// <param name="level">The level of logging to be performed.</param>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="createMessage">Deferred function that created the message only if the implementing system decides to actually log the message.</param>
        /// <param name="args">Any arguments for the message.</param>
        public void Log(LoggingLevel level, string category, Func<string> createMessage, params object[] args)
        {
            if (ShouldLog(level, category))
            {
                foreach (var logger in _loggers)
                {
                    logger.Log(level, category, createMessage, args);
                }
            }
        }


        /// <summary>
        /// Used to log the details of an exception.
        /// </summary>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="exception">The exception to log.</param>
        public void LogException(string category, Exception exception)
        {
            if (ShouldLog(LoggingLevel.Error, category))
            {
                foreach (var logger in _loggers)
                {
                    logger.LogException(category, exception);
                }
            }
        }


        /// <summary>
        /// Sets that logging level of a category.
        /// </summary>
        /// <param name="category">The category to set the logging of.</param>
        /// <param name="level">The level to log at.</param>
        public void SetCategoryLogging(string category, LoggingLevel level)
        {
            foreach (var logger in _loggers)
            {
                logger.SetCategoryLogging(category, level);
            }
        }
        

        /// <summary>
        /// Returns <c>true</c> if the message should be written to the log.
        /// </summary>
        /// <returns><c>true</c> if should be written to the log.</returns>
        public bool ShouldLog(LoggingLevel level, string category)
        {
            return _loggers.Any(logger => logger.ShouldLog(level, category));
        }


        #endregion

    }
}