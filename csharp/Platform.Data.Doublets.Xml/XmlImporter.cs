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

        public TLinkAddress Import(XmlReader reader, string documentName, CancellationToken cancellationToken)
        {
            TLinkAddress documentLinkAddress = _storage.CreateDocument(documentName);
            return Import(reader, documentLinkAddress, cancellationToken);
        }

        public TLinkAddress Import(string file, CancellationToken cancellationToken)
        {
            var documentLinkAddress = _storage.CreateDocument(file);
            using var reader = XmlReader.Create(file);
            return Import(reader,  documentLinkAddress, cancellationToken);
        }

        private TLinkAddress Import(XmlReader reader, TLinkAddress documentLinkAddress, CancellationToken cancellationToken)
        {
            var rootElementLinkAddress = ImportNodes(reader, cancellationToken);
            _storage.AttachElementToDocument(documentLinkAddress, rootElementLinkAddress);
            return documentLinkAddress;
        }

        private TLinkAddress ImportNodes(XmlReader reader, CancellationToken cancellationToken)
        {
            var elements = new Stack<XmlElement<TLinkAddress>>();
            while (reader.Read())
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        var element = new XmlElement<TLinkAddress> { LocalName = reader.LocalName, Prefix = reader.NamespaceURI != null ? new XmlPrefix(){Prefix = reader.Prefix, NamespaceUri = reader.NamespaceURI} : null };
                        // var element = new XmlElement<TLinkAddress> { Name = reader.Name };
                        elements.Push(element);
                        // Save IsEmptyElement field before moving to the next attribute (reader value will change)
                        var isEmptyElement = reader.IsEmptyElement;
                        while (reader.MoveToNextAttribute())
                        {
                            var attributeAddress = _storage.CreateAttribute(reader.Prefix, reader.LocalName, reader.Value);
                            element.Children.Add(attributeAddress); 
                        }
                        if (isEmptyElement)
                        {
                            goto case XmlNodeType.EndElement;
                        }
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        
                        var element = elements.Pop();
                        var xmlElementAddress = _storage.CreateElement(element.Prefix, element.LocalName, element.Children);
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
                    case XmlNodeType.Attribute:
                    {
                        
                        break;
                    }
                }
            }
            throw new Exception("Could not import XML document");
        }
    }
}
