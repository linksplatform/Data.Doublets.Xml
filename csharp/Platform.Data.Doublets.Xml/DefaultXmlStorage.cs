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
        private TLinkAddress _unicodeSymbolMarker;
        private TLinkAddress _unicodeSequenceMarker;

        private class Unindex : ISequenceIndex<TLinkAddress>
        {
            public bool Add(IList<TLinkAddress>? sequence) => true;

            public bool MightContain(IList<TLinkAddress>? sequence) => true;
        }
        public readonly TLinkAddress Any;
        public static readonly TLinkAddress Zero = default;
        public static readonly TLinkAddress One = Arithmetic.Increment(Zero);
        public readonly BalancedVariantConverter<TLinkAddress> BalancedVariantConverter;
        public readonly IConverter<IList<TLinkAddress>?, TLinkAddress> ListToSequenceConverter;
        public readonly TLinkAddress MeaningRoot;
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
        public readonly DefaultSequenceAppender<TLinkAddress> DefaultSequenceAppender;
        public ILinks<TLinkAddress> Links { get; }
        public TLinkAddress DocumentMarker { get; }

        public TLinkAddress ElementMarker { get; }

        public TLinkAddress TextElementMarker { get; }
        public TLinkAddress AttributeMarker { get; }
        public TLinkAddress ObjectMarker { get; }
        public TLinkAddress MemberMarker { get; }
        public TLinkAddress ValueMarker { get; }
        public TLinkAddress StringMarker { get; }
        public TLinkAddress EmptyStringMarker { get; }
        public TLinkAddress NumberMarker { get; }
        public TLinkAddress NegativeNumberMarker { get; }
        public TLinkAddress ArrayMarker { get; }
        public TLinkAddress EmptyArrayMarker { get; }
        public TLinkAddress TrueMarker { get; }
        public TLinkAddress FalseMarker { get; }
        public TLinkAddress NullMarker { get; }
        public DefaultXmlStorage(ILinks<TLinkAddress> links, IConverter<IList<TLinkAddress>?, TLinkAddress> listToSequenceConverter)
        {
            Links = links;
            ListToSequenceConverter = listToSequenceConverter;
            // Initializes constants
            Any = Links.Constants.Any;
            var markerIndex = One;
            MeaningRoot = links.GetOrCreate(markerIndex, markerIndex);
            var unicodeSymbolMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            var unicodeSequenceMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            DocumentMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            ElementMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            TextElementMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            AttributeMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            ObjectMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            MemberMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            ValueMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            StringMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            EmptyStringMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            NumberMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            NegativeNumberMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            ArrayMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            EmptyArrayMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            TrueMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            FalseMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            NullMarker = links.GetOrCreate(MeaningRoot, Arithmetic.Increment(ref markerIndex));
            BalancedVariantConverter = new(links);
            TargetMatcher<TLinkAddress> unicodeSymbolCriterionMatcher = new(Links, unicodeSymbolMarker);
            TargetMatcher<TLinkAddress> unicodeSequenceCriterionMatcher = new(Links, unicodeSequenceMarker);
            CharToUnicodeSymbolConverter<TLinkAddress> charToUnicodeSymbolConverter = new(Links, AddressToNumberConverter, unicodeSymbolMarker);
            UnicodeSymbolToCharConverter<TLinkAddress> unicodeSymbolToCharConverter = new(Links, NumberToAddressConverter, unicodeSymbolCriterionMatcher);
            StringToUnicodeSequenceConverter = new CachingConverterDecorator<string, TLinkAddress>(new StringToUnicodeSequenceConverter<TLinkAddress>(Links, charToUnicodeSymbolConverter, BalancedVariantConverter, unicodeSequenceMarker));
            RightSequenceWalker<TLinkAddress> sequenceWalker = new(Links, new DefaultStack<TLinkAddress>(), unicodeSymbolCriterionMatcher.IsMatched);
            UnicodeSequenceToStringConverter = new CachingConverterDecorator<TLinkAddress, string>(new UnicodeSequenceToStringConverter<TLinkAddress>(Links, unicodeSequenceCriterionMatcher, sequenceWalker, unicodeSymbolToCharConverter));
            BigIntegerToRawNumberSequenceConverter = new(links, AddressToNumberConverter, ListToSequenceConverter, NegativeNumberMarker);
            RawNumberSequenceToBigIntegerConverter = new(links, NumberToAddressConverter, NegativeNumberMarker);
            DecimalToRationalConverter = new(links, BigIntegerToRawNumberSequenceConverter);
            RationalToDecimalConverter = new(links, RawNumberSequenceToBigIntegerConverter);
            DefaultSequenceAppender = new(Links, new DefaultStack<TLinkAddress>(), DefaultSequenceRightHeightProvider);
        }
        public TLinkAddress CreateString(string content)
        {
            var @string = GetStringSequence(content);
            return Links.GetOrCreate(StringMarker, @string);
        }
        public TLinkAddress CreateStringValue(string content)
        {
            var @string = CreateString(content);
            return CreateValue(@string);
        }
        public TLinkAddress CreateNumber(decimal number)
        {
            var numberSequence = DecimalToRationalConverter.Convert(number);
            return Links.GetOrCreate(NumberMarker, numberSequence);
        }
        public TLinkAddress CreateNumberValue(decimal number)
        {
            var numberLink = CreateNumber(number);
            return CreateValue(numberLink);
        }
        public TLinkAddress CreateBooleanValue(bool value) => CreateValue(value ? TrueMarker : FalseMarker);
        public TLinkAddress CreateNullValue() => CreateValue(NullMarker);
        public TLinkAddress CreateDocument(string name)
        {
            var documentName = CreateString(name);
            return Links.GetOrCreate(DocumentMarker, documentName);
        }

        public TLinkAddress CreateElement(string name)
        {
            var elementName = CreateString(name);
            return Links.CreateAndUpdate(ElementMarker, elementName);
        }

        public TLinkAddress CreateElement(string name, TLinkAddress childrenSequence)
        {
            var elementLink = CreateElement(name);
            Links.GetOrCreate(elementLink, childrenSequence);
            return elementLink;
        }

        public List<TLinkAddress> GetChildrenElements(TLinkAddress element)
        {
            var parentAndChildrenElementsQuery = new Link<TLinkAddress>(element, _links.Constants.Any);
            var childElements = new List<TLinkAddress>();
            _links.Each(parentAndChildLink =>
            {
                var childElement = _links.GetTarget(parentAndChildLink);
                var childElementSource = _links.GetSource(childElement);
                if (!EqualityComparer.Equals(ElementMarker, childElementSource))
                {
                    return _links.Constants.Continue;
                }
                childElements.Add(childElement);
                return _links.Constants.Continue;
            }, parentAndChildrenElementsQuery);
            return childElements;
        }
        public TLinkAddress CreateObject()
        {
            var @object = Links.Create();
            return Links.Update(@object, newSource: ObjectMarker, newTarget: @object);
        }
        public TLinkAddress CreateObjectValue()
        {
            var @object = CreateObject();
            return CreateValue(@object);
        }
        public TLinkAddress CreateArray(IList<TLinkAddress>? array)
        {
            var arraySequence = array.Count == 0 ? EmptyArrayMarker : BalancedVariantConverter.Convert(array);
            return CreateArray(arraySequence);
        }
        public TLinkAddress CreateArray(TLinkAddress sequence) => Links.GetOrCreate(ArrayMarker, sequence);
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
            return Links.GetOrCreate(MemberMarker, nameLink);
        }
        public TLinkAddress CreateValue(TLinkAddress value) => Links.GetOrCreate(ValueMarker, value);
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
        public TLinkAddress AppendArrayValue(TLinkAddress arrayValue, TLinkAddress appendant)
        {
            var array = GetArray(arrayValue);
            var arraySequence = Links.GetTarget(array);
            TLinkAddress newArrayValue;
            if (EqualityComparer.Equals(arraySequence, EmptyArrayMarker))
            {
                newArrayValue = CreateArrayValue(appendant);
            }
            else
            {
                arraySequence = DefaultSequenceAppender.Append(arraySequence, appendant);
                newArrayValue = CreateArrayValue(arraySequence);
            }
            return newArrayValue;
        }
        public TLinkAddress GetDocumentOrDefault(string name)
        {
            var stringSequence = GetStringSequence(name);
            var @string = Links.SearchOrDefault(StringMarker, stringSequence);
            if (EqualityComparer.Equals(@string, default))
            {
                return default;
            }
            return Links.SearchOrDefault(DocumentMarker, @string);
        }

        public void GetChildren(XmlNode<TLinkAddress> node)
        {
            var any = _links.Constants.Any;
            _links.Each(new Link<TLinkAddress>(any, node.Link, any), fromElementToAnyLink =>
            {
                var child = _links.GetTarget(fromElementToAnyLink);
                var marker = _links.GetSource(child);
                var type = GetTypeFromMarker(marker);
                var childXmlElement = new XmlNode<TLinkAddress> { Link = child, Type = type };
                GetChildren(childXmlElement);
                node.Children.Enqueue(childXmlElement);
                return _links.Constants.Continue;
            });
        }

        private XmlNodeType GetTypeFromMarker(TLinkAddress marker)
        {
            if (EqualityComparer.Equals(ElementMarker, marker))
            {
                return XmlNodeType.Element;
            }
            if (EqualityComparer.Equals(TextElementMarker, marker))
            {
                return XmlNodeType.Element;
            }
            throw new NotSupportedException($"Marker {marker} is not supported");
        }

        private TLinkAddress GetStringSequence(string content) => content == "" ? EmptyStringMarker : StringToUnicodeSequenceConverter.Convert(content);
        public string GetString(TLinkAddress stringValue)
        {
            var current = stringValue;
            TLinkAddress source;
            for (int i = 0; i < 3; i++)
            {
                source = Links.GetSource(current);
                if (EqualityComparer.Equals(source, StringMarker))
                {
                    var sequence = Links.GetTarget(current);
                    var isEmpty = EqualityComparer.Equals(sequence, EmptyStringMarker);
                    return isEmpty ? "" : UnicodeSequenceToStringConverter.Convert(sequence);
                }
                current = Links.GetTarget(current);
            }
            throw new Exception("The passed link does not contain a string.");
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
                if (EqualityComparer.Equals(source, NumberMarker))
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
                if (EqualityComparer.Equals(source, ObjectMarker))
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
                if (EqualityComparer.Equals(source, ArrayMarker))
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
                    if (EqualityComparer.Equals(Links.GetSource(resultLinkTarget), ValueMarker))
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
        public TLinkAddress GetValueMarker(TLinkAddress value)
        {
            var target = Links.GetTarget(value);
            var targetSource = Links.GetSource(target);
            if (EqualityComparer.Equals(MeaningRoot, targetSource))
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
                var memberMarker = Links.GetSource(memberLink);
                if (EqualityComparer.Equals(memberMarker, MemberMarker))
                {
                    members.Add(Links.GetIndex(objectMemberLink));
                }
                return Links.Constants.Continue;
            }, query);
            return members;
        }
    }
}
