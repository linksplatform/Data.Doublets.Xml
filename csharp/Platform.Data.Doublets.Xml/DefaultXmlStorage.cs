using System.Collections.Generic;
using Platform.Numbers;
using Platform.Data.Numbers.Raw;
using Platform.Data.Doublets;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.Sequences.Frequencies.Cache;
using Platform.Data.Doublets.Sequences.Indexes;
using Platform.Data.Doublets.Unicode;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    /// <summary>
    /// <para>
    /// Represents the default xml storage.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="IXmlStorage{TLink}"/>
    public class DefaultXmlStorage<TLink> : IXmlStorage<TLink>
    {
        /// <summary>
        /// <para>
        /// The zero.
        /// </para>
        /// <para></para>
        /// </summary>
        private static readonly TLink _zero = default;
        /// <summary>
        /// <para>
        /// The zero.
        /// </para>
        /// <para></para>
        /// </summary>
        private static readonly TLink _one = Arithmetic.Increment(_zero);

        /// <summary>
        /// <para>
        /// The string to unicode sequence converter.
        /// </para>
        /// <para></para>
        /// </summary>
        private readonly StringToUnicodeSequenceConverter<TLink> _stringToUnicodeSequenceConverter;
        /// <summary>
        /// <para>
        /// The links.
        /// </para>
        /// <para></para>
        /// </summary>
        private readonly ILinks<TLink> _links;
        /// <summary>
        /// <para>
        /// The unicode symbol marker.
        /// </para>
        /// <para></para>
        /// </summary>
        private TLink _unicodeSymbolMarker;
        /// <summary>
        /// <para>
        /// The unicode sequence marker.
        /// </para>
        /// <para></para>
        /// </summary>
        private TLink _unicodeSequenceMarker;
        /// <summary>
        /// <para>
        /// The element marker.
        /// </para>
        /// <para></para>
        /// </summary>
        private TLink _elementMarker;
        /// <summary>
        /// <para>
        /// The text element marker.
        /// </para>
        /// <para></para>
        /// </summary>
        private TLink _textElementMarker;
        /// <summary>
        /// <para>
        /// The document marker.
        /// </para>
        /// <para></para>
        /// </summary>
        private TLink _documentMarker;

        /// <summary>
        /// <para>
        /// Represents the unindex.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <seealso cref="ISequenceIndex{TLink}"/>
        private class Unindex : ISequenceIndex<TLink>
        {
            /// <summary>
            /// <para>
            /// Determines whether this instance add.
            /// </para>
            /// <para></para>
            /// </summary>
            /// <param name="sequence">
            /// <para>The sequence.</para>
            /// <para></para>
            /// </param>
            /// <returns>
            /// <para>The bool</para>
            /// <para></para>
            /// </returns>
            public bool Add(IList<TLink> sequence) => true;
            /// <summary>
            /// <para>
            /// Determines whether this instance might contain.
            /// </para>
            /// <para></para>
            /// </summary>
            /// <param name="sequence">
            /// <para>The sequence.</para>
            /// <para></para>
            /// </param>
            /// <returns>
            /// <para>The bool</para>
            /// <para></para>
            /// </returns>
            public bool MightContain(IList<TLink> sequence) => true;
        }  

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="DefaultXmlStorage"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        /// <param name="indexSequenceBeforeCreation">
        /// <para>A index sequence before creation.</para>
        /// <para></para>
        /// </param>
        /// <param name="frequenciesCache">
        /// <para>A frequencies cache.</para>
        /// <para></para>
        /// </param>
        public DefaultXmlStorage(ILinks<TLink> links, bool indexSequenceBeforeCreation, LinkFrequenciesCache<TLink> frequenciesCache)
        {
            var linkToItsFrequencyNumberConverter = new FrequenciesCacheBasedLinkToItsFrequencyNumberConverter<TLink>(frequenciesCache);
            var sequenceToItsLocalElementLevelsConverter = new SequenceToItsLocalElementLevelsConverter<TLink>(links, linkToItsFrequencyNumberConverter);
            var optimalVariantConverter = new OptimalVariantConverter<TLink>(links, sequenceToItsLocalElementLevelsConverter);
            InitConstants(links);
            var charToUnicodeSymbolConverter = new CharToUnicodeSymbolConverter<TLink>(links, new AddressToRawNumberConverter<TLink>(), _unicodeSymbolMarker);
            var index = indexSequenceBeforeCreation ? new CachedFrequencyIncrementingSequenceIndex<TLink>(frequenciesCache) : (ISequenceIndex<TLink>)new Unindex();
            _stringToUnicodeSequenceConverter = new StringToUnicodeSequenceConverter<TLink>(links, charToUnicodeSymbolConverter, index, optimalVariantConverter, _unicodeSequenceMarker);
            _links = links;
        }

        /// <summary>
        /// <para>
        /// Inits the constants using the specified links.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>The links.</para>
        /// <para></para>
        /// </param>
        private void InitConstants(ILinks<TLink> links)
        {
            var markerIndex = _one;
            var meaningRoot = links.GetOrCreate(markerIndex, markerIndex);
            _unicodeSymbolMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            _unicodeSequenceMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            _elementMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            _textElementMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
            _documentMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(ref markerIndex));
        }
        /// <summary>
        /// <para>
        /// Creates the document using the specified name.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="name">
        /// <para>The name.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLink CreateDocument(string name) => Create(_documentMarker, name);
        /// <summary>
        /// <para>
        /// Creates the element using the specified name.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="name">
        /// <para>The name.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLink CreateElement(string name) => Create(_elementMarker, name);
        /// <summary>
        /// <para>
        /// Creates the text element using the specified content.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="content">
        /// <para>The content.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLink CreateTextElement(string content) => Create(_textElementMarker, content);
        /// <summary>
        /// <para>
        /// Creates the marker.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="marker">
        /// <para>The marker.</para>
        /// <para></para>
        /// </param>
        /// <param name="content">
        /// <para>The content.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        private TLink Create(TLink marker, string content) => _links.GetOrCreate(marker, _stringToUnicodeSequenceConverter.Convert(content));
        /// <summary>
        /// <para>
        /// Attaches the element to parent using the specified element to attach.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="elementToAttach">
        /// <para>The element to attach.</para>
        /// <para></para>
        /// </param>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        public void AttachElementToParent(TLink elementToAttach, TLink parent) => _links.GetOrCreate(parent, elementToAttach);

        /// <summary>
        /// <para>
        /// Gets the document using the specified name.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="name">
        /// <para>The name.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLink GetDocument(string name) => Get(_documentMarker, name);
        /// <summary>
        /// <para>
        /// Gets the text element using the specified content.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="content">
        /// <para>The content.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLink GetTextElement(string content) => Get(_textElementMarker, content);
        /// <summary>
        /// <para>
        /// Gets the element using the specified name.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="name">
        /// <para>The name.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLink GetElement(string name) => Get(_elementMarker, name);
        /// <summary>
        /// <para>
        /// Gets the marker.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="marker">
        /// <para>The marker.</para>
        /// <para></para>
        /// </param>
        /// <param name="content">
        /// <para>The content.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        private TLink Get(TLink marker, string content) => _links.SearchOrDefault(marker, _stringToUnicodeSequenceConverter.Convert(content));
        /// <summary>
        /// <para>
        /// Gets the children using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The childrens.</para>
        /// <para></para>
        /// </returns>
        public IList<TLink> GetChildren(TLink parent) {
            var childrens = new List<TLink>();
            _links.Each((link) => {
                childrens.Add(_links.GetTarget(link));
                return this._links.Constants.Continue;
            }, new Link<TLink>(_links.Constants.Any, parent, _links.Constants.Any));
            return childrens;
        }
    }
}
   