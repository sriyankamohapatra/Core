using System;
using System.Collections.Generic;

namespace Sfa.Core.Context
{
    /// <summary>
    /// Static storage mechanism.
    /// </summary>
    public class StaticContextStorage : IContextStorage
    {
        private static readonly Dictionary<string, object> Storage = new Dictionary<string, object>();

        /// <summary>
        /// Returns the strongly typed data for the given name.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/>of the object to be returned.</typeparam>
        /// <param name="name">The name of the object stored.</param>
        /// <returns>The object that was defined using <see cref="SetData{T}"/>.</returns>
        public T GetData<T>(string name)
        {
            return (T)Storage[name];
        }

        /// <summary>
        /// Sets the <paramref name="instance"/> to be stored for the context.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/>of the object to be stored.</typeparam>
        /// <param name="name">The name of the object stored.</param>
        /// <param name="instance">The instance to store.</param>
        public void SetData<T>(string name, T instance)
        {
            Storage[name] = instance;
        }

        /// <summary>
        /// Clears all the data for a given key.
        /// </summary>
        /// <param name="names">The key names of the items to clear.</param>
        /// <remarks>Implementation should probably call Dispose on each given data if appropriate.</remarks>
        public void CleanAndDispose(params string[] names)
        {
            foreach (var name in names)
            {
                var item = GetData<object>(name);
                (item as IDisposable)?.Dispose();
                SetData<object>(name, null);
                Storage.Remove(name);
            }
        }
    }
}