using System.Web.Mvc;

namespace Sfa.Core.Web.Mvc
{
    public static class ControllerContextExtensions
    {
        public static Controller BaseController(this ControllerContext context)
        {
            return context.Controller as Controller;
        }

        #region Action Call Types

        /// <summary>
        /// Determines whether the specified controller context is from a post.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <returns>
        ///   <c>true</c> if the specified controller context is from a post; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPost(this ControllerContext controllerContext)
        {
            return controllerContext.HttpContext.Request.HttpMethod == "POST";
        }

        /// <summary>
        /// Determines whether the specified controller context is from a get.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <returns>
        ///   <c>true</c> if the specified controller context is from a get; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGet(this ControllerContext controllerContext)
        {
            return controllerContext.HttpContext.Request.HttpMethod == "GET";
        }


        /// <summary>
        /// Determines whether the action is the primary post action.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <returns>
        ///   <c>true</c> if the action is the primary post action; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrimaryPostAction(this ControllerContext controllerContext)
        {
            return controllerContext.IsPost() && !controllerContext.IsChildAction;
        }

        public static bool IsPrimaryGetAction(this ControllerContext controllerContext)
        {
            return controllerContext.IsGet() && !controllerContext.IsChildAction;
        }

        public static bool IsPrimaryAction(this ControllerContext controllerContext)
        {
            return !controllerContext.IsChildAction;
        }

        #endregion
    }
}