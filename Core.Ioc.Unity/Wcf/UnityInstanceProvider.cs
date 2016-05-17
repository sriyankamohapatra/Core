using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Microsoft.Practices.Unity;
using Sfa.Core.IoC;

namespace Sfa.Core.Wcf
{

    /// <summary>
    ///     Dependency injection instance provider (using Unity application block).
    ///     This kind of behavior controls the lifecycle of a WCF service instance, so it is the best place
    ///     to inject the service dependencies.
    /// </summary>
    public class UnityInstanceProvider : IInstanceProvider
    {
        /// <summary>
        /// The service type.
        /// </summary>
        private readonly Type _serviceType;


        #region Constructors and Destructors

        /// <summary>
        ///     Initialises a new instance of the <see cref="UnityInstanceProvider" /> class.
        /// </summary>
        /// <param name="serviceType">The service contract type.</param>
        public UnityInstanceProvider(Type serviceType)
        {
            _serviceType = serviceType;
        }

        #endregion


        #region Public Methods and Operators

        /// <summary>
        ///     Gets a fresh service instance
        /// </summary>
        /// <param name="instanceContext">
        ///     The current <see cref="T:System.ServiceModel.InstanceContext" /> object.
        /// </param>
        /// <returns>A user-defined service object.</returns>
        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        /// <summary>
        ///     Gets a fresh service instance
        /// </summary>
        /// <param name="instanceContext">
        ///     The current <see cref="T:System.ServiceModel.InstanceContext" /> object.
        /// </param>
        /// <param name="message">The message that triggered the creation of a service object.</param>
        /// <returns>The service object.</returns>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            var instance = DependencyFactory.Container.Resolve(_serviceType);
            return instance;
        }

        /// <summary>
        /// Returns a flag stating whether or not the type can be resolved by the container.
        /// </summary>
        /// <typeparam name="T">The type to test resolution for.</typeparam>
        /// <returns><c>true</c> if the type can be resolved.</returns>
        public bool CanResolve<T>()
        {
            return DependencyFactory.Container.IsRegistered<T>();
        }

        /// <summary>
        /// Resolves the type against the IoC container.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <returns>The resolved instance.</returns>
        public T Resolve<T>()
        {
            return DependencyFactory.Container.Resolve<T>();
        }

        /// <summary>
        ///     Releases the specified service instance
        /// </summary>
        /// <param name="instanceContext">The service's instance context.</param>
        /// <param name="instance">The service object to be recycled.</param>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            var disposable = instance as IDisposable;
            disposable?.Dispose();
        }

        #endregion
    }
}