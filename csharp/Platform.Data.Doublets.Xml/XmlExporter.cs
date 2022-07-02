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
        public readonly EqualityComparer<TLinkAddress> DefaultEqualityComparer = EqualityComparer<TLinkAddress>.Default;
        public XmlExporter(DefaultXmlStorage<TLinkAddress> storage) => _storage = storage;

        public void Export(XmlWriter xmlWriter, string documentName, CancellationToken cancellationToken)
        {
            var documentAddress = _storage.GetDocumentLinkAddress(documentName); 
            Export(xmlWriter, documentAddress, cancellationToken);
        }

        public void Export(XmlWriter xmlWriter, TLinkAddress documentLinkAddress, CancellationToken cancellationToken)
        {
            Write(xmlWriter, documentLinkAddress, cancellationToken);
        }

        private void Write(XmlWriter xmlWriter, TLinkAddress documentLinkAddress, CancellationToken cancellationToken)
        {
            var childrenNodes = _storage.GetDocumentChildNodeLinkAddresses(documentLinkAddress);
            foreach (var childNode in childrenNodes)
            {
                ExportNode(xmlWriter, childNode);
            }
        }

        public void ExportNode(XmlWriter xmlWriter, TLinkAddress nodeLinkAddress)
        {
            if (_storage.IsTextNode(nodeLinkAddress))
            {
                ExportTextNode(xmlWriter, nodeLinkAddress);
            }
            else if (_storage.IsAttributeNode(nodeLinkAddress))
            {
                ExportAttributeNode(xmlWriter, nodeLinkAddress);
            }
            else if (_storage.IsElement(nodeLinkAddress))
            {
                ExportElement(xmlWriter, nodeLinkAddress);
            }
            else
            {
                throw new ArgumentException("The passed link address is not a text, attribute or element.", nameof(nodeLinkAddress));
            }
        }
        
        private void ExportElement(XmlWriter xmlWriter, TLinkAddress elementLinkAddress)
        {
            var elementName = _storage.GetElementName(elementLinkAddress);
            xmlWriter.WriteStartElement(elementName);
            var childrenNodesLinkAddressList = _storage.GetElementChildrenNodes(elementLinkAddress);    
            foreach (var childNodeLinkAddress in childrenNodesLinkAddressList)
            {
                ExportNode(xmlWriter, childNodeLinkAddress);
            }
            xmlWriter.WriteEndElement();
        }

        private void ExportAttributeNode(XmlWriter xmlWriter, TLinkAddress xmlNodeLinkAddress)
        {
            var attribute = _storage.GetAttribute(xmlNodeLinkAddress);
            xmlWriter.WriteAttributeString(attribute.Name, attribute.Value);
        }

        public void ExportTextNode(XmlWriter xmlWriter, TLinkAddress textNodeLinkAddress)
        {
            var text = _storage.GetTextNodeValue(textNodeLinkAddress);
            xmlWriter.WriteString(text);
        }
    }
}
