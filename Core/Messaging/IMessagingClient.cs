using System;

namespace Sfa.Core.Messaging
{
    /// <summary>
    /// Represents a client that can process messages
    /// </summary>
    public interface IMessagingClient
    {
        /// <summary>
        /// Creates the message.
        /// </summary>
        /// <typeparam name="T">The type of the message to create.</typeparam>
        /// <param name="message">The message instance.</param>
        /// <param name="delay">Any delay until the message gets enqueued.</param>
        void CreateMessage<T>(T message, TimeSpan delay = default(TimeSpan))
            where T : IMessage;
    }
}