using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Platform.Exceptions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    public class XmlExporter<TLinkAddress> where TLinkAddress : struct
    {
        private readonly IXmlStorage<TLinkAddress> _storage;
        private EqualityComparer<TLinkAddress> _defaultEqualityComparer = EqualityComparer<TLinkAddress>.Default;
        public XmlExporter(IXmlStorage<TLinkAddress> storage) => _storage = storage;
        public Task Export(string documentName, Stream stream, CancellationToken token) => Export(_storage.GetDocumentOrDefault(documentName), stream, token);

        public Task Export(TLinkAddress document, Stream stream, CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
            if (_defaultEqualityComparer.Equals(default, document))
            {
                throw new Exception("The document does not exist.");
            }
            using var writer = XmlWriter.Create(stream);
            Write(writer, document, token);
            }, token);
        }

        private void Write(XmlWriter writer, TLinkAddress context, CancellationToken token) => Write(writer, new XmlElement<TLinkAddress>{Parent = {Link = context} }, token);

        private void Write(XmlWriter writer, XmlElement<TLinkAddress> context, CancellationToken token)
        {
            foreach(TLinkAddress child in _storage.GetChildrenElements(context.Parent.Link))
            {
                Write(writer, child, token);
            }
        }
    }
}
