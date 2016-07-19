using System;

namespace Sfa.Core.Runtime.Caching
{
    /// <summary>
    /// Defines the interface for a generic cache.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Returns a flag indicating whether or not the cache contains an item for the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to look for in the cache.</param>
        /// <returns><c>true</c> if the cache contains the item for the key; otherwise, <c>false</c>.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Deletes the item in the cache with the given key.
        /// </summary>
        /// <param name="key">The key to delete from the cache.</param>
        void Delete(string key);

        /// <summary>
        /// Gets the item for the given <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> expected in the cache for the given key.</typeparam>
        /// <param name="key">The key that identifies the item stored in the cache.</param>
        /// <returns>The item stored under the given <see cref="key"/>.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Gets the item for the given <paramref name="key"/>. If the cache doesn't contain the item, then the function is called that gets the item and then it is added to the cache.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> expected in the cache for the given key.</typeparam>
        /// <param name="key">The key that identifies the item stored in the cache.</param>
        /// <param name="get">The function that provides the value if one isn't found in the cache.</param>
        /// <returns>The item stored under the given <see cref="key"/>.</returns>
        T GetOrAddAndGet<T>(string key, Func<T> get);

        /// <summary>
        /// Gets the item for the given <paramref name="key"/>. If the cache doesn't contain the item, then the function is called that gets the item and then it is added to the cache.
        /// The item is stored for a given length of time where this time is reapplied each time the item is accessed.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> expected in the cache for the given key.</typeparam>
        /// <param name="key">The key that identifies the item stored in the cache.</param>
        /// <param name="expirationFromLastAccess">The sliding time to store the item in the cache.</param>
        /// <param name="get">The function that provides the value if one isn't found in the cache.</param>
        /// <returns>The item stored under the given <see cref="key"/>.</returns>
        T GetOrAddAndGetSlidingExpiration<T>(string key, TimeSpan expirationFromLastAccess, Func<T> get);

        /// <summary>
        /// Gets the item for the given <paramref name="key"/>. If the cache doesn't contain the item, then the function is called that gets the item and then it is added to the cache.
        /// The item is stored to a point it time..
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> expected in the cache for the given key.</typeparam>
        /// <param name="key">The key that identifies the item stored in the cache.</param>
        /// <param name="expiresAt">The time at which the item should be evicted from the cache.</param>
        /// <param name="kind">The type of time being supplied.</param>
        /// <param name="get">The function that provides the value if one isn't found in the cache.</param>
        /// <returns>The item stored under the given <see cref="key"/>.</returns>
        T GetOrAddAndGetExactExpiration<T>(string key, DateTime expiresAt, DateTimeKind kind, Func<T> get);

        /// <summary>
        /// Adds the given item to the cache under the given key.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the item to be stored in the cache.</typeparam>
        /// <param name="key">The key to store the item under in the cache.</param>
        /// <param name="value">The value to store in the cache.</param>
        void Add<T>(string key, T value);

        /// <summary>
        /// Adds the given item to the cache under the given key with a sliding expiration.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the item to be stored in the cache.</typeparam>
        /// <param name="key">The key to store the item under in the cache.</param>
        /// <param name="value">The value to store in the cache.</param>
        /// <param name="expirationFromLastAccess">The time to store the item in the cache. Each time the item is accessed this time is reset. If the time is passed without the item being accessed, then item is cleared from the cache.</param>
        void AddSlidingExpiration<T>(string key, T value, TimeSpan expirationFromLastAccess);

        /// <summary>
        /// Adds the given item to the cache under the given key with an exact expiration time.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the item to be stored in the cache.</typeparam>
        /// <param name="key">The key to store the item under in the cache.</param>
        /// <param name="value">The value to store in the cache.</param>
        /// <param name="expiresAt">The time at which the item should be evicted from the cache.</param>
        /// <param name="kind">The type of time being supplied.</param>
        void AddExactExpiration<T>(string key, T value, DateTime expiresAt, DateTimeKind kind);
    }
}