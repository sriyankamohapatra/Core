using System.Web.Mvc;

namespace Sfa.Core.Web.Mvc
{
    public class ModelStateCheckFilterAttribute : ModelStateRedirectFilterAttribute
    {
        #region ActionFilterAttribute overrides

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                var controller = filterContext.BaseController();

                if (filterContext.IsPost())
                {
                    if (!controller.ModelState.IsValid)
                    {
                        filterContext.Result = OnActionExecutingRedirect(filterContext);
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }

        #endregion
    }
}