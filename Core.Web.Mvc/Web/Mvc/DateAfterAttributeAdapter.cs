using Sfa.Core.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Sfa.Core.Web.Mvc
{
    public class DateAfterAttributeAdapter : DataAnnotationsModelValidator<DateAfterAttribute>
    {
        public DateAfterAttributeAdapter(ModelMetadata metadata, ControllerContext context, DateAfterAttribute attribute) : base(metadata, context, attribute)
        {
        }

        public override System.Collections.Generic.IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var validationRule = new ModelClientValidationRule
            {
                ErrorMessage = Attribute.FormatErrorMessage(Metadata.GetDisplayName()),
                ValidationType = "dateafter"
            };

            validationRule.ValidationParameters.Add("othername", Attribute.OtherPropertyNameToBeAfter);

            yield return validationRule;
        }
    }

}
