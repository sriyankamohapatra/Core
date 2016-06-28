using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Sfa.Core.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static IEnumerable<SelectListItem> EnumToSelectList<T>(this HtmlHelper helper, bool includeTopValue = false,  string topText = "", string topValue = "")
        {
            var type = typeof(T);
            var values = Enum.GetValues(type);

            if (includeTopValue)
            {
                yield return new SelectListItem
                {
                    Text = topText,
                    Value = topValue
                };
            }

            foreach (T value in values)
            {
                yield return new SelectListItem
                {
                    Value = value.ToString(),
                    Text = value.GetDisplayName()
                };
            }
        }

        public static MvcForm BeginForm(this HtmlHelper helper, object htmlAttributes, object routeValues = null)
        {
            return helper.BeginForm(helper.ViewContext.RouteData.Values["Action"].ToString(),
                                    helper.ViewContext.RouteData.Values["Controller"].ToString(),
                                    routeValues, FormMethod.Post, htmlAttributes);
        }

        public static string GetDisplayName<TEnum>(this TEnum value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var type = typeof(TEnum);
            if (type == typeof(Enum))
            {
                type = value.GetType();
            }
            var memInfo = type.GetMember(value.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes.Length != 0)
            {
                return ((DisplayAttribute)attributes[0]).Name;
            }
            return value.ToString();
        }
    }
}