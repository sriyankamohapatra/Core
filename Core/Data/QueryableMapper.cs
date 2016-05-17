using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Sfa.Core.Data
{
    /// <summary>
    /// Container for registering and using queryable mappings.
    /// </summary>
    public class QueryableMapper
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>> Mappings = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>>();

        /// <summary>
        /// Register your projection mappers here.
        /// </summary>
        /// <typeparam name="TFrom">The type of instance that you will be mapping from.</typeparam>
        /// <typeparam name="TTo">The type of instance that is mapped to.</typeparam>
        /// <param name="mappingFunction"></param>
        /// <exception cref="ArgumentNullException">The mappingFunction must not be null.</exception>
        public static void Register<TFrom, TTo>(Func<IQueryable<TFrom>, IQueryable<TTo>> mappingFunction)
        {
            if (mappingFunction == null)
            {
                throw new ArgumentNullException(nameof(mappingFunction));
            }

            Mappings.GetOrAdd(typeof (TFrom), type => new ConcurrentDictionary<Type, object>())
                .TryAdd(typeof (TTo), mappingFunction);
        }


        /// <summary>
        /// Maps a <see cref="IQueryable{TFrom}"/> to a <see cref="IQueryable{TTo}"/> by calling the registered mapping function on the supplied instance.
        /// </summary>
        /// <typeparam name="TFrom">The type in the source queryable.</typeparam>
        /// <typeparam name="TTo">The type in the destination queryable.</typeparam>
        /// <param name="from">The source queryable to transform.</param>
        /// <returns>The mapped queryable.</returns>
        /// <exception cref="ArgumentNullException">The from instance must not be null.</exception>
        /// <exception cref="MissingMethodException">Throw if no matching function can be found for the supplied types.</exception>
        public static IQueryable<TTo> Map<TFrom, TTo>(IQueryable<TFrom> from)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            ConcurrentDictionary<Type, object> functions;
            if(!Mappings.TryGetValue(typeof(TFrom), out functions))
            {
                throw new MissingMethodException($"No mappings from {typeof(TFrom)} have been registered");
            }

            object mappingFunctionObj;
            if (functions == null || !functions.TryGetValue(typeof(TTo), out mappingFunctionObj))
            {
                throw new MissingMethodException($"No mapping from {typeof(TFrom)} to {typeof(TTo)} has been registered");
            }

            var mappingFunction = (Func<IQueryable<TFrom>, IQueryable<TTo>>)mappingFunctionObj;

            return mappingFunction(from);
        }
    }
}