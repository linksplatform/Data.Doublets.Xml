using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
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
        private readonly IXmlStorage<TLinkAddress> _storage;
        private readonly LinksListToSequenceConverterBase<TLinkAddress> _listToSequenceConverter;
        private readonly IEqualityComparer _equalityComparer;

        public XmlImporter(IXmlStorage<TLinkAddress> storage) : this(storage, new BalancedVariantConverter<TLinkAddress>(storage.Links)) {}

        public XmlImporter(IXmlStorage<TLinkAddress> storage, LinksListToSequenceConverterBase<TLinkAddress> listToSequenceConverter)
        {
            _storage = storage;
            _listToSequenceConverter = listToSequenceConverter;
            _equalityComparer = EqualityComparer<TLinkAddress>.Default;
        }

        public TLinkAddress Import(XmlReader reader, string documentName, CancellationToken token)
        {
            TLinkAddress document = default;
            document = _storage.CreateDocument(documentName);
            Read(reader, token, new XmlElement<TLinkAddress>{Link = document, Type = XmlNodeType.None});
            return document;
        }

        public TLinkAddress Import(string file, CancellationToken token)
        {
            var document = _storage.CreateDocument(file);
            using var reader = XmlReader.Create(file);
            Read(reader, token, new XmlElement<TLinkAddress>{Link = document, Type = XmlNodeType.None});
            return document;
        }


        private void Read(XmlReader reader, CancellationToken token, XmlElement<TLinkAddress> parent)
        {
            List<XmlElement<TLinkAddress>> elements = new();
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
                    {
                        var element = new XmlElement<TLinkAddress> { Name = reader.Name, Depth = reader.Depth, Type = XmlNodeType.Element};
                        elements.Add(element);
                        break;
                    }
                    case XmlNodeType.EndElement:
                        break;
                    case XmlNodeType.Text:
                    {
                        var element = new XmlElement<TLinkAddress> { Value = reader.Value, ValueType = reader.ValueType, Depth = reader.Depth, Type = XmlNodeType.Text};
                        elements.Add(element);
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
            for (int i = 0, depth = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                if (element.Depth <= elements[i+1].Depth)
                {
                    continue;
                }
                var children = new List<TLinkAddress>();
                for (int j = i; element.Depth == elements[i].Depth ; j--)
                {
                    switch (elements[j].Type)
                    {
                        case XmlNodeType.None:
                            break;
                        case XmlNodeType.Element:
                            break;
                        case XmlNodeType.Attribute:
                            break;
                        case XmlNodeType.Text:
                            var content = _storage.CreateTextElement()
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
                        case XmlNodeType.EndElement:
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
