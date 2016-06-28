using System;
using System.Collections.Generic;
using Sfa.Core.Context;
using System.Configuration;

namespace Sfa.Core.Logging
{
    /// <summary>
    /// Implements some default parts of a logger.
    /// </summary>
    public abstract class BaseLogger : ILogger
    {
        #region Fields

        private readonly IDictionary<string, LoggingLevel> _categoriesToLog = new Dictionary<string, LoggingLevel>();
        protected Func<LoggingLevel, string, string, string> MessageLayout = (level, category, message) => $"When:{ApplicationContext.NetworkContext.CurrentDateTime} Level:{level} Category:{category} Message:{message}";

        #endregion


        #region Constructors

        /// <summary>
        /// Set the Categories and levels from con fig file
        /// </summary>
        protected BaseLogger(string configSectionName = "loggingSettings")
        {
            if (!string.IsNullOrWhiteSpace(configSectionName))
            {
                var applicationSettings = (LoggingConfigurationSettings) ConfigurationManager.GetSection(configSectionName);

                if (applicationSettings != null)
                {
                    foreach (LogSetting logSetting in applicationSettings.Settings)
                    {
                        SetCategoryLogging(logSetting.Category, logSetting.Level);
                    }
                }
            }
        }


        #endregion


        #region Api

        public void SetLayout(Func<LoggingLevel, string, string, string> messageLayout)
        {
            MessageLayout = messageLayout;
        }

        #endregion


        #region ILogger Implementation

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level">The level of logging to be performed.</param>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="createMessage">Deferred function that created the message only if the implementing system decides to actually log the message.</param>
        /// <param name="args">Any arguments for the message.</param>
        public virtual void Log(LoggingLevel level, string category, Func<string> createMessage, params object[] args)
        {
            if (ShouldLog(level, category))
            {
                var message = string.Format(createMessage(), args);
                LogMessage(level, category, MessageLayout(level, category, message));
            }
        }


        /// <summary>
        /// Logs the details of an exception.
        /// </summary>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="exception">The exception to log.</param>n)
        public void LogException(string category, Exception exception)
        {
            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    OnLogException(category, innerException);
                }
            }
            else
            {
                OnLogException(category, exception);
            }
        }

        protected virtual void OnLogException(string category, Exception exception)
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
            _categoriesToLog[category] = level;
        }


        /// <summary>
        /// Returns true if the message should be written to the log.
        /// </summary>
        /// <returns>True if should be written to the log.</returns>
        public virtual bool ShouldLog(LoggingLevel queryLevel, string category)
        {
            Predicate<string> shouldLog = key => (_categoriesToLog.ContainsKey(key)
                             && _categoriesToLog[key] != LoggingLevel.None
                             && _categoriesToLog[key] <= queryLevel);

            return shouldLog(category) || shouldLog("*");
        }

        #endregion


        #region Simple logging Implementation

        /// <summary>
        /// Logs a message directly without checking ShouldLog.
        /// </summary>
        /// <param name="message">Message to be written to the log.</param>
        /// <param name="level">The level of logging to be performed.</param>
        /// <param name="category">The category that this message belongs to.</param>
        protected abstract void LogMessage(LoggingLevel level, string category, string message);

        #endregion
    }
}