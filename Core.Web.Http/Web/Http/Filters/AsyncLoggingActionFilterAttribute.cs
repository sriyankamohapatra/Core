using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Sfa.Core.Context;
using Sfa.Core.Logging;

namespace Sfa.Core.Web.Http.Filters
{
    /// <summary>
    /// Logs the entry and exit to a action as well as any exception.
    /// </summary>
    public class AsyncLoggingActionFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
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

            await base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext.Exception != null)
            {
                ApplicationContext.Logger.LogException("Audit", actionExecutedContext.Exception);
            }

            ApplicationContext.Logger.Log(LoggingLevel.Info, CoreLoggingCategory.Http, () => "Exiting Action [{0}.{1}]", actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName, actionExecutedContext.ActionContext.ActionDescriptor.ActionName);

            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}