using System.Linq;
using System.Web.Http.Controllers;
using Sfa.Core.Data;

namespace Sfa.Core.Web.Http.Controllers
{
    /// <summary>
    /// Extensions to the <see cref="System.Web.Http.Filters.HttpActionExecutedContext"/> class.
    /// </summary>
    public static class HttpActionExecutedContextExtensions
    {
        public static bool IsCommandAction(this HttpActionContext controllerContext)
        {
            return controllerContext.FirstCommandParameter() != null;
        }
        public static bool IsAsyncCommandAction(this HttpActionContext controllerContext)
        {
            return controllerContext.FirstAsyncCommandParameter() != null;
        }

        public static ICommand FirstCommandParameter(this HttpActionContext controllerContext)
        {
            if (controllerContext.ActionArguments.Any(o => o.Value is ICommand))
            {
                return (ICommand)controllerContext.ActionArguments.First(o => o.Value is ICommand).Value;
            }
            return null;
        }

        public static IAsyncCommand FirstAsyncCommandParameter(this HttpActionContext controllerContext)
        {
            if (controllerContext.ActionArguments.Any(o => o.Value is IAsyncCommand))
            {
                return (IAsyncCommand)controllerContext.ActionArguments.First(o => o.Value is IAsyncCommand).Value;
            }
            return null;
        }
    }
}