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
            TLinkAddress documentLinkAddress = _storage.CreateDocument(documentName);
            return Import(reader, documentLinkAddress, token);
        }

        public TLinkAddress Import(string file, CancellationToken token)
        {
            var documentLinkAddress = _storage.CreateDocument(file);
            using var reader = XmlReader.Create(file);
            return Import(reader,  documentLinkAddress, token);
        }

        private TLinkAddress Import(XmlReader reader, TLinkAddress documentLinkAddress, CancellationToken token)
        {
            var childNodeLinkAddressList = ImportNodes(reader, token);
            var childrenNodesSequenceLinkAddress = _storage.ListToSequenceConverter.Convert(childNodeLinkAddressList);
            var childrenNodesLinkAddress = _storage.CreateDocumentChildrenNodesLinkAddress(childrenNodesSequenceLinkAddress);
            _storage.AttachDocumentChildrenNodes(documentLinkAddress, childrenNodesLinkAddress);
            return documentLinkAddress;
        }

        private IList<TLinkAddress> ImportNodes(XmlReader reader, CancellationToken token)
        {
            var nodes = new List<TLinkAddress>();
            var elements = new Stack<XmlElement<TLinkAddress>>();
            while (reader.Read())
            {
                if (token.IsCancellationRequested)
                {
                    return nodes;
                }
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        var element = new XmlElement<TLinkAddress> { Name = reader.Name, Type = XmlNodeType.Element };
                        elements.Push(element);
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        var element = elements.Pop();
                        var childrenSequence = _listToSequenceConverter.Convert(element.Children);
                        var xmlElementAddress = _storage.CreateElement(reader.Name, childrenSequence);
                        var hasParent = elements.Count > 0;
                        if (hasParent)
                        {
                            var parent = elements.Peek();
                            parent.Children.Add(xmlElementAddress);
                        }
                        else
                        {
                            nodes.Add(xmlElementAddress);
                        }
                        break;
                    }
                    case XmlNodeType.Text:
                    {
                        var textNodeAddress = _storage.CreateTextNode(reader.Value);
                        var parent = elements.Peek();
                        parent.Children.Add(textNodeAddress);
                        break;
                    }
                    case XmlNodeType.None:
                        break;
                    case XmlNodeType.Attribute:
                    {
                        var attributeNodeAddress = _storage.CreateAttributeNode(reader.Name, reader.Value);
                        var parent = elements.Peek();
                        parent.Children.Add(attributeNodeAddress);
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
            return nodes;
        }
    }
}
