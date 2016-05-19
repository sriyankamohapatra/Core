using System;
using System.Globalization;
using System.Web.Mvc;

namespace Sfa.Core.Web.Mvc
{
    public class DateTimeModelBinder : IModelBinder
    {
        #region Explicit Interface Methods

        object IModelBinder.BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var culture = CultureInfo.GetCultureInfo("en-GB");
            var dayValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Day");
            var monthValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Month");
            var yearValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Year");

            // Drop out here to the default if the known day, month year can't be found.
            if (dayValue == null || monthValue == null || yearValue == null)
            {
                var defaultResult = new DefaultModelBinder().BindModel(controllerContext, bindingContext);
                return defaultResult;
            }

            var hourValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Hour");
            var minuteValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Minute");
            var secondValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Second");

            var hourAttempted = "00";
            if (hourValue != null)
            {
                hourAttempted = hourValue.AttemptedValue;
            }

            var minuteAttempted = "00";
            if (minuteValue != null)
            {
                minuteAttempted = minuteValue.AttemptedValue;
            }

            var secondAttempted = "00";
            if (secondValue != null)
            {
                secondAttempted = secondValue.AttemptedValue;
            }

            var attemptedDate = $"{dayValue.AttemptedValue}/{monthValue.AttemptedValue}/{yearValue.AttemptedValue} {hourAttempted}:{minuteAttempted}:{secondAttempted}";

            DateTime value;

            if (DateTime.TryParse(attemptedDate, culture, DateTimeStyles.None, out value))
            {
                if (bindingContext.ModelState[bindingContext.ModelName] == null)
                {
                    bindingContext.ModelState[bindingContext.ModelName] = new ModelState();
                }

                bindingContext.ModelState[bindingContext.ModelName].Value = new ValueProviderResult(attemptedDate, value.ToString(), culture);

                return value;
            }

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"{bindingContext.ModelName} is not a valid Date");

            bindingContext.ModelState[bindingContext.ModelName].Value = new ValueProviderResult(attemptedDate, attemptedDate, CultureInfo.InvariantCulture);

            return attemptedDate;
        }

        #endregion
    }
}