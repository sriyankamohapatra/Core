using System;
using System.ServiceModel;

namespace Sfa.Core.Wcf
{
    /// <summary>
    /// This service host is used to set up the service behavior that replaces the instance provider to use dependency injection.
    /// </summary>
    public class UnityServiceHost : ServiceHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityServiceHost"/> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="baseAddresses">The base addresses.</param>
        public UnityServiceHost(Type serviceType, Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        { }

        /// <summary>
        /// Opens the channel dispatchers.
        /// </summary>
        /// <param name="timeout">The <see cref="T:System.Timespan"/> that specifies how long the on-open operation has to complete before timing out.</param>
        protected override void OnOpen(TimeSpan timeout)
        {
            Description.Behaviors.Add(new UnityServiceBehavior());
            base.OnOpen(timeout);
        }
    }
}