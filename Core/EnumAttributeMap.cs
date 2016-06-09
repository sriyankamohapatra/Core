using System;
using System.Collections.Generic;
using System.Linq;

namespace Sfa.Core
{
    /// <summary>
    /// Represents access to the attributes attached to Enum values.
    /// Used for performance so that the values can be cached in memory.
    /// </summary>
    public class EnumAttributeMap
    {
        #region Core Properties

        /// <summary>
        /// Gets the enum value.
        /// </summary>
        /// <value>
        /// The enum value.
        /// </value>
        public object EnumValue { get; private set; }

        /// <summary>
        /// Gets the attributes that are attached to the EnumValue.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public IList<Attribute> Attributes { get; private set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumAttributeMap"/> class.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="attributes">The attributes.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="enumValue"/> or <paramref name="attributes"/> are null.</exception>
        public EnumAttributeMap(Enum enumValue, IEnumerable<Attribute> attributes)
        {
            if (enumValue == null)
            {
                throw new ArgumentNullException(nameof(enumValue));
            }
            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            EnumValue = enumValue;
            Attributes = attributes.ToList();
        }

        #endregion


        #region Public API

        /// <summary>
        /// Gets the first attribute of the type specified.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns>The first attribute of the type specified or <c>null</c> if no attribute of that type exists.</returns>
        public TAttribute GetFirstAttribute<TAttribute>() 
            where TAttribute : Attribute
        {
            return Attributes.OfType<TAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the first attribute of the type specified.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns>The first attribute of the type specified or <c>null</c> if no attribute of that type exists.</returns>
        public IEnumerable<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            return Attributes.OfType<TAttribute>();
        }

        #endregion
    }
}