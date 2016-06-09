using System;

namespace Sfa.Core
{
    /// <summary>
    /// Enum utilities
    /// </summary>
    public static class EnumTranslator
    {
        /// <summary>
        /// Enum utility for translating a int into a enum of a specified type or using a default value if
        /// the parsing of the int fails.
        /// </summary>
        /// <typeparam name="T">The type of the enum to translate the value into.</typeparam>
        /// <param name="value">The value to translate.</param>
        /// <param name="default">The value to use if the parsing of the int fails.</param>
        /// <returns>The translated enum.</returns>
        public static T Translate<T>(int? value, T @default)
            where T : struct // Would love to make this Enum but the complier currently doesn't allow this.
        {
            if (!value.HasValue || !Enum.IsDefined(typeof(T), value.Value))
            {
                return @default;
            }

            return (T)Enum.ToObject(typeof(T), value);
        }
    }
}