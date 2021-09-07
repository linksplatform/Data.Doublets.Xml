using System.Text;
using System.Xml;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Memory;
using Xunit;

namespace Platform.Data.Doublets.Xml.Tests
{
    public class XmlImporterAndExporterTests
    {
        [Theory][InlineData("<a/>")]
        void TestImport(string initialXml)
        {
            CreateLinks();
            var links = CreateLinks();
            var defaultXmlStorage = new DefaultXmlStorage<ulong>(links);

            //string initialXml = "<a/>";
            Encoding.UTF8.GetBytes(initialXml);
                                
            var xmlImporter = new XmlImporter<ulong>(defaultXmlStorage);
            //lImporter.Import(XmlReader.Create());

        }
        
        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename),
                UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        public static ILinks<ulong> CreateLinks() => CreateLinks<ulong>(new Platform.IO.TemporaryFile());
    }
}