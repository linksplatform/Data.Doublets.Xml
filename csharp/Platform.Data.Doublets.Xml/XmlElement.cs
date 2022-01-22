using System;
using System.Collections.Generic;
using System.Xml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    class XmlElement<TLink>
    {
        private static EqualityComparer<TLink> _equalityComparer = EqualityComparer<TLink>.Default;
        public XmlElement<TLink> Parent;
        public TLink Link;
        public string Name;
        public int Depth;
        public XmlNodeType Type;
        public string Value;
        public Type ValueType;
        public Stack<TLink> ChildrenSequence;

        public bool HasChild(XmlElement<TLink> parent, XmlElement<TLink> child) => Parent.Depth == child.Depth + 1;
    }
}
