using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Platform.Exceptions;
using Platform.Collections;
using Platform.Collections.Stacks;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.IO;
using Platform.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml {
    public class XmlImporter<TLinkAddress> where TLinkAddress : struct
    {
        private readonly DefaultXmlStorage<TLinkAddress> _storage;
        private readonly LinksListToSequenceConverterBase<TLinkAddress> _listToSequenceConverter;
        private readonly IEqualityComparer _equalityComparer;

        public XmlImporter(DefaultXmlStorage<TLinkAddress> storage) : this(storage, new BalancedVariantConverter<TLinkAddress>(storage.Links)) {}

        public XmlImporter(DefaultXmlStorage<TLinkAddress> storage, LinksListToSequenceConverterBase<TLinkAddress> listToSequenceConverter)
        {
            _storage = storage;
            _listToSequenceConverter = listToSequenceConverter;
            _equalityComparer = EqualityComparer<TLinkAddress>.Default;
        }

        public TLinkAddress Import(XmlReader reader, string documentName, CancellationToken token)
        {
            TLinkAddress document = default;
            document = _storage.CreateDocument(documentName);
            Read(reader, token, document);
            return document;
        }

        public TLinkAddress Import(string file, CancellationToken token)
        {
            var document = _storage.CreateDocument(file);
            using var reader = XmlReader.Create(file);
            Read(reader, token,  document);
            return document;
        }

        private void Read(XmlReader reader, CancellationToken token, TLinkAddress document)
        {
            var xmlNodes = ParseXmlElements(reader, token);
            var documentChildren = new List<TLinkAddress>();
            foreach (var xmlNode in xmlNodes)
            {
                var node = _storage.CreateNode(xmlNode);
                documentChildren.Add(node);
            }
            var documentChildrenSequence = _storage.ListToSequenceConverter.Convert(documentChildren);
            _storage.Attach(documentChildrenSequence, document);
        }

        private IList<XmlNode<TLinkAddress>> ParseXmlElements(XmlReader reader, CancellationToken token)
        {
            var xmlElements = new List<XmlNode<TLinkAddress>>();
            var parents = new Stack<XmlNode<TLinkAddress>>();
            var i = 0;
            while (reader.Read())
            {
                if (token.IsCancellationRequested)
                {
                    return xmlElements;
                }
                ++i;
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        var xmlElement = new XmlNode<TLinkAddress> { Name = reader.Name, Type = XmlNodeType.Element };
                        xmlElements.Add(xmlElement);
                        if (parents.Count > 0)
                        {
                            parents.Peek().Children.Enqueue(xmlElement);
                        }
                        parents.Push(xmlElement);
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {

                        break;
                    }
                    case XmlNodeType.Text:
                    {
                        var xmlElement = new XmlNode<TLinkAddress> { Value = reader.Value, ValueType = reader.ValueType, Type = XmlNodeType.Text};
                        xmlElements.Add(xmlElement);
                        parents.Peek().Children.Enqueue(xmlElement);
                        break;
                    }
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
            return xmlElements;
        }

        private void ReadElement(XmlReader reader, CancellationToken token, XmlNode<TLinkAddress> node)
        {
        }

        // private void Read(XmlReader reader, CancellationToken token, XmlElement<TLinkAddress> parent)
        // {
        //     Stack<XmlElement<TLinkAddress>> parents = new();
        //     Stack<XmlElement<TLinkAddress>> elements = new();
        //     // TODO: If path was loaded previously, skip it.
        //     while (reader.Read())
        //     {
        //         if (token.IsCancellationRequested)
        //         {
        //             return;
        //         }
        //         switch (reader.NodeType)
        //         {
        //             case XmlNodeType.Element:
        //             {
        //                 var elementName = reader.Name;
        //                 var element = _storage.CreateElement(elementName);
        //                 parents.Push(new XmlElement<TLinkAddress>(element, parents.PeekOrDefault()));
        //                 break;
        //             }
        //             case XmlNodeType.EndElement:
        //             {
        //                 var element = parents.Pop();
        //                 var children = new List<TLinkAddress>();
        //                 for (int i = elements.Count - 1; i >= 0; i--)
        //                 {
        //                     var currentSibling = elements.ElementAtOrDefault(i);
        //                     if (currentSibling == default)
        //                     {
        //                         continue;
        //                     }
        //                     if (_equalityComparer.Equals(element.Link, currentSibling.Parent.Link))
        //                     {
        //                         children.Add(currentSibling.Link);
        //                         elements.Pop();
        //                     }
        //                 }
        //                 if (children.Count != 0)
        //                 {
        //                     var childrenSequence = _listToSequenceConverter.Convert(children);
        //                     _storage.Attach(element.Link, childrenSequence);
        //                 }
        //                 elements.Push(element);
        //                 break;
        //             }
        //             case XmlNodeType.Text:
        //                 ConsoleHelpers.Debug("Text element import is started.");
        //                 var content = reader.Value;
        //                 ConsoleHelpers.Debug("Content: {0}{1}", content.Truncate(50), content.Length >= 50 ? "..." : "");
        //                 var textElement = _storage.CreateTextElement(content);
        //                 _storage.Attach(parent.Link, textElement);
        //                 ConsoleHelpers.Debug("Text element import is finished.");
        //                 break;
        //             case XmlNodeType.None:
        //                 break;
        //             case XmlNodeType.Attribute:
        //                 break;
        //             case XmlNodeType.CDATA:
        //                 break;
        //             case XmlNodeType.EntityReference:
        //                 break;
        //             case XmlNodeType.Entity:
        //                 break;
        //             case XmlNodeType.ProcessingInstruction:
        //                 break;
        //             case XmlNodeType.Comment:
        //                 break;
        //             case XmlNodeType.Document:
        //                 break;
        //             case XmlNodeType.DocumentType:
        //                 break;
        //             case XmlNodeType.DocumentFragment:
        //                 break;
        //             case XmlNodeType.Notation:
        //                 break;
        //             case XmlNodeType.Whitespace:
        //                 break;
        //             case XmlNodeType.SignificantWhitespace:
        //                 break;
        //             case XmlNodeType.EndEntity:
        //                 break;
        //             case XmlNodeType.XmlDeclaration:
        //                 break;
        //             default:
        //                 throw new ArgumentOutOfRangeException();
        //         }
        //     }
        // }


        private string ToXPath(Stack<string> path) => string.Join("/", path.Reverse());
    }
}
