using System.Web.Mvc;

namespace Sfa.Core.Web.Mvc
{
    public class ModelStateImportFilterAttribute : ModelStateTempDataTransferAttribute
    {
        #region ActionFilterAttribute overrides

        /// <summary>
        /// Called by the ASP.NET MVC framework after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Only copy from TempData if we are rendering a View/Partial
            if (filterContext.Result is ViewResult)
            {
                ImportModelStateFromTempData(filterContext);
            }
            else
            {
                RemoveModelStateFromTempData(filterContext);
            }

            base.OnActionExecuted(filterContext);
        }

        #endregion
    }
}