using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Platform.Collections.Stacks;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Exceptions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    public class XmlExporter<TLinkAddress> where TLinkAddress : struct
    {
        private readonly DefaultXmlStorage<TLinkAddress> _storage;
        private readonly EqualityComparer<TLinkAddress> _defaultEqualityComparer = EqualityComparer<TLinkAddress>.Default;
        public XmlExporter(DefaultXmlStorage<TLinkAddress> storage) => _storage = storage;

        public void Export(XmlWriter xmlWriter, string documentName, CancellationToken cancellationToken)
        {
            var documentAddress = _storage.GetDocument(documentName); 
            Export(xmlWriter, documentAddress, cancellationToken);
        }

        public void Export(XmlWriter xmlWriter, TLinkAddress documentLinkAddress, CancellationToken cancellationToken)
        {
            var rootElementLinkAddress = _storage.GetRootElement(documentLinkAddress);
            ExportElement(xmlWriter, documentLinkAddress, rootElementLinkAddress, cancellationToken);
            xmlWriter.Flush();
        }

        private void ExportNode(XmlWriter xmlWriter, TLinkAddress documentLinkAddress,TLinkAddress nodeLinkAddress, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            if (_storage.IsTextNode(nodeLinkAddress))
            {
                ExportTextNode(xmlWriter, nodeLinkAddress);
            }
            else if (_storage.IsAttribute(nodeLinkAddress))
            {
                ExportAttribute(xmlWriter, nodeLinkAddress);
            }
            else if (_storage.IsElementNode(nodeLinkAddress))
            {
                ExportElement(xmlWriter, documentLinkAddress, nodeLinkAddress, cancellationToken);
            }
            else
            {
                throw new ArgumentException($"{nodeLinkAddress} is not a node link address.");
            }
        }
        
        private void ExportElement(XmlWriter xmlWriter, TLinkAddress documentLinkAddress, TLinkAddress elementLinkAddress, CancellationToken cancellationToken)
        {
            var element = _storage.GetElement(elementLinkAddress);
            if (element.Prefix == null)
            {
                xmlWriter.WriteStartElement(element.LocalName);
            }
            else
            {
                xmlWriter.WriteStartElement(element.Prefix.Prefix, element.LocalName, element.Prefix.NamespaceUri);
            }
            var childrenNodesLinkAddresses = element.Children;    
            foreach (var childNodeLinkAddress in childrenNodesLinkAddresses)
            {
                ExportNode(xmlWriter, documentLinkAddress, childNodeLinkAddress, cancellationToken);
            }
            xmlWriter.WriteEndElement();
        }



        private void ExportAttribute(XmlWriter xmlWriter, TLinkAddress attributeLinkAddress)
        {
            var attribute = _storage.GetAttribute(attributeLinkAddress);
            xmlWriter.WriteAttributeString(attribute.Prefix, attribute.LocalName, null, attribute.Value);
        }

        private void ExportTextNode(XmlWriter xmlWriter, TLinkAddress textNodeLinkAddress)
        {
            var text = _storage.GetTextNode(textNodeLinkAddress);
            xmlWriter.WriteString(text);
        }
    }
}
