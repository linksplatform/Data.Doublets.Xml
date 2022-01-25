// using System.Collections.Generic;
// using Platform.Numbers;
// using Platform.Data.Numbers.Raw;
// using Platform.Data.Doublets;
// using Platform.Data.Doublets.Sequences.Frequencies.Cache;
// using Platform.Data.Doublets.Sequences.Frequencies.Counters;
// using Platform.Data.Doublets.Sequences.Indexes;
// using Platform.Data.Doublets.Unicode;
//
// #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
//
// namespace Platform.Data.Doublets.Xml
// {
//     /// <summary>
//     /// <para>
//     /// Represents the xml indexer.
//     /// </para>
//     /// <para></para>
//     /// </summary>
//     /// <seealso cref="IXmlStorage{TLinkAddress}"/>
//     public class XmlIndexer<TLinkAddress> : IXmlStorage<TLinkAddress>
//     {
//         private static readonly TLinkAddress _zero = default;
//         private static readonly TLinkAddress _one = Arithmetic.Increment(_zero);
//         private readonly CachedFrequencyIncrementingSequenceIndex<TLinkAddress> _index;
//         private readonly CharToUnicodeSymbolConverter<TLinkAddress> _charToUnicodeSymbolConverter;
//         private TLinkAddress _unicodeSymbolMarker;
//         private readonly TLinkAddress _nullConstant;
//
//         /// <summary>
//         /// <para>
//         /// Gets the cache value.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         public LinkFrequenciesCache<TLinkAddress> Cache { get; }
//
//         /// <summary>
//         /// <para>
//         /// Initializes a new <see cref="XmlIndexer"/> instance.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="links">
//         /// <para>A links.</para>
//         /// <para></para>
//         /// </param>
//         public XmlIndexer(ILinks<TLinkAddress> links)
//         {
//             _nullConstant = links.Constants.Null;
//             var totalSequenceSymbolFrequencyCounter = new TotalSequenceSymbolFrequencyCounter<TLinkAddress>(links);
//             Cache = new LinkFrequenciesCache<TLinkAddress>(links, totalSequenceSymbolFrequencyCounter);
//             _index = new CachedFrequencyIncrementingSequenceIndex<TLinkAddress>(Cache);
//             var addressToRawNumberConverter = new AddressToRawNumberConverter<TLinkAddress>();
//             InitConstants(links);
//             _charToUnicodeSymbolConverter = new CharToUnicodeSymbolConverter<TLinkAddress>(links, addressToRawNumberConverter, _unicodeSymbolMarker);
//         }
//
//         private void InitConstants(ILinks<TLinkAddress> links)
//         {
//             var markerIndex = _one;
//             var meaningRoot = links.GetOrCreate(markerIndex, markerIndex);
//             _unicodeSymbolMarker = links.GetOrCreate(meaningRoot, Arithmetic.Increment(markerIndex));
//             _ = links.GetOrCreate(meaningRoot, Arithmetic.Increment(markerIndex));
//             _ = links.GetOrCreate(meaningRoot, Arithmetic.Increment(markerIndex));
//             _ = links.GetOrCreate(meaningRoot, Arithmetic.Increment(markerIndex));
//             _ = links.GetOrCreate(meaningRoot, Arithmetic.Increment(markerIndex));
//         }
//
//         /// <summary>
//         /// <para>
//         /// Attaches the element to parent using the specified element to attach.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="elementToAttach">
//         /// <para>The element to attach.</para>
//         /// <para></para>
//         /// </param>
//         /// <param name="parent">
//         /// <para>The parent.</para>
//         /// <para></para>
//         /// </param>
//         public void AttachElementToParent(TLinkAddress elementToAttach, TLinkAddress parent)
//         {
//         }
//
//         /// <summary>
//         /// <para>
//         /// Returns the elements using the specified string.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="@string">
//         /// <para>The string.</para>
//         /// <para></para>
//         /// </param>
//         /// <returns>
//         /// <para>The elements.</para>
//         /// <para></para>
//         /// </returns>
//         public IList<TLinkAddress>? ToElements(string @string)
//         {
//             var elements = new TLinkAddress[@string.Length];
//             for (int i = 0; i < @string.Length; i++)
//             {
//                 elements[i] = _charToUnicodeSymbolConverter.Convert(@string[i]);
//             }
//             return elements;
//         }
//
//         /// <summary>
//         /// <para>
//         /// Creates the document using the specified name.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="name">
//         /// <para>The name.</para>
//         /// <para></para>
//         /// </param>
//         /// <returns>
//         /// <para>The null constant.</para>
//         /// <para></para>
//         /// </returns>
//         public TLinkAddress CreateDocument(string name)
//         {
//             _index.Add(ToElements(name));
//             return _nullConstant;
//         }
//
//         /// <summary>
//         /// <para>
//         /// Creates the element using the specified name.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="name">
//         /// <para>The name.</para>
//         /// <para></para>
//         /// </param>
//         /// <returns>
//         /// <para>The null constant.</para>
//         /// <para></para>
//         /// </returns>
//         public TLinkAddress CreateElement(string name)
//         {
//             _index.Add(ToElements(name));
//             return _nullConstant;
//         }
//
//         /// <summary>
//         /// <para>
//         /// Creates the text element using the specified content.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="content">
//         /// <para>The content.</para>
//         /// <para></para>
//         /// </param>
//         /// <returns>
//         /// <para>The null constant.</para>
//         /// <para></para>
//         /// </returns>
//         public TLinkAddress CreateTextElement(string content)
//         {
//             _index.Add(ToElements(content));
//             return _nullConstant;
//         }
//
//         /// <summary>
//         /// <para>
//         /// Gets the document using the specified name.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="name">
//         /// <para>The name.</para>
//         /// <para></para>
//         /// </param>
//         /// <exception cref="System.NotImplementedException">
//         /// <para></para>
//         /// <para></para>
//         /// </exception>
//         /// <returns>
//         /// <para>The link</para>
//         /// <para></para>
//         /// </returns>
//         public TLinkAddress GetDocument(string name)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         /// <summary>
//         /// <para>
//         /// Gets the element using the specified name.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="name">
//         /// <para>The name.</para>
//         /// <para></para>
//         /// </param>
//         /// <exception cref="System.NotImplementedException">
//         /// <para></para>
//         /// <para></para>
//         /// </exception>
//         /// <returns>
//         /// <para>The link</para>
//         /// <para></para>
//         /// </returns>
//         public TLinkAddress GetElement(string name)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         /// <summary>
//         /// <para>
//         /// Gets the text element using the specified content.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="content">
//         /// <para>The content.</para>
//         /// <para></para>
//         /// </param>
//         /// <exception cref="System.NotImplementedException">
//         /// <para></para>
//         /// <para></para>
//         /// </exception>
//         /// <returns>
//         /// <para>The link</para>
//         /// <para></para>
//         /// </returns>
//         public TLinkAddress GetTextElement(string content)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         /// <summary>
//         /// <para>
//         /// Gets the children using the specified parent.
//         /// </para>
//         /// <para></para>
//         /// </summary>
//         /// <param name="parent">
//         /// <para>The parent.</para>
//         /// <para></para>
//         /// </param>
//         /// <exception cref="System.NotImplementedException">
//         /// <para></para>
//         /// <para></para>
//         /// </exception>
//         /// <returns>
//         /// <para>A list of i list t link</para>
//         /// <para></para>
//         /// </returns>
//         public IList<TLinkAddress>? GetChildren(TLinkAddress parent)
//         {
//             throw new System.NotImplementedException();
//         }
//     }
// }
