namespace Sfa.Core.Context
{
    /// <summary>
    /// Defines the api for the storage mechanism for a context.
    /// </summary>
    public interface IContextStorage
    {
        /// <summary>
        /// Returns the strongly typed data for the given name.
        /// </summary>
        /// <typeparam name="T">The <see cref="System.Type"/>of the object to be returned.</typeparam>
        /// <param name="name">The name of the object stored.</param>
        /// <returns>The object that was defined using <see cref="System.Type"/>.</returns>
        T GetData<T>(string name);

        /// <summary>
        /// Sets the <paramref name="instance"/> to be stored for the context.
        /// </summary>
        /// <typeparam name="T">The <see cref="System.Type"/>of the object to be stored.</typeparam>
        /// <param name="name">The name of the object stored.</param>
        /// <param name="instance">The instance to store.</param>
        void SetData<T>(string name, T instance);

        /// <summary>
        /// Clears all the data for a given key.
        /// </summary>
        /// <param name="names">The key names of the items to clear.</param>
        /// <remarks>Implementation should call Dispose on each found data if appropriate.</remarks>
        void CleanAndDispose(params string[] names);
    }
}