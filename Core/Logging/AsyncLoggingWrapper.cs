using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Sfa.Core.Context;

namespace Sfa.Core.Logging
{
    /// <summary>
    /// Creates an in memory queue (FIFO) to asynchronously process the messages on a background thread.
    /// This then calls the appropriate log messages on all the loggers supplied.
    /// </summary>
    public class AsyncLoggingWrapper : ILogger, IDisposable
    {
        #region fields

        /// <summary>
        /// Internal logger made asynchronous by this wrapper.
        /// </summary>
        private static ILogger _logger;

        /// <summary>
        /// Queued log messages.
        /// </summary>
        private static BlockingCollection<QueueLoggingParams> _queue;

        #endregion


        #region constants

        /// <summary>
        /// Queue size when queue size info message is written to log.
        /// </summary>
        private const int QueueInfoInterval = 500;

        /// <summary>
        /// Queue size when queue size warning message is written to log.
        /// </summary>
        private const int QueueWarnInterval = 3000;

        /// <summary>
        /// Queue size when queue size error message is written to log.
        /// </summary>
        private const int QueueErrorInterval = 6000;

        #endregion


        #region constructors

        /// <summary>
        /// Create a new AsyncLoggingWrapper. Starts a background task to process items added to the
        /// logging queue.
        /// </summary>
        /// <param name="logger">The logger to be made asynchronous</param>
        public AsyncLoggingWrapper(ILogger logger)
        {
            _queue = new BlockingCollection<QueueLoggingParams>();
            _logger = logger;

            Task.Factory.StartNew(Process);
        }

        #endregion


        #region ILogger implementation

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level">The level of logging to be performed.</param>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="createMessage">Deferred function that created the message only if the implementing system decides to actually log the message.</param>
        /// <param name="args">Any arguments for the message.</param>
        public void Log(LoggingLevel level, string category, Func<string> createMessage, params object[] args)
        {
            if (_logger.ShouldLog(level, category))
            {
                var exception = args.OfType<Exception>().FirstOrDefault();
                AddToQueueCheckInterval(level, category, string.Format(createMessage(), args), exception);
            }
        }

        /// <summary>
        /// Used to log the details of an exception.
        /// </summary>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="exception">The exception to log.</param>
        public void LogException(string category, Exception exception)
        {
            Log(LoggingLevel.Error, category, () => exception.Message, exception);
        }

        /// <summary>
        /// Sets that logging level of a category.
        /// </summary>
        /// <param name="category">The category to set the logging of.</param>
        /// <param name="level">The level to log at.</param>
        public void SetCategoryLogging(string category, LoggingLevel level)
        {
            _logger.SetCategoryLogging(category, level);
        }

        /// <summary>
        /// Returns <c>true</c> if the message should be written to the log.
        /// </summary>
        /// <returns><c>true</c> if should be written to the log.</returns>
        public bool ShouldLog(LoggingLevel level, string category)
        {
            return _logger.ShouldLog(level, category);
        }

        #endregion


        #region IDisposable implementation

        /// <summary>
        /// Call protected dispose to tidy the queue.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Ensure that the asynchronous queue is tidied up.
        /// </summary>
        /// <param name="disposing">Only tidy the queue if disposing is <c>true</c>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_queue != null)
                {
                    _queue.CompleteAdding();

                    _queue.Dispose();
                    _queue = null;
                }
            }
        }

        #endregion


        #region Queueing Impl

        /// <summary>
        /// Consume the logging queue. If an exception is in the queue then pass to the underlying
        /// logger as an exception rather than a standard message.
        /// </summary>
        private void Process()
        {
            // Now consume the blocking collection with foreach.
            // Use bc.GetConsumingEnumerable() instead of just bc because the
            // former will block waiting for completion and the latter will
            // simply take a snapshot of the current state of the underlying collection.
            foreach (var logParam in _queue.GetConsumingEnumerable())
            {
                if (logParam.Exception == null)
                {
                    _logger.Log(logParam.Level, logParam.Category, () => logParam.Message);
                }
                else
                {
                    _logger.LogException(logParam.Category, logParam.Exception);
                }
            }
        }

        /// <summary>
        /// Add the passed logging parameters to the queue. If the queue size crosses one of the 
        /// queue interval boundaries then add a queue size log message to the queue. Unsure of what
        /// category queue size log messages should be so retaining category from original message.
        /// </summary>
        /// <param name="message">Message to be written to the log.</param>
        /// <param name="level">The level of logging to be performed.</param>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="exception">Any exception that might be getting logged.</param>
        private void AddToQueueCheckInterval(LoggingLevel level, string category, string message, Exception exception)
        {
            AddToQueue(level, category, message, exception);
            var queueCount = _queue.Count;
            if (queueCount > 0)
            {
                if (queueCount % QueueErrorInterval == 0)
                {
                    LogQueueLength(LoggingLevel.Error, category, queueCount);
                }
                else if (queueCount % QueueWarnInterval == 0)
                {
                    LogQueueLength(LoggingLevel.Warn, category, queueCount);
                }
                else if (queueCount % QueueInfoInterval == 0)
                {
                    LogQueueLength(LoggingLevel.Info, category, queueCount);
                }
            }
        }

        /// <summary>
        /// Add a queue length log to the queue.
        /// </summary>
        /// <param name="level">The level of the logging to be performed.</param>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="queueCount">The number of items currently in the queue.</param>
        private void LogQueueLength(LoggingLevel level, string category, int queueCount)
        {
            AddToQueue( level, 
                        category,
                        $"Logging queue length is '{queueCount}'",
                        null);
        }

        /// <summary>
        /// Add the passed logging parameters to the log queue. Add a timestamp to the message at the
        /// time it's added to the queue
        /// </summary>
        /// <param name="message">Message to be written to the log.</param>
        /// <param name="level">The level of logging to be performed.</param>
        /// <param name="category">The category that this message belongs to.</param>
        /// <param name="exception">If there's an exception associated with this message pass it here.</param>
        private void AddToQueue(LoggingLevel level, string category, string message, Exception exception)
        {
            var timeStampedMessage = message + $" (Original Timestamp: {ApplicationContext.NetworkContext.CurrentDateTime})";
            _queue.Add(new QueueLoggingParams
            {
                Message = timeStampedMessage,
                Level = level,
                Category = category,
                Exception = exception
            });
        }

        /// <summary>
        /// Helper object to store log messages on the queue for logging later.
        /// </summary>
        private class QueueLoggingParams
        {
            public string Message { get; set; }
            public LoggingLevel Level { get; set; }
            public string Category { get; set; }
            public Exception Exception { get; set; }
        }

        #endregion
    }
}
