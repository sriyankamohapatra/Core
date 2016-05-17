namespace Sfa.Core.Logging
{
    /// <summary>
    /// Defines the level of logging that is to be performed.
    /// </summary>
    public enum LoggingLevel
    {
        /// <summary>
        /// Defines that logging should not be performed.
        /// </summary>
        None = 0,

        /// <summary>
        /// Used for debug level logging.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Used for relating note worthy information about the system.
        /// </summary>
        Info = 2,

        /// <summary>
        /// Used for noting that something might not be correct, but the system has handled it.
        /// </summary>
        Warn = 3,

        /// <summary>
        /// Used to log an error.
        /// </summary>
        Error = 4
    }
}