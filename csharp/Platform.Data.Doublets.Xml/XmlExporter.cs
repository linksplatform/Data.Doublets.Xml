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
        public void Export(TLinkAddress document, XmlWriter xmlWriter, CancellationToken token)
        {
            if (DefaultEqualityComparer.Equals(default, document))
            {
                throw new Exception("The document does not exist.");
            }
            Write(xmlWriter, document, token);
        }

        private void Write(XmlWriter writer, TLinkAddress document, CancellationToken token)
        {
            var any = _storage.Links.Constants.Any;
            var documentSequenceLink = _storage.Links.SearchOrDefault(document, any);
            var sequence = _storage.Links.GetTarget(documentSequenceLink);
            RightSequenceWalker<TLinkAddress> rightSequenceWalker = new(_storage.Links, new DefaultStack<TLinkAddress>(), linkAddress =>
            {
                var source = _storage.Links.GetSource(linkAddress);
                var sourceOfSource = _storage.Links.GetSource(source);
                var isTextElement = DefaultEqualityComparer.Equals(source, _storage.TextElementMarker);
                var isAttribute = DefaultEqualityComparer.Equals(source, _storage.AttributeMarker);
                var isElement = DefaultEqualityComparer.Equals(sourceOfSource, _storage.ElementMarker);
                return isTextElement || isAttribute || isElement;
            });
        }
    }
}
