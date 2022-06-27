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
            var documentAddress = _storage.GetDocument(documentName); 
            Export(xmlWriter, documentAddress, cancellationToken);
        }

        public void Export(XmlWriter xmlWriter, TLinkAddress documentAddress, CancellationToken cancellationToken)
        {
            Write(xmlWriter, documentAddress, cancellationToken);
        }

        private void Write(XmlWriter writer, TLinkAddress document, CancellationToken cancellationToken)
        {
            var any = _storage.Links.Constants.Any;
            var documentSequenceLink = _storage.Links.SearchOrDefault(document, any);
            var xmlNodesSequenceLinkAddress = _storage.Links.GetTarget(documentSequenceLink);
            RightSequenceWalker<TLinkAddress> rightSequenceWalker = new(_storage.Links, new DefaultStack<TLinkAddress>(), linkAddress =>
            {
                var source = _storage.Links.GetSource(linkAddress);
                var sourceOfSource = _storage.Links.GetSource(source);
                var isTextElement = DefaultEqualityComparer.Equals(source, _storage.TextElementMarker);
                var isAttribute = DefaultEqualityComparer.Equals(source, _storage.AttributeMarker);
                var isElement = DefaultEqualityComparer.Equals(sourceOfSource, _storage.ElementMarker);
                return isTextElement || isAttribute || isElement;
            });
            var xmlNodesSequence = rightSequenceWalker.Walk(xmlNodesSequenceLinkAddress);
            foreach (var xmlNodeLinkAddress in xmlNodesSequence)
            {
                var isTextElement = DefaultEqualityComparer.Equals(xmlNodeLinkAddress, _storage.TextElementMarker);
                if (isTextElement)
                {
                    var text = _storage.GetTextElementValue(xmlNodeLinkAddress);
                    writer.WriteString(text);
                }
                var isAttribute = DefaultEqualityComparer.Equals(xmlNodeLinkAddress, _storage.AttributeMarker);
                if (isAttribute)
                {
                    var attributeName = _storage.GetAttributeName(xmlNodeLinkAddress);
                    var attributeValue = _storage.GetAttributeValue(xmlNodeLinkAddress);
                    writer.WriteAttributeString(attributeName, attributeValue);
                }
                var isElement = DefaultEqualityComparer.Equals(xmlNodeLinkAddress, _storage.ElementMarker);
                if (isElement)
                {
                    var elementName = _storage.GetElementName(xmlNodeLinkAddress);
                    writer.WriteStartElement(elementName);
                }
            }
        }
    }
}
