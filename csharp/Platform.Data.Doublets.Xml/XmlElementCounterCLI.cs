// using System;
// using System.IO;
// using Platform.IO;
//
// #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
//
// namespace Platform.Data.Doublets.Xml
// {
//     /// <summary>
//     /// <para>
//     /// Represents the xml element counter cli.
//     /// </para>
//     /// <para></para>
//     /// </summary>
//     /// <seealso cref="ICommandLineInterface"/>
//     public class XmlElementCounterCLI : ICommandLineInterface
//     {
//         /// <summary>
//         /// <para>
//         /// Runs the args.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="args">
//         /// <para>The args.</para>
//         /// <para></para>
//         /// </param>
//         public void Run(params string[] args)
//         {
//             var file = ConsoleHelpers.GetOrReadArgument(0, "Xml file", args);
//             var elementName = ConsoleHelpers.GetOrReadArgument(1, "Element name to count", args);
//             if (!File.Exists(file))
//             {
//                 Console.WriteLine("Entered xml file does not exists.");
//             }
//             else if (string.IsNullOrEmpty(elementName))
//             {
//                 Console.WriteLine("Entered element name is empty.");
//             }
//             else
//             {
//                 using var cancellation = new ConsoleCancellation();
//                 Console.WriteLine("Press CTRL+C to stop.");
//                 new XmlElementCounter().Count(file, elementName, cancellation.Token).Wait();
//             }
//         }
//     }
// }
