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
    public class XmlImporter<TLink> where TLink : struct
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
        
        public Task<TLink> Import(XmlReader reader, string documentName, CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
                TLink document = default;
                try
                {
                    document = _storage.CreateDocument(documentName);
                    Read(reader, token, new ElementContext(document));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToStringWithAllInnerExceptions());
                }
                return document;
            }, token);
        }


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
                        elements.Push(elementName);
                        ConsoleHelpers.Debug("{0} import is started.", elements.Count <= 20 ? ToXPath(elements) : elementName); // XPath
                        var element = _storage.CreateElement(elementName);
                        parentContexts.Push(context);
                        // TODO: Do not attach child directly. Create a List for all child elements and then use BanacedVariantConverter to convert that list to sequence link
                        _storage.Attach(context.Parent, element);
                        context = new ElementContext(element);
                        break;
                    case XmlNodeType.EndElement:
                        ConsoleHelpers.Debug("{0} import is finished.", elements.Count <= 20 ? ToXPath(elements) : elements.Peek()); // XPath
                        elements.Pop();
                        // Restoring scope
                        context = parentContexts.Pop();
                        break;
                    case XmlNodeType.Text:
                        ConsoleHelpers.Debug("Text element import is started.");
                        var content = reader.Value;
                        ConsoleHelpers.Debug("Content: {0}{1}", content.Truncate(50), content.Length >= 50 ? "..." : "");
                        var textElement = _storage.CreateTextElement(content);
                        _storage.Attach(context.Parent, textElement);
                        ConsoleHelpers.Debug("Text element import is finished.");
                        break;
                    case XmlNodeType.None:
                        break;
                    case XmlNodeType.Attribute:
                        break;
                    case XmlNodeType.CDATA:
                        break;
                    case XmlNodeType.EntityReference:
                        break;
                    case XmlNodeType.Entity:
                        break;
                    case XmlNodeType.ProcessingInstruction:
                        break;
                    case XmlNodeType.Comment:
                        break;
                    case XmlNodeType.Document:
                        break;
                    case XmlNodeType.DocumentType:
                        break;
                    case XmlNodeType.DocumentFragment:
                        break;
                    case XmlNodeType.Notation:
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    case XmlNodeType.SignificantWhitespace:
                        break;
                    case XmlNodeType.EndEntity:
                        break;
                    case XmlNodeType.XmlDeclaration:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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
