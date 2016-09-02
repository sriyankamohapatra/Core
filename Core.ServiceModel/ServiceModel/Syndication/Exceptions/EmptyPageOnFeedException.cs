using System.Runtime.Serialization;

namespace Sfa.Core.ServiceModel.Syndication.Exceptions
{
    /// <summary>
    /// Thrown when a feed was read but an existing bookmark was not matched.
    /// </summary>
    public class EmptyPageOnFeedException : BaseFeedReadException
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="url">The URL of the empty page.</param>
        public EmptyPageOnFeedException(string url)
            : base($"An empty page was found in the ATOM feed at page [{url}].", url)
        {
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
        protected EmptyPageOnFeedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}