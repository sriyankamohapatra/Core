using Microsoft.ServiceBus.Messaging;
using Sfa.Core.Context;
using Sfa.Core.Logging;

namespace Sfa.Core.Messaging
{
    /// <summary>
    /// Adds extra methods to the <see cref="QueueClient"/> class.
    /// </summary>
    public static class QueueClientExtensions
    {
        /// <summary>
        /// Deletes all the messages in the Queue.
        /// </summary>
        /// <param name="client">The client instance to add the method too.</param>
        /// <returns>The client instance.</returns>
        public static QueueClient DeleteAllMessages(this QueueClient client)
        {
            ApplicationContext.Logger.Log(LoggingLevel.Info, CoreLoggingCategory.Diagnostics, () => "Deleting all messages off the queue {0}", client.Path);
            while (client.Peek() != null)
            {
                // Mode is RecieveAndDelete so we don't need to do anything else here.
                var brokeredMessages = client.ReceiveBatch(10);

                foreach (var brokeredMessage in brokeredMessages)
                {
                    ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => "Completing message with id {0}", brokeredMessage.MessageId);
                }
            }

            return client;
        }
    }
}