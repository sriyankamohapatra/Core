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
            var dateValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var dayValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Day");
            var monthValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Month");
            var yearValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Year");
            var hourValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Hour");
            var minuteValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Minute");
            var secondValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Second");

            DateTime value;
            string attemptedDate;

            // Drop out here to the default if the known day, month year can't be found.
            if (dayValue == null && monthValue == null && yearValue == null && dateValue == null)
            {
                var defaultResult = new DefaultModelBinder().BindModel(controllerContext, bindingContext);
                return defaultResult;
            }

            if (dayValue != null || monthValue != null || yearValue != null)
            {
                var hourAttempted = "00";
                if (IsDoubleDigitNumber(hourValue))
                {
                    hourAttempted = hourValue.AttemptedValue;
                }

                var minuteAttempted = "00";
                if (IsDoubleDigitNumber(minuteValue))
                {
                    minuteAttempted = minuteValue.AttemptedValue;
                }

                var secondAttempted = "00";
                if (IsDoubleDigitNumber(secondValue))
                {
                    secondAttempted = secondValue.AttemptedValue;
                }

                attemptedDate = $"{dayValue?.AttemptedValue.Trim()}/{monthValue?.AttemptedValue.Trim()}/{yearValue?.AttemptedValue.Trim()} {hourAttempted}:{minuteAttempted}:{secondAttempted}";

                if (attemptedDate == "// 00:00:00")
                {
                    var defaultResult = new DefaultModelBinder().BindModel(controllerContext, bindingContext);
                    return defaultResult;
                }
            }
            else
            {
                attemptedDate = dateValue.AttemptedValue;
            }

            if (DateTime.TryParse(attemptedDate, culture, DateTimeStyles.None, out value))
            {
                if (bindingContext.ModelState[bindingContext.ModelName] == null)
                {
                    bindingContext.ModelState[bindingContext.ModelName] = new ModelState();
                }

                bindingContext.ModelState[bindingContext.ModelName].Value = new ValueProviderResult(attemptedDate, value.ToString(culture), culture);

                return value;
            }

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"{bindingContext.ModelMetadata.DisplayName} is not a valid Date");

            bindingContext.ModelState[bindingContext.ModelName].Value = new ValueProviderResult(attemptedDate, attemptedDate, culture);

            return attemptedDate;
        }

        private bool IsDoubleDigitNumber(ValueProviderResult number)
        {
            int dummy;
            return !string.IsNullOrWhiteSpace(number?.AttemptedValue) && number.AttemptedValue.Length == 2 && int.TryParse(number.AttemptedValue, out dummy);
        }

        #endregion
    }
}