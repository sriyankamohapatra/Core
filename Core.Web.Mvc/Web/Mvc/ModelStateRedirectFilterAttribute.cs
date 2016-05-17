using System.Web.Mvc;

namespace Sfa.Core.Web.Mvc
{
    public abstract class ModelStateRedirectFilterAttribute : ModelStateTempDataTransferAttribute
    {
        #region Utilities

        protected ActionResult OnActionExecutingRedirect(ActionExecutingContext filterContext)
        {
            //if (filterContext.HttpContext.Request.IsAjaxRequest())
            //{
            //    return ProcessAjax(filterContext);
            //}

            return ProcessNormal(filterContext);
        }

        protected virtual ActionResult ProcessNormal(ActionExecutingContext filterContext)
        {
            ExportModelStateToTempData(filterContext);

            return new RedirectToRouteResult(filterContext.RouteData.Values);
        }

        //protected virtual ActionResult ProcessAjax(ActionExecutingContext filterContext)
        //{
        //    var errors = filterContext.Controller.ViewData.ModelState.ToSerializableDictionary();
        //    var json = new JavaScriptSerializer().Serialize(errors);

        //    return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, json);
        //}

        #endregion
    }
}