using System.Xml.Linq;

namespace Sfa.Core.Xml.Linq
{
    /// <summary>
    /// Extends the standard <see cref="XDocument"/> class with more functions.
    /// </summary>
    public static class XDocumentExtensions
    {
        /// <summary>
        /// Parses the xml string into a new <see cref="XDocument"/> with all namespaces removed if specified within the xml string.
        /// </summary>
        /// <param name="rawXml">The raw xml string.</param>
        /// <returns>A new <see cref="XDocument"/> representation of the raw xml string.</returns>
        public static XDocument ParseAndRemoveAllNamespaces(string rawXml)
        {
            var xmlDocumentWithoutNs = XElement.Parse(rawXml).CloneAndRemoveAllNamespaces();

            return new XDocument(xmlDocumentWithoutNs);
        }

        /// <summary>
        /// Lower case the name of all the elements for the current XDocument.
        /// </summary>
        public static void LowerCaseAllElementNames(this XDocument xDoc)
        {
            foreach (var element in xDoc.Elements())
            {
                element.LowerCaseAllElementNames();
            }
        }

        /// <summary>
        /// Lower case the name of all the attributes for the current XDocument.
        /// </summary>
        public static void LowerCaseAllAttributeNames(this XDocument xDoc)
        {
            foreach (var element in xDoc.Elements())
            {
                element.LowerCaseAllAttributeNames();
            }
        }

        /// <summary>
        /// Lower case the name of all the elements and attributes for the current XDocument.
        /// </summary>
        public static void LowerCaseAllElementAndAttributeNames(this XDocument xDoc)
        {
            xDoc.LowerCaseAllElementNames();
            xDoc.LowerCaseAllAttributeNames();
        }
    }
}