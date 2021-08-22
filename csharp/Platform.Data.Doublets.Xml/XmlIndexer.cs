using System.Collections.Generic;
using Platform.Numbers;
using Platform.Data.Numbers.Raw;
using Platform.Data.Doublets;
using Platform.Data.Doublets.Sequences.Frequencies.Cache;
using Platform.Data.Doublets.Sequences.Frequencies.Counters;
using Platform.Data.Doublets.Sequences.Indexes;
using Platform.Data.Doublets.Unicode;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    /// <summary>
    /// <para>
    /// Represents the xml indexer.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="IXmlStorage{TLink}"/>
    public class XmlIndexer<TLink> : IXmlStorage<TLink>
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
        /// The index.
        /// </para>
        /// <para></para>
        /// </summary>
        private readonly CachedFrequencyIncrementingSequenceIndex<TLink> _index;
        /// <summary>
        /// <para>
        /// The char to unicode symbol converter.
        /// </para>
        /// <para></para>
        /// </summary>
        private readonly CharToUnicodeSymbolConverter<TLink> _charToUnicodeSymbolConverter;
        /// <summary>
        /// <para>
        /// The unicode symbol marker.
        /// </para>
        /// <para></para>
        /// </summary>
        private TLink _unicodeSymbolMarker;
        /// <summary>
        /// <para>
        /// The null constant.
        /// </para>
        /// <para></para>
        /// </summary>
        private readonly TLink _nullConstant;

        /// <summary>
        /// <para>
        /// Gets the cache value.
        /// </para>
        /// <para></para>
        /// </summary>
        public LinkFrequenciesCache<TLink> Cache { get; }

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="XmlIndexer"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="links">
        /// <para>A links.</para>
        /// <para></para>
        /// </param>
        public XmlIndexer(ILinks<TLink> links)
        {
            _nullConstant = links.Constants.Null;
            var totalSequenceSymbolFrequencyCounter = new TotalSequenceSymbolFrequencyCounter<TLink>(links);
            Cache = new LinkFrequenciesCache<TLink>(links, totalSequenceSymbolFrequencyCounter);
            _index = new CachedFrequencyIncrementingSequenceIndex<TLink>(Cache);
            var addressToRawNumberConverter = new AddressToRawNumberConverter<TLink>();
            InitConstants(links);
            _charToUnicodeSymbolConverter = new CharToUnicodeSymbolConverter<TLink>(links, addressToRawNumberConverter, _unicodeSymbolMarker);
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
            _unicodeSymbolMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(markerIndex));
            _ = links.GetOrCreate(meaningRoot, Arithmetic.Increment(markerIndex));
            _ = links.GetOrCreate(meaningRoot, Arithmetic.Increment(markerIndex));
            _ = links.GetOrCreate(meaningRoot, Arithmetic.Increment(markerIndex));
            _ = links.GetOrCreate(meaningRoot, Arithmetic.Increment(markerIndex));
        }

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
        public void AttachElementToParent(TLink elementToAttach, TLink parent)
        {
        }

        /// <summary>
        /// <para>
        /// Returns the elements using the specified string.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="@string">
        /// <para>The string.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The elements.</para>
        /// <para></para>
        /// </returns>
        public IList<TLink> ToElements(string @string)
        {
            var elements = new TLink[@string.Length];
            for (int i = 0; i < @string.Length; i++)
            {
                elements[i] = _charToUnicodeSymbolConverter.Convert(@string[i]);
            }
            return elements;
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
        /// <para>The null constant.</para>
        /// <para></para>
        /// </returns>
        public TLink CreateDocument(string name)
        {
            _index.Add(ToElements(name));
            return _nullConstant;
        }

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
        /// <para>The null constant.</para>
        /// <para></para>
        /// </returns>
        public TLink CreateElement(string name)
        {
            _index.Add(ToElements(name));
            return _nullConstant;
        }

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
        /// <para>The null constant.</para>
        /// <para></para>
        /// </returns>
        public TLink CreateTextElement(string content)
        {
            _index.Add(ToElements(content));
            return _nullConstant;
        }

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
        /// <exception cref="System.NotImplementedException">
        /// <para></para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLink GetDocument(string name)
        {
            throw new System.NotImplementedException();
        }

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
        /// <exception cref="System.NotImplementedException">
        /// <para></para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLink GetElement(string name)
        {
            throw new System.NotImplementedException();
        }

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
        /// <exception cref="System.NotImplementedException">
        /// <para></para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        public TLink GetTextElement(string content)
        {
            throw new System.NotImplementedException();
        }

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
        /// <exception cref="System.NotImplementedException">
        /// <para></para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>A list of i list t link</para>
        /// <para></para>
        /// </returns>
        public IList<IList<TLink>> GetChildren(TLink parent)
        {
            throw new System.NotImplementedException();
        }
    }
}
