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
    public class DefaultXmlStorage<TLinkAddress>  /* : IXmlStorage<TLinkAddress> */ where TLinkAddress : struct
    {
        private class Unindex : ISequenceIndex<TLinkAddress>
        {
            public bool Add(IList<TLinkAddress>? sequence) => true;

            public bool MightContain(IList<TLinkAddress>? sequence) => true;
        }

        #region Fields

        private static readonly TLinkAddress _zero = default;
        private static readonly TLinkAddress _one = Arithmetic.Increment(_zero);
        public readonly TLinkAddress Any;
        public readonly BalancedVariantConverter<TLinkAddress> BalancedVariantConverter;
        public readonly IConverter<IList<TLinkAddress>, TLinkAddress> ListToSequenceConverter;
        public readonly TLinkAddress Type;
        public readonly EqualityComparer<TLinkAddress> EqualityComparer = EqualityComparer<TLinkAddress>.Default;
        private readonly StringToUnicodeSequenceConverter<TLinkAddress> _stringToUnicodeSequenceConverter;
        public readonly ILinks<TLinkAddress> Links;
        private TLinkAddress _unicodeSymbolType;
        private TLinkAddress _unicodeSequenceType;

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

        public TLinkAddress DocumentType { get; }

        public TLinkAddress DocumentNameType { get; }

        public TLinkAddress ElementType { get; }
        public TLinkAddress ElementFullNameType { get; }
        public TLinkAddress ElementNamePrefixType { get; }
        public TLinkAddress ElementLocalNameType { get; }
        public TLinkAddress ElementChildrenNodesType { get; }

        public TLinkAddress EmptyElementChildrenNodesSequenceType { get; }

        public TLinkAddress TextNodeType { get; }

        public TLinkAddress AttributeType { get; }
        public TLinkAddress AttributeNamePrefixType { get; }
        public TLinkAddress AttributeLocalNameType { get; }
        public TLinkAddress AttributeFullNameType { get; }
        public TLinkAddress AttributeValueType { get; }

        public TLinkAddress ObjectType { get; }

        public TLinkAddress MemberType { get; }

        public TLinkAddress ValueType { get; }

        public TLinkAddress StringType { get; }

        private TLinkAddress IntegerType { get; }

        private TLinkAddress DecimalType { get; }

        private TLinkAddress DurationType { get; }

        private TLinkAddress DateTimeType { get; }

        private TLinkAddress DateType { get; }

        private TLinkAddress TimeType { get; }

        public TLinkAddress EmptyStringType { get; }

        public TLinkAddress NumberType { get; }

        public TLinkAddress NegativeNumberType { get; }

        public TLinkAddress ArrayType { get; }

        public TLinkAddress EmptyArrayType { get; }

        public TLinkAddress TrueType { get; }

        public TLinkAddress FalseType { get; }

        public TLinkAddress NullType { get; }

        #endregion

        #region Constructors

        public DefaultXmlStorage(ILinks<TLinkAddress> links, IConverter<IList<TLinkAddress>, TLinkAddress> listToSequenceConverter)
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
            ElementFullNameType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ElementFullNameType)));
            ElementNamePrefixType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ElementNamePrefixType)));
            ElementLocalNameType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ElementLocalNameType)));
            ElementChildrenNodesType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ElementChildrenNodesType)));
            EmptyElementChildrenNodesSequenceType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(EmptyElementChildrenNodesSequenceType)));
            TextNodeType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(TextNodeType)));
            
            // Attribute
            AttributeType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(AttributeType)));
            AttributeNamePrefixType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(AttributeNamePrefixType)));
            AttributeLocalNameType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(AttributeLocalNameType)));
            AttributeFullNameType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(AttributeFullNameType)));
            AttributeValueType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(AttributeValueType)));
            
            ObjectType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ObjectType)));
            MemberType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(MemberType)));
            ValueType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ValueType)));
            StringType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(StringType)));
            IntegerType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(IntegerType)));
            DecimalType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(DecimalType)));
            DurationType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(DurationType)));
            DateTimeType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(DateTimeType)));
            DateType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(DateType)));
            TimeType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(TimeType)));
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

        #endregion

        #region ValueTypes

        // #region Boolean
        //
        // public TLinkAddress CreateBooleanValue(bool value) => CreateValue(value ? TrueType : FalseType);
        //
        // #endregion
        //
        // #region Integer
        //
        // public TLinkAddress CreateInteger(int integer)
        // {
        //     var convertedInteger = AddressToNumberConverter.Convert(integer);
        // }
        //
        // public bool IsInteger(TLinkAddress possibleIntegerLinkAddressType)
        // {
        //     var possibleIntegerType = Links.GetSource(possibleIntegerLinkAddressType);
        //     return EqualityComparer.Equals(possibleIntegerType, IntegerType);
        // }
        //
        // public void EnsureIsInteger(TLinkAddress possibleIntegerLinkAddressType)
        // {
        //     if (!IsInteger(possibleIntegerLinkAddressType))
        //     {
        //         throw new ArgumentException($"{possibleIntegerLinkAddressType} is not an integer link address type.");
        //     }
        // }
        //
        // #endregion
        //
        // #region Decimal
        //
        // public bool IsDecimal(TLinkAddress possibleDecimalLinkAddressType)
        // {
        //     var possibleDecimalType = Links.GetSource(possibleDecimalLinkAddressType);
        //     return EqualityComparer.Equals(possibleDecimalType, DecimalType);
        // }
        //
        // public void EnsureIsDecimal(TLinkAddress possibleDecimalLinkAddressType)
        // {
        //     if (!IsDecimal(possibleDecimalLinkAddressType))
        //     {
        //         throw new ArgumentException($"{possibleDecimalLinkAddressType} is not an decimal link address type.");
        //     }
        // }
        //
        // #endregion

        #region String

        public TLinkAddress GetStringOrDefault(string content)
        {
            var stringSequenceLinkAddress = ConvertStringToSequence(content);
            return Links.SearchOrDefault(StringType, stringSequenceLinkAddress);
        }

        public bool IsStringExists(string content)
        {
            var stringLinkAddress = GetStringOrDefault(content);
            return !EqualityComparer.Equals(stringLinkAddress, default);
        }

        public TLinkAddress CreateString(string content)
        {
            var @string = ConvertStringToSequence(content);
            return Links.GetOrCreate(StringType, @string);
        }

        private TLinkAddress ConvertStringToSequence(string content) => StringToUnicodeSequenceConverter.Convert(content);

        public bool IsString(TLinkAddress possibleStringLinkAddress)
        {
            var possibleStringType = Links.GetSource(possibleStringLinkAddress);
            return EqualityComparer.Equals(possibleStringType, StringType);
        }

        public void EnsureIsString(TLinkAddress possibleStringLinkAddress)
        {
            if (!IsString(possibleStringLinkAddress))
            {
                throw new ArgumentException($"{possibleStringLinkAddress} is not a string");
            }
        }

        public string GetString(TLinkAddress stringLinkAddress)
        {
            // (StringType -> "")
            // stringLinkAddress
            EnsureIsString(stringLinkAddress);
            var stringSequence = Links.GetTarget(stringLinkAddress);
            return UnicodeSequenceToStringConverter.Convert(stringSequence);
        }

        #endregion

        // #region Duratoin
        //
        // public bool IsDuration(TLinkAddress possibleDurationLinkAddressType)
        // {
        //     var possibleDurationType = Links.GetSource(possibleDurationLinkAddressType);
        //     return EqualityComparer.Equals(possibleDurationType, DurationType);
        // }
        //
        // public void EnsureIsDuration(TLinkAddress possibleDurationLinkAddressType)
        // {
        //     if (!IsDuration(possibleDurationLinkAddressType))
        //     {
        //         throw new ArgumentException($"{possibleDurationLinkAddressType} is not an duration link address type.");
        //     }
        // }
        //
        // #endregion
        //
        // #region DateTime
        //
        // public bool IsDateTime(TLinkAddress possibleDateTimeLinkAddressType)
        // {
        //     var possibleDateTimeType = Links.GetSource(possibleDateTimeLinkAddressType);
        //     return EqualityComparer.Equals(possibleDateTimeType, DateTimeType);
        // }
        //
        // public void EnsureIsDateTime(TLinkAddress possibleDateTimeLinkAddressType)
        // {
        //     if (!IsDateTime(possibleDateTimeLinkAddressType))
        //     {
        //         throw new ArgumentException($"{possibleDateTimeLinkAddressType} is not an dateTime link address type.");
        //     }
        // }
        //
        // #endregion
        //
        // #region Date
        //
        // public bool IsDate(TLinkAddress possibleDateLinkAddressType)
        // {
        //     var possibleDateType = Links.GetSource(possibleDateLinkAddressType);
        //     return EqualityComparer.Equals(possibleDateType, DateType);
        // }
        //
        // public void EnsureIsDate(TLinkAddress possibleDateLinkAddressType)
        // {
        //     if (!IsDate(possibleDateLinkAddressType))
        //     {
        //         throw new ArgumentException($"{possibleDateLinkAddressType} is not an date link address type.");
        //     }
        // }
        //
        // #endregion
        //
        // #region Time
        //
        // public bool IsTime(TLinkAddress possibleTimeLinkAddressType)
        // {
        //     var possibleTimeType = Links.GetSource(possibleTimeLinkAddressType);
        //     return EqualityComparer.Equals(possibleTimeType, TimeType);
        // }
        //
        // public void EnsureIsTime(TLinkAddress possibleTimeLinkAddressType)
        // {
        //     if (!IsTime(possibleTimeLinkAddressType))
        //     {
        //         throw new ArgumentException($"{possibleTimeLinkAddressType} is not an time link address type.");
        //     }
        // }
        //
        // #endregion

        #endregion

        public bool IsNode(TLinkAddress possibleXmlNode)
        {
            var isElement = IsElementNode(possibleXmlNode);
            var isTextNode = IsTextNode(possibleXmlNode);
            var isAttribute = IsAttribute(possibleXmlNode);
            return isElement || isTextNode || isAttribute;
        }

        #region Document

        public TLinkAddress GetRootElement(TLinkAddress documentLinkAddress)
        {
            EnsureIsDocument(documentLinkAddress);
            TLinkAddress rootElement = default;
            Links.Each(new Link<TLinkAddress>(Links.Constants.Any, documentLinkAddress, Links.Constants.Any), link =>
            {
                var possibleRootElementLinkAddress = Links.GetTarget(link);
                if (!IsElementNode(possibleRootElementLinkAddress))
                {
                    return Links.Constants.Continue;
                }
                rootElement = possibleRootElementLinkAddress;
                return Links.Constants.Break;
            });
            if (EqualityComparer.Equals(rootElement, default))
            {
                throw new Exception("Root element is not found.");
            }
            return rootElement;
        }


        public TLinkAddress CreateDocument(string name, TLinkAddress childrenNodesLink)
        {
            if (!IsElementNode(childrenNodesLink))
            {
                throw new ArgumentException($"The passed link address is not a document children nodes link address", nameof(childrenNodesLink));
            }
            var documentLinkAddress = CreateDocument(name);
            Links.GetOrCreate(documentLinkAddress, childrenNodesLink);
            return documentLinkAddress;
        }

        public void EnsureIsDocument(TLinkAddress possibleDocumentLinkAddressLink)
        {
            if (!IsDocument(possibleDocumentLinkAddressLink))
            {
                throw new ArgumentException($"{possibleDocumentLinkAddressLink} is not a document link address");
            }
        }

        public TLinkAddress AttachElementToDocument(TLinkAddress documentLinkAddress, TLinkAddress elementLinkAddress)
        {
            EnsureIsDocument(documentLinkAddress);
            EnsureIsElement(elementLinkAddress);
            return Links.GetOrCreate(documentLinkAddress, elementLinkAddress);
        }

        public TLinkAddress CreateDocument(string name)
        {
            var documentNameLinkAddress = CreateDocumentName(name);
            return Links.GetOrCreate(DocumentType, documentNameLinkAddress);
        }

        public TLinkAddress CreateDocumentName(string name)
        {
            var documentNameStringLinkAddress = CreateString(name);
            var documentNameLinkAddress = Links.GetOrCreate(DocumentNameType, documentNameStringLinkAddress);
            return documentNameLinkAddress;
        }

        public bool IsDocumentName(TLinkAddress possibleDocumentNameLinkAddress)
        {
            var possibleDocumentNameType = Links.GetSource(possibleDocumentNameLinkAddress);
            return EqualityComparer.Equals(possibleDocumentNameType, DocumentNameType);
        }
        
        public TLinkAddress GetDocumentNameOrDefault(string name)
        {
            var stringSequenceLinkAddress = GetStringOrDefault(name);
            return Links.SearchOrDefault(DocumentNameType, stringSequenceLinkAddress);
        }

        public void EnsureIsDocumentName(TLinkAddress possibleDocumentNameLinkAddress)
        {
            if (!IsDocumentName(possibleDocumentNameLinkAddress))
            {
                throw new ArgumentException($"{possibleDocumentNameLinkAddress} is not a document name link address");
            }
        }
        
        public string GetDocumentNameByDocumentLinkAddress(TLinkAddress documentNameLinkAddress)
        {
            EnsureIsDocumentName(documentNameLinkAddress);
            var documentNameStringLinkAddress = Links.GetTarget(documentNameLinkAddress);
            return GetString(documentNameStringLinkAddress);
        }
        
        public string GetDocumentNameByDocumentNameLinkAddress(TLinkAddress documentNameLinkAddress)
        {
            EnsureIsDocumentName(documentNameLinkAddress);
            var documentNameStringLinkAddress = Links.GetTarget(documentNameLinkAddress);
            return GetString(documentNameStringLinkAddress);
        }

        public TLinkAddress GetDocumentOrDefault(string name)
        {
            TLinkAddress documentNameLinkAddress = GetDocumentNameOrDefault(name);
            return Links.SearchOrDefault(DocumentType, documentNameLinkAddress);
        }

        public TLinkAddress GetDocument(string name)
        {
            var documentLinkAddress = GetDocumentOrDefault(name);
            if (EqualityComparer.Equals(documentLinkAddress, default))
            {
                throw new Exception($"Document with name {name} not found");
            }
            return documentLinkAddress;
        }

        private bool IsDocument(TLinkAddress possibleDocumentLinkAddress)
        {
            var possibleDocumentType = Links.GetSource(possibleDocumentLinkAddress);
            return EqualityComparer.Equals(possibleDocumentType, DocumentType);
        }

        #endregion

        #region Node
        
        #region TextNode

        public bool IsTextNode(TLinkAddress textNodeLinkAddress)
        {
            var possibleTextNodeType = Links.GetSource(textNodeLinkAddress);
            return EqualityComparer.Equals(possibleTextNodeType, TextNodeType);
        }

        public TLinkAddress CreateTextNode(string text)
        {
            var contentLink = CreateString(text);
            return Links.GetOrCreate(TextNodeType, contentLink);
        }

        public void EnsureIsTextNode(TLinkAddress possibleTextNodeLinkAddress)
        {
            if (!IsTextNode(possibleTextNodeLinkAddress))
            {
                throw new ArgumentException($"{possibleTextNodeLinkAddress} is not a text node link address");
            }
        }


        public string GetTextNode(TLinkAddress textNodeLinkAddress)
        {
            /*
             * (TextNode: TextNodeType -> TextNodeValue )
             * (TextNodeType: (Type: 1 -> 1) -> "TextNodeType")
             * (TextNodeValue: StringType -> "")
             */
            EnsureIsTextNode(textNodeLinkAddress);
            var contentLink = Links.GetTarget(textNodeLinkAddress);
            return GetString(contentLink);
        }

        #endregion

        #region Attribute

        public bool IsAttribute(TLinkAddress attributeLinkAddress)
        {
            var possibleAttributeType = Links.GetSource(attributeLinkAddress);
            return EqualityComparer.Equals(possibleAttributeType, AttributeType);
        }

        public void EnsureIsAttribute(TLinkAddress possibleAttributeLinkAddress)
        {
            if (!IsAttribute(possibleAttributeLinkAddress))
            {
                throw new AggregateException($"{possibleAttributeLinkAddress} is not an attribute node link address");
            }
        }

        public TLinkAddress CreateAttributeNamePrefix(string prefix)
        {
            var stringLinkAddress = CreateString(prefix);
            return Links.GetOrCreate(AttributeNamePrefixType, stringLinkAddress);
        }

        public TLinkAddress CreateAttributeLocalName(string localName)
        {
            var stringLinkAddress = CreateString(localName);
            return Links.GetOrCreate(AttributeLocalNameType, stringLinkAddress);
        }

        public TLinkAddress CreateAttributeFullName(string prefix, string localName)
        {
            var prefixLinkAddress = CreateAttributeNamePrefix(prefix);
            var localNameLinkAddress = CreateAttributeLocalName(localName);
            return Links.GetOrCreate(AttributeFullNameType, Links.GetOrCreate(prefixLinkAddress, localNameLinkAddress));
        }

        public TLinkAddress CreateAttributeValue(string value)
        {
            var stringLinkAddress = CreateString(value);
            return Links.GetOrCreate(AttributeValueType, stringLinkAddress);
        }
        
        public TLinkAddress CreateAttribute(string prefix, string localName, string value)
        {
            var attributeFullName = CreateAttributeFullName(prefix, localName);
            var attributeValue = CreateAttributeValue(value);
            var attributeOptions = Links.GetOrCreate(attributeFullName, attributeValue);
            return Links.GetOrCreate(AttributeType, attributeOptions);
        }

        public bool IsAttributeLocalName(TLinkAddress linkAddress)
        {
            return EqualityComparer.Equals(Links.GetSource(linkAddress), AttributeLocalNameType);
        }
        

        public void EnsureIsAttributeLocalName(TLinkAddress linkAddress)
        {
            if (!IsAttributeLocalName(linkAddress))
            {
                throw new Exception($"The source link address of {Links.Format(linkAddress)} must be {AttributeLocalNameType}");
            }
        }

        public bool IsAttributeNamePrefix(TLinkAddress linkAddress)
        {
            return EqualityComparer.Equals(Links.GetSource(linkAddress), AttributeNamePrefixType);
        }

        public void EnsureIsAttributeNamePrefix(TLinkAddress linkAddress)
        {
            if (!IsAttributeNamePrefix(linkAddress))
            {
                throw new Exception($"The source link address of {Links.Format(linkAddress)} must be {AttributeNamePrefixType}");
            }
        }

        public bool IsAttributeFullName(TLinkAddress linkAddress)
        {
            return EqualityComparer.Equals(Links.GetSource(linkAddress), AttributeFullNameType);
        }
        
        public void EnsureIsAttributeFullName(TLinkAddress linkAddress)
        {
            if (!IsAttributeFullName(linkAddress))
            {
                throw new Exception($"The source link address of {Links.Format(linkAddress)} must be {AttributeFullNameType}");
            }
        }
        
        public bool IsAttributeValue(TLinkAddress linkAddress)
        {
            return EqualityComparer.Equals(Links.GetSource(linkAddress), AttributeValueType);
        }
        
        public void EnsureIsAttributeValue(TLinkAddress linkAddress)
        {
            if (!IsAttributeFullName(linkAddress))
            {
                throw new Exception($"The source link address of {Links.Format(linkAddress)} must be {AttributeValueType}");
            }
        }

        public XmlAttribute GetAttribute(TLinkAddress attributeLinkAddress)
        {
            // (Attribute: AttributeType -> AttributeOptions) 
            // attributeLinkAddress;
            EnsureIsAttribute(attributeLinkAddress);
            // (AttributeOptions: AttributeFullName -> AttributeValue)
            var optionsLinkAddress = Links.GetTarget(attributeLinkAddress);
            // (AttributeFullName: AttributeFullNameType -> (AttributeNamePrefix -> AttributeLocalName) )
            var fullNameLinkAddress = Links.GetSource(optionsLinkAddress);
            EnsureIsAttributeFullName(fullNameLinkAddress);
            // (AttributeNamePrefix -> AttributeLocalName)
            var namePrefixAndLocalNameLinkAddress = Links.GetTarget(fullNameLinkAddress);
            // (AttributeNamePrefix: AttributeNamePrefixType -> (StringType -> "") )
            var namePrefixLinkAddress = Links.GetSource(namePrefixAndLocalNameLinkAddress);
            EnsureIsAttributeNamePrefix(namePrefixLinkAddress);
            // (AttributeLocalName: AttributeLocalNameType -> (StringType -> "") )
            var localNameLinkAddress = Links.GetTarget(namePrefixAndLocalNameLinkAddress);
            EnsureIsAttributeLocalName(localNameLinkAddress);
            // (AttributeValue: AttributeValueType -> (StringType -> "") )
            var valueLinkAddress = Links.GetTarget(optionsLinkAddress);
            return new XmlAttribute
            {
                NamePrefix = GetString(Links.GetTarget(namePrefixLinkAddress)),
                LocalName = GetString(Links.GetTarget(localNameLinkAddress)),
                Value = GetString(Links.GetTarget(valueLinkAddress))
            };
        }

        #endregion

        #region ElementNode
        

        public void EnsureIsElement(TLinkAddress possibleElementLinkAddress)
        {
            if (!IsElementNode(possibleElementLinkAddress))
            {
                throw new ArgumentException($"The passed link address is not an element link address", nameof(possibleElementLinkAddress));
            }
        }

        public string GetElementName(TLinkAddress elementLinkAddress)
        {
            EnsureIsElement(elementLinkAddress);
            var elementNameToChildNodesSequenceLinkAddress = Links.GetTarget(elementLinkAddress);
            var elementNameLinkAddress = Links.GetSource(elementNameToChildNodesSequenceLinkAddress);
            return GetString(elementNameLinkAddress);
        }

        public bool IsElementFullName(TLinkAddress linkAddress)
        {
            return EqualityComparer.Equals(Links.GetSource(linkAddress), ElementFullNameType);
        }
        
        public void EnsureIsElementFullName(TLinkAddress linkAddress)
        {
            if (!IsElementFullName(linkAddress))
            {
                throw new Exception($"The source link address of {Links.Format(linkAddress)} must be {ElementFullNameType}");
            }
        }
        
        public bool IsElementNamePrefix(TLinkAddress linkAddress)
        {
            return EqualityComparer.Equals(Links.GetSource(linkAddress), ElementNamePrefixType);
        }
        
        public void EnsureIsElementNamePrefix(TLinkAddress linkAddress)
        {
            if (!IsElementNamePrefix(linkAddress))
            {
                throw new Exception($"The source link address of {Links.Format(linkAddress)} must be {ElementNamePrefixType}");
            }
        }
        
        public bool IsElementLocalName(TLinkAddress linkAddress)
        {
            return EqualityComparer.Equals(Links.GetSource(linkAddress), ElementLocalNameType);
        }
        
        public void EnsureIsElementLocalName(TLinkAddress linkAddress)
        {
            if (!IsElementLocalName(linkAddress))
            {
                throw new Exception($"The source link address of {Links.Format(linkAddress)} must be {ElementLocalNameType}");
            }
        }
        
        public bool IsElementChildrenNodes(TLinkAddress linkAddress)
        {
            return EqualityComparer.Equals(Links.GetSource(linkAddress), ElementChildrenNodesType);
        }
        
        public void EnsureIsElementChildrenNodes(TLinkAddress linkAddress)
        {
            if (!IsElementChildrenNodes(linkAddress))
            {
                throw new Exception($"The source link address of {Links.Format(linkAddress)} must be {ElementChildrenNodesType}");
            }
        }

        public XmlElement<TLinkAddress> GetElement(TLinkAddress elementLinkAddress)
        {
            /*
             * (Element: ElementType -> (ElementFullName -> ElementChildrenNodes) )
             * (ElementFullName: ElementFullNameType -> (ElementNamePrefix -> ElementLocalName) )
             * (ElementNamePrefix: ElementNamePrefixName -> (StringType -> "") )
             * (ElementLocalName: ElementLocalNameName -> (StringType -> "") )
             * (ElementChildrenNodes: ElementChildrenNodesType -> [])
             */ 
            // elementLinkAddress
            EnsureIsElement(elementLinkAddress);
            // (ElementFullName -> ElementChildrenNodes)
            var optionsLinkAddress = Links.GetTarget(elementLinkAddress);
            // (ElementFullName: ElementFullNameType -> (ElementNamePrefix -> ElementLocalName) )
            var fullNameLinkAddress = Links.GetSource(optionsLinkAddress);
            EnsureIsElementFullName(fullNameLinkAddress);
            // (ElementNamePrefix -> ElementLocalName)
            var namePrefixAndLocalNameLinkAddress = Links.GetTarget(fullNameLinkAddress);
            // (ElementNamePrefix: ElementNamePrefixName -> (StringType -> "") )
            var namePrefixLinkAddress = Links.GetSource(namePrefixAndLocalNameLinkAddress);
            EnsureIsElementNamePrefix(namePrefixLinkAddress);
            // (ElementLocalName: ElementLocalNameName -> (StringType -> "") )
            var localNameLinkAddress = Links.GetTarget(namePrefixAndLocalNameLinkAddress);
            EnsureIsElementLocalName(localNameLinkAddress);
            // (ElementChildrenNodes: ElementChildrenNodesType -> [])
            var childrenNodesLinkAddress = Links.GetTarget(optionsLinkAddress);
            EnsureIsElementChildrenNodes(childrenNodesLinkAddress);
            
            RightSequenceWalker<TLinkAddress> childrenNodesRightSequenceWalker = new(Links, new DefaultStack<TLinkAddress>(), IsNode);

            return new XmlElement<TLinkAddress>
            {
                NamePrefix = GetString(Links.GetTarget(namePrefixLinkAddress)),
                LocalName = GetString(Links.GetTarget(localNameLinkAddress)),
                Children = childrenNodesRightSequenceWalker.Walk(childrenNodesLinkAddress).ToList()
            };
        }

        // public TLinkAddress AttachNodesToElement(TLinkAddress elementLinkAddress, List<TLinkAddress> nodesLinkAddresses)
        // {
        //     if (nodesLinkAddresses.Count == 0)
        //     {
        //         AttachNodesToElement(elementLinkAddress, EmptyElementChildrenNodesSequenceType);
        //     }
        //     return AttachNodesToElement(elementLinkAddress, ListToSequenceConverter.Convert(nodesLinkAddresses));
        // }
        //
        //
        // public TLinkAddress AttachNodesToElement(TLinkAddress elementLinkAddress, TLinkAddress nodesSequenceLinkAddress)
        // {
        //     if (EqualityComparer.Equals(nodesSequenceLinkAddress, default))
        //     {
        //         nodesSequenceLinkAddress = EmptyElementChildrenNodesSequenceType;
        //     }
        //     return Links.GetOrCreate(elementLinkAddress, nodesSequenceLinkAddress);
        // }

        public TLinkAddress CreateElement(string namePrefix, string localName, List<TLinkAddress> nodesLinkAddresses)
        {
            return CreateElement(namePrefix, localName, ListToSequenceConverter.Convert(nodesLinkAddresses));
        }

        public TLinkAddress CreateElementNamePrefix(string namePrefix)
        {
            // (ElementNamePrefix: ElementNamePrefixType -> (StringType -> "") )
            return Links.GetOrCreate(ElementNamePrefixType, CreateString(namePrefix));
        }
        
        public TLinkAddress CreateElementLocalName(string localName)
        {
            // (ElementLocalName: ElementLocalNameType (StringType -> "") )
            return Links.GetOrCreate(ElementLocalNameType, CreateString(localName));
        }
        
        public TLinkAddress CreateElementFullName(string namePrefix, string localName)
        {
            // (ElementNamePrefix: ElementNamePrefixType (StringType -> "") )
            var elementNamePrefixLinkAddress = CreateElementNamePrefix(namePrefix);
            // (ElementLocalName: ElementLocalNameType (StringType -> "") )
            var elementLocalNameLinkAddress = CreateElementLocalName(localName);
            // (ElementFullName: ElementFullNameType -> (ElementNamePrefix -> ElementLocalName) )
            return Links.GetOrCreate(ElementFullNameType, Links.GetOrCreate(elementNamePrefixLinkAddress, elementLocalNameLinkAddress));
        }
        
        public TLinkAddress CreateElement(string namePrefix, string localName, TLinkAddress childrenNodesSequenceLinkAddress)
        {
            if(EqualityComparer.Equals(childrenNodesSequenceLinkAddress, default))
            {
                childrenNodesSequenceLinkAddress = EmptyElementChildrenNodesSequenceType;
            }
           
            // (ElementChildrenNodes: ElementChildrenNodesSequenceType -> [])
            var elementChildrenLinkAddress = Links.GetOrCreate(ElementChildrenNodesType, childrenNodesSequenceLinkAddress);
            // (ElementFullName: ElementFullNameType -> (ElementNamePrefix -> ElementLocalName) )
            var fullNameLinkAddress = CreateElementFullName(namePrefix, localName);
            // (Element: ElementType -> ( ElementFullName -> ElementChildrenNodes )
            return Links.GetOrCreate(ElementType, Links.GetOrCreate(fullNameLinkAddress, elementChildrenLinkAddress));
        }
        
        public bool IsChildrenNodes(TLinkAddress possibleChildrenNodesLinkAddress)
        {
            var possibleChildrenNodesType = Links.GetSource(possibleChildrenNodesLinkAddress);
            return EqualityComparer.Equals(possibleChildrenNodesType, ElementChildrenNodesType);
        }

        public bool IsEmptyChildrenNodes(TLinkAddress possibleChildrenNodesLinkAddress)
        {
            return EqualityComparer.Equals(possibleChildrenNodesLinkAddress, EmptyElementChildrenNodesSequenceType);
        }

        public IList<TLinkAddress> GetChildrenNodes(TLinkAddress elementLinkAddress)
        {
            var childrenNodes = new List<TLinkAddress>();
            EnsureIsElement(elementLinkAddress);
            var elementNameToChildNodesSequenceLinkAddress = Links.GetTarget(elementLinkAddress);
            var childrenNodesLinkAddress = Links.GetTarget(elementNameToChildNodesSequenceLinkAddress);
            if (IsEmptyChildrenNodes(childrenNodesLinkAddress))
            {
                return childrenNodes;
            }
            
            RightSequenceWalker<TLinkAddress> childrenNodesRightSequenceWalker = new(Links, new DefaultStack<TLinkAddress>(), IsNode);
            return childrenNodesRightSequenceWalker.Walk(childrenNodesLinkAddress).ToList();
        }

        public bool IsElementNode(TLinkAddress elementLinkAddress)
        {
            var possibleElementType = Links.GetSource(elementLinkAddress);
            return EqualityComparer.Equals(possibleElementType, ElementType);
        }

        #endregion

        #endregion
    }
}
