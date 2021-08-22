using System;
using System.Linq;
using System.Collections.Generic;
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
    class XmlExporter<TLink>
    {
        /// <summary>
        /// <para>
        /// The storage.
        /// </para>
        /// <para></para>
        /// </summary>
        private readonly IXmlStorage<TLink> _storage;

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
        public Task Export(string documentName, string fileName, CancellationToken token) 
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var document = _storage.GetDocument(documentName);
                    using (var writer = XmlWriter.Create(fileName))
                    {
                        Write(writer, token, new ElementContext(document));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToStringWithAllInnerExceptions());
                }
            }, token);
        }

        /// <summary>
        /// <para>
        /// Writes the writer.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="writer">
        /// <para>The writer.</para>
        /// <para></para>
        /// </param>
        /// <param name="token">
        /// <para>The token.</para>
        /// <para></para>
        /// </param>
        /// <param name="context">
        /// <para>The context.</para>
        /// <para></para>
        /// </param>
        private void Write(XmlWriter writer, CancellationToken token, ElementContext context)
        {
            var parentContexts = new Stack<ElementContext>();
            var elements = new Stack<string>(); // Path
                                                // TODO: If path was loaded previously, skip it.
            foreach(TLink lvl in _storage.GetChildren(parent: context.Parent))
            {
                Write(writer: writer, token: token, context: new ElementContext(lvl));
            }
        }
        
        /// <summary>
        /// <para>
        /// Represents the element context.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <seealso cref="XmlElementContext"/>
        private class ElementContext : XmlElementContext
        {
            /// <summary>
            /// <para>
            /// The parent.
            /// </para>
            /// <para></para>
            /// </summary>
            public readonly TLink Parent;

            /// <summary>
            /// <para>
            /// Initializes a new <see cref="ElementContext"/> instance.
            /// </para>
            /// <para></para>
            /// </summary>
            /// <param name="parent">
            /// <para>A parent.</para>
            /// <para></para>
            /// </param>
            public ElementContext(TLink parent) => Parent = parent;
        }

    }
}
