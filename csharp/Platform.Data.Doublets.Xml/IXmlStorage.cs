#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;

namespace Platform.Data.Doublets.Xml
{
    /// <summary>
    /// <para>
    /// Defines the xml storage.
    /// </para>
    /// <para></para>
    /// </summary>
    public interface IXmlStorage<TLink>
    {
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
        TLink CreateDocument(string name);
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
        TLink CreateElement(string name);
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
        TLink CreateTextElement(string content);
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
        TLink GetDocument(string name);
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
        TLink GetElement(string name);
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
        TLink GetTextElement(string content);
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
        /// <para>A list of i list t link</para>
        /// <para></para>
        /// </returns>
        IList<IList<TLink>> GetChildren(TLink parent);
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
        void AttachElementToParent(TLink elementToAttach, TLink parent);
    }
}
