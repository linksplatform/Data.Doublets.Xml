using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Platform.Exceptions;
using Platform.Collections;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.IO;
using Platform.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml {
    /// <summary>
    /// <para>
    /// Represents the xml importer.
    /// </para>
    /// <para></para>
    /// </summary>
    public class XmlImporter<TLink>
    {
        private readonly IXmlStorage<TLink> _storage;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="XmlImporter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="storage">
        /// <para>A storage.</para>
        /// <para></para>
        /// </param>
        public XmlImporter(IXmlStorage<TLink> storage) => _storage = storage;

        /// <summary>
        /// <para>
        /// Imports the file.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="file">
        /// <para>The file.</para>
        /// <para></para>
        /// </param>
        /// <param name="token">
        /// <para>The token.</para>
        /// <para></para>
        /// </param>
        public Task Import(string file, CancellationToken token) =>
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var document = _storage.CreateDocument(file);
                    using var reader = XmlReader.Create(file);
                    Read(reader, token, new ElementContext(document));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToStringWithAllInnerExceptions());
                }

            }, token);
        
        public Task Import(XmlReader reader, string documentName, CancellationToken token) =>
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var document = _storage.CreateDocument(documentName);
                    Read(reader, token, new ElementContext(document));
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToStringWithAllInnerExceptions());
                }

            }, token);

        private void Read(XmlReader reader, CancellationToken token, ElementContext context)
        {
            var parentContexts = new Stack<ElementContext>();
            var elements = new Stack<string>(); // Path
            // TODO: If path was loaded previously, skip it.
            while (reader.Read())
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        var elementName = reader.Name;
                        context.IncrementChildNameCount(elementName);
                        elementName = $"{elementName}[{context.ChildrenNamesCounts[elementName]}]";
                        if (!reader.IsEmptyElement)
                        {
                            elements.Push(elementName);
                            ConsoleHelpers.Debug("{0} starting...", elements.Count <= 20 ? ToXPath(elements) : elementName); // XPath
                            var element = _storage.CreateElement(name: elementName);
                            parentContexts.Push(context);
                            _storage.AttachElementToParent(elementToAttach: element, parent: context.Parent);
                            context = new ElementContext(element);
                        }
                        else
                        {
                            ConsoleHelpers.Debug("{0} finished.", elementName);
                        }
                        break;
                    case XmlNodeType.EndElement:
                        ConsoleHelpers.Debug("{0} finished.", elements.Count <= 20 ? ToXPath(elements) : elements.Peek()); // XPath
                        elements.Pop();
                        // Restoring scope
                        context = parentContexts.Pop();
                        if (elements.Count == 1)
                        {
                            if (context.TotalChildren % 10 == 0)
                                Console.WriteLine(context.TotalChildren);
                        }
                        break;
                    case XmlNodeType.Text:
                        ConsoleHelpers.Debug("Starting text element...");
                        var content = reader.Value;
                        ConsoleHelpers.Debug("Content: {0}{1}", content.Truncate(50), content.Length >= 50 ? "..." : "");
                        var textElement = _storage.CreateTextElement(content: content);
                        _storage.AttachElementToParent(textElement, context.Parent);
                        ConsoleHelpers.Debug("Text element finished.");
                        break;
                }
            }
        }

        private string ToXPath(Stack<string> path) => string.Join("/", path.Reverse());

        private class ElementContext : XmlElementContext
        {
            public readonly TLink Parent;
            public ElementContext(TLink parent) => Parent = parent;
        }
    }
}
