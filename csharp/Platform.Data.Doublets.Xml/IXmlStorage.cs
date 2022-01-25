using System;
using System.Collections.Generic;

namespace Platform.Data.Doublets.Xml;
public interface IXmlStorage<TLink> where TLink : struct
    {
        ILinks<TLink> Links { get; }
        TLink DocumentMarker { get; }

        TLink ElementMarker { get; }

        TLink TextElementMarker { get; }
        TLink ObjectMarker { get; }
        TLink MemberMarker { get; }
        TLink ValueMarker { get; }
        TLink StringMarker { get; }
        TLink EmptyStringMarker { get; }
        TLink NumberMarker { get; }
        TLink NegativeNumberMarker { get; }
        TLink ArrayMarker { get; }
        TLink EmptyArrayMarker { get; }
        TLink TrueMarker { get; }
        TLink FalseMarker { get; }
        TLink NullMarker { get; }
        TLink CreateString(string content);
        TLink CreateStringValue(string content);
        TLink CreateNumber(decimal number);
        TLink CreateNumberValue(decimal number);
        TLink CreateBooleanValue(bool value);
        TLink CreateNullValue();
        TLink CreateDocument(string name);

        TLink CreateElement(string name);

        TLink CreateTextElement(string content);

        List<TLink> GetChildrenElements(TLink element);
        TLink CreateObject();
        TLink CreateObjectValue();
        TLink CreateArray(IList<TLink>? array);
        TLink CreateArray(TLink sequence);
        TLink CreateArrayValue(IList<TLink>? array);
        TLink CreateArrayValue(TLink sequence);
        TLink CreateMember(string name);
        TLink CreateValue(TLink value);
        TLink AttachObject(TLink parent);
        TLink AttachString(TLink parent, string content);
        TLink AttachNumber(TLink parent, decimal number);
        TLink AttachBoolean(TLink parent, bool value);
        TLink AttachNull(TLink parent);
        TLink AttachArray(TLink parent, IList<TLink>? array);
        TLink AttachMemberToObject(TLink @object, string keyName);
        TLink Attach(TLink parent, TLink child);
        TLink AppendArrayValue(TLink arrayValue, TLink appendant);
        TLink GetDocumentOrDefault(string name);
        string GetString(TLink stringValue);
        decimal GetNumber(TLink valueLink);
        TLink GetObject(TLink objectValueLink);
        TLink GetArray(TLink arrayValueLink);
        TLink GetArraySequence(TLink array);
        TLink GetValueLink(TLink parent);
        TLink GetValueMarker(TLink value);
        List<TLink> GetMembersLinks(TLink @object);
    }
