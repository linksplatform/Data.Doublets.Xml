using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Platform.Collections.Stacks;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Exceptions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    public class XmlExporter<TLinkAddress> where TLinkAddress : struct
    {
        private readonly DefaultXmlStorage<TLinkAddress> _storage;
        private readonly EqualityComparer<TLinkAddress> _defaultEqualityComparer = EqualityComparer<TLinkAddress>.Default;
        public XmlExporter(DefaultXmlStorage<TLinkAddress> storage) => _storage = storage;

        public void Export(XmlWriter xmlWriter, string documentName, CancellationToken cancellationToken)
        {
            var documentAddress = _storage.GetDocument(documentName); 
            Export(xmlWriter, documentAddress, cancellationToken);
        }

        public void Export(XmlWriter xmlWriter, TLinkAddress documentLinkAddress, CancellationToken cancellationToken)
        {
            var rootElementLinkAddress = _storage.GetRootElement(documentLinkAddress);
            ExportElement(xmlWriter, rootElementLinkAddress);
            xmlWriter.Flush();
        }

        private void ExportNode(XmlWriter xmlWriter, TLinkAddress nodeLinkAddress)
        {
            if (_storage.IsTextNode(nodeLinkAddress))
            {
                ExportTextNode(xmlWriter, nodeLinkAddress);
            }
            else if (_storage.IsAttributeNode(nodeLinkAddress))
            {
                ExportAttributeNode(xmlWriter, nodeLinkAddress);
            }
            else if (_storage.IsElementNode(nodeLinkAddress))
            {
                ExportElement(xmlWriter, nodeLinkAddress);
            }
            else
            {
                throw new ArgumentException($"{nodeLinkAddress} is not a node link address.");
            }
        }
        
        private void ExportElement(XmlWriter xmlWriter, TLinkAddress elementLinkAddress)
        {
            var elementName = _storage.GetElementName(elementLinkAddress);
            xmlWriter.WriteStartElement(elementName);
            var childrenNodesLinkAddressList = _storage.GetChildrenNodes(elementLinkAddress);    
            foreach (var childNodeLinkAddress in childrenNodesLinkAddressList)
            {
                ExportNode(xmlWriter, childNodeLinkAddress);
            }
            xmlWriter.WriteEndElement();
        }

        private void ExportAttributeNode(XmlWriter xmlWriter, TLinkAddress xmlNodeLinkAddress)
        {
            var attribute = _storage.GetAttribute(xmlNodeLinkAddress);
            var attributeNameSemicolonIndex = attribute.Name.IndexOf(':');
            if(attributeNameSemicolonIndex == -1)
            {
                xmlWriter.WriteAttributeString(attribute.Name, attribute.Value);
            }
            else
            {
                var attributeName = attribute.Name.Substring(attributeNameSemicolonIndex + 1);
                xmlWriter.WriteAttributeString("xmlns", attributeName, null, attribute.Value);
            }
        }

        private void ExportTextNode(XmlWriter xmlWriter, TLinkAddress textNodeLinkAddress)
        {
            var text = _storage.GetTextNode(textNodeLinkAddress);
            xmlWriter.WriteString(text);
        }
    }
}
