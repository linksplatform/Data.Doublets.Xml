using System.Collections.Generic;
using Platform.Numbers;
using Platform.Data.Numbers.Raw;
using Platform.Data.Doublets;
using Platform.Data.Doublets.Sequences.Converters;
using Platform.Data.Doublets.Sequences.Frequencies.Cache;
using Platform.Data.Doublets.Sequences.Frequencies.Counters;
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
        private static readonly TLink _zero = default;
        private static readonly TLink _one = Arithmetic.Increment(_zero);
        private readonly StringToUnicodeSequenceConverter<TLink> _stringToUnicodeSequenceConverter;
        private readonly ILinks<TLink> _links;
        private TLink _unicodeSymbolMarker;
        private TLink _unicodeSequenceMarker;
        private TLink _elementMarker;
        private TLink _textElementMarker;
        private TLink _documentMarker;

        private class Unindex : ISequenceIndex<TLink>
        {
            public bool Add(IList<TLink> sequence) => true;

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

        public DefaultXmlStorage(ILinks<TLink> links, bool indexSequenceBeforeCreation = false) :
            this(links, indexSequenceBeforeCreation,
                new LinkFrequenciesCache<TLink>(links,
                    new TotalSequenceSymbolFrequencyCounter<TLink>(links))) { }

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
        public IList<TLink> GetChildren(TLink parent)
        {
            List<TLink> childrens = new List<TLink>();
            _links.Each((link) => {
                childrens.Add(_links.GetTarget(link));
                return this._links.Constants.Continue;
            }, new Link<TLink>(_links.Constants.Any, parent, _links.Constants.Any));
            return childrens;
        }
    }
}
