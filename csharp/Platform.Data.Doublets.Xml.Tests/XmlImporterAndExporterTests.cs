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
        private const string XmlPrefixTag = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
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

        private Stream GetUtf8Stream(string @string)
        {
            var utf8Encoding = Encoding.UTF8;
            var encodedXmlBytes = utf8Encoding.GetBytes(@string);
            var encodedXmlStream = new MemoryStream(encodedXmlBytes);
            return encodedXmlStream;
        }
        
        private TLinkAddress Import(DefaultXmlStorage<TLinkAddress> xmlStorage, string documentName, string xml)
        {
            var encodedStream = GetUtf8Stream(xml); 
            var xmlReader = XmlReader.Create(encodedStream);
            XmlImporter<TLinkAddress> xmlImporter = new(xmlStorage);
            CancellationTokenSource importCancellationTokenSource = new();
            CancellationToken cancellationToken = importCancellationTokenSource.Token;
            return xmlImporter.Import(xmlReader, documentName, cancellationToken);
        }

        private string Export(string documentName, DefaultXmlStorage<TLinkAddress> xmlStorage)
        {
            var xmlExporter = new XmlExporter<TLinkAddress>(xmlStorage);
            var exportCancellationTokenSource = new CancellationTokenSource();
            var exportCancellationToken = exportCancellationTokenSource.Token;
            var memoryStream = new MemoryStream();
            var xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings{Indent = false});
            xmlExporter.Export(xmlWriter, documentName, exportCancellationToken);
            var exportedXml = Encoding.UTF8.GetString(memoryStream.ToArray());
            string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (exportedXml.StartsWith(byteOrderMarkUtf8))
            {
                exportedXml = exportedXml.Remove(0, byteOrderMarkUtf8.Length);
            }
            return exportedXml;
        }
        
        private string Export(TLinkAddress documentLink, DefaultXmlStorage<TLinkAddress> xmlStorage)
        {
            var xmlExporter = new XmlExporter<TLinkAddress>(xmlStorage);
            var exportCancellationTokenSource = new CancellationTokenSource();
            var exportCancellationToken = exportCancellationTokenSource.Token;
            var memoryStream = new MemoryStream();
            var xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings{Indent = false});
            xmlExporter.Export(xmlWriter, documentLink, exportCancellationToken);
            var exportedXml = Encoding.UTF8.GetString(memoryStream.ToArray());
            string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (exportedXml.StartsWith(byteOrderMarkUtf8))
            {
                exportedXml = exportedXml.Remove(0, byteOrderMarkUtf8.Length);
            }
            return exportedXml;
        }

        private string MinimizeXml(string xml)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            return xmlDocument.InnerXml;
        }

        [Theory]
        [InlineData($"{XmlPrefixTag}<users />")]
        [InlineData($"{XmlPrefixTag}<user name=\"Gambardella\" />")]
        [InlineData($"{XmlPrefixTag}<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\"><uses-permission android:name=\"android.permission.READ_CONTACTS\" /></manifest>")]
        [InlineData($"{XmlPrefixTag}<users><user name=\"Gambardella\" /></users>")]
        [InlineData($"{XmlPrefixTag}<users><user>Gambardella</user><user>Matthew</user></users>")]
        [InlineData($"{XmlPrefixTag}<users><user role=\"admin\">Gambardella</user><user role=\"moderator\">Matthew</user></users>")]
        public void Test(string initialXml)
        {
            var documentName = "documentName";
            var documentLink = Import(_xmlStorage, documentName, initialXml);
            var exportedXmlByDocumentLink = Export(documentLink, _xmlStorage);
            var exportedXmlByName = Export(documentName, _xmlStorage);
            Assert.Equal(initialXml, exportedXmlByDocumentLink);
            Assert.Equal(initialXml, exportedXmlByName);
        }
    }
}
