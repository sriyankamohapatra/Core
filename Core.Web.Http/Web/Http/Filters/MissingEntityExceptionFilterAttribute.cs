using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Sfa.Core.Exceptions;

namespace Sfa.Core.Web.Http.Filters
{
    /// <summary>
    /// Handles Missing Entity Exceptions at the filter level.
    /// </summary>
    public class MissingEntityExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);
            var missingEntityException = actionExecutedContext.Exception as MissingEntityException;

            if (missingEntityException != null)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }
    }
}