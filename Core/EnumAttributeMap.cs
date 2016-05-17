using System;
using System.Collections.Generic;
using System.Linq;

namespace Sfa.Core
{
    /// <summary>
    /// Represents access to the attributes attached to Enum values.
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
        public EnumAttributeMap(object enumValue, IList<Attribute> attributes)
        {
            EnumValue = enumValue;
            Attributes = attributes;
        }

        #endregion


        #region Public API

        /// <summary>
        /// Gets the first attribute of the type specified.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns>The first attribute of the type specified or <c>null</c> if no attribute of that type exists.</returns>
        public TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
        {
            return (TAttribute)Attributes.FirstOrDefault(a => a is TAttribute);
        }

        #endregion
    }
}