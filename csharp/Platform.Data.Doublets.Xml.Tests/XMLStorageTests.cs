using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Memory;
using Xunit;

namespace Platform.Data.Doublets.Xml.Tests
{
    public class XMLStorageTests
    {
        [Fact]
        private void CounstructorTest()
        {
            var links = CreateLinks();
            new DefaultXmlStorage<ulong>(links);
        }
        
        [Fact]
        private void CreateDocumentTest(){
            var links = CreateLinks();
            var defaultXmlStorage = new DefaultXmlStorage<ulong>(links);
            defaultXmlStorage.CreateDocument("documentFilename"); 
        }
        
        private static ILinks<TLink> CreateLinks<TLink>(string linksDbFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(linksDbFilename),
                UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        private static ILinks<ulong> CreateLinks() => CreateLinks<ulong>(new Platform.IO.TemporaryFile());
    }
}
