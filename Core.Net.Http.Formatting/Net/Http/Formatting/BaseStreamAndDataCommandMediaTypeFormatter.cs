using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Sfa.Core.IO;
using Sfa.Core.Xml.Linq;

namespace Sfa.Core.Net.Http.Formatting
{
    /// <summary>
    /// Base class for formatting of http reads where a data stream and data are being sent.
    /// </summary>
    public abstract class BaseStreamAndDataCommandMediaTypeFormatter<T, TData> : MediaTypeFormatter 
        where T : new()
    {
        #region Constants

        private const string MediaTypeTextXml = "text/xml";
        private const string MediaTypeApplicationXml = "application/xml";
        private const string MediaTypeApplicationJson = "application/json";
        private const string MediaTypeMultipartFormData = "multipart/form-data";
        private const string MediaTypeApplicationOctetStream = "application/octet-stream";

        #endregion


        #region Fields

        private readonly Action<T, byte[]> _addData;

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected BaseStreamAndDataCommandMediaTypeFormatter(Action<T, byte[]> addData)
        {
            _addData = addData;

            SupportedMediaTypes.Add(new MediaTypeHeaderValue(MediaTypeTextXml));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(MediaTypeApplicationXml));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(MediaTypeApplicationJson));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(MediaTypeMultipartFormData));
        }

        #endregion


        #region MediaTypeFormatter Overrides


        /// <summary>
        /// Queries whether this <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> can deserialise an object of the specified type.
        /// </summary>
        /// 
        /// <returns>
        /// true if the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> can deserialise the type; otherwise, false.
        /// </returns>
        /// <param name="type">The type to deserialise.</param>
        public override bool CanReadType(Type type)
        {
            return type == typeof(T);
        }

        /// <summary>
        /// Queries whether this <see cref="T:BaseStreamAndDataCommandMediaTypeFormatter"/> can serialize an object of the specified type.
        /// </summary>
        /// 
        /// <returns>
        /// true if the <see cref="T:BaseStreamAndDataCommandMediaTypeFormatter"/> can serialize the type; otherwise, false.
        /// </returns>
        /// <param name="type">The type to serialize.</param>
        public override bool CanWriteType(Type type)
        {
            return false;
        }

        /// <summary>
        /// Asynchronously deserialises an object of the specified type.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task"/> whose result will be an object of the given type.
        /// </returns>
        /// <param name="type">The type of the object to deserialise.</param>
        /// <param name="readStream">The <see cref="T:System.IO.Stream"/> to read.</param>
        /// <param name="content">The <see cref="T:System.Net.Http.HttpContent"/>, if available. It may be null.</param>
        /// <param name="formatterLogger">The <see cref="T:System.Net.Http.Formatting.IFormatterLogger"/> to log events to.</param>
        /// <exception cref="T:HttpResponseException">Derived types need to support reading.</exception>
        public async override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            try
            {
                if (content.IsMimeMultipartContent())
                {
                    return await ReadFromMultipartStreamAsync(content);
                }
                if (MediaTypeApplicationJson == content.Headers.ContentType?.MediaType)
                {
                    return await ReadFromJsonStreamAsync(content);
                }
                if (MediaTypeTextXml == content.Headers.ContentType?.MediaType || MediaTypeApplicationXml == content.Headers.ContentType?.MediaType)
                {
                    return await ReadFromXmlStreamAsync(content);
                }

                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            catch (Exception exception)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(exception.ToString())
                };
                throw new HttpResponseException(msg);
            }
        }

        #endregion


        #region Read Helpers

        /// <summary>
        /// Need to map the incoming type that contains the data to the required end type
        /// </summary>
        /// <param name="source">The deserialised type containing the data from the incoming request.</param>
        /// <returns>The final destination type populated from the source type.</returns>
        protected abstract Task<T> MapAsync(TData source);

        private async Task<T> ReadFromMultipartStreamAsync(HttpContent content)
        {
            var parts = await content.ReadAsMultipartAsync();

            var value = new T();
            
            var jsonContent = parts.Contents.FirstOrDefault(x => new MediaTypeHeaderValue(MediaTypeApplicationJson).Equals(x.Headers.ContentType));
            if (jsonContent != null)
            {
                value = JsonConvert.DeserializeObject<T>(await jsonContent.ReadAsStringAsync());
            }

            var xmlContent = parts.Contents.FirstOrDefault(x => new MediaTypeHeaderValue(MediaTypeTextXml).Equals(x.Headers.ContentType) || new MediaTypeHeaderValue(MediaTypeApplicationXml).Equals(x.Headers.ContentType));
            if (xmlContent != null)
            {
                var xml = await xmlContent.ReadAsStringAsync();
                var xmlSerializer = new XmlSerializer(typeof(T));

                var xmlDoc = XDocumentExtensions.ParseAndRemoveAllNamespaces(xml);
                using (var reader = xmlDoc.CreateReader())
                {
                    value = (T)xmlSerializer.Deserialize(reader);
                }
            }

            var fileContent = parts.Contents.FirstOrDefault(x => new MediaTypeHeaderValue(MediaTypeApplicationOctetStream).Equals(x.Headers.ContentType));

            if (fileContent != null)
            {
                using (var dataStream = await fileContent.ReadAsStreamAsync())
                {
                    var imagebuffer = StreamExtensions.ToByteArray(dataStream);
                    _addData(value, imagebuffer);
                }
            }

            return value;
        }

        private async Task<T> ReadFromJsonStreamAsync(HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return await ConvertFromRawJsonAsync(json);
        }

        /// <summary>
        /// Converts the raw json string into the expected incoming type.
        /// </summary>
        /// <param name="rawJson">The raw json string from the request.</param>
        /// <returns>The expected object that the json represents.</returns>
        protected virtual Task<T> ConvertFromRawJsonAsync(string rawJson)
        {
            var encoded = JsonConvert.DeserializeObject<TData>(rawJson);
            return MapAsync(encoded);
        }
        

        private async Task<T> ReadFromXmlStreamAsync(HttpContent content)
        {
            var xml = await content.ReadAsStringAsync();
            return await ConvertFromRawXmlAsync(xml);
        }


        /// <summary>
        /// Converts the raw xml string into the expected incoming type.
        /// </summary>
        /// <param name="rawXml">The raw xml string from the request.</param>
        /// <returns>The expected object that the xml represents.</returns>
        protected virtual Task<T> ConvertFromRawXmlAsync(string rawXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(TData));

            var xmlDoc = XDocument.Parse(rawXml);
            using (var reader = xmlDoc.CreateReader())
            {
                var encoded = (TData)xmlSerializer.Deserialize(reader);
                return MapAsync(encoded);
            }
        }

        #endregion
    }
}