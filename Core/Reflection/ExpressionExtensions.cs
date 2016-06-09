using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Sfa.Core.Reflection
{
    /// <summary>
    /// Extension available of the <see cref="Expression"/> class.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="TInstance">The type of the value object.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The propertyExpression.</param>
        /// <returns>The name of the property.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if the expression is not for a property.</exception>
        public static string GetPropertyName<TInstance, TProperty>(this Expression<Func<TInstance, TProperty>> propertyExpression)
        {
            return EnsureProperty(propertyExpression).Member.Name;
        }

        /// <summary>
        /// Gets the PropertyInfo for this property.
        /// </summary>
        /// <typeparam name="TInstance">The type of the value object.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>The PropertyInfo for the property.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if the expression is not for a property.</exception>
        public static PropertyInfo GetProperty<TInstance, TProperty>(this Expression<Func<TInstance, TProperty>> propertyExpression)
        {
            return typeof(TInstance).GetProperty(propertyExpression.GetPropertyName());
        }

        /// <summary>
        /// Gets the property backing field.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property.</param>
        /// <returns>The fieldInfo for the given property.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if the expression is not for a property.</exception>
        /// <exception cref="System.Exception">Throw when the field cannot be found for the given property.</exception>
        public static FieldInfo GetPropertyBackingField<TInstance, TProperty>(this Expression<Func<TInstance, TProperty>> propertyExpression)
        {
            propertyExpression.EnsureProperty();

            var propertyInfo = propertyExpression.GetProperty();
            var propertyName = propertyInfo.Name;   

            var fieldName = $"_{propertyName.Substring(0, 1).ToLower()}{propertyName.Substring(1)}";
            var field = typeof(TInstance).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
            {
                throw new Exception($"The backing field {fieldName} for Property {propertyName} can not be found. Have you given the backing field a different name to the property?");
            }

            return field;
        }

        #region Helpers

        /// <summary>
        /// Ensures that the expression is for a property.
        /// </summary>
        /// <typeparam name="TInstance">The type of the object that contains the property.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>The <see cref="MemberExpression"/> that represents the property.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if the expression is not for a property.</exception>
        public static MemberExpression EnsureProperty<TInstance, TProperty>(this Expression<Func<TInstance, TProperty>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("The expression must be a property expression.", nameof(propertyExpression));
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The provided lambda expression must refer to a property of : " + typeof(TInstance).Name);
            }

            return memberExpression;
        }

        #endregion
    }
}