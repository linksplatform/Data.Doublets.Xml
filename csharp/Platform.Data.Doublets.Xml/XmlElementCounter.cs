// using System;
// using System.Collections.Generic;
// using System.Threading;
// using System.Threading.Tasks;
// using System.Xml;
// using System.Linq;
// using Platform.Exceptions;
// using Platform.IO;
//
// #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
//
// namespace Platform.Data.Doublets.Xml
// {
//     /// <summary>
//     /// <para>
//     /// Represents the xml element counter.
//     /// </para>
//     /// <para></para>
//     /// </summary>
//     public class XmlElementCounter
//     {
//         /// <summary>
//         /// <para>
//         /// Initializes a new <see cref="XmlElementCounter"/> instance.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         public XmlElementCounter() { }
//
//         /// <summary>
//         /// <para>
//         /// Counts the file.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="file">
//         /// <para>The file.</para>
//         /// <para></para>
//         /// </param>
//         /// <param name="elementName">
//         /// <para>The element name.</para>
//         /// <para></para>
//         /// </param>
//         /// <param name="cancellationToken">
//         /// <para>The cancellationToken.</para>
//         /// <para></para>
//         /// </param>
//         public Task Count(string file, string elementName, CancellationToken cancellationToken)
//         {
//             return Task.Factory.StartNew(() =>
//             {
//                 try
//                 {
//                     var context = new RootElementContext();
//                     using (var reader = XmlReader.Create(file))
//                     {
//                         Count(reader, elementName, cancellationToken, context);
//                     }
//                     Console.WriteLine($"Total elements with specified name: {context.TotalElements}, total content length: {context.TotalContentsLength}.");
//                 }
//                 catch (Exception ex)
//                 {
//                     Console.WriteLine(ex.ToStringWithAllInnerExceptions());
//                 }
//             }, cancellationToken);
//         }
//
//         private void Count(XmlReader reader, string elementNameToCount, CancellationToken cancellationToken, XmlElementContext context)
//         {
//             var rootContext = (RootElementContext)context;
//             var parentContexts = new Stack<XmlElementContext>();
//             var elements = new Stack<string>(); // Path
//             // TODO: If path was loaded previously, skip it.
//             while (reader.Read())
//             {
//                 if (cancellationToken.IsCancellationRequested)
//                 {
//                     return;
//                 }
//                 switch (reader.NodeType)
//                 {
//                     case XmlNodeType.Element:
//                         var elementName = reader.Name;
//                         context.IncrementChildNameCount(elementName);
//                         elementName = $"{elementName}[{context.ChildrenNamesCounts[elementName]}]";
//                         if (!reader.IsEmptyElement)
//                         {
//                             elements.Push(elementName);
//                             ConsoleHelpers.Debug("{0} starting...", elements.Count <= 20 ? ToXPath(elements) : elementName); // XPath
//                             parentContexts.Push(context);
//                             context = new XmlElementContext();
//                         }
//                         else
//                         {
//                             ConsoleHelpers.Debug("{0} finished.", elementName);
//                         }
//                         break;
//
//                     case XmlNodeType.EndElement:
//                         ConsoleHelpers.Debug("{0} finished.", elements.Count <= 20 ? ToXPath(elements) : elements.Peek()); // XPath
//                         var topElement = elements.Pop();
//                         // Restoring scope
//                         context = parentContexts.Pop();
//                         if (topElement.StartsWith(elementNameToCount))
//                         {
//                             rootContext.TotalElements++;
//                             // TODO: Check for 0x00 part/symbol at 198102797 line and 13 position.
//                             //if (rootContext.TotalPages > 3490000)
//                             //    selfCancel = true;
//                             if (context.ChildrenNamesCounts[elementNameToCount] % 10000 == 0)
//                             {
//                                 Console.WriteLine(topElement);
//                             }
//                         }
//                         break;
//
//                     case XmlNodeType.Text:
//                         ConsoleHelpers.Debug("Starting text element...");
//                         var content = reader.Value;
//                         rootContext.TotalContentsLength += (ulong)content.Length;
//                         ConsoleHelpers.Debug($"Content length is: {content.Length}");
//                         ConsoleHelpers.Debug("Text element finished.");
//                         break;
//                 }
//             }
//         }
//
//         private string ToXPath(Stack<string> path) => string.Join("/", path.Reverse());
//
//         private class RootElementContext : XmlElementContext
//         {
//             public ulong TotalElements;
//             public ulong TotalContentsLength;
//         }
//     }
// }
