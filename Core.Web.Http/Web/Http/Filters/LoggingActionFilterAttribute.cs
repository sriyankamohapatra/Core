using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Sfa.Core.Context;
using Sfa.Core.Logging;

namespace Sfa.Core.Web.Http.Filters
{
    /// <summary>
    /// Logs the entry and exit to a action as well as any exception.
    /// </summary>
    public class LoggingActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var @params = new StringBuilder("Params:");
            foreach (var parameter in actionContext.ActionArguments)
            {
                @params.AppendFormat("[{0}:{1}] ", parameter.Key, parameter.Value);
            }

            ApplicationContext.Logger.Log(LoggingLevel.Info, CoreLoggingCategory.Http, () => "Entering action [{0}.{1}] {2}",
                actionContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                actionContext.ActionDescriptor.ActionName,
                @params);

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
            {
                ApplicationContext.Logger.LogException(CoreLoggingCategory.Audit, actionExecutedContext.Exception);
            }

            ApplicationContext.Logger.Log(LoggingLevel.Info, CoreLoggingCategory.Http, () => "Exiting Action [{0}.{1}]", actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName, actionExecutedContext.ActionContext.ActionDescriptor.ActionName);

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}