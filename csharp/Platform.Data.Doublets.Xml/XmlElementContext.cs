using System.Collections.Generic;

﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    /// <summary>
    /// <para>
    /// Represents the xml element context.
    /// </para>
    /// <para></para>
    /// </summary>
    internal class XmlElementContext
    {
        /// <summary>
        /// <para>
        /// The children names counts.
        /// </para>
        /// <para></para>
        /// </summary>
        public readonly Dictionary<string, int> ChildrenNamesCounts;
        /// <summary>
        /// <para>
        /// The total children.
        /// </para>
        /// <para></para>
        /// </summary>
        public int TotalChildren;

        /// <summary>
        /// <para>
        /// Initializes a new <see cref="XmlElementContext"/> instance.
        /// </para>
        /// <para></para>
        /// </summary>
        public XmlElementContext() => ChildrenNamesCounts = new Dictionary<string, int>();

        /// <summary>
        /// <para>
        /// Increments the child name count using the specified name.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="name">
        /// <para>The name.</para>
        /// <para></para>
        /// </param>
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
