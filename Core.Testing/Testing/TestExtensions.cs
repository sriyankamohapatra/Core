using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Context;
using Sfa.Core.Entities;
using Sfa.Core.Equality;
using Sfa.Core.Reflection;
using TestContext = Sfa.Core.Context.TestContext;

namespace Sfa.Core.Testing
{
    /// <summary>
    /// Extensions to help with testing.
    /// </summary>
    public static class TestExtensions
    {
        /// <summary>
        /// Defines the amount of seconds to consider equal when testing that a <see cref="DateTime"/>
        /// is equal when the property has been specified for fuzzy testing.
        /// </summary>
        public static int DateTimeSecondsThreshold = 60;

        private const string FailedTestMessage = "Try using EnableComparisonLogging(); to see where the objects are different.";
        private const string FailedTestMessageForSingleValues = FailedTestMessage + " Expected {0}, Received {1}";

        #region Repository Comparison

        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <param name="dateTimeProperties">Any date time properties to fuzzy check against.</param>
        /// <returns>The instance. If any fuzzy date times were supplied, they will now be set to match those of the entity in the database.</returns>
        public static T ShouldBeInTheRespository<T>(this T entity, params Expression<Func<T, DateTime>>[] dateTimeProperties)
            where T : class, IEntity<int>
        {
            return entity.ShouldBeInTheRespository<T, int>(dateTimeProperties);
        }


        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <param name="dateTimeProperties">Any date time properties to fuzzy check against.</param>
        /// <returns>The instance. If any fuzzy date times were supplied, they will now be set to match those of the entity in the database.</returns>
        public static T ShouldBeInTheRespository<T>(this T entity, params Expression<Func<T, DateTime?>>[] dateTimeProperties)
            where T : class, IEntity<int>
        {
            return entity.ShouldBeInTheRespository<T, int>(dateTimeProperties);
        }


        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <param name="nullableDateTimeProperties">Any nullable date time properties to fuzzy check against.</param>
        /// <param name="dateTimeProperties">Any date time properties to fuzzy check against.</param>
        /// <returns>The instance. If any fuzzy date times were supplied, they will now be set to match those of the entity in the database.</returns>
        public static T ShouldBeInTheRespository<T>(this T entity, Expression<Func<T, DateTime?>>[] nullableDateTimeProperties, Expression<Func<T, DateTime>>[] dateTimeProperties)
            where T : class, IEntity<int>
        {
            return entity.ShouldBeInTheRespository<T, int>(nullableDateTimeProperties, dateTimeProperties);
        }


        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <param name="afterload">An action that can be performed once the load from the repository has taken place, but before the equality check is performed.</param>
        /// <returns>The instance.</returns>
        public static T ShouldBeInTheRespository<T>(this T entity, Action<T> afterload = null)
            where T : class, IEntity<int>
        {
            return entity.ShouldBeInTheRespository<T, int>(afterload);
        }


        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <typeparam name="TId">The type of the Id property on the entity.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <param name="afterload">An action that can be performed once the load from the repository has taken place, but before the equality check is performed.</param>
        /// <returns>The instance.</returns>
        public static T ShouldBeInTheRespository<T, TId>(this T entity, Action<T> afterload = null)
            where T : class, IEntity<TId>
            where TId : IEquatable<TId>
        {
            //TestContext.Reset();
            var loaded = TestContext.Repository.Load<T, TId>(entity.Id);

            Assert.IsNotNull(loaded, "The entity of type {0} could not be found in the database", typeof(T));

            afterload?.Invoke(loaded);

            loaded.ShouldHaveSameValueAs(entity);

            return entity;
        }


        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <typeparam name="TId">The type of the Id property on the entity.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <param name="dateTimeProperties">Any date time properties to fuzzy check against.</param>
        /// <returns>The instance. If any fuzzy date times were supplied, they will now be set to match those of the entity in the database.</returns>
        public static T ShouldBeInTheRespository<T, TId>(this T entity, params Expression<Func<T, DateTime>>[] dateTimeProperties)
            where T : class, IEntity<TId>
            where TId : IEquatable<TId>
        {
            var loaded = TestContext.Repository.Load<T, TId>(entity.Id);

            Assert.IsNotNull(loaded, "The entity of type {0} could not be found in the database", typeof(T));
            loaded.ShouldHaveSameValueAs(entity, dateTimeProperties);

            return entity;
        }


        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <typeparam name="TId">The type of the Id property on the entity.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <param name="nullableDateTimeProperties">Any nullable date time properties to fuzzy check against.</param>
        /// <param name="dateTimeProperties">Any date time properties to fuzzy check against.</param>
        /// <returns>The instance. If any fuzzy date times were supplied, they will now be set to match those of the entity in the database.</returns>
        public static T ShouldBeInTheRespository<T, TId>(this T entity, Expression<Func<T, DateTime?>>[] nullableDateTimeProperties, Expression<Func<T, DateTime>>[] dateTimeProperties)
            where T : class, IEntity<TId>
            where TId : IEquatable<TId>
        {
            var loaded = TestContext.Repository.Load<T, TId>(entity.Id);

            Assert.IsNotNull(loaded, "The entity of type {0} could not be found in the database", typeof(T));
            loaded.ShouldHaveSameValueAs(entity, nullableDateTimeProperties, dateTimeProperties);

            return entity;
        }


        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <typeparam name="TId">The type of the Id property on the entity.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <param name="dateTimeProperties">Any date time properties to fuzzy check against.</param>
        /// <returns>The instance. If any fuzzy date times were supplied, they will now be set to match those of the entity in the database.</returns>
        public static T ShouldBeInTheRespository<T, TId>(this T entity, params Expression<Func<T, DateTime?>>[] dateTimeProperties)
            where T : class, IEntity<TId>
            where TId : IEquatable<TId>
        {
            var loaded = TestContext.Repository.Load<T, TId>(entity.Id);

            Assert.IsNotNull(loaded, "The entity of type {0} could not be found in the database", typeof(T));
            loaded.ShouldHaveSameValueAs(entity, dateTimeProperties);

            return entity;
        }


        /// <summary>
        /// Asserts that the entity should not exist in the database. Loads by Id.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <returns>The instance.</returns>
        public static T ShouldNotBeInTheRepository<T>(this T entity)
            where T : class, IEntity<int>
        {
            return entity.ShouldNotBeInTheRepository<T, int>();
        }


        /// <summary>
        /// Asserts that the entity should not exist in the database. Loads by Id.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <typeparam name="TId">The type of the Id property on the entity.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <returns>The instance.</returns>
        public static T ShouldNotBeInTheRepository<T, TId>(this T entity)
            where T : class, IEntity<TId>
            where TId : IEquatable<TId>
        {
            var loaded = TestContext.Repository.Load<T, TId>(entity.Id);

            Assert.IsNull(loaded, "The entity of type {0} should not be found in the database", typeof(T));

            return entity;
        }


        #endregion


        #region Async Repository Comparison

        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <param name="dateTimeProperties">Any date time properties to fuzzy check against.</param>
        /// <returns>The instance. If any fuzzy date times were supplied, they will now be set to match those of the entity in the database.</returns>
        public static async Task<T> ShouldBeInTheRespositoryAsync<T>(this T entity, params Expression<Func<T, DateTime>>[] dateTimeProperties)
            where T : class, IEntity<int>
        {
            return await entity.ShouldBeInTheRespositoryAsync<T, int>(dateTimeProperties).ConfigureAwait(false);
        }


        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <param name="dateTimeProperties">Any date time properties to fuzzy check against.</param>
        /// <returns>The instance. If any fuzzy date times were supplied, they will now be set to match those of the entity in the database.</returns>
        public static async Task<T> ShouldBeInTheRespositoryAsync<T>(this T entity, params Expression<Func<T, DateTime?>>[] dateTimeProperties)
            where T : class, IEntity<int>
        {
            return await entity.ShouldBeInTheRespositoryAsync<T, int>(dateTimeProperties).ConfigureAwait(false);
        }


        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <returns>The instance.</returns>
        public static async Task<T> ShouldBeInTheRespositoryAsync<T>(this T entity)
            where T : class, IEntity<int>
        {
            return await entity.ShouldBeInTheRespositoryAsync<T, int>().ConfigureAwait(false);
        }


        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <typeparam name="TId">The type of the Id property on the entity.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <returns>The instance.</returns>
        public static async Task<T> ShouldBeInTheRespositoryAsync<T, TId>(this T entity)
            where T : class, IEntity<TId>
            where TId : IEquatable<TId>
        {
            //TestContext.Reset();
            var loaded = await AsyncTestContext.Repository.LoadAsync<T, TId>(entity.Id);

            Assert.IsNotNull(loaded, "The entity of type {0} could not be found in the database", typeof(T));
            loaded.ShouldHaveSameValueAs(entity);

            return entity;
        }


        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <typeparam name="TId">The type of the Id property on the entity.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <param name="dateTimeProperties">Any date time properties to fuzzy check against.</param>
        /// <returns>The instance. If any fuzzy date times were supplied, they will now be set to match those of the entity in the database.</returns>
        public static async Task<T> ShouldBeInTheRespositoryAsync<T, TId>(this T entity, params Expression<Func<T, DateTime>>[] dateTimeProperties)
            where T : class, IEntity<TId>
            where TId : IEquatable<TId>
        {
            var loaded = await AsyncTestContext.Repository.LoadAsync<T, TId>(entity.Id);

            Assert.IsNotNull(loaded, "The entity of type {0} could not be found in the database", typeof(T));
            loaded.ShouldHaveSameValueAs(entity, dateTimeProperties);

            return entity;
        }


        /// <summary>
        /// Asserts that the entity should exist in the database. Loads by Id and then checks that the properties match.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <typeparam name="TId">The type of the Id property on the entity.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <param name="dateTimeProperties">Any date time properties to fuzzy check against.</param>
        /// <returns>The instance. If any fuzzy date times were supplied, they will now be set to match those of the entity in the database.</returns>
        public static async Task<T> ShouldBeInTheRespositoryAsync<T, TId>(this T entity, params Expression<Func<T, DateTime?>>[] dateTimeProperties)
            where T : class, IEntity<TId>
            where TId : IEquatable<TId>
        {
            var loaded = await AsyncTestContext.Repository.LoadAsync<T, TId>(entity.Id);

            Assert.IsNotNull(loaded, "The entity of type {0} could not be found in the database", typeof(T));
            loaded.ShouldHaveSameValueAs(entity, dateTimeProperties);

            return entity;
        }


        /// <summary>
        /// Asserts that the entity should not exist in the database. Loads by Id.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <returns>The instance.</returns>
        public static async Task<T> ShouldNotBeInTheRepositoryAsync<T>(this T entity)
            where T : class, IEntity<int>
        {
            return await entity.ShouldNotBeInTheRepositoryAsync<T, int>().ConfigureAwait(false);
        }


        /// <summary>
        /// Asserts that the entity should not exist in the database. Loads by Id.
        /// </summary>
        /// <typeparam name="T">The type of the entity to load.</typeparam>
        /// <typeparam name="TId">The type of the Id property on the entity.</typeparam>
        /// <param name="entity">The entity to check against.</param>
        /// <returns>The instance.</returns>
        public static async Task<T> ShouldNotBeInTheRepositoryAsync<T, TId>(this T entity)
            where T : class, IEntity<TId>
            where TId : IEquatable<TId>
        {
            var loaded = await AsyncTestContext.Repository.LoadAsync<T, TId>(entity.Id);

            Assert.IsNull(loaded, "The entity of type {0} should not be found in the database", typeof(T));

            return entity;
        }


        #endregion


        #region Value Comparison

        /// <summary>
        /// Asserts that the instances have the same field values.
        /// </summary>
        /// <typeparam name="T">The type of the instances.</typeparam>
        /// <param name="actual">The instance to check.</param>
        /// <param name="expected">The expected instance to test against.</param>
        /// <returns>The actual instance supplied.</returns>
        public static T ShouldHaveSameValueAs<T>(this T actual, T expected)
        {
            Assert.IsTrue(FieldValueEqualityComparer.AreEqual(expected, actual), FailedTestMessageForSingleValues, expected, actual);
            return actual;
        }


        /// <summary>
        /// Asserts that the instances have the same field values.
        /// </summary>
        /// <typeparam name="T">The type of the instances.</typeparam>
        /// <param name="actual">The instance to check.</param>
        /// <param name="expected">The expected instance to test against.</param>
        /// <param name="dateTimeProperties">Any date time properties to fuzzy check against.</param>
        /// <returns>The actual instance supplied. If any fuzzy date times were supplied, they will now be set to match those of the entity in the database.</returns>
        public static T ShouldHaveSameValueAs<T>(this T actual, T expected, params Expression<Func<T, DateTime>>[] dateTimeProperties)
        {
            AssertFuzzyDateEquals(actual, expected, dateTimeProperties);
            Assert.IsTrue(FieldValueEqualityComparer.AreEqual(expected, actual), FailedTestMessageForSingleValues, expected, actual);
            return actual;
        }


        /// <summary>
        /// Asserts that the instances have the same field values.
        /// </summary>
        /// <typeparam name="T">The type of the instances.</typeparam>
        /// <param name="actual">The instance to check.</param>
        /// <param name="expected">The expected instance to test against.</param>
        /// <param name="dateTimeProperties">Any date time properties to fuzzy check against.</param>
        /// <returns>The actual instance supplied. If any fuzzy date times were supplied, they will now be set to match those of the entity in the database.</returns>
        public static T ShouldHaveSameValueAs<T>(this T actual, T expected, params Expression<Func<T, DateTime?>>[] dateTimeProperties)
        {
            AssertFuzzyDateEquals(actual, expected, dateTimeProperties);
            Assert.IsTrue(FieldValueEqualityComparer.AreEqual(expected, actual), FailedTestMessageForSingleValues, expected, actual);
            return actual;
        }


        /// <summary>
        /// Asserts that the instances have the same field values.
        /// </summary>
        /// <typeparam name="T">The type of the instances.</typeparam>
        /// <param name="actual">The instance to check.</param>
        /// <param name="expected">The expected instance to test against.</param>
        /// <param name="nullableDateTimeProperties">Any nullable date time properties to fuzzy check against.</param>
        /// <param name="dateTimeProperties">Any date time properties to fuzzy check against.</param>
        /// <returns>The actual instance supplied. If any fuzzy date times were supplied, they will now be set to match those of the entity in the database.</returns>
        public static T ShouldHaveSameValueAs<T>(this T actual, T expected, Expression<Func<T, DateTime?>>[] nullableDateTimeProperties, Expression<Func<T, DateTime>>[] dateTimeProperties)
        {
            AssertFuzzyDateEquals(actual, expected, nullableDateTimeProperties, dateTimeProperties);
            Assert.IsTrue(FieldValueEqualityComparer.AreEqual(expected, actual), FailedTestMessageForSingleValues, expected, actual);
            return actual;
        }

        /// <summary>
        /// Asserts that the instances do not have the same field values.
        /// </summary>
        /// <typeparam name="T">The type of the instances.</typeparam>
        /// <param name="actual">The instance to check.</param>
        /// <param name="expected">The expected instance to test against.</param>
        /// <returns>The actual instance supplied.</returns>
        public static T ShouldNotHaveSameValueAs<T>(this T actual, T expected)
        {
            Assert.IsFalse(FieldValueEqualityComparer.AreEqual(expected, actual), FailedTestMessageForSingleValues, expected, actual);
            return actual;
        }
        
        #endregion


        #region List Comparison

        /// <summary>
        /// Asserts that the lists have the same instances in them by performing field comparisons.
        /// </summary>
        /// <typeparam name="T">The type of the instances.</typeparam>
        /// <param name="actualCollection">The actual collection to check.</param>
        /// <param name="expectedCollection">The collection of items expected to be found.</param>
        /// <param name="sortOrder">An optional list of functions that sorts the lists before comparison.</param>
        /// <returns>The actual list.</returns>
        public static IEnumerable<T> ShouldHaveSameValuesAs<T>(this IEnumerable<T> actualCollection, IEnumerable<T> expectedCollection, Expression<Func<IEnumerable<T>, IEnumerable<T>>> sortOrder = null)
        {
            if (sortOrder != null)
            {
                actualCollection = sortOrder.Compile().Invoke(actualCollection);
                expectedCollection = sortOrder.Compile().Invoke(expectedCollection);
            }

            Assert.IsTrue(FieldValueEqualityComparer.AreEqual(expectedCollection, actualCollection), FailedTestMessage);
            return actualCollection;
        }


        /// <summary>
        /// Asserts that the lists have the same instances in them by performing field comparisons.
        /// </summary>
        /// <typeparam name="T">The type of the instances.</typeparam>
        /// <param name="actualCollection">The actual collection to check.</param>
        /// <param name="expectedCollection">The collection of items expected to be found.</param>
        /// <returns>The actual list.</returns>
        public static IList<T> ShouldHaveSameValuesAs<T>(this IList<T> actualCollection, IList<T> expectedCollection)
        {
            Assert.IsTrue(FieldValueEqualityComparer.AreEqual(expectedCollection, actualCollection), FailedTestMessage);
            return actualCollection;
        }
        
        #endregion


        #region Fuzzy date time helpers

        private static void AssertFuzzyDateEquals<T>(T actual, T expected, params Expression<Func<T, DateTime>>[] dateTimeProperties)
        {
            foreach (var dateTimeProperty in dateTimeProperties)
            {
                AssertFuzzyDateEquals(actual, expected, dateTimeProperty);
            }
        }

        private static void AssertFuzzyDateEquals<T>(T actual, T expected, Expression<Func<T, DateTime?>>[] nullableDateTimeProperties, Expression<Func<T, DateTime>>[] dateTimeProperties)
        {
            foreach (var dateTimeProperty in dateTimeProperties)
            {
                AssertFuzzyDateEquals(actual, expected, dateTimeProperty);
            }
            foreach (var dateTimeProperty in nullableDateTimeProperties)
            {
                AssertFuzzyDateEquals(actual, expected, dateTimeProperty);
            }
        }

        private static void AssertFuzzyDateEquals<T>(T actual, T expected, Expression<Func<T, DateTime>> dateTimeProperty)
        {
            var func = dateTimeProperty.Compile();

            var actualValue = func(actual);
            var expectedValue = func(expected);

            Assert.IsTrue((actualValue - expectedValue).TotalSeconds < DateTimeSecondsThreshold, "The fuzzy check for the {0} values was outside the threshold of {1}s. Expected: {2}, but was actually {3}", dateTimeProperty.GetPropertyName(), DateTimeSecondsThreshold, expectedValue, actualValue);

            // Its within the range so we now set the expected to have the same value
            Console.WriteLine($"Updating the expected value for {dateTimeProperty.GetPropertyName()} on {actual.GetType()} to be {actualValue}");
            expected.SetPropertyValue(dateTimeProperty, actualValue);
        }

        private static void AssertFuzzyDateEquals<T>(T actual, T expected, params Expression<Func<T, DateTime?>>[] dateTimeProperties)
        {
            foreach (var dateTimeProperty in dateTimeProperties)
            {
                AssertFuzzyDateEquals(actual, expected, dateTimeProperty);
            }
        }

        private static void AssertFuzzyDateEquals<T>(T actual, T expected, Expression<Func<T, DateTime?>> dateTimeProperty)
        {
            var func = dateTimeProperty.Compile();

            var actualValue = func(actual);
            var expectedValue = func(expected);

            if (!actualValue.HasValue && !expectedValue.HasValue)
            {
                // Both null, so nothing more to do here
                return;
            }

            if (!actualValue.HasValue || !expectedValue.HasValue)
            {
                var nullInstance = actualValue.HasValue ? "expected" : "actual";
                var nonNullInstance = actualValue.HasValue ? "actual" : "expected";
                var value = actualValue ?? expectedValue.Value;

                Assert.Fail("The properties {0} are not equal, the {1} property is null, but the {2} has a value of {3}", dateTimeProperty.GetPropertyName(), nullInstance, nonNullInstance, value);
            }

            Assert.IsTrue((actualValue.Value - expectedValue.Value).TotalSeconds < DateTimeSecondsThreshold, "The fuzzy check for the {0} values was outside the threshold of {1}s", dateTimeProperty.GetPropertyName(), DateTimeSecondsThreshold);

            // Its within the range so we now set the expected to have the same value
            Console.WriteLine($"Updating the expected value for {dateTimeProperty.GetPropertyName()} on {actual.GetType()} to be {actualValue}");
            expected.SetPropertyValue(dateTimeProperty, actualValue);
        }
        
        #endregion
    }
}