using System;
using System.Runtime.Caching;

namespace Sfa.Core.Runtime.Caching
{
    /// <summary>
    /// An <see cref="ICache"/> implementation of <see cref="MemoryCache"/>.
    /// </summary>
    public class MemoryCacheAdapter : ICache
    {
        #region ICache Implementation

        /// <summary>
        /// Returns a flag indicating whether or not the cache contains an item for the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to look for in the cache.</param>
        /// <returns><c>true</c> if the cache contains the item for the key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(string key)
        {
            return MemoryCache.Default.Contains(key);
        }

        /// <summary>
        /// Deletes the item in the cache with the given key.
        /// </summary>
        /// <param name="key">The key to delete from the cache.</param>
        public void Delete(string key)
        {
            MemoryCache.Default.Remove(key);
        }

        /// <summary>
        /// Gets the item for the given <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> expected in the cache for the given key.</typeparam>
        /// <param name="key">The key that identifies the item stored in the cache.</param>
        /// <returns>The item stored under the given <see cref="key"/>.</returns>
        public T Get<T>(string key)
        {
            return (T) MemoryCache.Default.Get(key);
        }

        /// <summary>
        /// Gets the item for the given <paramref name="key"/>. If the cache doesn't contain the item, then the function is called that gets the item and then it is added to the cache.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> expected in the cache for the given key.</typeparam>
        /// <param name="key">The key that identifies the item stored in the cache.</param>
        /// <param name="get">The function that provides the value if one isn't found in the cache.</param>
        /// <returns>The item stored under the given <see cref="key"/>.</returns>
        public T GetOrAddAndGet<T>(string key, Func<T> get)
        {
            return GetOrAddOrReplace(key, new CacheItemPolicy(), get);
        }

        /// <summary>
        /// Gets the item for the given <paramref name="key"/>. If the cache doesn't contain the item, then the function is called that gets the item and then it is added to the cache.
        /// The item is stored for a given length of time where this time is reapplied each time the item is accessed.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> expected in the cache for the given key.</typeparam>
        /// <param name="key">The key that identifies the item stored in the cache.</param>
        /// <param name="slidingExpiration">The sliding time to store the item in the cache.</param>
        /// <param name="get">The function that provides the value if one isn't found in the cache.</param>
        /// <returns>The item stored under the given <see cref="key"/>.</returns>
        public T GetOrAddAndGetSlidingExpiration<T>(string key, TimeSpan slidingExpiration, Func<T> get)
        {
            return GetOrAddOrReplace(key, new CacheItemPolicy { SlidingExpiration = slidingExpiration }, get);
        }

        /// <summary>
        /// Gets the item for the given <paramref name="key"/>. If the cache doesn't contain the item, then the function is called that gets the item and then it is added to the cache.
        /// The item is stored to a point it time..
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> expected in the cache for the given key.</typeparam>
        /// <param name="key">The key that identifies the item stored in the cache.</param>
        /// <param name="absoluteExpiration">The time at which the item should be evicted from the cache.</param>
        /// <param name="get">The function that provides the value if one isn't found in the cache.</param>
        /// <returns>The item stored under the given <see cref="key"/>.</returns>
        public T GetOrAddAndGetExactExpiration<T>(string key, DateTime absoluteExpiration, Func<T> get)
        {
            return GetOrAddOrReplace(key, new CacheItemPolicy { AbsoluteExpiration = new DateTimeOffset(absoluteExpiration) }, get);
        }

        /// <summary>
        /// Adds the given item to the cache under the given key.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the item to be stored in the cache.</typeparam>
        /// <param name="key">The key to store the item under in the cache.</param>
        /// <param name="value">The value to store in the cache.</param>
        public void Add<T>(string key, T value)
        {
            AddOrReplace(key, value, new CacheItemPolicy());
        }

        /// <summary>
        /// Adds the given item to the cache under the given key with a sliding expiration.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the item to be stored in the cache.</typeparam>
        /// <param name="key">The key to store the item under in the cache.</param>
        /// <param name="value">The value to store in the cache.</param>
        /// <param name="slidingExpiration">The time to store the item in the cache. Each time the item is accessed this time is reset. If the time is passed without the item being accessed, then item is cleared from the cache.</param>
        public void AddSlidingExpiration<T>(string key, T value, TimeSpan slidingExpiration)
        {
            AddOrReplace(key, value, new CacheItemPolicy { SlidingExpiration = slidingExpiration, RemovedCallback =
                arguments =>
                {
                    Console.WriteLine("removed");
                }
            });
        }

        /// <summary>
        /// Adds the given item to the cache under the given key with an exact expiration time.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the item to be stored in the cache.</typeparam>
        /// <param name="key">The key to store the item under in the cache.</param>
        /// <param name="value">The value to store in the cache.</param>
        /// <param name="absoluteExpiration">The time at which the item should be evicted from the cache.</param>
        public void AddExactExpiration<T>(string key, T value, DateTime absoluteExpiration)
        {
            AddOrReplace(key, value, new CacheItemPolicy { AbsoluteExpiration = new DateTimeOffset(absoluteExpiration) });
        }

        #endregion


        #region Helpers
        private T GetOrAddOrReplace<T>(string key, CacheItemPolicy policy, Func<T> get)
        {
            var cache = MemoryCache.Default;
            var value = (T)cache.Get(key);

            if (value == null)
            {
                value = get();
                AddOrReplace(key, value, policy);
            }

            return value;
        }

        private void AddOrReplace<T>(string key, T value, CacheItemPolicy policy)
        {
            var cache = MemoryCache.Default;

            var existing = cache.AddOrGetExisting(key, value, policy);

            if (existing != null)
            {
                cache.Remove(key);
                cache.Add(key, value, policy);
            }
        }

        #endregion
    }
}