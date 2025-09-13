using System.Text;
using System.Xml;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.IO;
using Platform.Memory;
using TLinkAddress = System.UInt64;

namespace Platform.Data.Doublets.Xml.Importer
{
    internal static class XmlImporter
    {
        public static void Main(string[] args)
        {
            var argumentIndex = 0;
            var xmlFilePath = ConsoleHelpers.GetOrReadArgument(argumentIndex++, "XML file path", args);
            if (!File.Exists(xmlFilePath))
            {
                Console.WriteLine($"${xmlFilePath} file does not exist.");
            }
            var linksFilePath = ConsoleHelpers.GetOrReadArgument(argumentIndex++, "Links storage file path", args);
            var defaultDocumentName = Path.GetFileNameWithoutExtension(xmlFilePath);
            var documentName = ConsoleHelpers.GetOrReadArgument(argumentIndex++, $"Document name (default: {defaultDocumentName})", args);
            if (string.IsNullOrWhiteSpace(documentName))
            {
                documentName = defaultDocumentName;
            }
            var growSizeString = ConsoleHelpers.GetOrReadArgument(argumentIndex, "Database grow size in bytes (optional, default: auto)", args);
            var growSize = ParseGrowSize(growSizeString);
            var xmlReader = XmlReader.Create(xmlFilePath);
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            var fileMappedResizableDirectMemory = new FileMappedResizableDirectMemory(linksFilePath);
            var unitedMemoryLinks = growSize;
            const IndexTreeType indexTreeType = IndexTreeType.Default;
            using UnitedMemoryLinks<TLinkAddress> memoryAdapter = new(fileMappedResizableDirectMemory, unitedMemoryLinks, linksConstants, indexTreeType);
            var links = memoryAdapter.DecorateWithAutomaticUniquenessAndUsagesResolution();
            var balancedVariantConverter = new BalancedVariantConverter<TLinkAddress>(links);
            var storage = new DefaultXmlStorage<TLinkAddress>(links, balancedVariantConverter);
            var importer = new Platform.Data.Doublets.Xml.XmlImporter<TLinkAddress>(storage);
            using ConsoleCancellation cancellation = new();
            var cancellationToken = cancellation.Token;
            Console.WriteLine("Press CTRL+C to stop.");
            importer.Import(xmlReader, documentName, cancellationToken); 
            Console.WriteLine("Import completed successfully.");
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
