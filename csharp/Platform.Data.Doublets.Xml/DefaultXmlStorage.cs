using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Platform.Collections.Stacks;
using Platform.Converters;
using Platform.Numbers;
using Platform.Data.Numbers.Raw;
using Platform.Data.Doublets.CriterionMatchers;
using Platform.Data.Doublets.Numbers.Rational;
using Platform.Data.Doublets.Numbers.Raw;
using Platform.Data.Doublets.Sequences;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.Sequences.HeightProviders;
using Platform.Data.Doublets.Sequences.Indexes;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Data.Doublets.Unicode;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    public class DefaultXmlStorage<TLinkAddress> /* : IXmlStorage<TLinkAddress> */ where TLinkAddress : struct
    {
        private static readonly TLinkAddress _zero = default;
        private static readonly TLinkAddress _one = Arithmetic.Increment(_zero);
        private readonly StringToUnicodeSequenceConverter<TLinkAddress> _stringToUnicodeSequenceConverter;
        private readonly ILinks<TLinkAddress> _links;
        private TLinkAddress _unicodeSymbolType;
        private TLinkAddress _unicodeSequenceType;

        private class Unindex : ISequenceIndex<TLinkAddress>
        {
            public bool Add(IList<TLinkAddress>? sequence) => true;

            public bool MightContain(IList<TLinkAddress>? sequence) => true;
        }
        public readonly TLinkAddress Any;
        public readonly BalancedVariantConverter<TLinkAddress> BalancedVariantConverter;
        public readonly IConverter<IList<TLinkAddress>?, TLinkAddress> ListToSequenceConverter;
        public readonly TLinkAddress Type;
        public readonly EqualityComparer<TLinkAddress> EqualityComparer = EqualityComparer<TLinkAddress>.Default;

        // Converters that are able to convert link's address (UInt64 value) to a raw number represented with another UInt64 value and back
        public readonly RawNumberToAddressConverter<TLinkAddress> NumberToAddressConverter = new();
        public readonly AddressToRawNumberConverter<TLinkAddress> AddressToNumberConverter = new();

        // Converters between BigInteger and raw number sequence
        public readonly BigIntegerToRawNumberSequenceConverter<TLinkAddress> BigIntegerToRawNumberSequenceConverter;
        public readonly RawNumberSequenceToBigIntegerConverter<TLinkAddress> RawNumberSequenceToBigIntegerConverter;

        // Converters between decimal and rational number sequence
        public readonly DecimalToRationalConverter<TLinkAddress> DecimalToRationalConverter;
        public readonly RationalToDecimalConverter<TLinkAddress> RationalToDecimalConverter;

        // Converters between string and unicode sequence
        public readonly IConverter<string, TLinkAddress> StringToUnicodeSequenceConverter;
        public readonly IConverter<TLinkAddress, string> UnicodeSequenceToStringConverter;
        public readonly DefaultSequenceRightHeightProvider<TLinkAddress> DefaultSequenceRightHeightProvider;
        public ILinks<TLinkAddress> Links { get; }
        public TLinkAddress DocumentType { get; }
        public TLinkAddress DocumentNameType { get; }

        public TLinkAddress ElementType { get; }
        
        public TLinkAddress ChildrenNodesType { get; }

        public TLinkAddress TextElementType { get; }
        public TLinkAddress AttributeElementType { get; }
        public TLinkAddress ObjectType { get; }
        public TLinkAddress MemberType { get; }
        public TLinkAddress ValueType { get; }
        public TLinkAddress StringType { get; }
        public TLinkAddress EmptyStringType { get; }
        public TLinkAddress NumberType { get; }
        public TLinkAddress NegativeNumberType { get; }
        public TLinkAddress ArrayType { get; }
        public TLinkAddress EmptyArrayType { get; }
        public TLinkAddress TrueType { get; }
        public TLinkAddress FalseType { get; }
        public TLinkAddress NullType { get; }
        public DefaultXmlStorage(ILinks<TLinkAddress> links, IConverter<IList<TLinkAddress>?, TLinkAddress> listToSequenceConverter)
        {
            Links = links;
            ListToSequenceConverter = listToSequenceConverter;
            // Initializes constants
            Any = Links.Constants.Any;
            TLinkAddress zero = default;
            var one = zero.Increment();
            Type = links.GetOrCreate(one, one);
            var typeIndex = one;
            var unicodeSymbolType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeIndex));
            var unicodeSequenceType = links.GetOrCreate(Type, Arithmetic.Increment(ref typeIndex));
            BalancedVariantConverter = new(links);
            TargetMatcher<TLinkAddress> unicodeSymbolCriterionMatcher = new(Links, unicodeSymbolType);
            TargetMatcher<TLinkAddress> unicodeSequenceCriterionMatcher = new(Links, unicodeSequenceType);
            CharToUnicodeSymbolConverter<TLinkAddress> charToUnicodeSymbolConverter = new(Links, AddressToNumberConverter, unicodeSymbolType);
            UnicodeSymbolToCharConverter<TLinkAddress> unicodeSymbolToCharConverter = new(Links, NumberToAddressConverter, unicodeSymbolCriterionMatcher);
            StringToUnicodeSequenceConverter = new CachingConverterDecorator<string, TLinkAddress>(new StringToUnicodeSequenceConverter<TLinkAddress>(Links, charToUnicodeSymbolConverter, BalancedVariantConverter, unicodeSequenceType));
            DocumentType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(DocumentType)));
            DocumentNameType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(DocumentNameType)));
            ElementType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ElementType)));
            ChildrenNodesType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ChildrenNodesType)));
            TextElementType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(TextElementType)));
            AttributeElementType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(AttributeElementType)));
            ObjectType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ObjectType)));
            MemberType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(MemberType)));
            ValueType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ValueType)));
            StringType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(StringType)));
            EmptyStringType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(EmptyStringType)));
            NumberType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(NumberType)));
            NegativeNumberType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(NegativeNumberType)));
            ArrayType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ArrayType)));
            EmptyArrayType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(EmptyArrayType)));
            TrueType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(TrueType)));
            FalseType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(FalseType)));
            NullType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(NullType)));
            RightSequenceWalker<TLinkAddress> unicodeSymbolSequenceWalker = new(Links, new DefaultStack<TLinkAddress>(), unicodeSymbolCriterionMatcher.IsMatched);
            UnicodeSequenceToStringConverter<TLinkAddress> unicodeSequenceToStringConverter = new UnicodeSequenceToStringConverter<TLinkAddress>(Links, unicodeSequenceCriterionMatcher, unicodeSymbolSequenceWalker, unicodeSymbolToCharConverter, unicodeSequenceType);
            UnicodeSequenceToStringConverter = new CachingConverterDecorator<TLinkAddress, string>(unicodeSequenceToStringConverter);
            BigIntegerToRawNumberSequenceConverter = new(links, AddressToNumberConverter, ListToSequenceConverter, NegativeNumberType);
            RawNumberSequenceToBigIntegerConverter = new(links, NumberToAddressConverter, NegativeNumberType);
            DecimalToRationalConverter = new(links, BigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter = new(links, RawNumberSequenceToBigIntegerConverter);
        }
        public TLinkAddress CreateString(string content)
        {
            var @string = GetStringSequence(content);
            return Links.GetOrCreate(StringType, @string);
        }
        public TLinkAddress CreateStringValue(string content)
        {
            var @string = CreateString(content);
            return CreateValue(@string);
        }
        public TLinkAddress CreateNumber(decimal number)
        {
            var numberSequence = DecimalToRationalConverter.Convert(number);
            return Links.GetOrCreate(NumberType, numberSequence);
        }
        public TLinkAddress CreateNumberValue(decimal number)
        {
            var numberLink = CreateNumber(number);
            return CreateValue(numberLink);
        }
        public TLinkAddress CreateBooleanValue(bool value) => CreateValue(value ? TrueType : FalseType);
        public TLinkAddress CreateNullValue() => CreateValue(NullType);

        public TLinkAddress CreateDocument(TLinkAddress elementsSequence)
        {
            return _links.GetOrCreate(DocumentType, elementsSequence);
        }
        
        public TLinkAddress CreateDocumentName(string name)
        {
            var documentName = CreateString(name);
            return Links.GetOrCreate(DocumentNameType, documentName);
        }

        public TLinkAddress CreateElement(string name)
        {
            var elementName = CreateString(name);
            return Links.GetOrCreate(ElementType, elementName);
        }

        public string GetElementName(TLinkAddress elementLinkAddress)
        {
            var elementType = Links.GetSource(elementLinkAddress);
            if (!EqualityComparer.Equals(elementType, ElementType))
            {
                throw new Exception("The passed link address is not an element link address.");
            }
            var elementNameLinkAddress = Links.GetTarget(elementLinkAddress);
            return UnicodeSequenceToStringConverter.Convert(elementNameLinkAddress);
        }

        public TLinkAddress CreateElementChildrenNodes(TLinkAddress elementLinkAddress, TLinkAddress childrenNodesSequenceLinkAddress)
        {
            var childrenNodesLinkAddress = Links.GetOrCreate(ChildrenNodesType, childrenNodesSequenceLinkAddress);
            return Links.GetOrCreate(elementLinkAddress, childrenNodesLinkAddress);
        }

        public TLinkAddress CreateElement(string name, TLinkAddress childrenNodesSequenceLinkAddress)
        {
            var elementLinkAddress = CreateElement(name);
            return CreateElementChildrenNodes(elementLinkAddress, childrenNodesSequenceLinkAddress);
        }

        public bool IsElementChildrenElements(TLinkAddress possibleChildrenNodesLinkAddress)
        {
            var possibleChildrenNodesType = Links.GetSource(possibleChildrenNodesLinkAddress);
            return EqualityComparer.Equals(possibleChildrenNodesType, ChildrenNodesType);
        }

        public TLinkAddress GetChildrenNodesSequence(TLinkAddress childrenNodesLinkAddress)
        {
            return Links.GetTarget(childrenNodesLinkAddress);
        }

        public bool IsNode(TLinkAddress possibleXmlNode)
        {
            var isElement = IsElement(possibleXmlNode);
            var isTextNode = IsTextNode(possibleXmlNode);
            var isAttributeNode = IsAttributeNode(possibleXmlNode);
            return isElement || isTextNode || isAttributeNode;
        }

        public List<TLinkAddress> GetChildrenNodes(TLinkAddress elementLinkAddress)
        {
            var childElements = new List<TLinkAddress>();
            _links.Each(new Link<TLinkAddress>(elementLinkAddress, _links.Constants.Any), parentToChildLink =>
            {
                var possibleChildrenNodesLinkAddress = _links.GetSource(parentToChildLink);
                if (!IsElementChildrenElements(possibleChildrenNodesLinkAddress))
                {
                    return Links.Constants.Continue;
                }
                var childrenElementsSequenceLinkAddress = GetChildrenNodesSequence(possibleChildrenNodesLinkAddress);
                RightSequenceWalker<TLinkAddress> rightSequenceWalker = new(Links, new DefaultStack<TLinkAddress>(), );
                var childrenElementsSequence = rightSequenceWalker.Walk(childrenElementsSequenceLinkAddress);
                return _links.Constants.Continue;
            });
            return childElements;
        }
        public TLinkAddress CreateObject()
        {
            var @object = Links.Create();
            return Links.Update(@object, newSource: ObjectType, newTarget: @object);
        }
        public TLinkAddress CreateObjectValue()
        {
            var @object = CreateObject();
            return CreateValue(@object);
        }
        public TLinkAddress CreateArray(IList<TLinkAddress>? array)
        {
            var arraySequence = array.Count == 0 ? EmptyArrayType : BalancedVariantConverter.Convert(array);
            return CreateArray(arraySequence);
        }
        public TLinkAddress CreateArray(TLinkAddress sequence) => Links.GetOrCreate(ArrayType, sequence);
        public TLinkAddress CreateArrayValue(IList<TLinkAddress>? array)
        {
            var arrayLink = CreateArray(array);
            return CreateValue(arrayLink);
        }
        public TLinkAddress CreateArrayValue(TLinkAddress sequence)
        {
            var array = CreateArray(sequence);
            return CreateValue(array);
        }
        public TLinkAddress CreateMember(string name)
        {
            var nameLink = CreateString(name);
            return Links.GetOrCreate(MemberType, nameLink);
        }
        public TLinkAddress CreateValue(TLinkAddress value) => Links.GetOrCreate(ValueType, value);
        public TLinkAddress AttachObject(TLinkAddress parent) => Attach(parent, CreateObjectValue());
        public TLinkAddress AttachString(TLinkAddress parent, string content)
        {
            var @string = CreateString(content);
            var stringValue = CreateValue(@string);
            return Attach(parent, stringValue);
        }
        public TLinkAddress AttachNumber(TLinkAddress parent, decimal number)
        {
            var numberLink = CreateNumber(number);
            var numberValue = CreateValue(numberLink);
            return Attach(parent, numberValue);
        }
        public TLinkAddress AttachBoolean(TLinkAddress parent, bool value)
        {
            var booleanValue = CreateBooleanValue(value);
            return Attach(parent, booleanValue);
        }
        public TLinkAddress AttachNull(TLinkAddress parent)
        {
            var nullValue = CreateNullValue();
            return Attach(parent, nullValue);
        }
        public TLinkAddress AttachArray(TLinkAddress parent, IList<TLinkAddress>? array)
        {
            var arrayValue = CreateArrayValue(array);
            return Attach(parent, arrayValue);
        }
        public TLinkAddress AttachMemberToObject(TLinkAddress @object, string keyName)
        {
            var member = CreateMember(keyName);
            return Attach(@object, member);
        }
        public TLinkAddress Attach(TLinkAddress child, TLinkAddress parent) => Links.GetOrCreate(child, parent);

        public TLinkAddress GetDocumentNameLinkAddress(string name)
        {
            TLinkAddress documentNameLinkAddress = default;
            Links.Each(new Link<TLinkAddress>(DocumentNameType, _links.Constants.Any), link =>
            {
                var documentNameStringLinkAddress = _links.GetTarget(link);
                var documentNameStringSequenceLinkAddress = _links.GetSource(documentNameStringLinkAddress);
                if (EqualityComparer.Equals(documentNameStringSequenceLinkAddress, StringToUnicodeSequenceConverter.Convert(name)))
                {
                    documentNameLinkAddress = _links.GetIndex(link);
                }
                return _links.Constants.Continue;
            });
            return documentNameLinkAddress;
        }

        public bool IsDocumentName(TLinkAddress documentNameLinkAddress)
        {
            var documentNameType = Links.GetSource(documentNameLinkAddress);
            return EqualityComparer.Equals(documentNameType, DocumentNameType);
        }

        public bool IsTextNode(TLinkAddress textElementLinkAddress)
        {
            var possibleTextElementType = Links.GetSource(textElementLinkAddress);
            return EqualityComparer.Equals(possibleTextElementType, TextElementType);
        }

        public bool IsAttributeNode(TLinkAddress attributeElementLinkAddress)
        {
            var possibleAttributeElementType = Links.GetSource(attributeElementLinkAddress);
            return EqualityComparer.Equals(possibleAttributeElementType, AttributeElementType);
        }
        
        public bool IsElement(TLinkAddress elementLinkAddress)
        {
            var possibleElementType = Links.GetSource(elementLinkAddress);
            return EqualityComparer.Equals(possibleElementType, ElementType);
        }

        public TLinkAddress GetDocumentLinkAddress(string name)
        {
            TLinkAddress documentLinkAddress = default;
            Links.Each(new Link<TLinkAddress>(), link =>
            {
                var possibleDocumentNameLinkAddress = _links.GetTarget(link);
                if (!IsDocumentName(possibleDocumentNameLinkAddress))
                {
                    return _links.Constants.Continue;
                }
                if (!EqualityComparer.Equals(possibleDocumentNameLinkAddress, GetDocumentNameLinkAddress(name)))
                {
                    return _links.Constants.Continue;
                }
                var possibleDocumentLinkAddress = _links.GetSource(link);
                if (!IsDocument(possibleDocumentLinkAddress))
                {
                    return _links.Constants.Continue;
                }
                documentLinkAddress = possibleDocumentLinkAddress;
                return _links.Constants.Break;
            });
            return documentLinkAddress;
        }
        

        private bool IsDocument(TLinkAddress documentLinkAddress)
        {
            var documentType = Links.GetSource(documentLinkAddress);
            return EqualityComparer.Equals(documentType, DocumentType);
        }

        public void GetChildrenNodes()
        {
            var any = _links.Constants.Any;
            _links.Each(new Link<TLinkAddress>(any, node.Link, any), fromElementToAnyLink =>
            {
                var child = _links.GetTarget(fromElementToAnyLink);
                var Type = _links.GetSource(child);
                var type = GetTypeFromType(Type);
                var childXmlElement = new XmlNode<TLinkAddress> { Link = child, Type = type };
                GetChildrenNodes(childXmlElement);
                node.Children.Enqueue(childXmlElement);
                return _links.Constants.Continue;
            });
        }

        private XmlNodeType GetTypeFromType(TLinkAddress Type)
        {
            if (EqualityComparer.Equals(ElementType, Type))
            {
                return XmlNodeType.Element;
            }
            if (EqualityComparer.Equals(TextElementType, Type))
            {
                return XmlNodeType.Element;
            }
            throw new NotSupportedException($"Type {Type} is not supported");
        }

        private TLinkAddress GetStringSequence(string content) => content == "" ? EmptyStringType : StringToUnicodeSequenceConverter.Convert(content);

        public bool IsString(TLinkAddress stringLinkAddress)
        {
            var stringType = _links.GetSource(stringLinkAddress);
            return EqualityComparer.Equals(stringType, StringType);
        }
        
        public string GetStringValue(TLinkAddress stringValue)
        {
            var stringSequence = _links.GetTarget(stringValue);
            var stringSequenceString = UnicodeSequenceToStringConverter.Convert(stringSequence);
            return stringSequenceString;
        }
        public decimal GetNumber(TLinkAddress valueLink)
        {
            var current = valueLink;
            TLinkAddress source;
            TLinkAddress target;
            for (int i = 0; i < 3; i++)
            {
                source = Links.GetSource(current);
                target = Links.GetTarget(current);
                if (EqualityComparer.Equals(source, NumberType))
                {
                    return RationalToDecimalConverter.Convert(target);
                }
                current = target;
            }
            throw new Exception("The passed link does not contain a number.");
        }
        public TLinkAddress GetObject(TLinkAddress objectValueLink)
        {
            var current = objectValueLink;
            TLinkAddress source;
            for (int i = 0; i < 3; i++)
            {
                source = Links.GetSource(current);
                if (EqualityComparer.Equals(source, ObjectType))
                {
                    return current;
                }
                current = Links.GetTarget(current);
            }
            throw new Exception("The passed link does not contain an object.");
        }
        public TLinkAddress GetArray(TLinkAddress arrayValueLink)
        {
            var current = arrayValueLink;
            TLinkAddress source;
            for (int i = 0; i < 3; i++)
            {
                source = Links.GetSource(current);
                if (EqualityComparer.Equals(source, ArrayType))
                {
                    return current;
                }
                current = Links.GetTarget(current);
            }
            throw new Exception("The passed link does not contain an array.");
        }
        public TLinkAddress GetArraySequence(TLinkAddress array) => Links.GetTarget(array);
        public TLinkAddress GetValueLink(TLinkAddress parent)
        {
            var query = new Link<TLinkAddress>(index: Any, source: parent, target: Any);
            var resultLinks = Links.All(query);
            switch (resultLinks.Count)
            {
                case 0:
                    return default;
                case 1:
                    var resultLinkTarget = Links.GetTarget(resultLinks[0]);
                    if (EqualityComparer.Equals(Links.GetSource(resultLinkTarget), ValueType))
                    {
                        return resultLinkTarget;
                    }
                    else
                    {
                        throw new InvalidOperationException("The passed link is not a value.");
                    }
                case > 1:
                    throw new InvalidOperationException("More than 1 value found.");
                default:
                    throw new InvalidOperationException("The list elements length is negative.");
            }
        }
        public TLinkAddress GetValueType(TLinkAddress value)
        {
            var target = Links.GetTarget(value);
            var targetSource = Links.GetSource(target);
            if (EqualityComparer.Equals(Type, targetSource))
            {
                return target;
            }
            return targetSource;
        }
        public List<TLinkAddress> GetMembersLinks(TLinkAddress @object)
        {
            Link<TLinkAddress> query = new(index: Any, source: @object, target: Any);
            List<TLinkAddress> members = new();
            Links.Each(objectMemberLink =>
            {
                var memberLink = Links.GetTarget(objectMemberLink);
                var memberType = Links.GetSource(memberLink);
                if (EqualityComparer.Equals(memberType, MemberType))
                {
                    members.Add(Links.GetIndex(objectMemberLink));
                }
                return Links.Constants.Continue;
            }, query);
            return members;
        }

        private void CreateAttributeElement(XmlNode<TLinkAddress> node)
        {
            var attributeName = _stringToUnicodeSequenceConverter.Convert(node.Name);
            var attributeValue = _stringToUnicodeSequenceConverter.Convert(node.Value);
            var attribute = _links.GetOrCreate(attributeName, attributeValue);
            node.Link = _links.GetOrCreate(AttributeElementType, attribute);
        }

        public TLinkAddress CreateNode(XmlNode<TLinkAddress> xmlNode)
        {
            if (XmlNodeType.Element == xmlNode.Type)
            {
                xmlNode.Link = CreateElement(xmlNode.Name);
            }
            else if (XmlNodeType.Text == xmlNode.Type)
            {
                xmlNode.Link = CreateTextElement(xmlNode.Value);
            }
            else if (XmlNodeType.Attribute == xmlNode.Type)
            {
                CreateAttributeElement(xmlNode.Name, xmlNode.Value);
            }
            if (0 == xmlNode.Children.Count)
            {
                return xmlNode.Link;
            }
            var childrenLinks = new List<TLinkAddress>();
            foreach (var childXmlElement in xmlNode.Children)
            {
                var childNode = CreateNode(childXmlElement);
                childrenLinks.Add(childNode);
            }
            var childrenSequence = ListToSequenceConverter.Convert(childrenLinks);
            return _links.GetOrCreate(xmlNode.Link, childrenSequence);
        }

        public TLinkAddress CreateAttributeElement(string name, string value)
        {
            var nameLinkAddress = CreateString(name);
            var valueLinkAddress = CreateString(value);
            var attributeValueLinkAddress = Links.GetOrCreate(nameLinkAddress, valueLinkAddress);
            return Links.GetOrCreate(AttributeElementType, attributeValueLinkAddress);
        }

        public XmlAttribute GetAttributeForElement(TLinkAddress parentElementLinkAddress)
        {
            Links.Each(new Link<TLinkAddress>())
        }

        public string GetAttributeName(TLinkAddress attributeLinkAddress)
        {
            var attributeType = Links.GetSource(attributeLinkAddress);
            if (!EqualityComparer.Equals(attributeType, AttributeElementType))
            {
                throw new Exception("The passed link address is not an attribute link address.");
            }
            var attributeValueLinkAddress = Links.GetTarget(attributeLinkAddress);
            var attributeNameLinkAddress = Links.GetSource(attributeValueLinkAddress);
            return UnicodeSequenceToStringConverter.Convert(attributeNameLinkAddress);
        }

        public string GetAttributeValue(TLinkAddress attributeLinkAddress)
        {
            var attributeType = Links.GetSource(attributeLinkAddress);
            if (!EqualityComparer.Equals(attributeType, AttributeElementType))
            {
                throw new Exception("The passed link address is not an attribute link address.");
            }
            var attributeValueLinkAddress = Links.GetTarget(attributeLinkAddress);
            var attributeValueValueLinkAddress = Links.GetSource(attributeValueLinkAddress);
            return UnicodeSequenceToStringConverter.Convert(attributeValueValueLinkAddress);
        }
        
        public TLinkAddress CreateTextElement(string text)
        {
            var contentLink = CreateString(text);
            return Links.GetOrCreate(TextElementType, contentLink);
        }

        public string GetTextElementValue(TLinkAddress textElementLinkAddress)
        {
            var textElementType = Links.GetSource(textElementLinkAddress);
            if (!EqualityComparer.Equals(textElementType, TextElementType))
            {
                throw new Exception("The passed link address is not a text element link address.");
            }
            var contentLink = Links.GetTarget(textElementLinkAddress);
            return UnicodeSequenceToStringConverter.Convert(contentLink);
        }

        public void GetElementName(XmlNode<TLinkAddress> node)
        {
            var nameSequence = _links.GetTarget(node.Link);
            node.Name = UnicodeSequenceToStringConverter.Convert(nameSequence);
        }
    }
}
