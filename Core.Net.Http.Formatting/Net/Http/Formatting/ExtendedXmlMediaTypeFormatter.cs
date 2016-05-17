using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace Sfa.Core.Net.Http.Formatting
{
    /// <summary>
    /// Extends the base <see cref="XmlMediaTypeFormatter"/> class to be able to define new media types that should just be treated as xml.
    /// </summary>
    public class ExtendedXmlMediaTypeFormatter : XmlMediaTypeFormatter
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="mediaTypeToTreatAsXml">the media type that will be treated as xml.</param>
        public ExtendedXmlMediaTypeFormatter(string mediaTypeToTreatAsXml)
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaTypeToTreatAsXml));
        }
    }
}