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
            TLinkAddress document = _storage.CreateDocument(documentName);
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

        private void Read(XmlReader reader, CancellationToken token, TLinkAddress documentAddress)
        {
            var documentChildren = ParseXmlElements(reader, token);
            var documentChildrenSequence = _storage.ListToSequenceConverter.Convert(documentChildren);
            _storage.Links.GetOrCreate(documentAddress, documentChildrenSequence);
        }

        private IList<TLinkAddress> ParseXmlElements(XmlReader reader, CancellationToken token)
        {
            var documentElements = new List<TLinkAddress>();
            var parents = new Stack<XmlElement<TLinkAddress>>();
            while (reader.Read())
            {
                if (token.IsCancellationRequested)
                {
                    return documentElements;
                }
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        var xmlNode = new XmlElement<TLinkAddress> { Name = reader.Name, Type = XmlNodeType.Element };
                        parents.Push(xmlNode);
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        var element = parents.Pop();
                        var childrenSequence = _listToSequenceConverter.Convert(element.Children);
                        var xmlElementAddress = _storage.CreateElement(reader.Name, childrenSequence);
                        var hasParent = parents.Count > 0;
                        if (hasParent)
                        {
                            var parent = parents.Peek();
                            parent.Children.Add(xmlElementAddress);
                        }
                        else
                        {
                            documentElements.Add(xmlElementAddress);
                        }
                        break;
                    }
                    case XmlNodeType.Text:
                    {
                        var textElementAddress = _storage.CreateTextElement(reader.Value);
                        var parent = parents.Peek();
                        parent.Children.Add(textElementAddress);
                        break;
                    }
                    case XmlNodeType.None:
                        break;
                    case XmlNodeType.Attribute:
                    {
                        var attributeElementAddress = _storage.CreateAttributeElement(reader.Name, reader.Value);
                        var parent = parents.Peek();
                        parent.Children.Add(attributeElementAddress);
                        break;
                    }
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
            return documentElements;
        }
    }
}
