#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;

namespace Platform.Data.Doublets.Xml
{
    public interface IXmlStorage<TLink>
    {
        TLink CreateDocument(string name);
        TLink CreateElement(string name);
        TLink CreateTextElement(string content);
        TLink GetDocument(string name);
        TLink GetElement(string name);
        TLink GetTextElement(string content);
        IList<IList<TLink>> GetChildren(TLink parent);
        void AttachElementToParent(TLink elementToAttach, TLink parent);
    }
}
