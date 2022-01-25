using System;
using System.Collections.Generic;

namespace Platform.Data.Doublets.Xml;
public interface IXmlStorage<TLinkAddress> where TLinkAddress : struct
    {
        ILinks<TLinkAddress> Links { get; }
        TLinkAddress DocumentMarker { get; }

        TLinkAddress ElementMarker { get; }

        TLinkAddress TextElementMarker { get; }
        TLinkAddress ObjectMarker { get; }
        TLinkAddress MemberMarker { get; }
        TLinkAddress ValueMarker { get; }
        TLinkAddress StringMarker { get; }
        TLinkAddress EmptyStringMarker { get; }
        TLinkAddress NumberMarker { get; }
        TLinkAddress NegativeNumberMarker { get; }
        TLinkAddress ArrayMarker { get; }
        TLinkAddress EmptyArrayMarker { get; }
        TLinkAddress TrueMarker { get; }
        TLinkAddress FalseMarker { get; }
        TLinkAddress NullMarker { get; }
        TLinkAddress CreateString(string content);
        TLinkAddress CreateStringValue(string content);
        TLinkAddress CreateNumber(decimal number);
        TLinkAddress CreateNumberValue(decimal number);
        TLinkAddress CreateBooleanValue(bool value);
        TLinkAddress CreateNullValue();
        TLinkAddress CreateDocument(string name);

        TLinkAddress CreateElement(string name);

        TLinkAddress CreateTextElement(string content);

        List<TLinkAddress> GetChildrenElements(TLinkAddress element);
        TLinkAddress CreateObject();
        TLinkAddress CreateObjectValue();
        TLinkAddress CreateArray(IList<TLinkAddress>? array);
        TLinkAddress CreateArray(TLinkAddress sequence);
        TLinkAddress CreateArrayValue(IList<TLinkAddress>? array);
        TLinkAddress CreateArrayValue(TLinkAddress sequence);
        TLinkAddress CreateMember(string name);
        TLinkAddress CreateValue(TLinkAddress value);
        TLinkAddress AttachObject(TLinkAddress parent);
        TLinkAddress AttachString(TLinkAddress parent, string content);
        TLinkAddress AttachNumber(TLinkAddress parent, decimal number);
        TLinkAddress AttachBoolean(TLinkAddress parent, bool value);
        TLinkAddress AttachNull(TLinkAddress parent);
        TLinkAddress AttachArray(TLinkAddress parent, IList<TLinkAddress>? array);
        TLinkAddress AttachMemberToObject(TLinkAddress @object, string keyName);
        TLinkAddress Attach(TLinkAddress parent, TLinkAddress child);
        TLinkAddress AppendArrayValue(TLinkAddress arrayValue, TLinkAddress appendant);
        TLinkAddress GetDocumentOrDefault(string name);
        string GetString(TLinkAddress stringValue);
        decimal GetNumber(TLinkAddress valueLink);
        TLinkAddress GetObject(TLinkAddress objectValueLink);
        TLinkAddress GetArray(TLinkAddress arrayValueLink);
        TLinkAddress GetArraySequence(TLinkAddress array);
        TLinkAddress GetValueLink(TLinkAddress parent);
        TLinkAddress GetValueMarker(TLinkAddress value);
        List<TLinkAddress> GetMembersLinks(TLinkAddress @object);
    }
