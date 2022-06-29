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
            foreach (var xmlNodeLinkAddress in xmlNodesSequence)
            {
                if (_storage.IsTextElement(xmlNodeLinkAddress))
                {
                    ExportTextElement(xmlWriter, xmlNodeLinkAddress);
                }
                if (_storage.IsAttributeElement(xmlNodeLinkAddress))
                {
                    ExportAttributeElement(xmlWriter, xmlNodeLinkAddress);
                }
                if (_storage.IsElement(xmlNodeLinkAddress))
                {
                    ExportElement(xmlWriter, xmlNodeLinkAddress);
                }
            }
        }

        private void ExportElement(XmlWriter xmlWriter, TLinkAddress xmlNodeLinkAddress)
        {
            var elementName = _storage.GetElementName(xmlNodeLinkAddress);
            xmlWriter.WriteStartElement(elementName);
            var childrenElementLinkAddressList = _storage.GetChildrenElements(xmlNodeLinkAddress);
            foreach (var childElementLinkAddress in childrenElementLinkAddressList)
            {
                
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
