using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Platform.Exceptions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    class XmlExporter<TLink>
    {
        private readonly IXmlStorage<TLink> _storage;

        public XmlExporter(IXmlStorage<TLink> storage) => _storage = storage;

        public Task Export(string documentName, string fileName, CancellationToken token) 
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var document = _storage.GetDocument(documentName);
                    using (var writer = XmlWriter.Create(fileName))
                    {
                        Write(writer, token, new ElementContext(document));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToStringWithAllInnerExceptions());
                }
            }, token);
        }

        private void Write(XmlWriter writer, CancellationToken token, ElementContext context)
        {
            var parentContexts = new Stack<ElementContext>();
            var elements = new Stack<string>(); // Path
                                                // TODO: If path was loaded previously, skip it.
            foreach(TLink lvl in _storage.GetChildren(parent: context.Parent))
            {
                Write(writer: writer, token: token, context: new ElementContext(lvl));
            }
        }
        
        private class ElementContext : XmlElementContext
        {
            public readonly TLink Parent;

            public ElementContext(TLink parent) => Parent = parent;
        }

    }
}
