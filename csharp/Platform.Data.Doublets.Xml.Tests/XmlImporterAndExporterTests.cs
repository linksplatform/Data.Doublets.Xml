using System.Text;
using System.Threading;
using System.IO;
using Xunit;
using TLinkAddress = System.UInt64;
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
        private static Platform.Data.Doublets.Sequences.Converters.BalancedVariantConverter<TLinkAddress> _balancedVariantConverter;

        public static ILinks<TLinkAddress> CreateLinks() => CreateLinks<TLinkAddress>(new IO.TemporaryFile());

        public static ILinks<TLinkAddress> CreateLinks<TLinkAddress>(string dataDbFilename)
        {
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLinkAddress>(new FileMappedResizableDirectMemory(dataDbFilename), UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        public static DefaultXmlStorage<TLinkAddress> CreateXmlStorage(ILinks<TLinkAddress> links) => new (links, _balancedVariantConverter);

        public TLinkAddress Import(DefaultXmlStorage<TLinkAddress> storage, string documentName, Stream xmlStream)
        {
            var utf8XmlReader = XmlReader.Create(xmlStream);
            XmlImporter<TLinkAddress> jsonImporter = new(storage);
            CancellationTokenSource importCancellationTokenSource = new();
            CancellationToken cancellationToken = importCancellationTokenSource.Token;
            return jsonImporter.Import(utf8XmlReader, documentName, cancellationToken);
        }

        public void Export(TLinkAddress documentLink, DefaultXmlStorage<TLinkAddress> storage, MemoryStream stream)
        {
            XmlExporter<TLinkAddress> xmlExporter = new(storage);
            CancellationTokenSource exportCancellationTokenSource = new();
            CancellationToken exportCancellationToken = exportCancellationTokenSource.Token;
            var xmlWriter = XmlWriter.Create(stream);
            xmlExporter.Export(documentLink, xmlWriter, exportCancellationToken);
        }

        [Theory]
        [InlineData($"{XmlPrefixTag}<book><author>Gambardella</author><author>Matthew</author></book>")]
        [InlineData($"{XmlPrefixTag}<catalog><book><author>Gambardella, Matthew</author></book></catalog>")]
        public void Test(string initialXml)
        {
            var links = CreateLinks();
            _balancedVariantConverter = new(links);
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
