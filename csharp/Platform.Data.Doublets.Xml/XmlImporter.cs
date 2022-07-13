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
        
        public XmlImporter(DefaultXmlStorage<TLinkAddress> storage) : this(storage, new BalancedVariantConverter<TLinkAddress>(storage.Links)) {}

        public XmlImporter(DefaultXmlStorage<TLinkAddress> storage, LinksListToSequenceConverterBase<TLinkAddress> listToSequenceConverter)
        {
            _storage = storage;
            _listToSequenceConverter = listToSequenceConverter;
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
            var rootElementLinkAddress = ImportNodes(reader, token);
            _storage.AttachElementToDocument(documentLinkAddress, rootElementLinkAddress);
            return documentLinkAddress;
        }

        private TLinkAddress ImportNodes(XmlReader reader, CancellationToken token)
        {
            var elements = new Stack<XmlElement<TLinkAddress>>();
            while (reader.Read())
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        var element = new XmlElement<TLinkAddress> { Name = reader.Name };
                        elements.Push(element);
                        if (reader.IsEmptyElement)
                        {
                            goto case XmlNodeType.EndElement;
                        }
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        var element = elements.Pop();
                        var xmlElementAddress = _storage.CreateElement(element.Name, element.Children);
                        var hasParent = elements.Count > 0;
                        if (hasParent)
                        {
                            var parent = elements.Peek();
                            parent.Children.Add(xmlElementAddress);
                        }
                        else
                        {
                            return xmlElementAddress;
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
            throw new Exception("Could not import XML document");
        }
    }
}
