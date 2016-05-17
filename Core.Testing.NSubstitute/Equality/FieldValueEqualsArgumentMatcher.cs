using NSubstitute.Core.Arguments;

namespace Sfa.Core.Equality
{
    /// <summary>
    /// An <see cref="IArgumentMatcher"/> implementation that uses field value equality to compare arguments.
    /// </summary>
    public class FieldValueEqualsArgumentMatcher : IArgumentMatcher
    {
        private static readonly ArgumentFormatter DefaultArgumentFormatter = new ArgumentFormatter();
        private readonly object _value;


        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">The value to compare against.</param>
        public FieldValueEqualsArgumentMatcher(object value)
        {
            _value = value;
        }

        /// <summary>
        /// Returns a string representation of the base expected value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DefaultArgumentFormatter.Format(_value, false);
        }

        /// <summary>
        /// Flag indicating if the argument matches the base value.
        /// </summary>
        /// <param name="argument">The new argument to check against.</param>
        /// <returns><c>true</c> if they match; otherwise, <c>false</c>.</returns>
        public bool IsSatisfiedBy(object argument)
        {
            return new FieldValueEqualityComparer().Equals(_value, argument);
        }
    }
}