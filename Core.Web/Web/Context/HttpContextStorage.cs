using System;
using System.Web;
using Sfa.Core.Context;

namespace Sfa.Core.Web.Context
{
    /// <summary>
    /// HttpContext implementation of a CallContextStorgae.
    /// </summary>
    public class HttpContextStorage : CallContextStorage
    {
        /// <summary>
        /// Returns the strongly typed data for the given name.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/>of the object to be returned.</typeparam>
        /// <param name="name">The name of the object stored.</param>
        /// <returns>The object that was defined using <see cref="CallContextStorage.SetData{T}"/>.</returns>
        public override T GetData<T>(string name)
        {
            return (T)HttpContext.Current.Items[name];
        }

        /// <summary>
        /// Sets the <paramref name="instance"/> to be stored for the context.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/>of the object to be stored.</typeparam>
        /// <param name="name">The name of the object stored.</param>
        /// <param name="instance">The instance to store.</param>
        public override void SetData<T>(string name, T instance)
        {
            HttpContext.Current.Items[name] = instance;
        }
    }
}