using System;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;

namespace Sfa.Core.IoC
{
    /// <summary>
    /// Extensions to <see cref="IUnityContainer"/>.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Finds any <see cref="IUnityInstaller"/> instances in the supplied list of assemblies and installs them into the conatiner.
        /// </summary>
        /// <param name="container">The conatiner to install into.</param>
        /// <param name="assemblies">The list of assemblies to look into for implementations of <see cref="IUnityInstaller"/>.</param>
        /// <returns>The container to enable method chaining.</returns>
        public static IUnityContainer Install(this IUnityContainer container, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var installers = from type in assembly.GetTypes()
                                 where typeof(IUnityInstaller).IsAssignableFrom(type)
                                 select (IUnityInstaller)Activator.CreateInstance(type);

                foreach (var installer in installers)
                {
                    installer.Install(container);
                }
            }

            return container;
        }

        /// <summary>
        /// Installs the installers into the conatiner
        /// </summary>
        /// <param name="container">The conatiner to install into.</param>
        /// <param name="installers">The installers to install.</param>
        /// <returns>The container to enable method chaining.</returns>
        public static IUnityContainer Install(this IUnityContainer container, params IUnityInstaller[] installers)
        {
            foreach (var installer in installers)
            {
                installer.Install(container);
            }

            return container;
        }
    }
}