using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Sfa.Core.Exceptions;

namespace Sfa.Core.Logging
{
    /// <summary>
    /// Logger that logs to application insights logger
    /// </summary>
    public class ApplicationInsightsLogger : BaseLogger
    {
        #region fields

        private readonly TelemetryClient _telemetryClient;
        private readonly IDictionary<string, string> _properties;
        private Func<LoggingLevel, string, Dictionary<string, string>> _setDynamicProperties = (level, category) => new Dictionary<string, string>();

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new telemetry client
        /// </summary>
        public ApplicationInsightsLogger()
            :this(new Dictionary<string, string>())
        {
        }

        /// <summary>
        /// Properties constructor.
        /// </summary>
        /// <param name="properties">Properties are sent to application insights for extra information.</param>
        public ApplicationInsightsLogger(IDictionary<string, string> properties)
        {
            _properties = properties;
            _telemetryClient = new TelemetryClient();
        }

        #endregion


        #region Api

        /// <summary>
        /// Provides a way to add additional dynamic properties for each call
        /// </summary>
        /// <param name="setDynamicProperties">The method that will provide the dynamic properties.</param>
        public void SetDynamicProperties(Func<LoggingLevel, string, Dictionary<string, string>> setDynamicProperties)
        {
            _setDynamicProperties = setDynamicProperties;
        }

        #endregion


        #region ILogger Implementation

        /// <summary>
        /// Logs a message.  Checks ShouldLog before writing to log.  If first
        /// member of <see cref="args"/> is an exception then switches to exception specific
        /// logging code.
        /// </summary>
        /// <param name="level">The level of logging to be performed.</param>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="createMessage">Deferred function that created the message only if the implementing system decides to actually log the message.</param>
        /// <param name="args">Any arguments for the message.</param>
        public override void Log(LoggingLevel level, string category, Func<string> createMessage, params object[] args)
        {
            if (ShouldLog(level, category))
            {
                var message = string.Format(createMessage(), args);
                var exception = args.OfType<Exception>().FirstOrDefault();

                if (exception == null)
                { 
                    LogMessage(level, category, MessageLayout(level, category, message));
                }
                else
                {
                    LogException(category, exception);
                }
            }
        }

        /// <summary>
        /// Return <c>true</c> if the telemetry client is enabled.  \1est 
        /// </summary>
        /// <param name="level">Level is ignored for this implementation.</param>
        /// <param name="category">Category is ignored for this implementation.</param>
        /// <returns></returns>
        public override bool ShouldLog(LoggingLevel level, string category)
        {
            return _telemetryClient.IsEnabled();
        }

        /// <summary>
        /// Logs the details of an exception.
        /// </summary>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="exception">The exception to log.</param>
        protected override void OnLogException(string category, Exception exception)
        {
            _telemetryClient.TrackException(exception, GetAllProperties(LoggingLevel.Error, category, exception as BusinessLogicException));
        }

        /// <summary>
        /// Logs a message directly without checking ShouldLog.
        /// </summary>
        /// <param name="message">Message to be written to the log.</param>
        /// <param name="level">The level of logging to be performed.</param>
        /// <param name="category">The category that this message belongs to.</param>
        protected override void LogMessage(LoggingLevel level, string category, string message)
        {
            var severityLevel = LoggingLevelToSeverityLevel(level);
            if (severityLevel != null)
            {
                if(level == LoggingLevel.Debug)
                {
                    _telemetryClient.TrackTrace(message, severityLevel.Value, GetAllProperties(level, category));
                }
                else if (level == LoggingLevel.Error)
                {
                    _telemetryClient.TrackException(new Exception(message), GetAllProperties(level, category));
                }
                else
                {
                    _telemetryClient.TrackEvent(message, GetAllProperties(level, category));
                }
            }
        }
        
        #endregion


        #region Helpers

        private Dictionary<string, string> GetAllProperties(LoggingLevel level, string category, BusinessLogicException exception = null)
        {
            var properties = new Dictionary<string, string>(_properties);
            foreach (var dynamicProperty in _setDynamicProperties(level, category))
            {
                properties.Add(dynamicProperty.Key, dynamicProperty.Value);
            }
            if (exception != null)
            {
                foreach (var loggingProperty in exception.LoggingProperties)
                {
                    properties.Add(loggingProperty.Key, loggingProperty.Value?.ToString() ?? "null");
                }
            }
            return properties;
        }

        /// <summary>
        /// Converts a LoggingLevel to a SeverityLevel. If the LoggingLevel is None then returns null.
        /// </summary>
        /// <param name="level">LoggingLevel to convert to a severity level.</param>
        /// <returns>Converted severity level. Null if LoggingLevel is None</returns>
        private SeverityLevel? LoggingLevelToSeverityLevel(LoggingLevel level)
        {
            switch (level)
            {
                case LoggingLevel.None:
                    return null;

                case LoggingLevel.Debug:
                    return SeverityLevel.Verbose;

                case LoggingLevel.Info:
                    return SeverityLevel.Information;

                case LoggingLevel.Warn:
                    return SeverityLevel.Warning;

                case LoggingLevel.Error:
                    return SeverityLevel.Error;

                default:
                    throw new ArgumentOutOfRangeException(nameof(level));
            }
        }

        #endregion
    }
}