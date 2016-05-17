using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Sfa.Core.Web.Http.Controllers;

namespace Sfa.Core.Web.Http.Filters
{
    /// <summary>
    /// Sets up any contexts for command actions.
    /// </summary>
    public class CommandContextActionFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.IsCommandAction())
            {
                OnSetupCommandContexts();
            }

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            OnTeardownCommandContexts(actionExecutedContext);

            base.OnActionExecuted(actionExecutedContext);
        }

        /// <summary>
        /// Override this in base classes to set up your contexts.
        /// </summary>
        /// <returns>A continuation <see cref="Task"/>.</returns>
        protected virtual void OnSetupCommandContexts()
        {
        }

        /// <summary>
        /// Override this in base classes to tear down your contexts.
        /// </summary>
        /// <returns>A continuation <see cref="Task"/>.</returns>
        protected virtual void OnTeardownCommandContexts(HttpActionExecutedContext actionExecutedContext)
        {
        }
    }
}