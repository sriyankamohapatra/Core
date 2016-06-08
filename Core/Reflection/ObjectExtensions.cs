using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace Sfa.Core.Reflection
{
    /// <summary>
    /// Extension available to all <see cref="Object"/>s
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets the propertyInfo for a given property in an instance.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="expression">The property.</param>
        /// <returns>The property info for the expression provided.</returns>
        public static PropertyInfo GetPropertyInfo<TInstance, TProperty>(this TInstance instance, Expression<Func<TInstance, TProperty>> expression)
        {
            var body = expression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("The provided lambda expression must refer to a property of : " + typeof(TInstance).Name);
            }

            var member = body.Member as PropertyInfo;
            if (member == null)
            {
                throw new ArgumentException("The provided lambda expression must refer to a property of : " + typeof(TInstance).Name);
            }

            return member;
        }

        /// <summary>
        /// Sets the specified value against the instance's property.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyGetExpression">The property get expression.</param>
        /// <param name="value">The value.</param>
        /// <returns>The instance.</returns>
        public static TInstance SetPropertyValue<TInstance, TProperty>(this TInstance instance, Expression<Func<TInstance, TProperty>> propertyGetExpression, TProperty value)
        {
            propertyGetExpression.EnsureProperty();

            // if the property is nested, i.e. target.level1.level2...levelN.Property
            // then we need to recurse through the property to get to LevelN so the we can se the value on this 
            // instance to the property value.

            // First, lets check that the lambda is indeed a property access
            var memberSelectorExpression = propertyGetExpression.Body as MemberExpression;

            // we have our top level member access which is (target.level1.level2...levelN) pointing to Property
            var currentTarget = instance as object;

            var stack = new Stack<MemberExpression>();
            var currentMemberAccess = memberSelectorExpression;
            while (currentMemberAccess != null)
            {
                stack.Push(currentMemberAccess);
                currentMemberAccess = currentMemberAccess.Expression as MemberExpression;
            }

            while (stack.Count > 0)
            {
                var property = (PropertyInfo)stack.Pop().Member;

                if (stack.Count != 0)
                {
                    currentTarget = property.GetValue(currentTarget, null);
                }
                else
                {
                    property.SetValue(currentTarget, value, null);
                }
            }

            return instance;




            //var entityParameterExpression = (ParameterExpression)(((MemberExpression)(propertyGetExpression.Body)).Expression);
            //var valueParameterExpression = Expression.Parameter(typeof(TProperty));

            //var setter = Expression.Lambda<Action<TInstance, TProperty>>(
            //    Expression.Assign(propertyGetExpression.Body, valueParameterExpression),
            //    entityParameterExpression,
            //    valueParameterExpression);

            //setter.Compile().Invoke(instance, value);
            //return instance;
        }

        /// <summary>
        /// Returns all the fields within the objects hierarchy that don't have the attribute defined on them.
        /// </summary>
        /// <typeparam name="T">The <see ref="System.Type"/> of the <see ref="System.Attribute"> to find.</see></typeparam>
        /// <param name="target">The target.</param>
        /// <returns>A list of all the fields within the targets hierarchy that don't have the attribute defined.</returns>
        public static IEnumerable<FieldInfo> GetAllFieldsWithoutAttribute<T>(this object target)
            where T : Attribute
        {
            return (target.GetType().GetAllFieldsWithoutAttribute<T>());
        }


        /// <summary>
        /// Gets a private field through reflection.
        /// </summary>
        /// <param name="type">The type who owns the private field.</param>
        /// <param name="fieldName">The name of the private field to find.</param>
        /// <remarks>Using hard coded strings and accessing fields by their names leads to brittle code. Use with caution"</remarks>
        /// <returns>The private field info for the provided name.</returns>
        /// <exception cref="ArgumentException">When no matching field can be found.</exception>
        public static FieldInfo GetPrivateField(this Type type, string fieldName)
        {
            Contract.Requires(type != null);
            Contract.Requires(fieldName != null);

            var fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic);
            if (fieldInfo == null)
            {
                throw new ArgumentException($"The private field {fieldName} cannot be found on type {type}");
            }
            return fieldInfo;
        }


        /// <summary>
        /// Gets the value from a private field for an object instance.
        /// </summary>
        /// <param name="owner">The instance whose value we want to find from a private field.</param>
        /// <param name="fieldName">The name of the private field to get the value from.</param>
        /// <returns>The value from the matching field.</returns>
        public static object GetPrivateFieldValue(this object owner, string fieldName)
        {
            return owner.GetType().GetPrivateField(fieldName).GetValue(owner);
        }


        /// <summary>
        /// Sets a private fields value for an instance.
        /// </summary>
        /// <param name="owner">The instance to set the new value on.</param>
        /// <param name="fieldName">The name of the private field to set.</param>
        /// <param name="value">The value to be set in the field.</param>
        /// <returns>The owner instance.</returns>
        public static T SetPrivateFieldValue<T>(this T owner, string fieldName, object value)
        {
            owner.GetType().GetPrivateField(fieldName).SetValue(owner, value);
            return owner;
        }
    }
}