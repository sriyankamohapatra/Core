using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sfa.Core.Reflection
{
    public static class ObjectCopier
    {
        /// <summary>
        /// Copies the specified source.
        /// </summary>
        /// <typeparam name="T">The type of the object to copy.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>A new binary copy of the source.</returns>
        public static T Copy<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException($"The type [{typeof (T)}] must be serializable.", nameof(source));
            }

            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}