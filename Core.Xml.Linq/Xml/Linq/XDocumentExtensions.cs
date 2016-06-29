using System;
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
        /// <param name="xDoc">The <see cref="XDocument"/> instance to copy.</param>
        /// <returns>A new copy of a <see cref="XDocument"/> with the namespaces removed.</returns>
        public static XDocument CopyWithoutNamespaces(this XDocument xDoc)
        {
            if (xDoc == null)
            {
                throw new ArgumentNullException(nameof(xDoc));
            }

            var copy = xDoc.Root.CopyWithoutNamespaces();

            return new XDocument(copy);
        }

        /// <summary>
        /// Lower case the name of all the elements for the current XDocument.
        /// </summary>
        public static void LowerCaseAllElementNames(this XDocument xDoc)
        {
            if (xDoc == null)
            {
                throw new ArgumentNullException(nameof(xDoc));
            }

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
            if (xDoc == null)
            {
                throw new ArgumentNullException(nameof(xDoc));
            }

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