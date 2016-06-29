using System;
using System.Linq;
using System.Xml.Linq;

namespace Sfa.Core.Xml.Linq
{
    /// <summary>
    /// Extends the standard <see cref="XElement"/> class with more functions.
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Creates a new copy of the element with all of the namespaces removed from the element and all of its children.
        /// </summary>
        /// <param name="element">The element to remove namespaces from.</param>
        /// <returns>A new instance with all namespaces removed.</returns>
        public static XElement CopyWithoutNamespaces(this XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!element.HasElements)
            {
                var xElement = new XElement(element.Name.LocalName) { Value = element.Value };

                foreach (var attribute in element.Attributes())
                {
                    xElement.Add(attribute);
                }

                return xElement;
            }
            return new XElement(element.Name.LocalName, element.Elements().Select(CopyWithoutNamespaces));
        }

        /// <summary>
        /// Lower case the element name of the current element and all its child elements.
        /// </summary>
        public static XElement LowerCaseAllElementNames(this XElement xElement)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }

            xElement.Name = xElement.Name.ToString().ToLower();

            foreach (var element in xElement.Elements())
            {
                element.LowerCaseAllElementNames();
            }

            return xElement;
        }

        /// <summary>
        /// Lower case the element name of the current element and all its child elements.
        /// </summary>
        public static XElement LowerCaseAllAttributeNames(this XElement xElement)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }

            // Replacing attributes here as XAttribute.Name has no setter.
            foreach (var attribute in xElement.Attributes())
            {
                if (!attribute.IsNamespaceDeclaration && string.IsNullOrEmpty(attribute.Name.NamespaceName))
                {
                    var newAttribute = new XAttribute(attribute.Name.LocalName.ToLower(), attribute.Value);
                    attribute.Remove();
                    xElement.Add(newAttribute);
                }
            }

            foreach (var element in xElement.Elements())
            {
                element.LowerCaseAllAttributeNames();
            }

            return xElement;
        }
    }
}