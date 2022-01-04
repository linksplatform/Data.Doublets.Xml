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
    /// <summary>
    /// <para>
    /// Represents the xml exporter.
    /// </para>
    /// <para></para>
    /// </summary>
    public class XmlExporter<TLink> where TLink : struct
    {
        private readonly IXmlStorage<TLink> _storage;
        private EqualityComparer<TLink> _defaultEqualityComparer = EqualityComparer<TLink>.Default;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="XmlExporter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="storage">
        /// <para>A storage.</para>
        /// <para></para>
        /// </param>
        public XmlExporter(IXmlStorage<TLink> storage) => _storage = storage;

        /// <summary>
        /// <para>
        /// Exports the document name.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="documentName">
        /// <para>The document name.</para>
        /// <para></para>
        /// </param>
        /// <param name="fileName">
        /// <para>The file name.</para>
        /// <para></para>
        /// </param>
        /// <param name="token">
        /// <para>The token.</para>
        /// <para></para>
        /// </param>
        public Task Export(string documentName, Stream stream, CancellationToken token) => Export(_storage.GetDocumentOrDefault(documentName), stream, token);

        public Task Export(TLink document, Stream stream, CancellationToken token)
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

        private void Write(XmlWriter writer, TLink context, CancellationToken token) => Write(writer, new ElementContext(context), token);

        private void Write(XmlWriter writer, ElementContext context, CancellationToken token)
        {
            foreach(TLink child in _storage.GetChildrenElements(context.Parent))
            {
                Write(writer, child, token);
            }
        }

        private class ElementContext : XmlElementContext
        {
            public readonly TLink Parent;
            public ElementContext(TLink parent) => Parent = parent;
        }

    }
}
