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

        private void Write(XmlWriter xmlWriter, TLinkAddress document, CancellationToken cancellationToken)
        {
            var any = _storage.Links.Constants.Any;
            var documentSequenceLink = _storage.Links.SearchOrDefault(document, any);
            var xmlNodesSequenceLinkAddress = _storage.Links.GetTarget(documentSequenceLink);
            RightSequenceWalker<TLinkAddress> rightSequenceWalker = new(_storage.Links, new DefaultStack<TLinkAddress>() /*, linkAddress =>
            {
                var source = _storage.Links.GetSource(linkAddress);
                var isTextNode = _storage.IsTextNode(source);
                var isAttributeNode = _storage.IsAttributeNode(source);
                var isElement = _storage.IsElement(source);
                return isTextNode || isAttributeNode || isElement;
            } */);
            var xmlNodesSequence = rightSequenceWalker.Walk(xmlNodesSequenceLinkAddress);
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
                throw new NotSupportedException("The passed link address is not a text, attribute or element.");
            }
        }
        
        private void ExportElement(XmlWriter xmlWriter, TLinkAddress elementLinkAddress)
        {
            var currentElementLinkAddress = elementLinkAddress;
            var endElementCount = 0;
            do
            {
                var elementName = _storage.GetElementName(currentElementLinkAddress);
                xmlWriter.WriteStartElement(elementName);
                var attrubute = _storage.GetAttributeForElement(currentElementLinkAddress);
                var 
                var childrenElementLinkAddressList = _storage.GetChildrenNodes(currentElementLinkAddress);    
                foreach (var childElementLinkAddress in childrenElementLinkAddressList)
                {
                    
                }
                endElementCount++;
            } while (true);
            for (int i = 0; i < endElementCount; i++)
            {
                xmlWriter.WriteEndElement();
            }            
        }

        private void ExportAttributeNode(XmlWriter xmlWriter, TLinkAddress xmlNodeLinkAddress)
        {
            var attributeName = _storage.GetAttributeName(xmlNodeLinkAddress);
            var attributeValue = _storage.GetAttributeValue(xmlNodeLinkAddress);
            xmlWriter.WriteAttributeString(attributeName, attributeValue);
        }

        public void ExportTextNode(XmlWriter xmlWriter, TLinkAddress textNodeLinkAddress)
        {
            var text = _storage.GetTextNodeValue(textNodeLinkAddress);
            xmlWriter.WriteString(text);
        }
    }
}
