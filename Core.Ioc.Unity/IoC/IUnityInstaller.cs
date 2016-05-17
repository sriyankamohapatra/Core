using Microsoft.Practices.Unity;

namespace Sfa.Core.IoC
{
    /// <summary>
    /// Allows configuration of the conatiner via installer classes.
    /// </summary>
    public interface IUnityInstaller
    {
        /// <summary>
        /// Configure the container.
        /// </summary>
        /// <param name="container">The container.</param>
        void Install(IUnityContainer container);
    }
}