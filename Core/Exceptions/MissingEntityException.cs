using System;
using System.Runtime.Serialization;

namespace Sfa.Core.Exceptions
{
    public class MissingEntityException : BusinessLogicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MissingEntityException"/> class.
        /// </summary>
        /// <param name="entityType">The Type of the entity that couldn't be found.</param>
        public MissingEntityException(Type entityType)
            : base($"The instance requested cannot be found for type {entityType}")
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:MissingEntityException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized 
        /// object data about the exception being thrown. </param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual 
        /// information about the source or destination. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or 
        /// <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected MissingEntityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}