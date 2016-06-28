using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Sfa.Core.Context;
using Sfa.Core.Logging;

namespace Sfa.Core.Data
{
    /// <summary>
    /// Utility class to help with access to the cloud storage account
    /// </summary>
    public class AzureCloudStorageAccount
    {
        /// <summary>
        /// Open the connection to the storage account for the given connection string.
        /// </summary>
        /// <param name="connectionStringName">The connection string to connect with.</param>
        /// <returns>The storage account for the connection string.</returns>
        public static CloudStorageAccount Open(string connectionStringName)
        {
            var connectionString = CloudConfigurationManager.GetSetting(connectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                const string message = "No connection string was found to be able to connect to the storage account";
                ApplicationContext.Logger.Log(LoggingLevel.Error, CoreLoggingCategory.Diagnostics, () => message);
                throw new StorageException(message);
            }

            return CloudStorageAccount.Parse(connectionString);
        }
    }
}