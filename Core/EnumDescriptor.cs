using System;
using System.Collections.Generic;
using System.Linq;

namespace Sfa.Core
{

    /// <summary>
    /// Provides a descriptive functionality for Enums through
    /// the use of a custom attribute.
    /// </summary>
    public static class EnumDescriptor
    {
        /// <summary>
        /// Gets the value from the attribute based on the property.
        /// </summary>
        /// <param name="value">The enum to get the attribute and value from.</param>
        /// <param name="property">The property to get the value from.</param>
        /// <returns>The value from the attribute.</returns>
        public static string GetPropertyValue<TEnum, TAttribute>(this TEnum value, Func<TAttribute, string> property)
            where TAttribute : EnumDescriptorAttribute
        {
            return value.GetPropertyValue<TEnum, TAttribute, string>(property);
        }

        /// <summary>
        /// Gets the value from the attribute based on the property.
        /// </summary>
        /// <param name="value">The enum to get the attribute and value from.</param>
        /// <param name="property">The property to get the value from.</param>
        /// <returns>The value from the attribute.</returns>
        public static TValue GetPropertyValue<TEnum, TAttribute, TValue>(this TEnum value, Func<TAttribute, TValue> property)
            where TAttribute : EnumDescriptorAttribute
        {
            var attribute = value.GetEnumValueAttribute<TAttribute>();

            if (attribute == null)
            {
                throw new NotImplementedException($"Missing attribute of type {typeof(TAttribute)} on enum {typeof(TEnum)} for value {value}");
            }

            return property(attribute);
        }

        /// <summary>
        /// Gets the enum from attribute property value.
        /// </summary>
        /// <param name="propertyValue">The value that should match up with a property.</param>
        /// <param name="property">The property on the attribute to match with.</param>
        /// <returns>The matched enum or the default if no match.</returns>
        public static TEnum GetEnumFromPropertyValue<TEnum, TAttribute>(this string propertyValue, Func<TAttribute, string> property)
            where TAttribute : EnumDescriptorAttribute
        {
            var returnValue = default(TEnum);
            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                var fi = enumValue.GetType().GetField(enumValue.ToString());
                var attributes = (TAttribute[])fi.GetCustomAttributes(typeof(TAttribute), false);

                if (attributes.Length > 0 && property(attributes[0]).Equals(propertyValue))
                {
                    returnValue = enumValue;
                    break;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Gets specified attribute defined on specified enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue">The target.</param>
        /// <returns></returns>
        public static T GetEnumValueAttribute<T>(this object enumValue) where T : Attribute
        {
            return enumValue.GetEnumCustomAttributes<T>().FirstOrDefault();
        }

        private static IEnumerable<T> GetEnumCustomAttributes<T>(this object enumValue) where T : Attribute
        {
            return enumValue
                .GetType()
                .GetEnumAttributeMaps()
                .For(enumValue)
                .Attributes
                .OfType<T>();
        }

    }
}