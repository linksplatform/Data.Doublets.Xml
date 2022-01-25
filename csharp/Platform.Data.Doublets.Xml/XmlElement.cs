using System;
using System.Collections.Generic;
using System.Xml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    class XmlElement<TLinkAddress>
    {
        private static EqualityComparer<TLinkAddress> _equalityComparer = EqualityComparer<TLinkAddress>.Default;
        public XmlElement<TLinkAddress> Parent;
        public TLinkAddress Link;
        public string Name;
        public int Depth;
        public XmlNodeType Type;
        public string Value;
        public Type ValueType;
        public Stack<TLinkAddress> ChildrenSequence;

        public bool HasChild(XmlElement<TLinkAddress> parent, XmlElement<TLinkAddress> child) => Parent.Depth == child.Depth + 1;
    }
}
