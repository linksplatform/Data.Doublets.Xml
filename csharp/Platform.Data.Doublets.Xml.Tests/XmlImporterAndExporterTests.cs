using System.Text;
using System.Threading;
using System.IO;
using Xunit;
using TLink = System.UInt64;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Memory;
using Platform.Data.Doublets.Memory;
using System.Text.RegularExpressions;
using System.Xml;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Threading;

namespace Platform.Data.Doublets.Xml.Tests
{
    public class XmlImportAndExportTests
    {
        private const string XmlPrefixTag = "<?xml version=\"1.0\"?>";
        public static BalancedVariantConverter<TLink> BalancedVariantConverter;

        public static ILinks<TLink> CreateLinks() => CreateLinks<TLink>(new IO.TemporaryFile());

        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        public static DefaultXmlStorage<TLink> CreateXmlStorage(ILinks<TLink> links) => new (links, BalancedVariantConverter);

        public TLink Import(IXmlStorage<TLink> storage, string documentName, Stream xmlStream)
        {
            var utf8XmlReader = XmlReader.Create(xmlStream);
            XmlImporter<TLink> jsonImporter = new(storage);
            CancellationTokenSource importCancellationTokenSource = new();
            CancellationToken cancellationToken = importCancellationTokenSource.Token;
            return jsonImporter.Import(utf8XmlReader, documentName, cancellationToken).AwaitResult();
        }

        public void Export(TLink documentLink, IXmlStorage<TLink> storage, MemoryStream stream)
        {
            XmlExporter<TLink> xmlExporter = new(storage);
            CancellationTokenSource exportCancellationTokenSource = new();
            CancellationToken exportCancellationToken = exportCancellationTokenSource.Token;
            xmlExporter.Export(documentLink, stream, exportCancellationToken);
        }

        [Theory]
        [InlineData($"{XmlPrefixTag}<catalog><book><author>Gambardella, Matthew</author></book></catalog>")]
        public void Test(string initialXml)
        {
            var links = CreateLinks();
            BalancedVariantConverter = new(links);
            var storage = CreateXmlStorage(links);
            var encodedXml = Encoding.UTF8.GetBytes(initialXml);
            var encodedXmlStream = new MemoryStream(encodedXml);
            var documentLink = Import(storage, "documentName", encodedXmlStream);
            var exportedXmlStream = new MemoryStream();
            Export(documentLink, storage, exportedXmlStream);
            var exportedXml = Encoding.UTF8.GetString(encodedXmlStream.ToArray());
            var minimizedInitialXml = Regex.Replace(initialXml, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
            Assert.Equal(minimizedInitialXml, exportedXml);
        }
    }
}
