using System.Collections.Generic;
using Platform.Collections.Stacks;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Memory;
using Xunit;
using Xunit.Abstractions;
using TLinkAddress = System.UInt64;

namespace Platform.Data.Doublets.Xml.Tests
{
public class XmlStorageTests
    {
        private readonly ITestOutputHelper output;
        public static BalancedVariantConverter<TLinkAddress> BalancedVariantConverter;

        public XmlStorageTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        public static ILinks<TLinkAddress> CreateLinks() => CreateLinks<TLinkAddress>(new Platform.IO.TemporaryFile());

        public static ILinks<TLinkAddress> CreateLinks<TLinkAddress>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLinkAddress>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLinkAddress>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLinkAddress>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        public static DefaultXmlStorage<TLinkAddress> CreateXmlStorage()
        {
            var links = CreateLinks();
            return CreateXmlStorage(links);
        }

        public static DefaultXmlStorage<TLinkAddress> CreateXmlStorage(ILinks<TLinkAddress> links)
        {
            BalancedVariantConverter = new(links);
            return new DefaultXmlStorage<TLinkAddress>(links, BalancedVariantConverter);
        }

        [Fact]
        public void ConstructorsTest() => CreateXmlStorage();

        [Fact]
        public void CreateDocumentTest()
        {
            var defaultXmlStorage = CreateXmlStorage();
            defaultXmlStorage.CreateDocumentName("documentName");
        }

        [Fact]
        public void GetDocumentTest()
        {
            var defaultXmlStorage = CreateXmlStorage();
            var createdDocumentLink = defaultXmlStorage.CreateDocumentName("documentName");
            var foundDocumentLink = defaultXmlStorage.GetDocumentLinkAddress("documentName");
            Assert.Equal(createdDocumentLink, foundDocumentLink);
        }

        [Fact]
        public void CreateObjectTest()
        {
            var defaultXmlStorage = CreateXmlStorage();
            var object0 = defaultXmlStorage.CreateObjectValue();
            var object1 = defaultXmlStorage.CreateObjectValue();
            Assert.NotEqual(object0, object1);
        }

        [Fact]
        public void CreateStringTest()
        {
            var defaultXmlStorage = CreateXmlStorage();
            defaultXmlStorage.CreateString("string");
        }

        [Fact]
        public void CreateMemberTest()
        {
            var defaultXmlStorage = CreateXmlStorage();
            var document = defaultXmlStorage.CreateDocumentName("documentName");
            defaultXmlStorage.AttachObject(document);
            defaultXmlStorage.CreateMember("keyName");
        }

        [Fact]
        public void AttachObjectValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");
            TLinkAddress documentValueLink = defaultXmlStorage.AttachObject(document);
            TLinkAddress createdObjectValue = links.GetTarget(documentValueLink);

            TLinkAddress valueMarker = links.GetSource(createdObjectValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLinkAddress createdObject = links.GetTarget(createdObjectValue);
            TLinkAddress objectMarker = links.GetSource(createdObject);
            Assert.Equal(objectMarker, defaultXmlStorage.ObjectMarker);

            TLinkAddress foundDocumentValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdObjectValue, foundDocumentValue);
        }

        [Fact]
        public void AttachStringValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");
            TLinkAddress documentStringLink = defaultXmlStorage.AttachString(document, "stringName");
            TLinkAddress createdStringValue = links.GetTarget(documentStringLink);

            TLinkAddress valueMarker = links.GetSource(createdStringValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLinkAddress createdString = links.GetTarget(createdStringValue);
            TLinkAddress stringMarker = links.GetSource(createdString);
            Assert.Equal(stringMarker, defaultXmlStorage.StringMarker);

            TLinkAddress foundStringValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdStringValue, foundStringValue);
        }

        [Fact]
        public void AttachNumberToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage = CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");
            TLinkAddress documentNumberLink = defaultXmlStorage.AttachNumber(document, 2021);
            TLinkAddress createdNumberValue = links.GetTarget(documentNumberLink);

            TLinkAddress valueMarker = links.GetSource(createdNumberValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLinkAddress createdNumber = links.GetTarget(createdNumberValue);
            TLinkAddress numberMarker = links.GetSource(createdNumber);
            Assert.Equal(numberMarker, defaultXmlStorage.NumberMarker);

            TLinkAddress foundNumberValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdNumberValue, foundNumberValue);
        }

        [Fact]
        public void AttachTrueValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");

            TLinkAddress documentTrueValueLink = defaultXmlStorage.AttachBoolean(document, true);
            TLinkAddress createdTrueValue = links.GetTarget(documentTrueValueLink);

            TLinkAddress valueMarker = links.GetSource(createdTrueValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLinkAddress trueMarker = links.GetTarget(createdTrueValue);
            Assert.Equal(trueMarker, defaultXmlStorage.TrueMarker);

            TLinkAddress foundTrueValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdTrueValue, foundTrueValue);
        }

        [Fact]
        public void AttachFalseValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");

            TLinkAddress documentFalseValueLink = defaultXmlStorage.AttachBoolean(document, false);
            TLinkAddress createdFalseValue = links.GetTarget(documentFalseValueLink);

            TLinkAddress valueMarker = links.GetSource(createdFalseValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLinkAddress falseMarker = links.GetTarget(createdFalseValue);
            Assert.Equal(falseMarker, defaultXmlStorage.FalseMarker);

            TLinkAddress foundFalseValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdFalseValue, foundFalseValue);
        }

        [Fact]
        public void AttachNullValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");

            TLinkAddress documentNullValueLink = defaultXmlStorage.AttachNull(document);
            TLinkAddress createdNullValue = links.GetTarget(documentNullValueLink);

            TLinkAddress valueMarker = links.GetSource(createdNullValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLinkAddress nullMarker = links.GetTarget(createdNullValue);
            Assert.Equal(nullMarker, defaultXmlStorage.NullMarker);

            TLinkAddress foundNullValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdNullValue, foundNullValue);
        }

        [Fact]
        public void AttachEmptyArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");

            TLinkAddress documentArrayValueLink = defaultXmlStorage.AttachArray(document, new TLinkAddress[0]);
            TLinkAddress createdArrayValue = links.GetTarget(documentArrayValueLink);
            output.WriteLine(links.Format(createdArrayValue));


            TLinkAddress valueMarker = links.GetSource(createdArrayValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLinkAddress createdArrayLink = links.GetTarget(createdArrayValue);
            TLinkAddress arrayMarker = links.GetSource(createdArrayLink);
            Assert.Equal(arrayMarker, defaultXmlStorage.ArrayMarker);

            TLinkAddress createArrayContents = links.GetTarget(createdArrayLink);
            Assert.Equal(createArrayContents, defaultXmlStorage.EmptyArrayMarker);

            TLinkAddress foundArrayValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
        }

        [Fact]
        public void AttachArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");

            TLinkAddress arrayElement = defaultXmlStorage.CreateString("arrayElement");
            TLinkAddress[] array = new TLinkAddress[] { arrayElement, arrayElement, arrayElement };


            TLinkAddress documentArrayValueLink = defaultXmlStorage.AttachArray(document, array);
            TLinkAddress createdArrayValue = links.GetTarget(documentArrayValueLink);

            DefaultStack<TLinkAddress> stack = new();
            RightSequenceWalker<TLinkAddress> rightSequenceWalker = new(links, stack, arrayElementLink => links.GetSource(arrayElementLink) == defaultXmlStorage.ValueMarker);
            IEnumerable<TLinkAddress> arrayElementsValuesLink = rightSequenceWalker.Walk(createdArrayValue);
            Assert.NotEmpty(arrayElementsValuesLink);

            output.WriteLine(links.Format(createdArrayValue));


            TLinkAddress valueMarker = links.GetSource(createdArrayValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLinkAddress createdArrayLink = links.GetTarget(createdArrayValue);
            TLinkAddress arrayMarker = links.GetSource(createdArrayLink);
            Assert.Equal(arrayMarker, defaultXmlStorage.ArrayMarker);

            TLinkAddress createdArrayContents = links.GetTarget(createdArrayLink);
            Assert.Equal(links.GetTarget(createdArrayContents), arrayElement);


            TLinkAddress foundArrayValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
        }

        [Fact]
        public void GetObjectFromDocumentObjectValueLinkTest()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");
            TLinkAddress documentObjectValueLink = defaultXmlStorage.AttachObject(document);
            TLinkAddress objectValueLink = links.GetTarget(documentObjectValueLink);
            TLinkAddress objectFromGetObject = defaultXmlStorage.GetObject(documentObjectValueLink);
            output.WriteLine(links.Format(objectValueLink));
            output.WriteLine(links.Format(objectFromGetObject));
            Assert.Equal(links.GetTarget(objectValueLink), objectFromGetObject);
        }

        [Fact]
        public void GetObjectFromObjectValueLinkTest()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");
            TLinkAddress documentObjectValueLink = defaultXmlStorage.AttachObject(document);
            TLinkAddress objectValueLink = links.GetTarget(documentObjectValueLink);
            TLinkAddress objectFromGetObject = defaultXmlStorage.GetObject(objectValueLink);
            Assert.Equal(links.GetTarget(objectValueLink), objectFromGetObject);
        }

        [Fact]
        public void AttachStringValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");
            TLinkAddress documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLinkAddress @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress memberStringValueLink = defaultXmlStorage.AttachString(memberLink, "stringValue");
            TLinkAddress stringValueLink = links.GetTarget(memberStringValueLink);
            List<TLinkAddress> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(stringValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachNumberValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");
            TLinkAddress documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLinkAddress @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress memberNumberValueLink = defaultXmlStorage.AttachNumber(memberLink, 123);
            TLinkAddress numberValueLink = links.GetTarget(memberNumberValueLink);
            List<TLinkAddress> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(numberValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachObjectValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");
            TLinkAddress documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLinkAddress @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress memberObjectValueLink = defaultXmlStorage.AttachObject(memberLink);
            TLinkAddress objectValueLink = links.GetTarget(memberObjectValueLink);
            List<TLinkAddress> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(objectValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachArrayValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");
            TLinkAddress documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLinkAddress @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress arrayElement = defaultXmlStorage.CreateString("arrayElement");
            TLinkAddress[] array = { arrayElement, arrayElement, arrayElement };
            TLinkAddress memberArrayValueLink = defaultXmlStorage.AttachArray(memberLink, array);
            TLinkAddress arrayValueLink = links.GetTarget(memberArrayValueLink);
            List<TLinkAddress> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(arrayValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachTrueValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");
            TLinkAddress documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLinkAddress @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress memberTrueValueLink = defaultXmlStorage.AttachBoolean(memberLink, true);
            TLinkAddress trueValueLink = links.GetTarget(memberTrueValueLink);
            List<TLinkAddress> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(trueValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachFalseValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");
            TLinkAddress documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLinkAddress @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress memberFalseValueLink = defaultXmlStorage.AttachBoolean(memberLink, false);
            TLinkAddress falseValueLink = links.GetTarget(memberFalseValueLink);
            List<TLinkAddress> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(falseValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachNullValueToKey()
        {
            ILinks<TLinkAddress> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLinkAddress document = defaultXmlStorage.CreateDocumentName("documentName");
            TLinkAddress documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLinkAddress @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLinkAddress memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLinkAddress memberNullValueLink = defaultXmlStorage.AttachNull(memberLink);
            TLinkAddress nullValueLink = links.GetTarget(memberNullValueLink);
            List<TLinkAddress> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(nullValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }
    }
}
