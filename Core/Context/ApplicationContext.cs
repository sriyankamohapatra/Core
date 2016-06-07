using System.Collections.Generic;
using System.Linq;
using Sfa.Core.Logging;

namespace Sfa.Core.Context
{
    /// <summary>
    /// Defines the context that can be used at any level of the application.
    /// </summary>
    public static class ApplicationContext
    {
        #region Fields

        private static IContextStorage _contextStorage;
        private static readonly string StorageKeyLogger = "logger";
        private static readonly string StorageKeyNetworkContext = "network";
        private static readonly HashSet<string> Keys = new HashSet<string>(); 

        #endregion


        #region Core Properties

        /// <summary>
        /// The logged for the system.
        /// </summary>
        public static ILogger Logger => _contextStorage.GetData<ILogger>(StorageKeyLogger);

        /// <summary>
        /// The network context for the system.
        /// </summary>
        public static INetworkContext NetworkContext => _contextStorage.GetData<INetworkContext>(StorageKeyNetworkContext);

        #endregion


        #region Life cycle

        /// <summary>
        /// Sets up the context with the appropriate dependencies.
        /// </summary>
        /// <param name="contextStorage">The context storage for the dependencies.</param>
        /// <param name="logger">The logger for the system.</param>
        /// <param name="networkContext">The network context to be used through the application.</param>
        public static void Setup(IContextStorage contextStorage, ILogger logger, INetworkContext networkContext)
        {
            _contextStorage = contextStorage;

            _contextStorage.SetData(StorageKeyLogger, logger);
            _contextStorage.SetData(StorageKeyNetworkContext, networkContext);
            Keys.Clear();
            Keys.Add(StorageKeyLogger);
            Keys.Add(StorageKeyNetworkContext);
        }

        /// <summary>
        /// Gets a generic type from the context.
        /// </summary>
        /// <typeparam name="T">the type of instance to return.</typeparam>
        /// <returns>The instance if stored in the context.</returns>
        public static void Set<T>(T instance)
        {
            var key = typeof (T).FullName;
            Keys.Add(key);
            _contextStorage.SetData(key, instance);
        }

        /// <summary>
        /// Gets a generic type from the context.
        /// </summary>
        /// <typeparam name="T">the type of instance to return.</typeparam>
        /// <returns>The instance if stored in the context.</returns>
        public static T Get<T>()
        {
            return _contextStorage.GetData<T>(typeof(T).FullName);
        }

        /// <summary>
        /// Tears down all the dependencies of the system.
        /// </summary>
        public static void TearDown()
        {
            _contextStorage.CleanAndDispose(Keys.ToArray());
            Keys.Clear();
        }

        #endregion
    }
}