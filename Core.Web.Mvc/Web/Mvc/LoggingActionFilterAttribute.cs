using System.Text;
using System.Web.Mvc;
using Sfa.Core.Context;
using Sfa.Core.Logging;

namespace Sfa.Core.Web.Mvc
{
    /// <summary>
    /// Logs the entry and exit to a action or result as well as any exception.
    /// </summary>
    public class LoggingActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Creates a log upon an action executing
        /// </summary>
        /// <param name="filterContext">The action executing context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                var @params = new StringBuilder("Params:");
                foreach (var parameter in filterContext.ActionParameters)
                {
                    @params.AppendFormat("[{0}:{1}] ", parameter.Key, parameter.Value);
                }

                ApplicationContext.Logger.Log(LoggingLevel.Info, CoreLoggingCategory.Audit, () => "Entering action [{0}.{1}] {2}",
                    filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                    filterContext.ActionDescriptor.ActionName,
                    @params);
            }
        }

        /// <summary>
        /// Creates a log upon an action executed
        /// </summary>
        /// <param name="filterContext">The action executing context.</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var actionName = filterContext.ActionDescriptor.ActionName;

            if (filterContext.Exception != null)
            {
                ApplicationContext.Logger.LogException(CoreLoggingCategory.Audit, filterContext.Exception);
            }
            else
            {
                ApplicationContext.Logger.Log(LoggingLevel.Info, CoreLoggingCategory.Audit,
                    () => "Action [{0}.{1}] was completed successfully.",
                controllerName,
                actionName);
            }
        }
    }
}