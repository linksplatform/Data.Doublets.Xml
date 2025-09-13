// using System;
// using System.IO;
// using Platform.IO;
// using Platform.Interfaces;
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
//     /// <seealso cref="ICli"/>
//     public class XmlElementCounterCLI : ICli
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
//         public int Run(params string[] args)
//         {
//             var file = ConsoleHelpers.GetOrReadArgument(0, "Xml file", args);
//             var elementName = ConsoleHelpers.GetOrReadArgument(1, "Element name to count", args);
//             if (!File.Exists(file))
//             {
//                 Console.WriteLine("Entered xml file does not exists.");
//                 return 1;
//             }
//             else if (string.IsNullOrEmpty(elementName))
//             {
//                 Console.WriteLine("Entered element name is empty.");
//                 return 1;
//             }
//             else
//             {
//                 using var cancellation = new ConsoleCancellation();
//                 Console.WriteLine("Press CTRL+C to stop.");
//                 new XmlElementCounter().Count(file, elementName, cancellation.Token).Wait();
//             }
//             return 0;
//         }
//     }
// }
