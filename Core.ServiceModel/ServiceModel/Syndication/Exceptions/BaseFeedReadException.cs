using System.Runtime.Serialization;
using Sfa.Core.Exceptions;

namespace Sfa.Core.ServiceModel.Syndication.Exceptions
{
    public abstract class BaseFeedReadException : BusinessLogicException
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="url">The URL of the empty page.</param>
        protected BaseFeedReadException(string message, string url)
            : base(message)
        {
            Url = url;
            TreatAsCommitSuccess = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BusinessLogicException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized 
        /// object data about the exception being thrown. </param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual 
        /// information about the source or destination. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or 
        /// <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected BaseFeedReadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// The URL that the empty page was found at.
        /// </summary>
        public string Url { get; protected set; }
    }
}