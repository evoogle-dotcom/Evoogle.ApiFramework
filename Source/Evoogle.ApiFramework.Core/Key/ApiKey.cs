// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Frozen;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Key.Internal;
using Evoogle.ApiFramework.Key.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Key;

/// <summary>
///     Represents an immutable, allocation-efficient API key that can hold a scalar or composite value.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiKey"/> is a discriminated union value type whose runtime variant is identified by
///         <see cref="ApiKind"/> (<see cref="ApiKeyKind"/>). Supported scalar arms are:
///         <see cref="string"/>, <see cref="int"/>, <see cref="long"/>, <see cref="Guid"/>,
///         <see cref="Ulid"/>, and <see cref="CultureInfo"/>. A <see cref="ApiKeyKind.Composite"/> arm
///         stores an ordered array of <see cref="ApiKeyPart"/> elements, each of which must be a scalar.
///     </para>
///     <para>
///         Scalar value-type arms (<see cref="int"/>, <see cref="long"/>, <see cref="Guid"/>,
///         <see cref="Ulid"/>) are stored without boxing in an explicit-layout union (<c>_valueUnion</c>).
///         Reference-type arms (<see cref="string"/>, <see cref="CultureInfo"/>) and the composite
///         part array are stored in a single <c>object?</c> reference slot (<c>_referenceValue</c>), keeping the
///         struct footprint minimal.
///     </para>
///     <para>
///         Composite keys may be <em>named</em> (every part carries a unique <see cref="ApiKeyPart.ApiName"/>,
///         rendered as <c>CustomerId=42|OrderNumber=1001</c>) or <em>ordered/positional</em> (parts are
///         unnamed, rendered as <c>42|1001</c>). Mixing named and unnamed parts within a single composite
///         is not permitted. Nested composites are also forbidden.
///     </para>
///     <para>
///         Hash codes for composite keys are pre-computed at construction time; scalar hash codes are
///         computed on-demand.
///     </para>
///     <para>
///         Use the static factory methods (<see cref="FromString"/>, <see cref="FromInt32"/>,
///         <see cref="FromInt64"/>, <see cref="FromGuid"/>, <see cref="FromUlid"/>,
///         <see cref="FromCulture(CultureInfo)"/>, <see cref="FromObject"/>, <see cref="Composite(ApiKey[])"/>) or the
///         implicit conversion operators to create instances. <see cref="Empty"/> (equivalent to
///         <c>default(<see cref="ApiKey"/>)</c>) represents the absence of a key.
///     </para>
///     <para>
///         <see cref="ApiKey"/> implements <see cref="IParsable{TSelf}"/> and
///         <see cref="ISpanParsable{TSelf}"/> for string parsing, and
///         <see cref="IFormattable"/> / <see cref="ISpanFormattable"/> for culture-aware formatting.
///         It also implements <see cref="IEquatable{T}"/> and <see cref="IComparable{T}"/> for use
///         as dictionary keys and in sorted collections.
///     </para>
/// </remarks>
[DebuggerDisplay("{ToDebuggerDisplay(),nq}")]
[JsonConverter(typeof(ApiKeyJsonConverter))]
public readonly struct ApiKey
    : IEquatable<ApiKey>
    , IComparable<ApiKey>
    , IFormattable
    , IParsable<ApiKey>
    , ISpanFormattable
    , ISpanParsable<ApiKey>
{
    #region Constants
    /// <summary>
    ///     The display text used for an empty key in debugger and ToString representations.
    /// </summary>
    public const string EmptyDisplayText = "<empty>";
    #endregion

    #region Fields
    /// <summary>
    ///     Represents an empty key with no value.
    /// </summary>
    /// <remarks>
    ///     Equivalent to <c>default(<see cref="ApiKey"/>)</c>. Used to represent the absence of a key value.
    ///     Has <see cref="ApiKind"/> equal to <see cref="ApiKeyKind.Empty"/>.
    /// </remarks>
    public static readonly ApiKey Empty = default;

    /// <summary>
    ///     Explicit-layout value union storing primitive types without boxing.
    /// </summary>
    /// <remarks>
    ///     Overlays <see cref="int"/>, <see cref="long"/>, <see cref="Guid"/>, and <see cref="Ulid"/> in the same
    ///     memory location, enabling zero-allocation storage of scalar keys.
    /// </remarks>
    private readonly ApiKeyValueUnion _valueUnion;

    /// <summary>
    ///     Reference slot for reference-type arms and composite arrays.
    /// </summary>
    /// <remarks>
    ///     Stores <see cref="string"/>, <see cref="CultureInfo"/>, or <see cref="ApiKeyPart"/>[] for composite keys.
    ///     Null for scalar value-type keys.
    /// </remarks>
    private readonly object? _referenceValue;

    /// <summary>
    ///     Frozen set of CLR types supported as scalar key values.
    /// </summary>
    /// <remarks>
    ///     Used by <c>IsScalarType</c> to validate type compatibility before creating <see cref="ApiKey"/> instances.
    ///     Supports: <see cref="string"/>, <see cref="int"/>, <see cref="long"/>, <see cref="Guid"/>,
    ///     <see cref="Ulid"/>, and <see cref="CultureInfo"/>.
    /// </remarks>
    private static readonly FrozenDictionary<Type, ApiKeyKind> _scalarTypeKinds = new Dictionary<Type, ApiKeyKind>
    {
        [typeof(string)] = ApiKeyKind.String,
        [typeof(int)] = ApiKeyKind.Int32,
        [typeof(long)] = ApiKeyKind.Int64,
        [typeof(Guid)] = ApiKeyKind.Guid,
        [typeof(Ulid)] = ApiKeyKind.Ulid,
        [typeof(CultureInfo)] = ApiKeyKind.Culture
    }.ToFrozenDictionary();

    /// <summary>
    ///     Pre-computed hash code for composite keys.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Cached during construction to optimize dictionary and hash-based collection operations,
    ///         which are primary use cases for <see cref="ApiKey"/>.
    ///     </para>
    ///     <para>
    ///         For scalar keys, the hash code is computed on-demand in <see cref="GetHashCode"/>.
    ///         Only composite keys benefit from pre-computation due to their array traversal cost.
    ///     </para>
    /// </remarks>
    private readonly int _hashCode;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the discriminated union kind for this key.
    /// </summary>
    /// <value>
    ///     An <see cref="ApiKeyKind"/> value indicating the runtime type of this key (e.g., <see cref="ApiKeyKind.Int32"/>,
    ///     <see cref="ApiKeyKind.Guid"/>, <see cref="ApiKeyKind.Composite"/>).
    /// </value>
    /// <remarks>
    ///     Used for type-safe access to the underlying value via pattern matching, type checking, and conversion methods.
    /// </remarks>
    public ApiKeyKind ApiKind { get; }

    #endregion

    #region Computed Properties
    /// <summary>
    ///     Gets whether this key has a value (is not empty).
    /// </summary>
    /// <value>
    ///     <see langword="true"/> if <see cref="ApiKind"/> is not <see cref="ApiKeyKind.Empty"/>; otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    ///     Use this property to check for the presence of a key value before accessing its components.
    ///     Equivalent to <c>!Equals(<see cref="Empty"/>)</c>.
    /// </remarks>
    public readonly bool HasValue => this.ApiKind != ApiKeyKind.Empty;

    /// <summary>Gets whether this key is a <see cref="string"/> value.</summary>
    /// <value><see langword="true"/> if <see cref="ApiKind"/> is <see cref="ApiKeyKind.String"/>; otherwise, <see langword="false"/>.</value>
    public readonly bool IsString => this.ApiKind == ApiKeyKind.String;

    /// <summary>Gets whether this key is an <see cref="int"/> value.</summary>
    /// <value><see langword="true"/> if <see cref="ApiKind"/> is <see cref="ApiKeyKind.Int32"/>; otherwise, <see langword="false"/>.</value>
    public readonly bool IsInt32 => this.ApiKind == ApiKeyKind.Int32;

    /// <summary>Gets whether this key is a <see cref="long"/> value.</summary>
    /// <value><see langword="true"/> if <see cref="ApiKind"/> is <see cref="ApiKeyKind.Int64"/>; otherwise, <see langword="false"/>.</value>
    public readonly bool IsInt64 => this.ApiKind == ApiKeyKind.Int64;

    /// <summary>Gets whether this key is a <see cref="Guid"/> value.</summary>
    /// <value><see langword="true"/> if <see cref="ApiKind"/> is <see cref="ApiKeyKind.Guid"/>; otherwise, <see langword="false"/>.</value>
    public readonly bool IsGuid => this.ApiKind == ApiKeyKind.Guid;

    /// <summary>Gets whether this key is a <see cref="Ulid"/> value.</summary>
    /// <value><see langword="true"/> if <see cref="ApiKind"/> is <see cref="ApiKeyKind.Ulid"/>; otherwise, <see langword="false"/>.</value>
    public readonly bool IsUlid => this.ApiKind == ApiKeyKind.Ulid;

    /// <summary>Gets whether this key is a <see cref="CultureInfo"/> value.</summary>
    /// <value><see langword="true"/> if <see cref="ApiKind"/> is <see cref="ApiKeyKind.Culture"/>; otherwise, <see langword="false"/>.</value>
    public readonly bool IsCulture => this.ApiKind == ApiKeyKind.Culture;

    /// <summary>Gets whether this key is a composite (multi-part) value.</summary>
    /// <value><see langword="true"/> if <see cref="ApiKind"/> is <see cref="ApiKeyKind.Composite"/>; otherwise, <see langword="false"/>.</value>
    /// <remarks>
    ///     Composite keys contain an array of <see cref="ApiKeyPart"/> elements, all of which must be scalar values.
    ///     Use <see cref="PartsAsSpan"/>, <see cref="PartsAsEnumerable"/>, or indexers to access individual parts.
    /// </remarks>
    public readonly bool IsComposite => this.ApiKind == ApiKeyKind.Composite;

    /// <summary>Gets whether this key is a scalar (single-value, non-composite) value.</summary>
    /// <value>
    ///     <see langword="true"/> if <see cref="ApiKind"/> is any scalar type (<see cref="ApiKeyKind.String"/>, <see cref="ApiKeyKind.Int32"/>,
    ///     <see cref="ApiKeyKind.Int64"/>, <see cref="ApiKeyKind.Guid"/>, <see cref="ApiKeyKind.Ulid"/>, <see cref="ApiKeyKind.Culture"/>);
    ///     otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    ///     Returns <see langword="false"/> for both <see cref="ApiKeyKind.Empty"/> and <see cref="ApiKeyKind.Composite"/>.
    ///     Scalar keys can be stored without allocation (for value types) or as a single reference (for <see cref="string"/>
    ///     and <see cref="CultureInfo"/>).
    /// </remarks>
    public readonly bool IsScalar => this.ApiKind != ApiKeyKind.Empty && this.ApiKind != ApiKeyKind.Composite;

    /// <summary>
    ///     Gets whether this is a named composite (all parts have non-null names).
    /// </summary>
    /// <value>
    ///     <see langword="true"/> if this is a composite where all parts have names; otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    ///     <para>
    ///         Named composites provide semantic meaning to each part (e.g., "CustomerId=42|OrderNumber=1001").
    ///         The framework enforces that all parts in a composite must be consistently named or unnamed; mixing is not allowed.
    ///     </para>
    ///     <para>
    ///         Returns <see langword="false"/> for non-composite keys and empty composites.
    ///     </para>
    /// </remarks>
    public readonly bool IsNamedComposite
    {
        get
        {
            if (!this.IsComposite)
            {
                return false;
            }

            var parts = (ApiKeyPart[])_referenceValue!;
            if (parts.Length == 0)
            {
                return false;
            }

            return string.IsNullOrWhiteSpace(parts[0].ApiName) is false;
        }
    }

    /// <summary>
    ///     Gets whether this is an ordered composite (all parts are unnamed/positional).
    /// </summary>
    /// <value>
    ///     <see langword="true"/> if this is a composite where all parts lack names; otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    ///     <para>
    ///         Ordered/positional composites rely on part sequence rather than names (e.g., "42|1001").
    ///         This format is more compact and efficient for performance-critical scenarios where semantic names are unnecessary.
    ///     </para>
    ///     <para>
    ///         The framework enforces that all parts in a composite must be consistently named or unnamed; mixing is not allowed.
    ///         Returns <see langword="false"/> for non-composite keys and empty composites.
    ///     </para>
    /// </remarks>
    public readonly bool IsOrderedComposite
    {
        get
        {
            if (!this.IsComposite)
            {
                return false;
            }

            var parts = (ApiKeyPart[])_referenceValue!;
            if (parts.Length == 0)
            {
                return false;
            }

            return string.IsNullOrWhiteSpace(parts[0].ApiName);
        }
    }

    /// <summary>
    ///     Gets the number of parts in this composite key.
    /// </summary>
    /// <value>
    ///     The number of <see cref="ApiKeyPart"/> elements if this is a composite; otherwise, 0.
    /// </value>
    /// <remarks>
    ///     For non-composite keys (scalars and <see cref="Empty"/>), this property returns 0.
    /// </remarks>
    public readonly int PartCount => this.IsComposite ? ((ApiKeyPart[])_referenceValue!).Length : 0;

    /// <summary>
    ///     Gets the parts of this composite key as a read-only span.
    /// </summary>
    /// <value>
    ///     A <see cref="ReadOnlySpan{T}"/> of <see cref="ApiKeyPart"/> if this is a composite; otherwise, an empty span.
    /// </value>
    /// <remarks>
    ///     Provides zero-allocation access to composite parts. Prefer this over <see cref="PartsAsEnumerable"/>
    ///     for performance-sensitive code.
    /// </remarks>
    public readonly ReadOnlySpan<ApiKeyPart> PartsAsSpan =>
        this.IsComposite ? ((ApiKeyPart[])_referenceValue!).AsSpan() : ReadOnlySpan<ApiKeyPart>.Empty;

    /// <summary>
    ///     Gets the parts of this composite key as an enumerable sequence.
    /// </summary>
    /// <value>
    ///     An <see cref="IEnumerable{T}"/> of <see cref="ApiKeyPart"/> if this is a composite; otherwise, an empty sequence.
    /// </value>
    /// <remarks>
    ///     Use <see cref="PartsAsSpan"/> instead when possible for better performance (zero allocation).
    /// </remarks>
    public readonly IEnumerable<ApiKeyPart> PartsAsEnumerable =>
         this.IsComposite ? EnumerateParts((ApiKeyPart[])_referenceValue!) : Enumerable.Empty<ApiKeyPart>();

    private static IEnumerable<ApiKeyPart> EnumerateParts(ApiKeyPart[] parts)
    {
        foreach (var part in parts)
        {
            yield return part;
        }
    }
    #endregion

    #region Constructors
    /// <summary>
    ///     Internal constructor used by factory methods to create a new <see cref="ApiKey"/> instance.
    /// </summary>
    /// <param name="kind">The discriminated kind.</param>
    /// <param name="valueUnion">Union value storage for value-type arms.</param>
    /// <param name="referenceValue">Reference storage for reference-type arms.</param>
    internal ApiKey(ApiKeyKind kind, in ApiKeyValueUnion valueUnion, object? referenceValue)
    {
        _valueUnion = valueUnion;
        _referenceValue = referenceValue;

        this.ApiKind = kind;

        // Pre-compute hash code for composites
        _hashCode = kind switch
        {
            ApiKeyKind.Composite => GetCompositeHash((ApiKeyPart[])referenceValue!),
            _ => 0  // Computed lazily for scalars
        };
    }
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates a composite key from an ordered sequence of part keys. Parts are treated as unnamed.
    /// </summary>
    /// <param name="apiKeyCollection">The ordered part keys.</param>
    /// <returns>The composite key or <see cref="Empty"/> if <paramref name="apiKeyCollection"/> is null or empty.</returns>
    public static ApiKey Composite(IEnumerable<ApiKey>? apiKeyCollection)
    {
        if (apiKeyCollection is null)
        {
            return Empty;
        }

        // Try fast path for common collection types
        if (apiKeyCollection is ICollection<ApiKey> collection)
        {
            if (collection.Count == 0)
            {
                return Empty;
            }

            var parts = new ApiKeyPart[collection.Count];
            var index = 0;
            foreach (var id in collection)
            {
                parts[index++] = new ApiKeyPart(null, id);
            }

            ValidateCompositeParts(parts);
            return new ApiKey(ApiKeyKind.Composite, default, parts);
        }

        // Slow path: materialize to list
        var list = new List<ApiKeyPart>();
        foreach (var id in apiKeyCollection)
        {
            list.Add(new ApiKeyPart(null, id));
        }

        if (list.Count == 0)
        {
            return Empty;
        }

        var partsArray = list.ToArray();
        ValidateCompositeParts(partsArray);

        return new ApiKey(ApiKeyKind.Composite, default, partsArray);
    }

    /// <summary>
    ///     Creates a composite key from ordered unnamed part keys (params overload).
    /// </summary>
    /// <param name="apiKeyArray">The ordered part keys.</param>
    /// <returns>The composite key or <see cref="Empty"/>.</returns>
    public static ApiKey Composite(params ApiKey[] apiKeyArray)
    {
        if (apiKeyArray is null || apiKeyArray.Length == 0)
        {
            return Empty;
        }

        var parts = new ApiKeyPart[apiKeyArray.Length];
        for (var i = 0; i < apiKeyArray.Length; i++)
        {
            parts[i] = new ApiKeyPart(null, apiKeyArray[i]);
        }

        ValidateCompositeParts(parts);

        return new ApiKey(ApiKeyKind.Composite, default, parts);
    }

    /// <summary>
    ///     Creates a composite key from part structures.
    /// </summary>
    /// <param name="apiKeyPartCollection">The sequence of parts.</param>
    /// <returns>The composite key or <see cref="Empty"/>.</returns>
    public static ApiKey Composite(IEnumerable<ApiKeyPart>? apiKeyPartCollection)
    {
        if (apiKeyPartCollection is null)
        {
            return Empty;
        }

        var clone = apiKeyPartCollection
            .Select(p => new ApiKeyPart(p.ApiName, p.ApiValue))
            .ToArray();

        if (clone.Length == 0)
        {
            return Empty;
        }

        ValidateCompositeParts(clone);

        return new ApiKey(ApiKeyKind.Composite, default, clone);
    }

    /// <summary>
    ///     Creates a composite key from parts (params overload).
    /// </summary>
    /// <param name="apiKeyPartArray">The sequence of parts.</param>
    /// <returns>The composite key or <see cref="Empty"/>.</returns>
    public static ApiKey Composite(params ApiKeyPart[] apiKeyPartArray)
    {
        if (apiKeyPartArray is null || apiKeyPartArray.Length == 0)
        {
            return Empty;
        }

        var clone = new ApiKeyPart[apiKeyPartArray.Length];
        for (var i = 0; i < apiKeyPartArray.Length; i++)
        {
            var part = apiKeyPartArray[i];
            clone[i] = new ApiKeyPart(part.ApiName, part.ApiValue);
        }

        ValidateCompositeParts(clone);

        return new ApiKey(ApiKeyKind.Composite, default, clone);
    }

    /// <summary>Creates a culture key from a <see cref="CultureInfo"/> instance.</summary>
    /// <param name="value">The culture value.</param>
    /// <returns>A new culture <see cref="ApiKey"/>.</returns>
    public static ApiKey FromCulture(CultureInfo value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        return new ApiKey(ApiKeyKind.Culture, default, value);
    }

    /// <summary>Creates a culture key from a culture name string.</summary>
    /// <param name="name">The culture name.</param>
    /// <returns>A new culture <see cref="ApiKey"/>.</returns>
    public static ApiKey FromCulture(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        return new ApiKey(ApiKeyKind.Culture, default, CultureInfo.GetCultureInfo(name, predefinedOnly: true));
    }

    /// <summary>Creates a string key.</summary>
    /// <param name="value">The string value.</param>
    /// <returns>A new string <see cref="ApiKey"/>.</returns>
    public static ApiKey FromString(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        return new ApiKey(ApiKeyKind.String, default, value);
    }

    /// <summary>Creates an Int32 key.</summary>
    /// <param name="value">The <see cref="int"/> value.</param>
    /// <returns>A new Int32 <see cref="ApiKey"/>.</returns>
    public static ApiKey FromInt32(int value) => new(ApiKeyKind.Int32, ApiKeyValueUnion.FromInt32(value), null);

    /// <summary>Creates an Int64 key.</summary>
    /// <param name="value">The <see cref="long"/> value.</param>
    /// <returns>A new Int64 <see cref="ApiKey"/>.</returns>
    public static ApiKey FromInt64(long value) => new(ApiKeyKind.Int64, ApiKeyValueUnion.FromInt64(value), null);

    /// <summary>Creates a Guid key.</summary>
    /// <param name="value">The <see cref="Guid"/> value.</param>
    /// <returns>A new Guid <see cref="ApiKey"/>.</returns>
    public static ApiKey FromGuid(Guid value) => new(ApiKeyKind.Guid, ApiKeyValueUnion.FromGuid(value), null);

    /// <summary>
    ///     Converts the specified <paramref name="value"/> into an <see cref="ApiKey"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="valueType">Optional type hint for the value. If null, uses <paramref name="value"/>.GetType().</param>
    /// <returns>The resulting <see cref="ApiKey"/>.</returns>
    /// <remarks>
    ///     Supports conversion from <see cref="string"/>, <see cref="int"/>, <see cref="long"/>, <see cref="Guid"/>, <see cref="Ulid"/>, and <see cref="CultureInfo"/>.
    ///     If the <paramref name="value"/> is already an <see cref="ApiKey"/>, it is returned as-is.
    ///     For other types, converts to string.
    /// </remarks>
    public static ApiKey FromObject(object value, Type? valueType = null)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        // Handle ApiKey passthrough
        if (value is ApiKey apiKey)
        {
            return apiKey;
        }

        // Determine the effective type
        var effectiveType = valueType ?? value.GetType();

        // Convert based on the effective type
        if (effectiveType == typeof(int))
        {
            return FromInt32((int)value);
        }

        if (effectiveType == typeof(long))
        {
            return FromInt64((long)value);
        }

        if (effectiveType == typeof(Guid))
        {
            return FromGuid((Guid)value);
        }

        if (effectiveType == typeof(Ulid))
        {
            return FromUlid((Ulid)value);
        }

        if (effectiveType == typeof(CultureInfo))
        {
            return FromCulture((CultureInfo)value);
        }

        if (effectiveType == typeof(string))
        {
            return FromString((string)value);
        }

        // Fallback: convert to string
        var stringValue = value.ToString();
        if (string.IsNullOrWhiteSpace(stringValue))
        {
            throw new ArgumentException("Value converted to null or empty string.", nameof(value));
        }

        return FromString(stringValue);
    }

    /// <summary>Creates a Ulid key.</summary>
    /// <param name="value">The <see cref="Ulid"/> value.</param>
    /// <returns>A new Ulid <see cref="ApiKey"/>.</returns>
    public static ApiKey FromUlid(Ulid value) => new(ApiKeyKind.Ulid, ApiKeyValueUnion.FromUlid(value), null);

    /// <summary>
    ///     Validates the supplied composite <paramref name="parts"/> ensuring no nested composites,
    ///     no mixing of named/unnamed, and uniqueness of names when named.
    /// </summary>
    /// <param name="parts">The parts to validate.</param>
    /// <exception cref="ApiKeyException">Thrown on invalid composite configuration.</exception>
    private static void ValidateCompositeParts(ApiKeyPart[] parts)
    {
        // No nested composites
        // No mixing (named <-> unnamed)
        var anyNamed = false;
        var anyUnnamed = false;
        foreach (var part in parts)
        {
            // Reject nested composites
            if (part.ApiValue.ApiKind == ApiKeyKind.Composite)
            {
                throw new ApiKeyException($"Nested composite parts are not allowed in {nameof(ApiKey)}.");
            }

            if (string.IsNullOrWhiteSpace(part.ApiName))
            {
                anyUnnamed = true;
            }
            else
            {
                anyNamed = true;
            }

            // Reject mixing of named/unnamed parts
            if (anyNamed && anyUnnamed)
            {
                throw new ApiKeyException($"Cannot mix named and unnamed parts in the same composite {nameof(ApiKey)}.");
            }
        }

        // At this point, the parts are all either named or unnamed.
        // If named, ensure uniqueness.
        if (anyNamed)
        {
            var duplicateNames = parts
                .GroupBy(p => p.ApiName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateNames.Count > 0)
            {
                // Reject duplicate names
                throw new ApiKeyException($"Duplicate part names in composite {nameof(ApiKey)}: [{duplicateNames.OrderBy(n => n).SafeToDelimitedString(',')}].");
            }
        }
    }
    #endregion

    #region Utility Methods
    /// <summary>
    ///     Gets the compatible scalar CLR types for <see cref="ApiKey"/> conversion.
    /// </summary>
    /// <returns>The compatible scalar CLR types for <see cref="ApiKey"/> conversion.</returns>
    public static Type[] GetCompatibleScalarTypes()
    {
        return [.. _scalarTypeKinds.Keys];
    }

    /// <summary>
    ///     Gets the compatible scalar CLR type for <see cref="ApiKey"/> conversion.
    ///     If the provided <paramref name="clrType"/> is already compatible, it is returned as-is; otherwise, returns <see cref="string"/> as a fallback.
    /// </summary>
    /// <param name="clrType">The CLR type to check.</param>
    /// <returns>The compatible scalar CLR type for <see cref="ApiKey"/> conversion.</returns>
    public static Type GetCompatibleScalarType(Type clrType)
    {
        ArgumentNullException.ThrowIfNull(clrType, nameof(clrType));

        // If already compatible, return as-is
        if (IsCompatibleScalarType(clrType))
        {
            return clrType;
        }

        // Default fallback to string for all other types
        return typeof(string);
    }

    /// <summary>
    ///    Determines whether the specified <see cref="Type"/> is a compatible scalar type for <see cref="ApiKey"/>.
    /// </summary>
    /// <param name="clrType">The type to check.</param>
    /// <returns>True if the type is a compatible scalar type; otherwise, false.</returns>
    public static bool IsCompatibleScalarType(Type clrType)
    {
        ArgumentNullException.ThrowIfNull(clrType, nameof(clrType));

        return _scalarTypeKinds.ContainsKey(clrType);
    }

    /// <summary>
    ///     Attempts to get the <see cref="ApiKeyKind"/> represented by a compatible scalar CLR type.
    /// </summary>
    /// <param name="clrType">The CLR type to check.</param>
    /// <param name="kind">The key kind associated with <paramref name="clrType"/>, when compatible.</param>
    /// <returns>True if the type is a compatible scalar type; otherwise, false.</returns>
    public static bool TryGetCompatibleScalarKind(Type clrType, out ApiKeyKind kind)
    {
        ArgumentNullException.ThrowIfNull(clrType, nameof(clrType));

        return _scalarTypeKinds.TryGetValue(clrType, out kind);
    }

    /// <summary>
    ///     Gets a string representation of composite <paramref name="parts"/> using '|' delimiters.
    /// </summary>
    /// <param name="parts">The composite parts.</param>
    /// <returns>The delimited string or null if <paramref name="parts"/> is null/empty.</returns>
    public static string? ToCompositeString(IEnumerable<ApiKeyPart>? parts)
    {
        if (parts is null)
        {
            return null;
        }

        var arr = parts as ApiKeyPart[] ?? [.. parts];
        if (arr.Length == 0)
        {
            return null;
        }

        // Fast path: single part
        if (arr.Length == 1)
        {
            return arr[0].ToString();
        }

        // Use string.Create for zero-allocation formatting (advanced)
        // OR simpler: pre-calculate size and use StringBuilder
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < arr.Length; i++)
        {
            if (i > 0)
            {
                stringBuilder.Append('|');
            }

            var part = arr[i];
            if (part.ApiName is not null)
            {
                stringBuilder.Append(part.ApiName);
                stringBuilder.Append('=');
            }
            stringBuilder.Append(part.ApiValue.ToString());
        }

        return stringBuilder.ToString();
    }
    #endregion

    #region AsOrThrow Methods
    /// <summary>Gets the string value or throws if the kind is not <see cref="ApiKeyKind.String"/>.</summary>
    public readonly string AsStringOrThrow() => this.ApiKind == ApiKeyKind.String ? (string)_referenceValue! : throw new ApiKeyException($"Kind {this.ApiKind} is not {nameof(ApiKeyKind.String)}.");

    /// <summary>Gets the Int32 value or throws if the kind is not <see cref="ApiKeyKind.Int32"/>.</summary>
    public readonly int AsInt32OrThrow() => this.ApiKind == ApiKeyKind.Int32 ? _valueUnion.Int32 : throw new ApiKeyException($"Kind {this.ApiKind} is not {nameof(ApiKeyKind.Int32)}.");

    /// <summary>Gets the Int64 value or throws if the kind is not <see cref="ApiKeyKind.Int64"/>.</summary>
    public readonly long AsInt64OrThrow() => this.ApiKind == ApiKeyKind.Int64 ? _valueUnion.Int64 : throw new ApiKeyException($"Kind {this.ApiKind} is not {nameof(ApiKeyKind.Int64)}.");

    /// <summary>Gets the Guid value or throws if the kind is not <see cref="ApiKeyKind.Guid"/>.</summary>
    public readonly Guid AsGuidOrThrow() => this.ApiKind == ApiKeyKind.Guid ? _valueUnion.Guid : throw new ApiKeyException($"Kind {this.ApiKind} is not {nameof(ApiKeyKind.Guid)}.");

    /// <summary>Gets the Ulid value or throws if the kind is not <see cref="ApiKeyKind.Ulid"/>.</summary>
    public readonly Ulid AsUlidOrThrow() => this.ApiKind == ApiKeyKind.Ulid ? _valueUnion.Ulid : throw new ApiKeyException($"Kind {this.ApiKind} is not {nameof(ApiKeyKind.Ulid)}.");

    /// <summary>Gets the culture value or throws if the kind is not <see cref="ApiKeyKind.Culture"/>.</summary>
    public readonly CultureInfo AsCultureOrThrow() => this.ApiKind == ApiKeyKind.Culture ? (CultureInfo)_referenceValue! : throw new ApiKeyException($"Kind {this.ApiKind} is not {nameof(ApiKeyKind.Culture)}.");
    #endregion

    #region TryGet Methods
    /// <summary>Attempts to get the string value if kind is <see cref="ApiKeyKind.String"/>.</summary>
    /// <param name="value">The output value or null.</param>
    /// <returns>True if successful.</returns>
    public readonly bool TryGet([NotNullWhen(true)] out string? value) { value = this.ApiKind == ApiKeyKind.String ? (string?)_referenceValue : null; return this.ApiKind == ApiKeyKind.String; }

    /// <summary>Attempts to get the Int32 value if kind is <see cref="ApiKeyKind.Int32"/>.</summary>
    /// <param name="value">The output Int32 value, or the default of <see langword="int"/> if the kind is not <see cref="ApiKeyKind.Int32"/>.</param>
    /// <returns><see langword="true"/> if the kind is <see cref="ApiKeyKind.Int32"/>; otherwise <see langword="false"/>.</returns>
    public readonly bool TryGet(out int value) { value = _valueUnion.Int32; return this.ApiKind == ApiKeyKind.Int32; }

    /// <summary>Attempts to get the Int64 value if kind is <see cref="ApiKeyKind.Int64"/>.</summary>
    /// <param name="value">The output Int64 value, or the default of <see langword="long"/> if the kind is not <see cref="ApiKeyKind.Int64"/>.</param>
    /// <returns><see langword="true"/> if the kind is <see cref="ApiKeyKind.Int64"/>; otherwise <see langword="false"/>.</returns>
    public readonly bool TryGet(out long value) { value = _valueUnion.Int64; return this.ApiKind == ApiKeyKind.Int64; }

    /// <summary>Attempts to get the Guid value if kind is <see cref="ApiKeyKind.Guid"/>.</summary>
    /// <param name="value">The output <see cref="Guid"/> value, or <see cref="Guid.Empty"/> if the kind is not <see cref="ApiKeyKind.Guid"/>.</param>
    /// <returns><see langword="true"/> if the kind is <see cref="ApiKeyKind.Guid"/>; otherwise <see langword="false"/>.</returns>
    public readonly bool TryGet(out Guid value) { value = _valueUnion.Guid; return this.ApiKind == ApiKeyKind.Guid; }

    /// <summary>Attempts to get the Ulid value if kind is <see cref="ApiKeyKind.Ulid"/>.</summary>
    /// <param name="value">The output <see cref="Ulid"/> value, or <see cref="Ulid.Empty"/> if the kind is not <see cref="ApiKeyKind.Ulid"/>.</param>
    /// <returns><see langword="true"/> if the kind is <see cref="ApiKeyKind.Ulid"/>; otherwise <see langword="false"/>.</returns>
    public readonly bool TryGet(out Ulid value) { value = _valueUnion.Ulid; return this.ApiKind == ApiKeyKind.Ulid; }

    /// <summary>Attempts to get the culture value if kind is <see cref="ApiKeyKind.Culture"/>.</summary>
    /// <param name="value">The output <see cref="CultureInfo"/> value, or <see langword="null"/> if the kind is not <see cref="ApiKeyKind.Culture"/>.</param>
    /// <returns><see langword="true"/> if the kind is <see cref="ApiKeyKind.Culture"/>; otherwise <see langword="false"/>.</returns>
    public readonly bool TryGet([NotNullWhen(true)] out CultureInfo? value) { value = this.ApiKind == ApiKeyKind.Culture ? (CultureInfo?)_referenceValue : null; return this.ApiKind == ApiKeyKind.Culture; }

    /// <summary>
    ///     Attempts to retrieve a part key by <paramref name="apiName"/> from a named composite.
    /// </summary>
    /// <param name="apiName">The part API name.</param>
    /// <param name="apiValue">Outputs the matching part value if found; otherwise default.</param>
    /// <returns>True if the named part was found.</returns>
    public readonly bool TryGetPart(string apiName, out ApiKey apiValue)
    {
        if (!this.IsComposite)
        {
            apiValue = default;
            return false;
        }
        foreach (var part in (ApiKeyPart[])_referenceValue!)
        {
            if (string.Equals(part.ApiName, apiName, StringComparison.Ordinal))
            {
                apiValue = part.ApiValue;
                return true;
            }
        }
        apiValue = default;
        return false;
    }
    #endregion

    #region Formatting
    /// <summary>
    ///     Returns a canonical string representation of the key or null if empty.
    /// </summary>
    public override readonly string? ToString()
    {
        var result = this.ApiKind switch
        {
            ApiKeyKind.Empty => string.Empty,
            ApiKeyKind.String => (string?)_referenceValue,
            ApiKeyKind.Int32 => _valueUnion.Int32.ToString(CultureInfo.InvariantCulture),
            ApiKeyKind.Int64 => _valueUnion.Int64.ToString(CultureInfo.InvariantCulture),
            ApiKeyKind.Guid => _valueUnion.Guid.ToString("D"),
            ApiKeyKind.Ulid => _valueUnion.Ulid.ToString(),
            ApiKeyKind.Culture => ((CultureInfo)_referenceValue!).Name,
            ApiKeyKind.Composite => ToCompositeString((ApiKeyPart[])_referenceValue!),
            _ => (string?)_referenceValue
        };

        return result.SafeToString();
    }

    /// <summary>
    ///    Formats the key using the specified format and format provider.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>The formatted string.</returns>
    public readonly string ToString(string? format, IFormatProvider? formatProvider)
    {
        var provider = formatProvider ?? CultureInfo.InvariantCulture;
        var result = this.ApiKind switch
        {
            ApiKeyKind.Empty => string.Empty,
            ApiKeyKind.String => (string?)_referenceValue,
            ApiKeyKind.Int32 => _valueUnion.Int32.ToString(format, provider),
            ApiKeyKind.Int64 => _valueUnion.Int64.ToString(format, provider),
            ApiKeyKind.Guid => string.IsNullOrEmpty(format) ? _valueUnion.Guid.ToString("D") : _valueUnion.Guid.ToString(format),
            ApiKeyKind.Ulid => _valueUnion.Ulid.ToString(),
            ApiKeyKind.Culture => ((CultureInfo)_referenceValue!).Name,
            ApiKeyKind.Composite => ToCompositeString((ApiKeyPart[])_referenceValue!),
            _ => (string?)_referenceValue
        };

        return result.SafeToString();
    }

    /// <summary>
    ///     Attempts to format the key into the specified span.
    /// </summary>
    /// <param name="destination">The destination span.</param>
    /// <param name="charsWritten">The number of characters written.</param>
    /// <param name="format">The format string.</param>
    /// <param name="provider">The format provider.</param>
    /// <returns>True if formatting succeeded.</returns>
    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        {
            var providerEffective = provider ?? CultureInfo.InvariantCulture;
            // Convert the incoming format span to a string (null if empty) and reuse the existing ToString(format, provider) logic.
            var fmt = format.Length == 0 ? null : format.ToString();
            var formattedText = this.ToString(fmt, providerEffective);

            if (string.IsNullOrEmpty(formattedText))
            {
                charsWritten = 0;
                return true;
            }

            if (formattedText.AsSpan().TryCopyTo(destination))
            {
                charsWritten = formattedText.Length;
                return true;
            }

            charsWritten = 0;
            return false;
        }
    }

    /// <summary>Returns a debugger-friendly display string.</summary>
    internal readonly string ToDebuggerDisplay() => this.HasValue ? $"{this.ApiKind}:{this}" : EmptyDisplayText;
    #endregion

    #region Parsing
    static ApiKey IParsable<ApiKey>.Parse(string text, IFormatProvider? provider)
    {
        if (TryParse(text, provider, out var apiKey))
        {
            return apiKey;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ApiKeyException("Text is null/empty.");
        }

        throw new ApiKeyException($"Text '{text}' is not a valid {nameof(ApiKey)}.");
    }

    static ApiKey ISpanParsable<ApiKey>.Parse(ReadOnlySpan<char> text, IFormatProvider? provider)
    {
        if (TryParse(text, provider, out var apiKey))
        {
            return apiKey;
        }

        if (text.IsEmpty)
        {
            throw new ApiKeyException("Text is null/empty.");
        }

        throw new ApiKeyException($"Text '{text}' is not a valid {nameof(ApiKey)}.");
    }

    /// <summary>
    ///     Attempts to parse <paramref name="text"/> into an key of the specified <paramref name="kind"/>.
    /// </summary>
    /// <param name="kind">The expected kind.</param>
    /// <param name="text">The textual representation.</param>
    /// <param name="apiKey">Outputs the parsed key on success; otherwise empty.</param>
    /// <returns>True if parsing succeeded.</returns>
    public static bool TryParse(ApiKeyKind kind, string? text, out ApiKey apiKey)
    {
        if (kind == ApiKeyKind.Composite)
        {
            apiKey = default;
            return false;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            apiKey = default;
            return false;
        }

        switch (kind)
        {
            case ApiKeyKind.String: return TryParseString(text, out apiKey);
            case ApiKeyKind.Int32: return TryParseInt32(text, out apiKey);
            case ApiKeyKind.Int64: return TryParseInt64(text, out apiKey);
            case ApiKeyKind.Guid: return TryParseGuid(text, out apiKey);
            case ApiKeyKind.Ulid: return TryParseUlid(text, out apiKey);
            case ApiKeyKind.Culture: return TryParseCulture(text, out apiKey);
        }

        apiKey = default;
        return false;
    }

    /// <summary>
    ///     Attempts to parse <paramref name="text"/> into an key matching the specified compatible scalar CLR type.
    /// </summary>
    /// <param name="clrType">The expected scalar CLR type.</param>
    /// <param name="text">The textual representation.</param>
    /// <param name="apiKey">Outputs the parsed key on success; otherwise empty.</param>
    /// <returns>True if the type is compatible and parsing succeeded.</returns>
    public static bool TryParse(Type clrType, string? text, out ApiKey apiKey)
    {
        if (TryGetCompatibleScalarKind(clrType, out var kind) is false)
        {
            apiKey = default;
            return false;
        }

        return TryParse(kind, text, out apiKey);
    }

    /// <summary>
    ///     Attempts to parse <paramref name="text"/> inferring the key kind heuristically.
    /// </summary>
    /// <param name="text">The textual representation.</param>
    /// <param name="apiKey">Outputs the parsed key on success.</param>
    /// <returns>True if parsing succeeded.</returns>
    public static bool TryParse(string? text, out ApiKey apiKey)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            apiKey = default;
            return false;
        }

        if (TryParseInt32(text, out apiKey)) { return true; }
        if (TryParseInt64(text, out apiKey)) { return true; }
        if (TryParseGuid(text, out apiKey)) { return true; }
        if (TryParseUlid(text, out apiKey)) { return true; }
        if (TryParseCulture(text, out apiKey)) { return true; }

        apiKey = FromString(text);
        return true;
    }

    /// <summary>
    ///     Attempts to parse <paramref name="text"/> into an <see cref="ApiKey"/>.
    /// </summary>
    /// <param name="text">The string to parse.</param>
    /// <param name="provider">Format provider (currently unused; parsing is culture-invariant).</param>
    /// <param name="result">Outputs the parsed key on success; otherwise <see cref="Empty"/>.</param>
    /// <returns><see langword="true"/> if parsing succeeded; otherwise <see langword="false"/>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? text, IFormatProvider? provider, [MaybeNullWhen(false)] out ApiKey result)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            result = default;
            return false;
        }

        // Provider is currently unused (parsing is culture-invariant/heuristic).
        return TryParse(text, out result);
    }

    /// <summary>
    ///     Attempts to parse <paramref name="text"/> into an <see cref="ApiKey"/>.
    /// </summary>
    /// <param name="text">The span to parse.</param>
    /// <param name="provider">Format provider (currently unused; parsing is culture-invariant).</param>
    /// <param name="result">Outputs the parsed key on success; otherwise <see cref="Empty"/>.</param>
    /// <returns><see langword="true"/> if parsing succeeded; otherwise <see langword="false"/>.</returns>
    public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, [MaybeNullWhen(false)] out ApiKey result)
    {
        if (text.IsEmpty)
        {
            result = default;
            return false;
        }

        // Convert span to string and reuse existing string-based parsing.
        return TryParse(text.ToString(), provider, out result);
    }

    /// <summary>Attempts to parse a culture key.</summary>
    private static bool TryParseCulture(string text, out ApiKey apiKey)
    {
        try
        {
            var cultureInfo = CultureInfo.GetCultureInfo(text, predefinedOnly: true);
            apiKey = FromCulture(cultureInfo);
            return true;
        }
        catch
        {
            apiKey = default;
            return false;
        }
    }

    /// <summary>Attempts to parse a Guid key.</summary>
    private static bool TryParseGuid(string text, out ApiKey apiKey)
    {
        if (Guid.TryParse(text, out var guid))
        {
            apiKey = FromGuid(guid);
            return true;
        }

        apiKey = default;
        return false;
    }

    /// <summary>Attempts to parse an Int32 key.</summary>
    private static bool TryParseInt32(string text, out ApiKey apiKey)
    {
        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i32))
        {
            apiKey = FromInt32(i32);
            return true;
        }

        apiKey = default;
        return false;
    }

    /// <summary>Attempts to parse an Int64 key.</summary>
    private static bool TryParseInt64(string text, out ApiKey apiKey)
    {
        if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i64))
        {
            apiKey = FromInt64(i64);
            return true;
        }

        apiKey = default;
        return false;
    }

    /// <summary>Attempts to parse a string key.</summary>
    private static bool TryParseString(string text, out ApiKey apiKey)
    {
        apiKey = FromString(text);
        return true;
    }

    /// <summary>Attempts to parse a Ulid key.</summary>
    private static bool TryParseUlid(string text, out ApiKey apiKey)
    {
        if (Ulid.TryParse(text, out var ulid))
        {
            apiKey = FromUlid(ulid);
            return true;
        }

        apiKey = default;
        return false;
    }
    #endregion

    #region Equality/Ordering Methods
    /// <summary>
    ///     Determines whether this instance is equal to <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The other key.</param>
    /// <returns>True if equal.</returns>
    public readonly bool Equals(ApiKey other)
    {
        if (this.ApiKind != other.ApiKind)
        {
            return false;
        }

        return this.ApiKind switch
        {
            ApiKeyKind.Empty => true,
            ApiKeyKind.String => string.Equals((string?)_referenceValue, (string?)other._referenceValue, StringComparison.Ordinal),
            ApiKeyKind.Int32 => _valueUnion.Int32 == other._valueUnion.Int32,
            ApiKeyKind.Int64 => _valueUnion.Int64 == other._valueUnion.Int64,
            ApiKeyKind.Guid => _valueUnion.Guid == other._valueUnion.Guid,
            ApiKeyKind.Ulid => _valueUnion.Ulid == other._valueUnion.Ulid,
            ApiKeyKind.Culture => string.Equals(((CultureInfo)_referenceValue!).Name, ((CultureInfo)other._referenceValue!).Name, StringComparison.OrdinalIgnoreCase),
            ApiKeyKind.Composite => ReferenceEquals(_referenceValue, other._referenceValue) || PartsEqual((ApiKeyPart[])_referenceValue!, (ApiKeyPart[])other._referenceValue!),
            _ => false
        };
    }
    /// <summary>
    ///     Compares two composite part arrays for structural equality.
    /// </summary>
    private static bool PartsEqual(ApiKeyPart[] leftParts, ApiKeyPart[] rightParts)
    {
        if (ReferenceEquals(leftParts, rightParts))
        {
            return true;
        }

        if (leftParts.Length != rightParts.Length)
        {
            return false;
        }

        for (var i = 0; i < leftParts.Length; i++)
        {
            if (!string.Equals(leftParts[i].ApiName, rightParts[i].ApiName, StringComparison.Ordinal))
            {
                return false;
            }

            if (!leftParts[i].ApiValue.Equals(rightParts[i].ApiValue))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>Determines whether this instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is ApiKey other && this.Equals(other);

    /// <summary>Returns a hash code for this key.</summary>
    public override readonly int GetHashCode()
    {
        // Composite: Use cached hash code from constructor
        if (this.ApiKind == ApiKeyKind.Composite)
        {
            return _hashCode;
        }

        // Scalars: Compute inline
        return this.ApiKind switch
        {
            ApiKeyKind.Empty => 0,
            ApiKeyKind.String => HashCode.Combine((int)this.ApiKind, (string?)_referenceValue),
            ApiKeyKind.Int32 => HashCode.Combine((int)this.ApiKind, _valueUnion.Int32),
            ApiKeyKind.Int64 => HashCode.Combine((int)this.ApiKind, _valueUnion.Int64),
            ApiKeyKind.Guid => HashCode.Combine((int)this.ApiKind, _valueUnion.Guid),
            ApiKeyKind.Ulid => HashCode.Combine((int)this.ApiKind, _valueUnion.Ulid),
            ApiKeyKind.Culture => HashCode.Combine((int)this.ApiKind, ((CultureInfo?)_referenceValue)?.Name?.ToUpperInvariant()),
            _ => (int)this.ApiKind
        };
    }

    /// <summary>Computes a hash code for a composite parts array.</summary>
    private static int GetCompositeHash(ApiKeyPart[] parts)
    {
        var hashCode = new HashCode();
        hashCode.Add((int)ApiKeyKind.Composite);
        foreach (var part in parts)
        {
            hashCode.Add(part.ApiName, StringComparer.Ordinal);
            hashCode.Add(part.ApiValue);
        }
        return hashCode.ToHashCode();
    }

    /// <summary>
    ///     Compares this instance to <paramref name="other"/> for ordering.
    /// </summary>
    /// <param name="other">The other key.</param>
    /// <returns>Negative, zero, or positive depending on ordering.</returns>
    public readonly int CompareTo(ApiKey other)
    {
        var kindCmp = this.ApiKind.CompareTo(other.ApiKind);
        if (kindCmp != 0)
        {
            return Math.Sign(kindCmp);
        }

        return this.ApiKind switch
        {
            ApiKeyKind.Empty => 0,
            ApiKeyKind.String => Math.Sign(string.Compare((string?)_referenceValue, (string?)other._referenceValue, StringComparison.Ordinal)),
            ApiKeyKind.Int32 => _valueUnion.Int32.CompareTo(other._valueUnion.Int32),
            ApiKeyKind.Int64 => _valueUnion.Int64.CompareTo(other._valueUnion.Int64),
            ApiKeyKind.Guid => _valueUnion.Guid.CompareTo(other._valueUnion.Guid),
            ApiKeyKind.Ulid => _valueUnion.Ulid.CompareTo(other._valueUnion.Ulid),
            ApiKeyKind.Culture => Math.Sign(string.Compare(((CultureInfo)_referenceValue!).Name, ((CultureInfo)other._referenceValue!).Name, StringComparison.OrdinalIgnoreCase)),
            ApiKeyKind.Composite => CompareParts((ApiKeyPart[])_referenceValue!, (ApiKeyPart[])other._referenceValue!),
            _ => 0
        };
    }

    /// <summary>Compares two composite part arrays for ordering.</summary>
    private static int CompareParts(ApiKeyPart[] leftParts, ApiKeyPart[] rightParts)
    {
        var lengthComparison = leftParts.Length.CompareTo(rightParts.Length);
        if (lengthComparison != 0)
        {
            return lengthComparison;
        }

        for (var i = 0; i < leftParts.Length; i++)
        {
            var apiNameComparison = Math.Sign(string.Compare(leftParts[i].ApiName, rightParts[i].ApiName, StringComparison.Ordinal));
            if (apiNameComparison != 0)
            {
                return apiNameComparison;
            }

            var apiValueComparison = leftParts[i].ApiValue.CompareTo(rightParts[i].ApiValue);
            if (apiValueComparison != 0)
            {
                return apiValueComparison;
            }
        }
        return 0;
    }
    #endregion

    #region Index Operators
    /// <summary>
    ///     Gets the part key at the specified <paramref name="index"/> in a composite.
    /// </summary>
    /// <param name="index">The zero-based part index.</param>
    /// <returns>The part key.</returns>
    /// <exception cref="ApiKeyException">Thrown if not a composite.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if index is out of range.</exception>
    public readonly ApiKey this[int index]
    {
        get
        {
            if (!this.IsComposite)
            {
                throw new ApiKeyException($"Indexing only applies to composite {nameof(ApiKey)}.");
            }

            var parts = (ApiKeyPart[])_referenceValue!;
            if ((uint)index >= (uint)parts.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Index out of range. Valid range: [0..{parts.Length - 1}].");
            }

            return parts[index].ApiValue;
        }
    }

    /// <summary>
    ///     Gets the named part key from a named composite.
    /// </summary>
    /// <param name="apiName">The part API name.</param>
    /// <returns>The part key.</returns>
    /// <exception cref="ApiKeyException">Thrown if part name not found.</exception>
    public readonly ApiKey this[string apiName] => this.TryGetPart(apiName, out var apiValue) ? apiValue : throw new ApiKeyException($"Part name '{apiName}' not found in composite {nameof(ApiKey)}.");
    #endregion

    #region Equality/Ordering Operators
    /// <summary>Equality operator that compares two <see cref="ApiKey"/> instances using <see cref="Equals(ApiKey)"/>.</summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True if <paramref name="left"/> equals <paramref name="right"/>.</returns>
    public static bool operator ==(ApiKey left, ApiKey right) => left.Equals(right);

    /// <summary>Inequality operator that compares two <see cref="ApiKey"/> instances using <see cref="Equals(ApiKey)"/>.</summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True if <paramref name="left"/> does not equal <paramref name="right"/>.</returns>
    public static bool operator !=(ApiKey left, ApiKey right) => !left.Equals(right);

    /// <summary>Less-than operator that compares two <see cref="ApiKey"/> instances using <see cref="CompareTo(ApiKey)"/>.</summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
    public static bool operator <(ApiKey left, ApiKey right) => left.CompareTo(right) < 0;

    /// <summary>Less-than-or-equal operator that compares two <see cref="ApiKey"/> instances using <see cref="CompareTo(ApiKey)"/>.</summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
    public static bool operator <=(ApiKey left, ApiKey right) => left.CompareTo(right) <= 0;

    /// <summary>Greater-than operator that compares two <see cref="ApiKey"/> instances using <see cref="CompareTo(ApiKey)"/>.</summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
    public static bool operator >(ApiKey left, ApiKey right) => left.CompareTo(right) > 0;

    /// <summary>Greater-than-or-equal operator that compares two <see cref="ApiKey"/> instances using <see cref="CompareTo(ApiKey)"/>.</summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
    public static bool operator >=(ApiKey left, ApiKey right) => left.CompareTo(right) >= 0;
    #endregion

    #region Conversion Operators
    /// <summary>Implicit conversion from <see cref="string"/> to <see cref="ApiKey"/>.</summary>
    public static implicit operator ApiKey(string value) => FromString(value);

    /// <summary>Implicit conversion from <see cref="int"/> to <see cref="ApiKey"/>.</summary>
    public static implicit operator ApiKey(int value) => FromInt32(value);

    /// <summary>Implicit conversion from <see cref="long"/> to <see cref="ApiKey"/>.</summary>
    public static implicit operator ApiKey(long value) => FromInt64(value);

    /// <summary>Implicit conversion from <see cref="Guid"/> to <see cref="ApiKey"/>.</summary>
    public static implicit operator ApiKey(Guid value) => FromGuid(value);

    /// <summary>Implicit conversion from <see cref="Ulid"/> to <see cref="ApiKey"/>.</summary>
    public static implicit operator ApiKey(Ulid value) => FromUlid(value);

    /// <summary>Implicit conversion from <see cref="CultureInfo"/> to <see cref="ApiKey"/>.</summary>
    public static implicit operator ApiKey(CultureInfo value) => FromCulture(value);

    /// <summary>Explicit conversion to <see cref="string"/> returns null if not a string key.</summary>
    public static explicit operator string?(ApiKey apiKey) => apiKey.ApiKind == ApiKeyKind.String ? (string?)apiKey._referenceValue : null;

    /// <summary>Explicit conversion to <see cref="int"/>; throws if kind mismatch.</summary>
    public static explicit operator int(ApiKey apiKey) => apiKey.AsInt32OrThrow();

    /// <summary>Explicit conversion to <see cref="long"/>; throws if kind mismatch.</summary>
    public static explicit operator long(ApiKey apiKey) => apiKey.AsInt64OrThrow();

    /// <summary>Explicit conversion to <see cref="Guid"/>; throws if kind mismatch.</summary>
    public static explicit operator Guid(ApiKey apiKey) => apiKey.AsGuidOrThrow();

    /// <summary>Explicit conversion to <see cref="Ulid"/>; throws if kind mismatch.</summary>
    public static explicit operator Ulid(ApiKey apiKey) => apiKey.AsUlidOrThrow();

    /// <summary>Explicit conversion to <see cref="CultureInfo"/> returns null if not a culture key.</summary>
    public static explicit operator CultureInfo?(ApiKey apiKey) => apiKey.ApiKind == ApiKeyKind.Culture ? (CultureInfo?)apiKey._referenceValue : null;
    #endregion
}
