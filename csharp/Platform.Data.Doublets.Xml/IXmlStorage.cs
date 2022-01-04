using System;
using System.Collections.Generic;

namespace Platform.Data.Doublets.Xml;

/// <summary>
///
/// </summary>
/// <typeparam name="TLink"></typeparam>
public interface IXmlStorage<TLink> where TLink : struct
    {
        /// <summary>
        /// <para>
        /// Gets the links value.
        /// </para>
        /// <para></para>
        /// </summary>
        ILinks<TLink> Links { get; }

        /// <summary>
        /// <para>
        /// Gets the document marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        TLink DocumentMarker { get; }

        TLink ElementMarker { get; }

        TLink TextElementMarker { get; }

        /// <summary>
        /// <para>
        /// Gets the object marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        TLink ObjectMarker { get; }

        /// <summary>
        /// <para>
        /// Gets the member marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        TLink MemberMarker { get; }

        /// <summary>
        /// <para>
        /// Gets the value marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        TLink ValueMarker { get; }

        /// <summary>
        /// <para>
        /// Gets the string marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        TLink StringMarker { get; }

        /// <summary>
        /// <para>
        /// Gets the empty string marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        TLink EmptyStringMarker { get; }

        /// <summary>
        /// <para>
        /// Gets the number marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        TLink NumberMarker { get; }

        /// <summary>
        /// <para>
        /// Gets the negative number marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        TLink NegativeNumberMarker { get; }

        /// <summary>
        /// <para>
        /// Gets the array marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        TLink ArrayMarker { get; }

        /// <summary>
        /// <para>
        /// Gets the empty array marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        TLink EmptyArrayMarker { get; }

        /// <summary>
        /// <para>
        /// Gets the true marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        TLink TrueMarker { get; }

        /// <summary>
        /// <para>
        /// Gets the false marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        TLink FalseMarker { get; }

        /// <summary>
        /// <para>
        /// Gets the null marker value.
        /// </para>
        /// <para></para>
        /// </summary>
        TLink NullMarker { get; }

        /// <summary>
        /// <para>
        /// Creates the string using the specified content.
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
        TLink CreateString(string content);

        /// <summary>
        /// <para>
        /// Creates the string value using the specified content.
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
        TLink CreateStringValue(string content);

        /// <summary>
        /// <para>
        /// Creates the number using the specified number.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="number">
        /// <para>The number.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink CreateNumber(decimal number);

        /// <summary>
        /// <para>
        /// Creates the number value using the specified number.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="number">
        /// <para>The number.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink CreateNumberValue(decimal number);

        /// <summary>
        /// <para>
        /// Creates the boolean value using the specified value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="value">
        /// <para>The value.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink CreateBooleanValue(bool value);

        /// <summary>
        /// <para>
        /// Creates the null value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink CreateNullValue();

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

        TLink CreateElement(string name);

        TLink CreateTextElement(string content);

        List<TLink> GetChildrenElements(TLink element);

        /// <summary>
        /// <para>
        /// Creates the object.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink CreateObject();

        /// <summary>
        /// <para>
        /// Creates the object value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink CreateObjectValue();

        /// <summary>
        /// <para>
        /// Creates the array using the specified array.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="array">
        /// <para>The array.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink CreateArray(IList<TLink> array);

        /// <summary>
        /// <para>
        /// Creates the array using the specified sequence.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="sequence">
        /// <para>The sequence.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink CreateArray(TLink sequence);

        /// <summary>
        /// <para>
        /// Creates the array value using the specified array.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="array">
        /// <para>The array.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink CreateArrayValue(IList<TLink> array);

        /// <summary>
        /// <para>
        /// Creates the array value using the specified sequence.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="sequence">
        /// <para>The sequence.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink CreateArrayValue(TLink sequence);

        /// <summary>
        /// <para>
        /// Creates the member using the specified name.
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
        TLink CreateMember(string name);

        /// <summary>
        /// <para>
        /// Creates the value using the specified value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="value">
        /// <para>The value.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink CreateValue(TLink value);

        /// <summary>
        /// <para>
        /// Attaches the object using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink AttachObject(TLink parent);

        /// <summary>
        /// <para>
        /// Attaches the string using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <param name="content">
        /// <para>The content.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink AttachString(TLink parent, string content);

        /// <summary>
        /// <para>
        /// Attaches the number using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <param name="number">
        /// <para>The number.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink AttachNumber(TLink parent, decimal number);

        /// <summary>
        /// <para>
        /// Attaches the boolean using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <param name="value">
        /// <para>The value.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink AttachBoolean(TLink parent, bool value);

        /// <summary>
        /// <para>
        /// Attaches the null using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink AttachNull(TLink parent);

        /// <summary>
        /// <para>
        /// Attaches the array using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <param name="array">
        /// <para>The array.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink AttachArray(TLink parent, IList<TLink> array);

        /// <summary>
        /// <para>
        /// Attaches the member to object using the specified object.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="object">
        /// <para>The object.</para>
        /// <para></para>
        /// </param>
        /// <param name="keyName">
        /// <para>The key name.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink AttachMemberToObject(TLink @object, string keyName);

        /// <summary>
        /// <para>
        /// Attaches the parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <param name="child">
        /// <para>The child.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink Attach(TLink parent, TLink child);

        /// <summary>
        /// <para>
        /// Appends the array value using the specified array value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="arrayValue">
        /// <para>The array value.</para>
        /// <para></para>
        /// </param>
        /// <param name="appendant">
        /// <para>The appendant.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The new array value.</para>
        /// <para></para>
        /// </returns>
        TLink AppendArrayValue(TLink arrayValue, TLink appendant);

        /// <summary>
        /// <para>
        /// Gets the document or default using the specified name.
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
        TLink GetDocumentOrDefault(string name);

        /// <summary>
        /// <para>
        /// Gets the string using the specified string value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="stringValue">
        /// <para>The string value.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="Exception">
        /// <para>The passed link does not contain a string.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The string</para>
        /// <para></para>
        /// </returns>
        string GetString(TLink stringValue);

        /// <summary>
        /// <para>
        /// Gets the number using the specified value link.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="valueLink">
        /// <para>The value link.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="Exception">
        /// <para>The passed link does not contain a number.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The decimal</para>
        /// <para></para>
        /// </returns>
        decimal GetNumber(TLink valueLink);

        /// <summary>
        /// <para>
        /// Gets the object using the specified object value link.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="objectValueLink">
        /// <para>The object value link.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="Exception">
        /// <para>The passed link does not contain an object.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink GetObject(TLink objectValueLink);

        /// <summary>
        /// <para>
        /// Gets the array using the specified array value link.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="arrayValueLink">
        /// <para>The array value link.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="Exception">
        /// <para>The passed link does not contain an array.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink GetArray(TLink arrayValueLink);

        /// <summary>
        /// <para>
        /// Gets the array sequence using the specified array.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="array">
        /// <para>The array.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink GetArraySequence(TLink array);

        /// <summary>
        /// <para>
        /// Gets the value link using the specified parent.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="parent">
        /// <para>The parent.</para>
        /// <para></para>
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// <para>More than 1 value found.</para>
        /// <para></para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>The list elements length is negative.</para>
        /// <para></para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>The passed link is not a value.</para>
        /// <para></para>
        /// </exception>
        /// <returns>
        /// <para>The link</para>
        /// <para></para>
        /// </returns>
        TLink GetValueLink(TLink parent);

        /// <summary>
        /// <para>
        /// Gets the value marker using the specified value.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="value">
        /// <para>The value.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The target source.</para>
        /// <para></para>
        /// </returns>
        TLink GetValueMarker(TLink value);

        /// <summary>
        /// <para>
        /// Gets the members links using the specified object.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="object">
        /// <para>The object.</para>
        /// <para></para>
        /// </param>
        /// <returns>
        /// <para>The members.</para>
        /// <para></para>
        /// </returns>
        List<TLink> GetMembersLinks(TLink @object);
    }
