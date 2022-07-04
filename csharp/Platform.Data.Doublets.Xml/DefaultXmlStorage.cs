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
        public readonly ILinks<TLinkAddress> Links;
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

        public TLinkAddress DocumentType { get; }
        public TLinkAddress DocumentNameType { get; }

        public TLinkAddress ElementType { get; }
        
        public TLinkAddress DocumentChildrenNodesType { get; }
        public TLinkAddress ElementChildrenNodesType { get; }
        public TLinkAddress EmptyElementChildrenNodesType { get; }

        public TLinkAddress TextNodeType { get; }
        public TLinkAddress AttributeNodeType { get; }
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
            ElementChildrenNodesType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(ElementChildrenNodesType)));
            EmptyElementChildrenNodesType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(EmptyElementChildrenNodesType)));
            DocumentChildrenNodesType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(DocumentChildrenNodesType)));
            TextNodeType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(TextNodeType)));
            AttributeNodeType = links.GetOrCreate(Type, StringToUnicodeSequenceConverter.Convert(nameof(AttributeNodeType)));
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

        public TLinkAddress CreateInteger(int integer)
        {
            var convertedInteger = AddressToNumberConverter.Convert(integer);
        }

        #region Boolean

        public TLinkAddress CreateBooleanValue(bool value) => CreateValue(value ? TrueType : FalseType);

        #endregion
        
        #region Integer

        public bool IsInteger(TLinkAddress possibleIntegerLinkAddressType)
        {
            var possibleIntegerType = Links.GetSource(possibleIntegerLinkAddressType);
            return EqualityComparer.Equals(possibleIntegerType, IntegerType);
        }
        
        public void EnsureIsInteger(TLinkAddress possibleIntegerLinkAddressType)
        {
            if (!IsInteger(possibleIntegerLinkAddressType))
            {
                throw new ArgumentException($"{possibleIntegerLinkAddressType} is not an integer link address type.");
            }
        }

        #endregion

        #region Decimal

        public bool IsDecimal(TLinkAddress possibleDecimalLinkAddressType)
        {
            var possibleDecimalType = Links.GetSource(possibleDecimalLinkAddressType);
            return EqualityComparer.Equals(possibleDecimalType, DecimalType);
        }

        public void EnsureIsDecimal(TLinkAddress possibleDecimalLinkAddressType)
        {
            if (!IsDecimal(possibleDecimalLinkAddressType))
            {
                throw new ArgumentException($"{possibleDecimalLinkAddressType} is not an decimal link address type.");
            }
        }

        #endregion

        #region String
        
        public TLinkAddress CreateString(string content)
        {
            var @string = ConvertStringToSequence(content);
            return Links.GetOrCreate(StringType, @string);
        }
        public TLinkAddress CreateStringValue(string content)
        {
            var @string = CreateString(content);
            return CreateValue(@string);
        }

        private TLinkAddress ConvertStringToSequence(string content) => content == "" ? EmptyStringType : StringToUnicodeSequenceConverter.Convert(content);

        public bool IsString(TLinkAddress possibleStringLinkAddress)
        {
            var possibleStringType = Links.GetSource(possibleStringLinkAddress);
            return EqualityComparer.Equals(possibleStringType, StringType);
        }

        public void EnsureIsString(TLinkAddress possibleStringLinkAddress)
        {
            if(!IsString(possibleStringLinkAddress))
            {
                throw new ArgumentException($"{possibleStringLinkAddress} is not a string");
            }
        }

        public string GetString(TLinkAddress stringLinkAddress)
        {
            EnsureIsString(stringLinkAddress);
            var stringSequence = Links.GetTarget(stringLinkAddress);
            return UnicodeSequenceToStringConverter.Convert(stringSequence);
        }

        #endregion

        #region Duratoin

        public bool IsDuration(TLinkAddress possibleDurationLinkAddressType)
        {
            var possibleDurationType = Links.GetSource(possibleDurationLinkAddressType);
            return EqualityComparer.Equals(possibleDurationType, DurationType);
        }

        public void EnsureIsDuration(TLinkAddress possibleDurationLinkAddressType)
        {
            if (!IsDuration(possibleDurationLinkAddressType))
            {
                throw new ArgumentException($"{possibleDurationLinkAddressType} is not an duration link address type.");
            }
        }

        #endregion

        #region DateTime

        public bool IsDateTime(TLinkAddress possibleDateTimeLinkAddressType)
        {
            var possibleDateTimeType = Links.GetSource(possibleDateTimeLinkAddressType);
            return EqualityComparer.Equals(possibleDateTimeType, DateTimeType);
        }

        public void EnsureIsDateTime(TLinkAddress possibleDateTimeLinkAddressType)
        {
            if (!IsDateTime(possibleDateTimeLinkAddressType))
            {
                throw new ArgumentException($"{possibleDateTimeLinkAddressType} is not an dateTime link address type.");
            }
        }

        #endregion

        #region Date

        public bool IsDate(TLinkAddress possibleDateLinkAddressType)
        {
            var possibleDateType = Links.GetSource(possibleDateLinkAddressType);
            return EqualityComparer.Equals(possibleDateType, DateType);
        }

        public void EnsureIsDate(TLinkAddress possibleDateLinkAddressType)
        {
            if (!IsDate(possibleDateLinkAddressType))
            {
                throw new ArgumentException($"{possibleDateLinkAddressType} is not an date link address type.");
            }
        }

        #endregion

        #region Time

        public bool IsTime(TLinkAddress possibleTimeLinkAddressType)
        {
            var possibleTimeType = Links.GetSource(possibleTimeLinkAddressType);
            return EqualityComparer.Equals(possibleTimeType, TimeType);
        }

        public void EnsureIsTime(TLinkAddress possibleTimeLinkAddressType)
        {
            if (!IsTime(possibleTimeLinkAddressType))
            {
                throw new ArgumentException($"{possibleTimeLinkAddressType} is not an time link address type.");
            }
        }

        #endregion
        
        
        

        

        
        
        public TLinkAddress CreateNullValue() => CreateValue(NullType);

        public TLinkAddress CreateDocument(string name, TLinkAddress childrenNodesLink)
        {
            if (!IsDocumentChildrenNodesLinkAddress(childrenNodesLink))
            {
                throw new ArgumentException($"The passed link address is not a document children nodes link address", nameof(childrenNodesLink));
            }
            var documentLinkAddress = CreateDocument(name);
            Links.GetOrCreate(documentLinkAddress, childrenNodesLink);
            return documentLinkAddress;
        }

        public TLinkAddress CreateDocumentChildrenNodesLinkAddress(TLinkAddress documentChildrenNodesSequenceLinkAddress)
        {
            return Links.GetOrCreate(DocumentChildrenNodesType, documentChildrenNodesSequenceLinkAddress);
        }

        public TLinkAddress CreateDocument(string name)
        {
            var documentName = CreateString(name);
            var documentNameLinkAddress = Links.GetOrCreate(DocumentNameType, documentName);
            return Links.GetOrCreate(DocumentType, documentNameLinkAddress);
        }

        public TLinkAddress CreateElement(string name)
        {
            var elementName = CreateString(name);
            return Links.GetOrCreate(ElementType, elementName);
        }
        
        public void EnsureIsElementLinkAddress(TLinkAddress possibleElementLinkAddress)
        {
            if (!IsElementLinkAddress(possibleElementLinkAddress))
            {
                throw new ArgumentException($"The passed link address is not an element link address", nameof(possibleElementLinkAddress));
            }
        }

        public string GetElementName(TLinkAddress elementLinkAddress)
        {
            EnsureIsElementLinkAddress(elementLinkAddress);
            GetString()
            var elementNameLinkAddress = Links.GetTarget(elementLinkAddress);
            return UnicodeSequenceToStringConverter.Convert(elementNameLinkAddress);
        }

        public TLinkAddress CreateElementChildrenNodes(TLinkAddress elementLinkAddress, TLinkAddress childrenNodesSequenceLinkAddress)
        {
            TLinkAddress childrenNodesLinkAddress;
            if (EqualityComparer.Equals(childrenNodesSequenceLinkAddress, default))
            {
                childrenNodesLinkAddress = EmptyElementChildrenNodesType;
            }
            else
            {
                childrenNodesLinkAddress = Links.GetOrCreate(ElementChildrenNodesType, childrenNodesSequenceLinkAddress);
            }
            return childrenNodesLinkAddress;
        }

        public TLinkAddress CreateElement(string name, TLinkAddress childrenNodesSequenceLinkAddress)
        {
            var elementLinkAddress = CreateElement(name);
            var elementChildrenNodesLinkAddress = CreateElementChildrenNodes(elementLinkAddress, childrenNodesSequenceLinkAddress);
            Links.GetOrCreate(elementLinkAddress, elementChildrenNodesLinkAddress);
            return elementLinkAddress;
        }

        public bool IsDocumentChildrenNodesLinkAddress(TLinkAddress possibleDocumentChildrenNodesLinkAddress)
        {
            var possibleDocumentChildrenNodesType = Links.GetSource(possibleDocumentChildrenNodesLinkAddress);
            return EqualityComparer.Equals(possibleDocumentChildrenNodesType, DocumentChildrenNodesType);
        }

        public bool IsElementChildrenNodes(TLinkAddress possibleElementChildrenNodesLinkAddress)
        {
            var possibleElementChildrenNodesType = Links.GetSource(possibleElementChildrenNodesLinkAddress);
            return EqualityComparer.Equals(possibleElementChildrenNodesType, ElementChildrenNodesType);
        }
        
        public TLinkAddress GetDocumentChildrenNodesSequence(TLinkAddress childrenNodesLinkAddress)
        {
            if (!IsDocumentChildrenNodesLinkAddress(childrenNodesLinkAddress))
            {
                throw new ArgumentException("The passed link address is not a document children nodes link address", nameof(childrenNodesLinkAddress));
            }
            return Links.GetTarget(childrenNodesLinkAddress);
        }
        
        public TLinkAddress GetElementChildrenNodesSequence(TLinkAddress childrenNodesLinkAddress)
        {
            return Links.GetTarget(childrenNodesLinkAddress);
        }

        public bool IsNode(TLinkAddress possibleXmlNode)
        {
            var isElement = IsElementLinkAddress(possibleXmlNode);
            var isTextNode = IsTextNode(possibleXmlNode);
            var isAttributeNode = IsAttributeNode(possibleXmlNode);
            return isElement || isTextNode || isAttributeNode;
        }

        public IList<TLinkAddress> GetDocumentChildNodeLinkAddresses(TLinkAddress documentLinkAddress)
        {
            if (!IsDocument(documentLinkAddress))
            {
                throw new ArgumentException("The passed link address is not a document link address.", nameof(documentLinkAddress));
            }
            TLinkAddress childrenNodesLinkAddress = default;
            Links.Each(new Link<TLinkAddress>(Links.Constants.Any, documentLinkAddress, Links.Constants.Any), link =>
            {
                var possibleChildrenNodesLinkAddress = Links.GetTarget(link);
                if (IsDocumentChildrenNodesLinkAddress(possibleChildrenNodesLinkAddress))
                {
                    childrenNodesLinkAddress = possibleChildrenNodesLinkAddress;
                    return Links.Constants.Break;
                }
                return Links.Constants.Continue;
            });
            if (EqualityComparer.Equals(childrenNodesLinkAddress, default))
            {
                throw new Exception("Document children nodes are not found.");
            }
            var childrenNodesSequenceLinkAddress = GetDocumentChildrenNodesSequence(childrenNodesLinkAddress);
            RightSequenceWalker<TLinkAddress> childrenNodesRightSequenceWalker = new(Links, new DefaultStack<TLinkAddress>(), IsNode);
            var a = ((ILinks<ulong>)(object)Links).FormatStructure((ulong)(object)childrenNodesSequenceLinkAddress, link => true);
            Console.WriteLine(a);
            var childNodeLinkAddressList = childrenNodesRightSequenceWalker.Walk(childrenNodesSequenceLinkAddress).ToList();
            return childNodeLinkAddressList;
        }

        public IList<TLinkAddress> GetElementChildrenNodes(TLinkAddress elementLinkAddress)
        {
            if (!IsDocument(elementLinkAddress))
            {
                throw new ArgumentException("The passed link address is not an element link address.", nameof(elementLinkAddress));
            }
            var childrenNodes = new List<TLinkAddress>();
            Links.Each(new Link<TLinkAddress>(elementLinkAddress, Links.Constants.Any), elementToAnyLink =>
            {
                var possibleChildrenNodesLinkAddress = Links.GetTarget(elementToAnyLink);
                if (!IsDocumentChildrenNodesLinkAddress(possibleChildrenNodesLinkAddress))
                {
                    return Links.Constants.Continue;
                }
                var childrenNodesSequenceLinkAddress = GetDocumentChildrenNodesSequence(possibleChildrenNodesLinkAddress);
                RightSequenceWalker<TLinkAddress> childrenNodesRightSequenceWalker = new(Links, new DefaultStack<TLinkAddress>(), IsNode);
                childrenNodes = childrenNodesRightSequenceWalker.Walk(childrenNodesSequenceLinkAddress).ToList();
                return Links.Constants.Continue;
            });
            return childrenNodes;
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
            Links.Each(new Link<TLinkAddress>(DocumentNameType, Links.Constants.Any), link =>
            {
                var documentNameStringLinkAddress = Links.GetTarget(link);
                var documentNameStringSequenceLinkAddress = Links.GetSource(documentNameStringLinkAddress);
                if (EqualityComparer.Equals(documentNameStringSequenceLinkAddress, StringToUnicodeSequenceConverter.Convert(name)))
                {
                    documentNameLinkAddress = Links.GetIndex(link);
                }
                return Links.Constants.Continue;
            });
            return documentNameLinkAddress;
        }

        public bool IsDocumentName(TLinkAddress documentNameLinkAddress)
        {
            var documentNameType = Links.GetSource(documentNameLinkAddress);
            return EqualityComparer.Equals(documentNameType, DocumentNameType);
        }

        public bool IsTextNode(TLinkAddress textNodeLinkAddress)
        {
            var possibleTextNodeType = Links.GetSource(textNodeLinkAddress);
            return EqualityComparer.Equals(possibleTextNodeType, TextNodeType);
        }

        public bool IsAttributeNode(TLinkAddress attributeNodeLinkAddress)
        {
            var possibleAttributeNodeType = Links.GetSource(attributeNodeLinkAddress);
            return EqualityComparer.Equals(possibleAttributeNodeType, AttributeNodeType);
        }
        
        public bool IsElementLinkAddress(TLinkAddress elementLinkAddress)
        {
            var possibleElementType = Links.GetSource(elementLinkAddress);
            return EqualityComparer.Equals(possibleElementType, ElementType);
        }

        public TLinkAddress GetDocumentLinkAddress(string name)
        {
            TLinkAddress documentLinkAddress = default;
            Links.Each(new Link<TLinkAddress>(), link =>
            {
                var possibleDocumentNameLinkAddress = Links.GetTarget(link);
                if (!IsDocumentName(possibleDocumentNameLinkAddress))
                {
                    return Links.Constants.Continue;
                }
                if (!EqualityComparer.Equals(possibleDocumentNameLinkAddress, GetDocumentNameLinkAddress(name)))
                {
                    return Links.Constants.Continue;
                }
                var possibleDocumentLinkAddress = Links.GetSource(link);
                if (!IsDocument(possibleDocumentLinkAddress))
                {
                    return Links.Constants.Continue;
                }
                documentLinkAddress = possibleDocumentLinkAddress;
                return Links.Constants.Break;
            });
            return documentLinkAddress;
        }
        

        private bool IsDocument(TLinkAddress possibleDocumentLinkAddress)
        {
            var possibleDocumentType = Links.GetSource(possibleDocumentLinkAddress);
            return EqualityComparer.Equals(possibleDocumentType, DocumentType);
        }

        private XmlNodeType GetTypeFromType(TLinkAddress Type)
        {
            if (EqualityComparer.Equals(ElementType, Type))
            {
                return XmlNodeType.Element;
            }
            if (EqualityComparer.Equals(TextNodeType, Type))
            {
                return XmlNodeType.Element;
            }
            throw new NotSupportedException($"Type {Type} is not supported");
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

        private TLinkAddress CreateAttributeNode(XmlAttribute xmlAttribute)
        {
            var attributeName = _stringToUnicodeSequenceConverter.Convert(xmlAttribute.Name);
            var attributeValue = _stringToUnicodeSequenceConverter.Convert(xmlAttribute.Value);
            var attribute = Links.GetOrCreate(attributeName, attributeValue);
            return Links.GetOrCreate(AttributeNodeType, attribute);
        }

        public TLinkAddress CreateAttributeNode(string name, string value)
        {
            var nameLinkAddress = CreateString(name);
            var valueLinkAddress = CreateString(value);
            var attributeValueLinkAddress = Links.GetOrCreate(nameLinkAddress, valueLinkAddress);
            return Links.GetOrCreate(AttributeNodeType, attributeValueLinkAddress);
        }

        public XmlAttribute GetAttribute(TLinkAddress attributeLinkAddress)
        {
            return new XmlAttribute
            {
                Name = GetAttributeName(attributeLinkAddress),
                Value = GetAttributeValue(attributeLinkAddress)
            };
        }

        public string GetAttributeName(TLinkAddress attributeLinkAddress)
        {
            var attributeType = Links.GetSource(attributeLinkAddress);
            if (!EqualityComparer.Equals(attributeType, AttributeNodeType))
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
            if (!EqualityComparer.Equals(attributeType, AttributeNodeType))
            {
                throw new Exception("The passed link address is not an attribute link address.");
            }
            var attributeValueLinkAddress = Links.GetTarget(attributeLinkAddress);
            var attributeValueValueLinkAddress = Links.GetSource(attributeValueLinkAddress);
            return UnicodeSequenceToStringConverter.Convert(attributeValueValueLinkAddress);
        }
        
        public TLinkAddress CreateTextNode(string text)
        {
            var contentLink = CreateString(text);
            return Links.GetOrCreate(TextNodeType, contentLink);
        }

        public string GetTextNodeValue(TLinkAddress textNodeLinkAddress)
        {
            var textNodeType = Links.GetSource(textNodeLinkAddress);
            if (!EqualityComparer.Equals(textNodeType, TextNodeType))
            {
                throw new Exception("The passed link address is not a text element link address.");
            }
            var contentLink = Links.GetTarget(textNodeLinkAddress);
            return UnicodeSequenceToStringConverter.Convert(contentLink);
        }
    }
}
