// using System;
// using System.IO;
// using Platform.IO;
// using Platform.Data.Doublets.Memory.United.Generic;
// using Platform.Interfaces;
//
// #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
//
// namespace Platform.Data.Doublets.Xml
// {
//     public class XmlExporterCLI : ICli
//     {
//         public int Run(params string[] args)
//         {
//             var linksFile = ConsoleHelpers.GetOrReadArgument(0, "Links file", args);
//             var exportFile = ConsoleHelpers.GetOrReadArgument(1, "Xml file", args);
//
//             if (File.Exists(exportFile))
//             {
//                 Console.WriteLine("Entered xml file does already exists.");
//                 return 1;
//             }
//             else
//             {
//                 using var cancellation = new ConsoleCancellation();
//                 var linksConstants = new LinksConstants<ulong>(enableExternalReferencesSupport: true);
//                 using var memoryAdapter = new UnitedMemoryLinks<uint>(linksFile);
//                 Console.WriteLine("Press CTRL+C to stop.");
//                 var links = memoryAdapter.DecorateWithAutomaticUniquenessAndUsagesResolution();
//                 if (!cancellation.NotRequested) return 1;
//                 var storage = new DefaultXmlStorage<uint>(links);
//                 var exporter = new XmlExporter<uint>(storage);
//                 exporter.Export(linksFile, exportFile, cancellation.Token).Wait();
//             }
//             return 0;
//         }
//     }
// }
