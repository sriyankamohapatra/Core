using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Sfa.Core.IoC;

namespace Sfa.Core.Wcf
{
    /// <summary>
    /// A custom service behavior which will register the dependency injection instance provider.
    /// </summary>
    public class UnityServiceBehavior : IServiceBehavior
    {
        #region IServiceBehavior Members

        /// <summary>
        /// Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">The service description of the service.</param>
        /// <param name="serviceHostBase">The host of the service.</param>
        /// <param name="endpoints">The service endpoints.</param>
        /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
        public void AddBindingParameters(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        { }

        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The host that is currently being built.</param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var handlers = DependencyFactory.ResolveAll<IErrorHandler>().ToList();
            foreach (var dispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = dispatcherBase as ChannelDispatcher;

                if (channelDispatcher != null)
                {
                    foreach (var errorHandler in handlers)
                    {
                        channelDispatcher.ErrorHandlers.Add(errorHandler);
                    }

                    foreach (var ed in channelDispatcher.Endpoints)
                    {
                        ed.DispatchRuntime.InstanceProvider = new UnityInstanceProvider(serviceDescription.ServiceType);
                    }
                }
            }
        }

        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { }

        #endregion
    }
}