using System.Collections.Generic;
using Platform.Collections.Stacks;
using Platform.Data.Doublets.Memory;
using Platform.Data.Doublets.Memory.United.Generic;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Memory;
using Xunit;
using Xunit.Abstractions;
using TLink = System.UInt64;

namespace Platform.Data.Doublets.Xml.Tests
{
public class XmlStorageTests
    {
        private readonly ITestOutputHelper output;
        public static BalancedVariantConverter<TLink> BalancedVariantConverter;

        public XmlStorageTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        public static ILinks<TLink> CreateLinks() => CreateLinks<TLink>(new Platform.IO.TemporaryFile());

        public static ILinks<TLink> CreateLinks<TLink>(string dataDBFilename)
        {
            var linksConstants = new LinksConstants<TLink>(enableExternalReferencesSupport: true);
            return new UnitedMemoryLinks<TLink>(new FileMappedResizableDirectMemory(dataDBFilename), UnitedMemoryLinks<TLink>.DefaultLinksSizeStep, linksConstants, IndexTreeType.Default);
        }

        public static DefaultXmlStorage<TLink> CreateXmlStorage()
        {
            var links = CreateLinks();
            return CreateXmlStorage(links);
        }

        public static DefaultXmlStorage<TLink> CreateXmlStorage(ILinks<TLink> links)
        {
            BalancedVariantConverter = new(links);
            return new DefaultXmlStorage<TLink>(links, BalancedVariantConverter);
        }

        [Fact]
        public void ConstructorsTest() => CreateXmlStorage();

        [Fact]
        public void CreateDocumentTest()
        {
            var defaultXmlStorage = CreateXmlStorage();
            defaultXmlStorage.CreateDocument("documentName");
        }

        [Fact]
        public void GetDocumentTest()
        {
            var defaultXmlStorage = CreateXmlStorage();
            var createdDocumentLink = defaultXmlStorage.CreateDocument("documentName");
            var foundDocumentLink = defaultXmlStorage.GetDocumentOrDefault("documentName");
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
            var document = defaultXmlStorage.CreateDocument("documentName");
            defaultXmlStorage.AttachObject(document);
            defaultXmlStorage.CreateMember("keyName");
        }

        [Fact]
        public void AttachObjectValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");
            TLink documentValueLink = defaultXmlStorage.AttachObject(document);
            TLink createdObjectValue = links.GetTarget(documentValueLink);

            TLink valueMarker = links.GetSource(createdObjectValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLink createdObject = links.GetTarget(createdObjectValue);
            TLink objectMarker = links.GetSource(createdObject);
            Assert.Equal(objectMarker, defaultXmlStorage.ObjectMarker);

            TLink foundDocumentValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdObjectValue, foundDocumentValue);
        }

        [Fact]
        public void AttachStringValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");
            TLink documentStringLink = defaultXmlStorage.AttachString(document, "stringName");
            TLink createdStringValue = links.GetTarget(documentStringLink);

            TLink valueMarker = links.GetSource(createdStringValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLink createdString = links.GetTarget(createdStringValue);
            TLink stringMarker = links.GetSource(createdString);
            Assert.Equal(stringMarker, defaultXmlStorage.StringMarker);

            TLink foundStringValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdStringValue, foundStringValue);
        }

        [Fact]
        public void AttachNumberToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage = CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");
            TLink documentNumberLink = defaultXmlStorage.AttachNumber(document, 2021);
            TLink createdNumberValue = links.GetTarget(documentNumberLink);

            TLink valueMarker = links.GetSource(createdNumberValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLink createdNumber = links.GetTarget(createdNumberValue);
            TLink numberMarker = links.GetSource(createdNumber);
            Assert.Equal(numberMarker, defaultXmlStorage.NumberMarker);

            TLink foundNumberValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdNumberValue, foundNumberValue);
        }

        [Fact]
        public void AttachTrueValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");

            TLink documentTrueValueLink = defaultXmlStorage.AttachBoolean(document, true);
            TLink createdTrueValue = links.GetTarget(documentTrueValueLink);

            TLink valueMarker = links.GetSource(createdTrueValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLink trueMarker = links.GetTarget(createdTrueValue);
            Assert.Equal(trueMarker, defaultXmlStorage.TrueMarker);

            TLink foundTrueValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdTrueValue, foundTrueValue);
        }

        [Fact]
        public void AttachFalseValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");

            TLink documentFalseValueLink = defaultXmlStorage.AttachBoolean(document, false);
            TLink createdFalseValue = links.GetTarget(documentFalseValueLink);

            TLink valueMarker = links.GetSource(createdFalseValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLink falseMarker = links.GetTarget(createdFalseValue);
            Assert.Equal(falseMarker, defaultXmlStorage.FalseMarker);

            TLink foundFalseValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdFalseValue, foundFalseValue);
        }

        [Fact]
        public void AttachNullValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");

            TLink documentNullValueLink = defaultXmlStorage.AttachNull(document);
            TLink createdNullValue = links.GetTarget(documentNullValueLink);

            TLink valueMarker = links.GetSource(createdNullValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLink nullMarker = links.GetTarget(createdNullValue);
            Assert.Equal(nullMarker, defaultXmlStorage.NullMarker);

            TLink foundNullValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdNullValue, foundNullValue);
        }

        [Fact]
        public void AttachEmptyArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");

            TLink documentArrayValueLink = defaultXmlStorage.AttachArray(document, new TLink[0]);
            TLink createdArrayValue = links.GetTarget(documentArrayValueLink);
            output.WriteLine(links.Format(createdArrayValue));


            TLink valueMarker = links.GetSource(createdArrayValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLink createdArrayLink = links.GetTarget(createdArrayValue);
            TLink arrayMarker = links.GetSource(createdArrayLink);
            Assert.Equal(arrayMarker, defaultXmlStorage.ArrayMarker);

            TLink createArrayContents = links.GetTarget(createdArrayLink);
            Assert.Equal(createArrayContents, defaultXmlStorage.EmptyArrayMarker);

            TLink foundArrayValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
        }

        [Fact]
        public void AttachArrayValueToDocumentTest()
        {
            var links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");

            TLink arrayElement = defaultXmlStorage.CreateString("arrayElement");
            TLink[] array = new TLink[] { arrayElement, arrayElement, arrayElement };


            TLink documentArrayValueLink = defaultXmlStorage.AttachArray(document, array);
            TLink createdArrayValue = links.GetTarget(documentArrayValueLink);

            DefaultStack<TLink> stack = new();
            RightSequenceWalker<TLink> rightSequenceWalker = new(links, stack, arrayElementLink => links.GetSource(arrayElementLink) == defaultXmlStorage.ValueMarker);
            IEnumerable<TLink> arrayElementsValuesLink = rightSequenceWalker.Walk(createdArrayValue);
            Assert.NotEmpty(arrayElementsValuesLink);

            output.WriteLine(links.Format(createdArrayValue));


            TLink valueMarker = links.GetSource(createdArrayValue);
            Assert.Equal(valueMarker, defaultXmlStorage.ValueMarker);

            TLink createdArrayLink = links.GetTarget(createdArrayValue);
            TLink arrayMarker = links.GetSource(createdArrayLink);
            Assert.Equal(arrayMarker, defaultXmlStorage.ArrayMarker);

            TLink createdArrayContents = links.GetTarget(createdArrayLink);
            Assert.Equal(links.GetTarget(createdArrayContents), arrayElement);


            TLink foundArrayValue = defaultXmlStorage.GetValueLink(document);
            Assert.Equal(createdArrayValue, foundArrayValue);
        }

        [Fact]
        public void GetObjectFromDocumentObjectValueLinkTest()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");
            TLink documentObjectValueLink = defaultXmlStorage.AttachObject(document);
            TLink objectValueLink = links.GetTarget(documentObjectValueLink);
            TLink objectFromGetObject = defaultXmlStorage.GetObject(documentObjectValueLink);
            output.WriteLine(links.Format(objectValueLink));
            output.WriteLine(links.Format(objectFromGetObject));
            Assert.Equal(links.GetTarget(objectValueLink), objectFromGetObject);
        }

        [Fact]
        public void GetObjectFromObjectValueLinkTest()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");
            TLink documentObjectValueLink = defaultXmlStorage.AttachObject(document);
            TLink objectValueLink = links.GetTarget(documentObjectValueLink);
            TLink objectFromGetObject = defaultXmlStorage.GetObject(objectValueLink);
            Assert.Equal(links.GetTarget(objectValueLink), objectFromGetObject);
        }

        [Fact]
        public void AttachStringValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLink @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLink memberStringValueLink = defaultXmlStorage.AttachString(memberLink, "stringValue");
            TLink stringValueLink = links.GetTarget(memberStringValueLink);
            List<TLink> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(stringValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachNumberValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLink @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLink memberNumberValueLink = defaultXmlStorage.AttachNumber(memberLink, 123);
            TLink numberValueLink = links.GetTarget(memberNumberValueLink);
            List<TLink> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(numberValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachObjectValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLink @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLink memberObjectValueLink = defaultXmlStorage.AttachObject(memberLink);
            TLink objectValueLink = links.GetTarget(memberObjectValueLink);
            List<TLink> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(objectValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachArrayValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLink @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLink arrayElement = defaultXmlStorage.CreateString("arrayElement");
            TLink[] array = { arrayElement, arrayElement, arrayElement };
            TLink memberArrayValueLink = defaultXmlStorage.AttachArray(memberLink, array);
            TLink arrayValueLink = links.GetTarget(memberArrayValueLink);
            List<TLink> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(arrayValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachTrueValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLink @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLink memberTrueValueLink = defaultXmlStorage.AttachBoolean(memberLink, true);
            TLink trueValueLink = links.GetTarget(memberTrueValueLink);
            List<TLink> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(trueValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachFalseValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLink @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLink memberFalseValueLink = defaultXmlStorage.AttachBoolean(memberLink, false);
            TLink falseValueLink = links.GetTarget(memberFalseValueLink);
            List<TLink> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(memberLink, objectMembersLinks[0]);
            Assert.Equal(falseValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }

        [Fact]
        public void AttachNullValueToKey()
        {
            ILinks<TLink> links = CreateLinks();
            var defaultXmlStorage =CreateXmlStorage(links);
            TLink document = defaultXmlStorage.CreateDocument("documentName");
            TLink documentObjectValue = defaultXmlStorage.AttachObject(document);
            TLink @object = defaultXmlStorage.GetObject(documentObjectValue);
            TLink memberLink = defaultXmlStorage.AttachMemberToObject(@object, "keyName");
            TLink memberNullValueLink = defaultXmlStorage.AttachNull(memberLink);
            TLink nullValueLink = links.GetTarget(memberNullValueLink);
            List<TLink> objectMembersLinks = defaultXmlStorage.GetMembersLinks(@object);
            Assert.Equal(nullValueLink, defaultXmlStorage.GetValueLink(objectMembersLinks[0]));
        }
    }
}
