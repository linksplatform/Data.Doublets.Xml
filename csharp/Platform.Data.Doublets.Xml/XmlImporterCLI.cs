// using System;
// using System.IO;
// using Platform.IO;
// using Platform.Data.Doublets.Memory.United.Generic;
//
// #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
//
// namespace Platform.Data.Doublets.Xml
// {
//     /// <summary>
//     /// <para>
//     /// Represents the xml importer cli.
//     /// </para>
//     /// <para></para>
//     /// </summary>
//     /// <seealso cref="ICommandLineInterface"/>
//     public class XmlImporterCLI : ICommandLineInterface
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
//             var linksFile = ConsoleHelpers.GetOrReadArgument(0, "Links file", args);
//             var file = ConsoleHelpers.GetOrReadArgument(1, "Xml file", args);
//
//             if (!File.Exists(file))
//             {
//                 Console.WriteLine("Entered xml file does not exists.");
//             }
//             else
//             {
//                 //const long gb32 = 34359738368;
//
//                 using (var cancellation = new ConsoleCancellation())
//                 using (var memoryAdapter = new UnitedMemoryLinks<uint>(linksFile))
//                 //using (var memoryAdapter = new UInt64UnitedMemoryLinks(linksFile, gb32))
//                 //using (var links = new UInt64Links(memoryAdapter))
//                 {
//                     Console.WriteLine("Press CTRL+C to stop.");
//                     var links = memoryAdapter.DecorateWithAutomaticUniquenessAndUsagesResolution();
//                     var indexer = new XmlIndexer<uint>(links);
//                     var indexingImporter = new XmlImporter<uint>(indexer);
//                     indexingImporter.Import(file, cancellation.Token).Wait();
//                     if (cancellation.NotRequested)
//                     {
//                         var cache = indexer.Cache;
//                         //var counter = new TotalSequenceSymbolFrequencyCounter<uint>(links);
//                         //var cache = new LinkFrequenciesCache<uint>(links, counter);
//                         Console.WriteLine("Frequencies cache ready.");
//                         var storage = new DefaultXmlStorage<uint>(links, false, cache);
//                         var importer = new XmlImporter<uint>(storage);
//                         importer.Import(file, cancellation.Token).Wait();
//                     }
//                 }
//             }
//         }
//     }
// }
