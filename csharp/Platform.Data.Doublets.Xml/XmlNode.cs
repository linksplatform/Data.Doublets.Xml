using System;
using System.Collections.Generic;
using System.Xml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    public class XmlNode<TLinkAddress>
    {
        private static EqualityComparer<TLinkAddress> _equalityComparer = EqualityComparer<TLinkAddress>.Default;
        public XmlNode<TLinkAddress> Parent;
        public TLinkAddress Link;
        public string Name;
        public int Depth;
        public XmlNodeType Type;
        public string Value;
        public Type ValueType;
        public Queue<XmlNode<TLinkAddress>> Children = new Queue<XmlNode<TLinkAddress>>();

        public bool HasChild(XmlNode<TLinkAddress> parent, XmlNode<TLinkAddress> child) => Parent.Depth == child.Depth + 1;
    }
}
