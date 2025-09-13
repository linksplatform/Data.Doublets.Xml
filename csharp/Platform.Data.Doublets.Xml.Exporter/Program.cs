using System.Xml;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.IO;
using Platform.Memory;
using TLinkAddress = System.UInt64;

namespace Platform.Data.Doublets.Xml.Exporter
{
    public static class XmlExporter
    {
        public static void Main(params string[] args)
        {
            var argumentIndex = 0;
            var linksFilePath = ConsoleHelpers.GetOrReadArgument(argumentIndex++, "Links file path", args);
            var xmlFilePath = ConsoleHelpers.GetOrReadArgument(argumentIndex++, "XML file path", args);
            var defaultDocumentName = Path.GetFileNameWithoutExtension(xmlFilePath);
            var documentName = ConsoleHelpers.GetOrReadArgument(argumentIndex++, $"Document name (default: {defaultDocumentName})", args);
            if (string.IsNullOrWhiteSpace(documentName))
            {
                documentName = defaultDocumentName;
            }
            var growSizeString = ConsoleHelpers.GetOrReadArgument(argumentIndex, "Database grow size in bytes (optional, default: auto)", args);
            var growSize = ParseGrowSize(growSizeString);
            if (!File.Exists(linksFilePath))
            {
                Console.WriteLine($"${linksFilePath} file does not exist.");
            }
            using FileStream xmlFileStream = new(xmlFilePath, FileMode.Append);
            var xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true
            };
            var xmlWriter = XmlWriter.Create(xmlFileStream, xmlWriterSettings);
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            using UnitedMemoryLinks<TLinkAddress> memoryAdapter = new (new FileMappedResizableDirectMemory(linksFilePath), growSize, linksConstants, IndexTreeType.Default);
            var links = memoryAdapter.DecorateWithAutomaticUniquenessAndUsagesResolution();
            var balancedVariantConverter = new BalancedVariantConverter<TLinkAddress>(links);
            var storage = new DefaultXmlStorage<TLinkAddress>(links, balancedVariantConverter);
            var exporter = new Platform.Data.Doublets.Xml.XmlExporter<TLinkAddress>(storage);
            var document = storage.GetDocument(documentName);
            using ConsoleCancellation cancellation = new ();
            var cancellationToken = cancellation.Token;
            Console.WriteLine("Press CTRL+C to stop.");
            exporter.Export(xmlWriter, document, cancellationToken);
        }

        private static long ParseGrowSize(string growSizeString)
        {
            if (string.IsNullOrWhiteSpace(growSizeString))
            {
                return UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep;
            }

            growSizeString = growSizeString.Trim().ToUpperInvariant();
            
            // Handle byte suffixes
            long multiplier = 1;
            if (growSizeString.EndsWith("KB"))
            {
                multiplier = 1024;
                growSizeString = growSizeString[..^2];
            }
            else if (growSizeString.EndsWith("MB"))
            {
                multiplier = 1024 * 1024;
                growSizeString = growSizeString[..^2];
            }
            else if (growSizeString.EndsWith("GB"))
            {
                multiplier = 1024 * 1024 * 1024;
                growSizeString = growSizeString[..^2];
            }
            else if (growSizeString.EndsWith("B"))
            {
                growSizeString = growSizeString[..^1];
            }

            if (!long.TryParse(growSizeString.Trim(), out long value))
            {
                Console.WriteLine($"Invalid grow size format: {growSizeString}. Using default value.");
                return UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep;
            }

            var totalBytes = value * multiplier;
            
            if (totalBytes <= 0)
            {
                Console.WriteLine($"Invalid grow size: {totalBytes}. Must be positive. Using default value.");
                return UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep;
            }
            
            return totalBytes;
        }
    }
}
