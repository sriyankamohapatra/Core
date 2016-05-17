using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Sfa.Core.Web.Http.Controllers;

namespace Sfa.Core.Web.Http.Filters
{
    /// <summary>
    /// Sets up any contexts for command actions.
    /// </summary>
    public class AsyncCommandContextActionFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (actionContext.IsAsyncCommandAction())
            {
                await OnSetupCommandContextsAsync();
            }

            await base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            await OnTeardownCommandContextsAsync(actionExecutedContext);

            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }

        /// <summary>
        /// Override this in base classes to set up your contexts.
        /// </summary>
        /// <returns>A continuation <see cref="Task"/>.</returns>
        protected virtual Task OnSetupCommandContextsAsync()
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Override this in base classes to tear down your contexts.
        /// </summary>
        /// <returns>A continuation <see cref="Task"/>.</returns>
        protected virtual Task OnTeardownCommandContextsAsync(HttpActionExecutedContext actionExecutedContext)
        {
            return Task.FromResult(0);
        }
    }
}