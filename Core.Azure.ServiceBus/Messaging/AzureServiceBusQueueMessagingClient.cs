using System;
using System.Diagnostics.Contracts;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Sfa.Core.Context;
using Sfa.Core.Logging;

namespace Sfa.Core.Messaging
{
    /// <summary>
    /// Azure ServiceBus implementation of the messaging client.
    /// </summary>
    public class AzureServiceBusQueueMessagingClient : IMessagingClient
    {
        #region Fields

        private readonly string _queueName;

        #endregion


        #region Life cycle

        /// <summary>
        /// Default constructor for the client.
        /// </summary>
        /// <param name="queueName">The name of the queue to use for this client.</param>
        public AzureServiceBusQueueMessagingClient(string queueName)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(queueName));

            _queueName = queueName;
        }

        /// <summary>
        /// Initialises the instance from its connection string.
        /// </summary>
        /// <returns>The initialised instance.</returns>
        public AzureServiceBusQueueMessagingClient Initialise()
        {
            ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => $"Initialising azure service bus client for queue: {_queueName}");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);

            if (!namespaceManager.QueueExists(_queueName))
            {
                ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => $"Creating azure service bus queue: {_queueName}");
                namespaceManager.CreateQueue(_queueName);
            }

            return this;
        }

        #endregion


        #region Properties

        /// <summary>
        /// The connection string for the instance.
        /// </summary>
        /// <remarks>Held in application or cloud settings under "asb:ConnectionString".</remarks>
        public string ConnectionString => CloudConfigurationManager.GetSetting("asb:ConnectionString");

        /// <summary>
        /// Creates a new <see cref="QueueClient"/> instance.
        /// </summary>
        /// <param name="mode">The recieve mode for the client.</param>
        /// <returns>The new instance.</returns>
        public QueueClient NewQueueClient(ReceiveMode mode = ReceiveMode.PeekLock)
        {
            return QueueClient.CreateFromConnectionString(ConnectionString, _queueName, mode);
        }

        #endregion


        #region IMessagingClient Implementation

        /// <summary>
        /// Creates the message on the queue.
        /// </summary>
        /// <typeparam name="T">The type of the message to create.</typeparam>
        /// <param name="message">The message instance.</param>
        /// <param name="delay">Any delay until the message gets enqueued.</param>
        public void CreateMessage<T>(T message, TimeSpan delay = default(TimeSpan)) where T : IMessage
        {
            var client = NewQueueClient();

            try
            {
                ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => "Creating azure service bus message of type {0} on path {1}", message.GetType(), client.Path);

                var brokeredMessage = new BrokeredMessage(message)
                {
                    Properties = { {"messageType", message.GetType().FullName} }
                };

                if (delay != default(TimeSpan))
                {
                    brokeredMessage.ScheduledEnqueueTimeUtc = ApplicationContext.NetworkContext.CurrentUtcDateTime.Add(delay);
                }

                client.Send(brokeredMessage);

                ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => "Created azure service bus message of type {0}", message.GetType());
            }
            finally
            {
                client?.Close();
            }
        } 

        #endregion
    }
}