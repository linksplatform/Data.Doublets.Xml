using System.Collections.Generic;
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
        private const string XmlDeclarationTag = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
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
            var xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = false });
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
            var xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = false });
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

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] { $"{XmlDeclarationTag}<users />" },
                new object[] { $"{XmlDeclarationTag}<user name=\"Gambardella\" />" },
                new object[] { $"{XmlDeclarationTag}<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\"><uses-permission android:name=\"android.permission.READ_CONTACTS\" /></manifest>" },
                new object[] { $"{XmlDeclarationTag}<users><user name=\"Gambardella\" /></users>" },
                new object[] { $"{XmlDeclarationTag}<users><user>Gambardella</user><user>Matthew</user></users>" },
                new object[] { $"{XmlDeclarationTag}<users><user role=\"admin\">Gambardella</user><user role=\"moderator\">Matthew</user></users>" },
                new object[]
                {
$@"{XmlDeclarationTag}
<root>
    <h:table xmlns:h=""http://www.w3.org/TR/html4/"">
      <h:tr>
        <h:td>Apples</h:td>
        <h:td>Bananas</h:td>
      </h:tr>
    </h:table>

    <f:table xmlns:f=""https://www.w3schools.com/furniture"">
      <f:name>African Coffee Table</f:name>
      <f:width>80</f:width>
      <f:length>120</f:length>
    </f:table>
</root> "
                },
                new object[]
                {
$@"{XmlDeclarationTag}
<root xmlns:h=""http://www.w3.org/TR/html4/""
xmlns:f=""https://www.w3schools.com/furniture"">
    <h:table>
      <h:tr>
        <h:td>Apples</h:td>
        <h:td>Bananas</h:td>
      </h:tr>
    </h:table>

    <f:table>
      <f:name>African Coffee Table</f:name>
      <f:width>80</f:width>
      <f:length>120</f:length>
    </f:table>
</root> "
                },
                new object[]
                {
$@"{XmlDeclarationTag}
<root>
    <table xmlns=""http://www.w3.org/TR/html4/"">
      <tr>
        <td>Apples</td>
        <td>Bananas</td>
      </tr>
    </table> 

    <table xmlns=""https://www.w3schools.com/furniture"">
      <name>African Coffee Table</name>
      <width>80</width>
      <length>120</length>
    </table>
</root> "
                }
            };

        [Theory]
        [MemberData(nameof(Data))]
        public void ExportByDocumentLink(string initialXml)
        {
            var documentName = "documentName";
            var documentLink = Import(_xmlStorage, documentName, initialXml);
            var exportedXmlByDocumentLink = Export(documentLink, _xmlStorage);
            Assert.Equal(initialXml, exportedXmlByDocumentLink);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ExportByDocumentName(string initialXml)
        {
            var documentName = "documentName";
            var documentLink = Import(_xmlStorage, documentName, initialXml);
            var exportedXmlByName = Export(documentName, _xmlStorage);
            Assert.Equal(initialXml, exportedXmlByName);
        }
    }
}
