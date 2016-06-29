using System;
using System.ComponentModel.DataAnnotations;

namespace Sfa.Core.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Ensures that where the property that this attribute is applied, that the other named property has 
    /// a date that is before this ones.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DateAfterAttribute : ValidationAttribute
    {
        /// <summary>
        /// The property name of the property on the model to which this Date will be compared against.
        /// </summary>
        public string OtherOtherPropertyName { get; }

        /// <summary>
        /// The full constructor.
        /// </summary>
        /// <param name="otherPropertyName">The property against which to compare.</param>
        /// <param name="errorString">The error message for validation.</param>
        public DateAfterAttribute(string otherPropertyName, string errorString)
            : base(errorString)
        {
            OtherOtherPropertyName = otherPropertyName;
        }
        /// <summary>
        /// The constructor using the standard error message.
        /// </summary>
        /// <param name="otherPropertyName">The property against which to compare.</param>
        public DateAfterAttribute(string otherPropertyName)
            : this(otherPropertyName, $"{{0}} should be after {otherPropertyName}")
        {
        }

        protected override ValidationResult IsValid(object myValue, ValidationContext validationContext)
        {
            var property = validationContext?.ObjectType?.GetProperty(OtherOtherPropertyName);
            if (property == null)
            {
                return new ValidationResult($"Unknown property {OtherOtherPropertyName}");
            }

            var otherValue = property.GetValue(validationContext.ObjectInstance, null);

            DateTime otherVaueAsDate;
            DateTime myValueAsDate;

            if (myValue == null || otherValue == null || !DateTime.TryParse(myValue.ToString(), out myValueAsDate) || !DateTime.TryParse(otherValue.ToString(), out otherVaueAsDate))
            {
                return null;
            }

            if (myValueAsDate <= otherVaueAsDate)
            {
                return new ValidationResult(ErrorMessage);
            }

            return null;
        }
    }

}