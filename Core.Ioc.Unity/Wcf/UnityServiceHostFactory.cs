using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Sfa.Core.Wcf
{
    /// <summary>
    /// Produces <see cref="UnityServiceHostFactory"/>s.
    /// </summary>
    public class UnityServiceHostFactory : ServiceHostFactory
    {
        /// <summary>
        /// Creates a <see cref="UnityServiceHostFactory"/> for a specified type of service with a specific base address. 
        /// </summary>
        /// <returns>
        /// A <see cref="UnityServiceHostFactory"/> for the type of service specified with a specific base address.
        /// </returns>
        /// <param name="serviceType">
        /// Specifies the type of service to host. 
        /// </param>
        /// <param name="baseAddresses">
        /// The <see cref="T:System.Array"/> of type <see cref="T:System.Uri"/> that contains the base addresses for the service hosted.
        /// </param>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            RegisterDependencies();
            return new UnityServiceHost(serviceType, baseAddresses);
        }

        /// <summary>
        /// Initialization logic that any derived type would use to set up the ServiceLocator provider.  Look to UnityServiceHostFactory to see how this is done, if implementing for 
        /// another IoC engine.
        /// </summary>
        protected virtual void RegisterDependencies()
        {
        }
    }
}