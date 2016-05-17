using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Sfa.Core.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DateAfterAttribute : ValidationAttribute
    {
        public string OtherPropertyNameToBeAfter { get; }

        /// <summary>
        /// The full constructor.
        /// </summary>
        /// <param name="propertyNameToCompare">The property against which to compare.</param>
        /// <param name="errorString">The error message for validation.</param>
        public DateAfterAttribute(string propertyNameToCompare, string errorString)
            : base(errorString)
        {
            OtherPropertyNameToBeAfter = propertyNameToCompare;
        }
        /// <summary>
        /// The full constructor.
        /// </summary>
        /// <param name="propertyNameToCompare">The property against which to compare.</param>
        public DateAfterAttribute(string propertyNameToCompare)
            : base($"{{0}} should be after {propertyNameToCompare}")
        {
            OtherPropertyNameToBeAfter = propertyNameToCompare;
        }

        protected override ValidationResult IsValid(object myValue, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(OtherPropertyNameToBeAfter);
            if (property == null)
            {
                return new ValidationResult($"Unknown property {OtherPropertyNameToBeAfter}");
            }

            var otherValue = property.GetValue(validationContext.ObjectInstance, null);

            DateTime otherVaueAsDate;
            DateTime myValueAsDate;

            if (!DateTime.TryParseExact(myValue.ToString(), "d/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out myValueAsDate)
                ||!DateTime.TryParseExact(otherValue.ToString(), "d/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out otherVaueAsDate))
            {
                return null;
            }

            if (myValue != null && otherValue != null 
                && DateTime.ParseExact(myValue.ToString(), "d/M/yyyy", CultureInfo.InvariantCulture) <= DateTime.ParseExact(otherValue.ToString(), "d/M/yyyy", CultureInfo.InvariantCulture))
            {
                return new ValidationResult(ErrorMessage);
            }

            return null;
        }
    }

}