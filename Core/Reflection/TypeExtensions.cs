﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sfa.Core.Reflection
{
    /// <summary>
    /// Extensions available for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns all the fields within the types hierarchy.
        /// </summary>
        /// <param name="targetType">The type whose fields are to be returned</param>
        /// <returns>A list of all the fields within the Types hierarchy.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="targetType"/> is <c>null</c>.</exception>
        public static IList<FieldInfo> GetAllFields(this Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            var fieldList = new List<FieldInfo>();
            while (targetType != null)
            {
                fieldList.AddRange(targetType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
                targetType = targetType.BaseType;
            }

            return fieldList;
        }

        /// <summary>
        /// Returns all the fields within the types hierarchy that don't have the attribute defined on them.
        /// </summary>
        /// <typeparam name="T">The <see ref="System.Type"/> of the <see ref="System.Attribute"> to find.</see></typeparam>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>A list of all the fields within the Types hierarchy that don't have the attribute defined.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="targetType"/> is <c>null</c>.</exception>
        public static IEnumerable<FieldInfo> GetAllFieldsWithoutAttribute<T>(this Type targetType) 
            where T : Attribute
        {
            return targetType.GetAllFields().Where(field => !field.IsDefined<T>());
        } 
    }
}