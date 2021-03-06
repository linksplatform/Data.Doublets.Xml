#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;

namespace Platform.Data.Doublets.Xml
{
    internal class XmlElementContext
    {
        public readonly Dictionary<string, int> ChildrenNamesCounts;
        public int TotalChildren;

        public XmlElementContext() => ChildrenNamesCounts = new Dictionary<string, int>();

        public void IncrementChildNameCount(string name)
        {
            if (ChildrenNamesCounts.TryGetValue(name, out int count))
            {
                ChildrenNamesCounts[name] = count + 1;
            }
            else
            {
                ChildrenNamesCounts[name] = 0;
            }
            TotalChildren++;
        }
    }
}
