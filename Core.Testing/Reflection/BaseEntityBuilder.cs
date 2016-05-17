using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sfa.Core.Context;
using Sfa.Core.Data;
using Sfa.Core.Entities;

namespace Sfa.Core.Reflection
{
    /// <summary>
    /// Base class for building entities
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class BaseEntityBuilder<T, TId, TBuilder> : ObjectBuilder<T>
        where T : class, IEntity<TId>
        where TBuilder : BaseEntityBuilder<T, TId, TBuilder>
    {
        #region Constants

        // The Id assigned to those objects that are not yet persisted.
        protected TId UnsavedId = default(TId);

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntityBuilder{T, TId, TBuilder}"/> class.
        /// </summary>
        /// <param name="defaultBo">The default entity.</param>
        protected BaseEntityBuilder(T defaultBo)
            : base(defaultBo)
        {

        }

        #endregion


        #region Mutators - single-valued fields.

        /// <summary>
        /// Sets the specified expression.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public new TBuilder Set<TValue>(Expression<Func<T, TValue>> expression, TValue value)
        {
            return (TBuilder)base.Set(expression, value);
        }

        /// <summary>
        /// Sets the maximum Id from the repository for this builders build type.
        /// </summary>
        /// <returns>The builder instance.</returns>
        /// <exception cref="System.Exception">The current TestContext Repository doesn't support this functionality</exception>
        public TBuilder WithMaxId(Expression<Func<T, TId>> idSetter)
        {
            var maxIdQueryable = TestContext.Repository as IMaxEntityId;

            if (maxIdQueryable == null)
            {
                throw new Exception("The current TestContext Repository doesn't support this functionality");
            }

            return Set(idSetter, maxIdQueryable.GetMaxId<T, TId>());
        }

        #endregion


        #region Mutators - list-valued fields.

        /// <summary>
        /// Replaces the specified expression.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public new TBuilder Replace<TValue>(Expression<Func<T, IList<TValue>>> expression, TValue value)
        {
            return (TBuilder)base.Replace(expression, value);
        }

        /// <summary>
        /// Prepends the specified expression.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public new TBuilder Prepend<TValue>(Expression<Func<T, IList<TValue>>> expression, TValue value)
        {
            return (TBuilder)base.Prepend(expression, value);
        }

        /// <summary>
        /// Appends the specified expression.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public new TBuilder Append<TValue>(Expression<Func<T, IList<TValue>>> expression, TValue value)
        {
            return (TBuilder)base.Append(expression, value);
        }

        #endregion


        #region Build
        

        /// <summary>
        /// Returns a flag stating id the entity has been persisted.
        /// </summary>
        protected virtual bool AmIPersisted => !Equals(Target.Id, UnsavedId);


        protected virtual void OnBeforePersistSelf()
        {

        }

        /// <summary>
        /// Builds from.
        /// </summary>
        /// <param name="toCopy">To copy.</param>
        /// <param name="resetId"><c>true</c> if the Id of the new instance should be reset; otherwise, <c>false</c>, which is the default.</param>
        /// <returns></returns>
        public TBuilder BuildFrom(T toCopy, bool resetId = false)
        {
            if (toCopy == null)
            {
                throw new NullReferenceException("toCopy cannot be null in BoBuilder.BuildFrom");
            }

            var builder = (TBuilder)base.BuildFrom(toCopy);
            if (resetId)
            {
                builder.Set(o => o.Id, UnsavedId);
            }

            return builder;
        }

        /// <summary>
        /// Builds as.
        /// </summary>
        /// <param name="toUse">To use.</param>
        /// <returns></returns>
        public new TBuilder BuildAs(T toUse)
        {
            return (TBuilder)base.BuildAs(toUse);
        }

        #endregion
    }
}