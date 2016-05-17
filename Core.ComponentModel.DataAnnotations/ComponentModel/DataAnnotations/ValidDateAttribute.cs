using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Sfa.Core.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IsValidDateAttribute : ValidationAttribute
    {
        /// <summary>
        /// The full constructor.
        /// </summary>
        /// <param name="propertyNameToCompare">The property against which to compare.</param>
        /// <param name="errorString">The error message for validation.</param>
        public IsValidDateAttribute(string errorString)
            : base(errorString)
        {
        }

        protected override ValidationResult IsValid(object myValue, ValidationContext validationContext)
        {
            DateTime myValueAsDate;

            if (!DateTime.TryParseExact(myValue.ToString(), "d/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out myValueAsDate))
            {
                return new ValidationResult($"{myValue.ToString()} is not a valid Date");
            }

            return null;
        }
    }

}