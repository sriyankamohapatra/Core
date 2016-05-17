using System;
using System.Collections.Generic;
using System.ServiceModel;
using Sfa.Core.Context;

namespace Sfa.Core.ServiceModel
{
    /// <summary>
    /// Enables the implementer to have custom data for the lifetime of an operation.
    /// </summary>
    public class WcfOperationContext : IExtension<OperationContext>, IContextStorage
    {
        private WcfOperationContext()
        {
            Items = new Dictionary<string, object>();
        }

        /// <summary>
        /// The items contained within this context.
        /// </summary>
        public IDictionary<string, object> Items { get; }

        /// <summary>
        /// The current instance for this operation
        /// </summary>
        public static WcfOperationContext Current
        {
            get
            {
                var context = OperationContext.Current.Extensions.Find<WcfOperationContext>();
                if (context == null)
                {
                    context = new WcfOperationContext();
                    OperationContext.Current.Extensions.Add(context);
                }
                return context;
            }
        }

        /// <summary>
        /// Enables an extension object to find out when it has been aggregated. Called when the extension is added to the <see cref="P:System.ServiceModel.IExtensibleObject`1.Extensions"/> property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        public void Attach(OperationContext owner) { }

        /// <summary>
        /// Enables an object to find out when it is no longer aggregated. Called when an extension is removed from the <see cref="P:System.ServiceModel.IExtensibleObject`1.Extensions"/> property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        public void Detach(OperationContext owner) { }


        #region IContextStorage Implementation


        /// <summary>
        /// Returns the strongly typed data for the given name.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/>of the object to be returned.</typeparam>
        /// <param name="name">The name of the object stored.</param>
        /// <returns>The object that was defined using <see cref="Type"/>.</returns>
        public T GetData<T>(string name)
        {
            return (T) Current.Items[name];
        }

        /// <summary>
        /// Sets the <paramref name="instance"/> to be stored for the context.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/>of the object to be stored.</typeparam>
        /// <param name="name">The name of the object stored.</param>
        /// <param name="instance">The instance to store.</param>
        public void SetData<T>(string name, T instance)
        {
            Current.Items[name] = instance;
        }

        /// <summary>
        /// Clears all the data for a given key.
        /// </summary>
        /// <param name="names">The key names of the items to clear.</param>
        /// <remarks>Implementation should call Dispose on each found data if appropriate.</remarks>
        public void CleanAndDispose(params string[] names)
        {
            foreach (var pair in Current.Items)
            {
                var item = pair.Value;
                (item as IDisposable)?.Dispose();
            }
            Current.Items.Clear();
        }

        #endregion


        #region IDisposable Implementation

        /// <summary>
        /// Provides a mechanism for releasing unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var pair in Current.Items)
                {
                    var item = pair.Value;
                    (item as IDisposable)?.Dispose();
                }
                Current.Items.Clear();
            }
        }

        #endregion
    }
}