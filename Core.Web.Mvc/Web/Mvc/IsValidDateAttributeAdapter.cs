using Sfa.Core.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Sfa.Core.Web.Mvc
{
    public class IsValidDateAttributeAdapter : DataAnnotationsModelValidator<IsValidDateAttribute>
    {
        public IsValidDateAttributeAdapter(ModelMetadata metadata, ControllerContext context, IsValidDateAttribute attribute) : base(metadata, context, attribute)
        {
        }

        public override System.Collections.Generic.IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var validationRule = new ModelClientValidationRule
            {
                ErrorMessage = Attribute.FormatErrorMessage(Metadata.GetDisplayName()),
                ValidationType = "isvaliddate"
            };
            yield return validationRule;
        }
    }

}
