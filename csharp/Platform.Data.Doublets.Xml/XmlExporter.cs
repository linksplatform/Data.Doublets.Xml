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
                var isTextElement = _storage.IsTextElement(source);
                var isAttributeElement = _storage.IsAttributeElement(source);
                var isElement = _storage.IsElement(source);
                return isTextElement || isAttributeElement || isElement;
            } */);
            var xmlNodesSequence = rightSequenceWalker.Walk(xmlNodesSequenceLinkAddress);
        }

        public void ExportNode(XmlWriter xmlWriter, TLinkAddress nodeLinkAddress)
        {
            if (_storage.IsTextNode(nodeLinkAddress))
            {
                ExportTextElement(xmlWriter, nodeLinkAddress);
            }
            else if (_storage.IsAttributeNode(nodeLinkAddress))
            {
                ExportAttributeElement(xmlWriter, nodeLinkAddress);
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

        private void ExportAttributeElement(XmlWriter xmlWriter, TLinkAddress xmlNodeLinkAddress)
        {
            var attributeName = _storage.GetAttributeName(xmlNodeLinkAddress);
            var attributeValue = _storage.GetAttributeValue(xmlNodeLinkAddress);
            xmlWriter.WriteAttributeString(attributeName, attributeValue);
        }

        public void ExportTextElement(XmlWriter xmlWriter, TLinkAddress textElementLinkAddress)
        {
            var text = _storage.GetTextElementValue(textElementLinkAddress);
            xmlWriter.WriteString(text);
        }
    }
}
