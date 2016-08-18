using System;
using Sfa.Core.Context;
using Sfa.Core.Logging;

namespace Sfa.$safeprojectname$.Domain
{
    /// <summary>
    /// Base class for all objects within the current domain.
    /// </summary>
    [Serializable]
    public abstract class BaseDomainObject
    {
        #region Logging


        protected static void LogAudit(Func<string> createMessage, params object[] args)
        {
            InfoLog(CoreLoggingCategory.Audit, createMessage, args);
        }

        protected static void LogDiagnostics(Func<string> createMessage, params object[] args)
        {
            DebugLog(CoreLoggingCategory.Diagnostics, createMessage, args);
        }

        protected static void LogException(Exception exception)
        {
            ErrorLog(CoreLoggingCategory.Diagnostics, exception);
        }

        private static void InfoLog(string category, Func<string> createMessage, params object[] args)
        {
            ApplicationContext.Logger.Log(LoggingLevel.Info, category, createMessage, args);
        }

        private static void DebugLog(string category, Func<string> createMessage, params object[] args)
        {
            ApplicationContext.Logger.Log(LoggingLevel.Info, category, createMessage, args);
        }

        private static void ErrorLog(string category, Exception exception)
        {
            ApplicationContext.Logger.LogException(category, exception);
        }

        #endregion
    }
}