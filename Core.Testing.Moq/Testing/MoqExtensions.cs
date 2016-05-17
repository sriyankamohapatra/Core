using System;
using System.Collections;
using System.Linq.Expressions;
using Moq;
using Moq.Language.Flow;

namespace Sfa.Core.Testing
{
    /// <summary>
    /// Extends Moq functionality to override the default EqualityComprare used for checking equality.
    /// </summary>
    public static class MoqExtensions
    {
        private static IEqualityComparer _equalityComparer;

        /// <summary>
        /// Used to override the equality comparer.
        /// </summary>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer"/> to use instead of the default object equality.</param>
        public static void SetEqualityComparer(IEqualityComparer equalityComparer)
        {
            if (equalityComparer == null)
            {
                throw new ArgumentNullException(nameof(equalityComparer));
            }

            _equalityComparer = equalityComparer;
        }

        /// <summary>
        /// Clears the current equality comparer.
        /// </summary>
        public static void ClearEqualityComparer()
        {
            _equalityComparer = null;
        }

        /// <summary>
        /// The Moq setup method to use when you want to use the custom equality compare
        /// </summary>
        /// <typeparam name="T">The type of the mock that is being setup against.</typeparam>
        /// <param name="mock">The mock instance.</param>
        /// <param name="expression">The expression expected to be called.</param>
        /// <returns>The setup mock expectation.</returns>
        public static ISetup<T> Expected<T>(this Mock<T> mock, Expression<Action<T>> expression)
            where T : class
        {
            if (mock == null)
            {
                throw new ArgumentNullException(nameof(mock));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (_equalityComparer == null)
            {
                throw new ArgumentException("You must set the custom equality comparer to use this method");
            }

            var updatedExpression = (Expression<Action<T>>)new MyExpressionVisitor().Visit(expression);
            return mock.Setup(updatedExpression);
        }

        /// <summary>
        /// The Moq setup method to use when you want to use the custom equality compare
        /// </summary>
        /// <typeparam name="T">The type of the mock that is being setup against.</typeparam>
        /// <typeparam name="TReturn">The type of the return object.</typeparam>
        /// <param name="mock">The mock instance.</param>
        /// <param name="expression">The expression expected to be called.</param>
        /// <returns>The setup mock expectation.</returns>
        public static ISetup<T, TReturn> Expected<T, TReturn>(this Mock<T> mock, Expression<Func<T, TReturn>> expression)
            where T : class
        {
            if (mock == null)
            {
                throw new ArgumentNullException(nameof(mock));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (_equalityComparer == null)
            {
                throw new ArgumentException("You must set the custom equality comparer to use this method");
            }

            var updatedExpression = (Expression<Func<T, TReturn>>)new MyExpressionVisitor().Visit(expression);
            return mock.Setup(updatedExpression);
        }

        /// <summary>
        /// Compare members based on the custom equality comparison.
        /// </summary>
        /// <typeparam name="T">The type of the instance that is being compared.</typeparam>
        /// <param name="instance">The actual instance to compare against.</param>
        /// <returns>The matched instance.</returns>
        public static T CustomEqualityCompare<T>(this T instance)
        {
            if (_equalityComparer == null)
            {
                throw new ArgumentException("You must set the custom equality comparer to use this method");
            }

            return It.Is<T>(t => _equalityComparer.Equals(instance, t));
        }

        /// <summary>
        /// Private class used to update constant node expressions so that they use the overridden <see cref="IEqualityComparer"/> instead.
        /// </summary>
        private class MyExpressionVisitor : ExpressionVisitor
        {
            /// <summary>
            /// Visits member expressions.
            /// </summary>
            /// <param name="node">The node to visit.</param>
            /// <returns>The new updated or existing node depending on the type of node being visited.</returns>
            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression.NodeType == ExpressionType.Constant || node.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    // We have to do this to get access to the base value and not the wrapped value
                    var param = Expression.Constant(Expression.Lambda(node).Compile().DynamicInvoke());
                    var call = Expression.Call(typeof(MoqExtensions), "CustomEqualityCompare", new[] { node.Type }, param);

                    return call;
                }

                return base.VisitMember(node);
            }
        }
    }
}