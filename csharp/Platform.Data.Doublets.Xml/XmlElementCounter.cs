using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Linq;
using Platform.Exceptions;
using Platform.IO;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    /// <summary>
    /// <para>
    /// Represents the xml element counter.
    /// </para>
    /// <para></para>
    /// </summary>
    public class XmlElementCounter
    {
        /// <summary>
        /// <para>
        /// Initializes a new <see cref="XmlElementCounter"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        public XmlElementCounter() { }

        /// <summary>
        /// <para>
        /// Counts the file.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="file">
        /// <para>The file.</para>
        /// <para></para>
        /// </param>
        /// <param name="elementName">
        /// <para>The element name.</para>
        /// <para></para>
        /// </param>
        /// <param name="token">
        /// <para>The token.</para>
        /// <para></para>
        /// </param>
        public Task Count(string file, string elementName, CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var context = new RootElementContext();
                    using (var reader = XmlReader.Create(file))
                    {
                        Count(reader, elementName, token, context);
                    }
                    Console.WriteLine($"Total elements with specified name: {context.TotalElements}, total content length: {context.TotalContentsLength}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToStringWithAllInnerExceptions());
                }
            }, token);
        }

        /// <summary>
        /// <para>
        /// Counts the reader.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="reader">
        /// <para>The reader.</para>
        /// <para></para>
        /// </param>
        /// <param name="elementNameToCount">
        /// <para>The element name to count.</para>
        /// <para></para>
        /// </param>
        /// <param name="token">
        /// <para>The token.</para>
        /// <para></para>
        /// </param>
        /// <param name="context">
        /// <para>The context.</para>
        /// <para></para>
        /// </param>
        private void Count(XmlReader reader, string elementNameToCount, CancellationToken token, XmlElementContext context)
        {
            var rootContext = (RootElementContext)context;
            var parentContexts = new Stack<XmlElementContext>();
            var elements = new Stack<string>(); // Path
            // TODO: If path was loaded previously, skip it.
            while (reader.Read())
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        var elementName = reader.Name;
                        context.IncrementChildNameCount(elementName);
                        elementName = $"{elementName}[{context.ChildrenNamesCounts[elementName]}]";
                        if (!reader.IsEmptyElement)
                        {
                            elements.Push(elementName);
                            ConsoleHelpers.Debug("{0} starting...", elements.Count <= 20 ? ToXPath(elements) : elementName); // XPath
                            parentContexts.Push(context);
                            context = new XmlElementContext();
                        }
                        else
                        {
                            ConsoleHelpers.Debug("{0} finished.", elementName);
                        }
                        break;

                    case XmlNodeType.EndElement:
                        ConsoleHelpers.Debug("{0} finished.", elements.Count <= 20 ? ToXPath(elements) : elements.Peek()); // XPath
                        var topElement = elements.Pop();
                        // Restoring scope
                        context = parentContexts.Pop();
                        if (topElement.StartsWith(elementNameToCount))
                        {
                            rootContext.TotalElements++;
                            // TODO: Check for 0x00 part/symbol at 198102797 line and 13 position.
                            //if (rootContext.TotalPages > 3490000)
                            //    selfCancel = true;
                            if (context.ChildrenNamesCounts[elementNameToCount] % 10000 == 0)
                            {
                                Console.WriteLine(topElement);
                            }
                        }
                        break;

                    case XmlNodeType.Text:
                        ConsoleHelpers.Debug("Starting text element...");
                        var content = reader.Value;
                        rootContext.TotalContentsLength += (ulong)content.Length;
                        ConsoleHelpers.Debug($"Content length is: {content.Length}");
                        ConsoleHelpers.Debug("Text element finished.");
                        break;
                }
            }
        }

        /// <summary>
        /// <para>
        /// Returns the x path using the specified path.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="path">
        /// <para>The path.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The string</para>
        /// <para></para>
        /// </returns>
        private string ToXPath(Stack<string> path) => string.Join("/", path.Reverse());

        /// <summary>
        /// <para>
        /// Represents the root element context.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <seealso cref="XmlElementContext"/>
        private class RootElementContext : XmlElementContext
        {
            /// <summary>
            /// <para>
            /// The total elements.
            /// </para>
            /// <para></para>
            /// </summary>
            public ulong TotalElements;
            /// <summary>
            /// <para>
            /// The total contents length.
            /// </para>
            /// <para></para>
            /// </summary>
            public ulong TotalContentsLength;
        }
    }
}
