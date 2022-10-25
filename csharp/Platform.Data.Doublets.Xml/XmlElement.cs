using System.Collections.Generic;
using System.Xml;

namespace Platform.Data.Doublets.Xml;

public class XmlElement<TLinkAddress>: XmlNode
{
    public string NamePrefix;
    public string LocalName;
    public List<TLinkAddress> Children = new List<TLinkAddress>();
}
