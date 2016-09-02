using System;
using System.Runtime.Serialization;

namespace Sfa.Core.ServiceModel.Syndication.Exceptions
{
    /// <summary>
    /// Thrown when a feed was read but an existing bookmark was not matched.
    /// </summary>
    public class GuidBookmarkNotMatchedException : BaseFeedReadException
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="bookmark">The bookmark that was not matched.</param>
        /// <param name="url">The URL that the empty page was found at.</param>
        public GuidBookmarkNotMatchedException(Guid bookmark, string url)
            : base($"The bookmark [{bookmark}] was not matched and we have reached the beginning of the feed.", url)
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
        protected GuidBookmarkNotMatchedException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}