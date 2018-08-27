using System.Xml;
using System.Xml.Linq;

namespace Nop.Plugin.ExternalSuppliers.STM.Extensions
{
    public static class DocumentExtensions
    {
        public static XmlDocument ToXmlDocument(this XElement xElement)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xElement.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XElement ToXElement(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XElement.Load(nodeReader);
            }
        }
    }
}
