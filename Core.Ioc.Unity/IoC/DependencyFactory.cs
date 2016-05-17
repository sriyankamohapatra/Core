using System.Collections.Generic;
using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Sfa.Core.IoC
{
    /// <summary>
    /// Simple wrapper for unity resolution.
    /// </summary>
    public class DependencyFactory
    {
        /// <summary>
        /// Public reference to the unity container which will 
        /// allow the ability to register instances or take 
        /// other actions on the container.
        /// </summary>
        public static IUnityContainer Container { get; private set; }

        /// <summary>
        /// Static constructor for DependencyFactory which will 
        /// initialize the unity container.
        /// </summary>
        static DependencyFactory()
        {
            var container = new UnityContainer();

            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            section?.Configure(container);
            Container = container;
        }

        /// <summary>
        /// Resolves the type parameter T to an instance of the appropriate type.
        /// </summary>
        /// <typeparam name="T">Type of object to return</typeparam>
        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        /// <summary>
        /// Resolves the type parameter T to all instance of the appropriate type.
        /// </summary>
        /// <typeparam name="T">Type of the objects to return</typeparam>
        public static IEnumerable<T> ResolveAll<T>()
        {
            return Container.ResolveAll<T>();
        }
    }
}