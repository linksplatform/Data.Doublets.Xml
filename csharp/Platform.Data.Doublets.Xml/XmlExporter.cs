using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Platform.Exceptions;
using Platform.Collections;
using Platform.IO;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    class XmlExporter<TLink>
    {
        private readonly IXmlStorage<TLink> _storage;

        public XmlExporter(IXmlStorage<TLink> storage) => _storage = storage;

        public Task Export(string file, CancellationToken token) { return default; }

        private string ToXPath(Stack<string> path) => string.Join("/", path.Reverse());
        
        private class ElementContext : XmlElementContext
        {
            public readonly TLink Parent;

            public ElementContext(TLink parent) => Parent = parent;
        }

    }
}
