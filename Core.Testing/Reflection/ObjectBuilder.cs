using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sfa.Core.Reflection
{
    /// <summary>
    /// Base class for a builder.
    /// </summary>
    /// <typeparam name="T">The type of the object to build.</typeparam>
    public abstract class ObjectBuilder<T>
    {
        #region Fields

        protected static readonly string LoggerCategory = typeof(ObjectBuilder<T>).FullName;
        protected T Target;

        #endregion


        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="ObjectBuilder{T}"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        protected ObjectBuilder(T defaultValue)
        {
            Target = defaultValue;
        }

        #endregion


        #region Mutators - single-valued fields.

        /// <summary>
        /// Sets the specified property value specified in the expression.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        public ObjectBuilder<T> Set<TValue>(Expression<Func<T, TValue>> expression, TValue value)
        {
            Target.SetPropertyValue(expression, value);

            return this;
        }

        #endregion


        #region Mutators - list-valued fields.

        /// <summary>
        /// Replaces the specified expression.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        public ObjectBuilder<T> Replace<TValue>(Expression<Func<T, IList<TValue>>> expression, TValue value)
        {
            var listField = GetListField(expression);
            listField.Clear();
            listField.Add(value);

            return this;
        }

        /// <summary>
        /// Prepends the specified expression.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        public ObjectBuilder<T> Prepend<TValue>(Expression<Func<T, IList<TValue>>> expression, TValue value)
        {
            GetListField(expression).Insert(0, value);

            return this;
        }

        /// <summary>
        /// Appends the specified expression.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="value">The value.</param>
        /// <returns>This instance.</returns>
        public ObjectBuilder<T> Append<TValue>(Expression<Func<T, IList<TValue>>> expression, TValue value)
        {
            GetListField(expression).Add(value);

            return this;
        }

        private IList<TValue> GetListField<TValue>(Expression<Func<T, IList<TValue>>> expression)
        {
            return (IList<TValue>)Target.GetPropertyInfo(expression).GetValue(Target, null);
        }

        #endregion


        #region Building

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>The build object.</returns>
        public T Build()
        {
            return Target;
        }

        /// <summary>
        /// Builds from.
        /// </summary>
        /// <param name="toCopy">To copy.</param>
        /// <returns>A new builder from the original source.</returns>
        public ObjectBuilder<T> BuildFrom(T toCopy)
        {
            Target = ObjectCopier.Copy(toCopy);

            return this;
        }

        /// <summary>
        /// Builds as.
        /// </summary>
        /// <param name="toUse">To use.</param>
        /// <returns>A new builder with the target set as the source.</returns>
        public ObjectBuilder<T> BuildAs(T toUse)
        {
            Target = toUse;

            return this;
        }

        #endregion
    }
}