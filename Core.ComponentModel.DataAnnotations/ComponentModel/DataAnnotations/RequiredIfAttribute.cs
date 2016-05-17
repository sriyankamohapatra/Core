using System;
using System.ComponentModel.DataAnnotations;

namespace Sfa.Core.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]

    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _propertyNameRequiredFor;
        private readonly string _requiredIfPropertyValue;
        private readonly string _errorstring;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="propertyNameRequiredFor">The property within the same type on which the value this property value is dependant on.</param>
        /// <param name="requiredIfPropertyValue">The value of the property when this instance becomes required.</param>
        /// <param name="errorString">The validation error message.</param>

        public RequiredIfAttribute(string propertyNameRequiredFor, string requiredIfPropertyValue, string errorString)
        {
            _propertyNameRequiredFor = propertyNameRequiredFor;
            _requiredIfPropertyValue = requiredIfPropertyValue;
            _errorstring = errorString;
        }
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_propertyNameRequiredFor);
            var fieldValue = property.GetValue(validationContext.ObjectInstance, null);

            if (_requiredIfPropertyValue.Equals(fieldValue?.ToString()) && string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult(_errorstring);
            }

            return null;
        }
    }
}