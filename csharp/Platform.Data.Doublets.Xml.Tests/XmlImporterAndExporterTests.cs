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
        private readonly BalancedVariantConverter<TLinkAddress> _balancedVariantConverter;
        private readonly ILinks<TLinkAddress> _links;
        private readonly DefaultXmlStorage<TLinkAddress> _xmlStorage;

        public XmlImportAndExportTests()
        {
            _links = CreateLinksStorage();
            _balancedVariantConverter = new BalancedVariantConverter<TLinkAddress>(_links);
            _xmlStorage = CreateXmlStorage(_links, _balancedVariantConverter);
        }

        private static ILinks<TLinkAddress> CreateLinksStorage() => CreateLinksStorage<TLinkAddress>(new IO.TemporaryFile());

        private static ILinks<TLinkAddress> CreateLinksStorage<TLinkAddress>(string dataDbFilename)
        {
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLinkAddress>(new FileMappedResizableDirectMemory(dataDbFilename), UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        private DefaultXmlStorage<TLinkAddress> CreateXmlStorage(ILinks<TLinkAddress> links, BalancedVariantConverter<TLinkAddress> balancedVariantConverter) => new DefaultXmlStorage<TLinkAddress>(links, balancedVariantConverter);

        private TLinkAddress Import(DefaultXmlStorage<TLinkAddress> xmlStorage, string documentName, Stream xmlStream)
        {
            var utf8XmlReader = XmlReader.Create(xmlStream);
            XmlImporter<TLinkAddress> jsonImporter = new(xmlStorage);
            CancellationTokenSource importCancellationTokenSource = new();
            CancellationToken cancellationToken = importCancellationTokenSource.Token;
            return jsonImporter.Import(utf8XmlReader, documentName, cancellationToken);
        }

        private void Export(TLinkAddress documentLink, DefaultXmlStorage<TLinkAddress> xmlStorage, MemoryStream stream)
        {
            XmlExporter<TLinkAddress> xmlExporter = new(xmlStorage);
            CancellationTokenSource exportCancellationTokenSource = new();
            CancellationToken exportCancellationToken = exportCancellationTokenSource.Token;
            var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings{Indent = false});
            xmlExporter.Export(xmlWriter, documentLink, exportCancellationToken);
        }

        [Theory]
        [InlineData($"{XmlPrefixTag}<users></users>")]
        [InlineData($"{XmlPrefixTag}<users><user>Gambardella</user><user>Matthew</user></users>")]
        [InlineData($"{XmlPrefixTag}<users><user role=\"admin\">Gambardella</user><user role=\"moderator\">Matthew</user></users>")]
        public void Test(string initialXml)
        {
            var utf8Encoding = Encoding.UTF8;
            var encodedXmlBytes = utf8Encoding.GetBytes(initialXml);
            var encodedXmlStream = new MemoryStream(encodedXmlBytes);
            var documentLink = Import(_xmlStorage, "documentName", encodedXmlStream);
            var exportedXmlStream = new MemoryStream(initialXml.Length);
            Export(documentLink, _xmlStorage, exportedXmlStream);
            var exportedXml = utf8Encoding.GetString(exportedXmlStream.ToArray());
            string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (exportedXml.StartsWith(byteOrderMarkUtf8))
            {
                exportedXml = exportedXml.Remove(0, byteOrderMarkUtf8.Length);
            }
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(exportedXml);
            var minimizedInitialXml = xmlDocument.InnerXml;
            Assert.Equal(exportedXml, minimizedInitialXml);
        }
    }
}
