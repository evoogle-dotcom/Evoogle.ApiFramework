// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity.Internal;
using Evoogle.ApiFramework.Identity.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     ApiId is a compact, tagged-discriminated union for identifiers.
///     ApiId is immutable and thread-safe.
///     Scalars are allocation-free; composites allocate an array of parts.
///     Uses a safe value-type union for primitives and a single reference slot for reference arms.
/// </summary>
/// <remarks>
///     TODO: Use coercion library in core framework for conversions between coercible types (e.g., string to CultureInfo, Int32 to Int64).
/// </remarks>
[DebuggerDisplay("{ToDebuggerDisplay(),nq}")]
[JsonConverter(typeof(ApiIdJsonConverter))]
public readonly struct ApiId : IEquatable<ApiId>, IComparable<ApiId>
{
    #region Fields
    /// <summary>
    ///     Represents the empty identifier (no value). Equivalent to default(<see cref="ApiId"/>).
    /// </summary>
    public static readonly ApiId Empty = default;

    // Union layout:
    //  - _val: explicit-layout union for value-type arms (int, long, Guid, Ulid)
    //  - _ref: single reference slot for reference arms (string, CultureInfo, ApiIdPart[])
    private readonly ApiIdValueUnion _val;
    private readonly object? _ref;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the discriminated <see cref="ApiIdKind"/> for this identifier instance.
    /// </summary>
    public ApiIdKind Kind { get; }

    /// <summary>Optional original text (diagnostics/round-tripping only; not used for equality).</summary>
    public string? OriginalString { get; }
    #endregion

    #region Computed Properties
    /// <summary>
    ///     Gets a value indicating whether this identifier represents a non-empty value.
    /// </summary>
    public bool HasValue => this.Kind != ApiIdKind.None;

    /// <summary>True if this identifier is a <see cref="string"/> value.</summary>
    public bool IsString => this.Kind == ApiIdKind.String;
    /// <summary>True if this identifier is an <see cref="int"/> value.</summary>
    public bool IsInt32 => this.Kind == ApiIdKind.Int32;
    /// <summary>True if this identifier is a <see cref="long"/> value.</summary>
    public bool IsInt64 => this.Kind == ApiIdKind.Int64;
    /// <summary>True if this identifier is a <see cref="Guid"/> value.</summary>
    public bool IsGuid => this.Kind == ApiIdKind.Guid;
    /// <summary>True if this identifier is a <see cref="Ulid"/> value.</summary>
    public bool IsUlid => this.Kind == ApiIdKind.Ulid;
    /// <summary>True if this identifier is a <see cref="CultureInfo"/> value.</summary>
    public bool IsCulture => this.Kind == ApiIdKind.Culture;
    /// <summary>True if this identifier is a composite value made up of parts.</summary>
    public bool IsComposite => this.Kind == ApiIdKind.Composite;
    /// <summary>True if this identifier is a scalar (non-composite, non-empty) value.</summary>
    public bool IsScalar => this.Kind != ApiIdKind.None && this.Kind != ApiIdKind.Composite;

    /// <summary>
    ///     True if this composite was built with all <see cref="ApiIdPart.Name"/> non-null.
    ///     Invariants guarantee there is no mixing; for non-composites returns false.
    /// </summary>
    public bool IsNamedComposite
    {
        get
        {
            if (!this.IsComposite)
            {
                return false;
            }

            var parts = (ApiIdPart[])_ref!;
            if (parts.Length == 0)
            {
                return false;
            }

            return string.IsNullOrWhiteSpace(parts[0].Name) is false;
        }
    }

    /// <summary>
    ///     True if this composite was built with all <see cref="ApiIdPart.Name"/> null (positional/ordered).
    ///     Invariants guarantee there is no mixing; for non-composites returns false.
    /// </summary>
    public bool IsOrderedComposite
    {
        get
        {
            if (!this.IsComposite)
            {
                return false;
            }

            var parts = (ApiIdPart[])_ref!;
            if (parts.Length == 0)
            {
                return false;
            }

            return string.IsNullOrWhiteSpace(parts[0].Name);
        }
    }

    /// <summary>
    ///     Gets the number of component parts. For composites this is the part count; for scalars this is 1; for empty identifiers 0.
    /// </summary>
    public int PartCount => this.IsComposite ? ((ApiIdPart[])_ref!).Length : (this.HasValue ? 1 : 0);
    /// <summary>
    ///     Gets a span of the composite parts, or an empty span if not composite.
    /// </summary>
    public ReadOnlySpan<ApiIdPart> Parts => this.IsComposite ? ((ApiIdPart[])_ref!).AsSpan() : ReadOnlySpan<ApiIdPart>.Empty;
    /// <summary>
    ///     Gets the <see cref="ApiId"/> value of the part at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">Zero-based index of the part.</param>
    /// <returns>The part value at the specified index.</returns>
    /// <exception cref="ApiIdentityException">Thrown if this identifier is not composite.</exception>
    public ApiId this[int index] => this.IsComposite ? ((ApiIdPart[])_ref!)[index].Value : throw new ApiIdentityException("Not composite.");
    #endregion

    #region Constructors
    /// <summary>
    ///     Internal constructor used by factory methods to create a new <see cref="ApiId"/> instance.
    /// </summary>
    /// <param name="kind">The discriminated kind.</param>
    /// <param name="val">Union value storage for value-type arms.</param>
    /// <param name="reference">Reference storage for reference-type arms.</param>
    /// <param name="original">Original string representation (optional).</param>
    internal ApiId(ApiIdKind kind, in ApiIdValueUnion val, object? reference, string? original)
    {
        _val = val;
        _ref = reference;

        this.Kind = kind;
        this.OriginalString = original;
    }
    #endregion

    #region Factory Methods
    /// <summary>Creates a culture identifier from a <see cref="CultureInfo"/> instance.</summary>
    /// <param name="value">The culture value.</param>
    /// <returns>A new culture <see cref="ApiId"/>.</returns>
    public static ApiId FromCulture(CultureInfo value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        return new ApiId(ApiIdKind.Culture, default, value, value.Name);
    }

    /// <summary>Creates a culture identifier from a culture name string.</summary>
    /// <param name="name">The culture name.</param>
    /// <returns>A new culture <see cref="ApiId"/>.</returns>
    public static ApiId FromCulture(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        return new ApiId(ApiIdKind.Culture, default, CultureInfo.GetCultureInfo(name, predefinedOnly: true), name);
    }

    /// <summary>Creates a string identifier.</summary>
    /// <param name="value">The string value.</param>
    /// <returns>A new string <see cref="ApiId"/>.</returns>
    public static ApiId FromString(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        return new ApiId(ApiIdKind.String, default, value, value);
    }

    /// <summary>Creates an Int32 identifier.</summary>
    /// <param name="value">The <see cref="int"/> value.</param>
    /// <returns>A new Int32 <see cref="ApiId"/>.</returns>
    public static ApiId FromInt32(int value) => new(ApiIdKind.Int32, ApiIdValueUnion.FromInt32(value), null, value.ToString(CultureInfo.InvariantCulture));
    /// <summary>Creates an Int64 identifier.</summary>
    /// <param name="value">The <see cref="long"/> value.</param>
    /// <returns>A new Int64 <see cref="ApiId"/>.</returns>
    public static ApiId FromInt64(long value) => new(ApiIdKind.Int64, ApiIdValueUnion.FromInt64(value), null, value.ToString(CultureInfo.InvariantCulture));
    /// <summary>Creates a Guid identifier.</summary>
    /// <param name="value">The <see cref="Guid"/> value.</param>
    /// <returns>A new Guid <see cref="ApiId"/>.</returns>
    public static ApiId FromGuid(Guid value) => new(ApiIdKind.Guid, ApiIdValueUnion.FromGuid(value), null, value.ToString("D"));
    /// <summary>Creates a Ulid identifier.</summary>
    /// <param name="value">The <see cref="Ulid"/> value.</param>
    /// <returns>A new Ulid <see cref="ApiId"/>.</returns>
    public static ApiId FromUlid(Ulid value) => new(ApiIdKind.Ulid, ApiIdValueUnion.FromUlid(value), null, value.ToString());

    /// <summary>
    ///     Creates a composite identifier from an ordered sequence of part identifiers. Parts are treated as unnamed.
    /// </summary>
    /// <param name="orderedParts">The ordered part identifiers.</param>
    /// <returns>The composite identifier or <see cref="Empty"/> if <paramref name="orderedParts"/> is null or empty.</returns>
    public static ApiId Composite(IEnumerable<ApiId> orderedParts)
    {
        if (orderedParts is null || !orderedParts.Any())
        {
            return Empty;
        }

        var parts = orderedParts.Select(p => new ApiIdPart(null, p)).ToArray();

        ValidateCompositeParts(parts);

        return new ApiId(ApiIdKind.Composite, default, parts, CompositeString(parts));
    }

    /// <summary>
    ///     Creates a composite identifier from ordered unnamed part identifiers (params overload).
    /// </summary>
    /// <param name="orderedParts">The ordered part identifiers.</param>
    /// <returns>The composite identifier or <see cref="Empty"/>.</returns>
    public static ApiId Composite(params ApiId[]? orderedParts)
    {
        if (orderedParts is null || orderedParts.Length == 0)
        {
            return Empty;
        }

        var parts = new ApiIdPart[orderedParts.Length];
        for (var i = 0; i < orderedParts.Length; i++)
        {
            parts[i] = new ApiIdPart(null, orderedParts[i]);
        }

        ValidateCompositeParts(parts);

        return new ApiId(ApiIdKind.Composite, default, parts, CompositeString(parts));
    }

    /// <summary>
    ///     Creates a composite identifier from named part structures.
    /// </summary>
    /// <param name="namedParts">The sequence of named parts.</param>
    /// <returns>The composite identifier or <see cref="Empty"/>.</returns>
    public static ApiId Composite(IEnumerable<ApiIdPart>? namedParts)
    {
        if (namedParts is null || !namedParts.Any())
        {
            return Empty;
        }

        var clone = namedParts.Select(p => new ApiIdPart(p.Name, p.Value)).ToArray();

        ValidateCompositeParts(clone);

        return new ApiId(ApiIdKind.Composite, default, clone, CompositeString(clone));
    }

    /// <summary>
    ///     Creates a composite identifier from named parts (params overload).
    /// </summary>
    /// <param name="namedParts">The named parts.</param>
    /// <returns>The composite identifier or <see cref="Empty"/>.</returns>
    public static ApiId Composite(params ApiIdPart[]? namedParts)
    {
        if (namedParts is null || namedParts.Length == 0)
        {
            return Empty;
        }

        var clone = (ApiIdPart[])namedParts.Clone();

        ValidateCompositeParts(clone);

        return new ApiId(ApiIdKind.Composite, default, clone, CompositeString(clone));
    }

    /// <summary>
    ///     Gets a string representation of composite <paramref name="parts"/> using '|' delimiters.
    /// </summary>
    /// <param name="parts">The composite parts.</param>
    /// <returns>The delimited string or null if <paramref name="parts"/> is null/empty.</returns>
    public static string? CompositeString(IEnumerable<ApiIdPart>? parts)
    {
        if (parts is null || !parts.Any())
        {
            return null;
        }

        return string.Join("|", parts);
    }

    /// <summary>
    ///     Validates the supplied composite <paramref name="parts"/> ensuring no nested composites,
    ///     no mixing of named/unnamed, and uniqueness of names when named.
    /// </summary>
    /// <param name="parts">The parts to validate.</param>
    /// <exception cref="ApiIdentityException">Thrown on invalid composite configuration.</exception>
    private static void ValidateCompositeParts(ApiIdPart[] parts)
    {
        // No nested composites
        // No mixing (named <-> unnamed)
        var anyNamed = false;
        var anyUnnamed = false;
        foreach (var p in parts)
        {
            if (p.Value.Kind == ApiIdKind.Composite)
            {
                throw new ApiIdentityException($"Nested composite parts are not allowed in {nameof(ApiId)}.");
            }

            if (string.IsNullOrWhiteSpace(p.Name))
            {
                anyUnnamed = true;
            }
            else
            {
                anyNamed = true;
            }

            if (anyNamed && anyUnnamed)
            {
                throw new ApiIdentityException($"Cannot mix named and unnamed parts in the same composite {nameof(ApiId)}.");
            }
        }

        // At this point, the parts are all either named or unnamed.
        // If named, ensure uniqueness.
        if (anyNamed)
        {
            var duplicateNames = parts
                .GroupBy(p => p.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateNames.Count > 0)
            {
                throw new ApiIdentityException($"Duplicate part names in composite {nameof(ApiId)}: [{duplicateNames.OrderBy(n => n).SafeToDelimitedString(',')}].");
            }
        }
    }
    #endregion

    #region AsOrThrow Methods
    /// <summary>Gets the string value or throws if the kind is not <see cref="ApiIdKind.String"/>.</summary>
    /// <returns>The string value.</returns>
    /// <exception cref="ApiIdentityException">Thrown if kind mismatch.</exception>
    public string AsStringOrThrow() => this.Kind == ApiIdKind.String ? (string)_ref! : throw new ApiIdentityException($"Kind {this.Kind} is not a string.");

    /// <summary>Gets the Int32 value or throws if the kind is not <see cref="ApiIdKind.Int32"/>.</summary>
    public int AsInt32OrThrow() => this.Kind == ApiIdKind.Int32 ? _val.Int32 : throw new ApiIdentityException($"Kind {this.Kind} is not an Int32.");

    /// <summary>Gets the Int64 value or throws if the kind is not <see cref="ApiIdKind.Int64"/>.</summary>
    public long AsInt64OrThrow() => this.Kind == ApiIdKind.Int64 ? _val.Int64 : throw new ApiIdentityException($"Kind {this.Kind} is not an Int64.");

    /// <summary>Gets the Guid value or throws if the kind is not <see cref="ApiIdKind.Guid"/>.</summary>
    public Guid AsGuidOrThrow() => this.Kind == ApiIdKind.Guid ? _val.Guid : throw new ApiIdentityException($"Kind {this.Kind} is not a Guid.");

    /// <summary>Gets the Ulid value or throws if the kind is not <see cref="ApiIdKind.Ulid"/>.</summary>
    public Ulid AsUlidOrThrow() => this.Kind == ApiIdKind.Ulid ? _val.Ulid : throw new ApiIdentityException($"Kind {this.Kind} is not a Ulid.");

    /// <summary>Gets the culture value or throws if the kind is not <see cref="ApiIdKind.Culture"/>.</summary>
    public CultureInfo AsCultureOrThrow() => this.Kind == ApiIdKind.Culture ? (CultureInfo)_ref! : throw new ApiIdentityException($"Kind {this.Kind} is not a Culture.");
    #endregion

    #region TryGet Methods
    /// <summary>Attempts to get the string value if kind is <see cref="ApiIdKind.String"/>.</summary>
    /// <param name="value">The output value or null.</param>
    /// <returns>True if successful.</returns>
    public bool TryGet(out string? value) { value = this.Kind == ApiIdKind.String ? (string?)_ref : null; return this.Kind == ApiIdKind.String; }

    /// <summary>Attempts to get the Int32 value if kind is <see cref="ApiIdKind.Int32"/>.</summary>
    public bool TryGet(out int value) { value = _val.Int32; return this.Kind == ApiIdKind.Int32; }

    /// <summary>Attempts to get the Int64 value if kind is <see cref="ApiIdKind.Int64"/>.</summary>
    public bool TryGet(out long value) { value = _val.Int64; return this.Kind == ApiIdKind.Int64; }

    /// <summary>Attempts to get the Guid value if kind is <see cref="ApiIdKind.Guid"/>.</summary>
    public bool TryGet(out Guid value) { value = _val.Guid; return this.Kind == ApiIdKind.Guid; }

    /// <summary>Attempts to get the Ulid value if kind is <see cref="ApiIdKind.Ulid"/>.</summary>
    public bool TryGet(out Ulid value) { value = _val.Ulid; return this.Kind == ApiIdKind.Ulid; }

    /// <summary>Attempts to get the culture value if kind is <see cref="ApiIdKind.Culture"/>.</summary>
    public bool TryGet(out CultureInfo? value) { value = this.Kind == ApiIdKind.Culture ? (CultureInfo?)_ref : null; return this.Kind == ApiIdKind.Culture; }

    /// <summary>
    ///     Attempts to retrieve a part identifier by <paramref name="name"/> from a named composite.
    /// </summary>
    /// <param name="name">The part name.</param>
    /// <param name="value">Outputs the matching part value if found; otherwise default.</param>
    /// <returns>True if the named part was found.</returns>
    public bool TryGetPart(string name, out ApiId value)
    {
        if (!this.IsComposite)
        {
            value = default;
            return false;
        }
        foreach (var p in (ApiIdPart[])_ref!)
        {
            if (string.Equals(p.Name, name, StringComparison.Ordinal))
            {
                value = p.Value;
                return true;
            }
        }
        value = default;
        return false;
    }
    #endregion

    #region Parsing
    /// <summary>
    ///     Attempts to parse <paramref name="text"/> into an identifier of the specified <paramref name="kind"/>.
    /// </summary>
    /// <param name="kind">The expected kind.</param>
    /// <param name="text">The textual representation.</param>
    /// <param name="id">Outputs the parsed identifier on success; otherwise empty.</param>
    /// <returns>True if parsing succeeded.</returns>
    public static bool TryParse(ApiIdKind kind, string? text, out ApiId id)
    {
        if (kind == ApiIdKind.Composite)
        {
            id = default;
            return false;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            id = default;
            return false;
        }

        switch (kind)
        {
            case ApiIdKind.String: return TryParseString(text, out id);
            case ApiIdKind.Int32: return TryParseInt32(text, out id);
            case ApiIdKind.Int64: return TryParseInt64(text, out id);
            case ApiIdKind.Guid: return TryParseGuid(text, out id);
            case ApiIdKind.Ulid: return TryParseUlid(text, out id);
            case ApiIdKind.Culture: return TryParseCulture(text, out id);
        }

        id = default;
        return false;
    }

    /// <summary>
    ///     Attempts to parse <paramref name="text"/> inferring the identifier kind heuristically.
    /// </summary>
    /// <param name="text">The textual representation.</param>
    /// <param name="id">Outputs the parsed identifier on success.</param>
    /// <returns>True if parsing succeeded.</returns>
    public static bool TryParse(string? text, out ApiId id)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            id = default;
            return false;
        }

        if (TryParseInt32(text, out id)) { return true; }
        if (TryParseInt64(text, out id)) { return true; }
        if (TryParseGuid(text, out id)) { return true; }
        if (TryParseUlid(text, out id)) { return true; }
        if (TryParseCulture(text, out id)) { return true; }

        id = FromString(text);
        return true;
    }

    /// <summary>
    ///     Parses <paramref name="text"/> into the specified <paramref name="kind"/> or throws.
    /// </summary>
    /// <param name="kind">The expected kind.</param>
    /// <param name="text">The textual representation.</param>
    /// <returns>The parsed identifier.</returns>
    /// <exception cref="ApiIdentityException">Thrown if parsing fails.</exception>
    public static ApiId Parse(ApiIdKind kind, string text) => TryParse(kind, text, out var id) ? id : throw new ApiIdentityException($"Text '{text}' is not a valid {kind} id.");

    /// <summary>
    ///     Parses <paramref name="text"/> inferring identifier kind or throws.
    /// </summary>
    /// <param name="text">The textual representation.</param>
    /// <returns>The parsed identifier.</returns>
    /// <exception cref="ApiIdentityException">Thrown if text is null/empty or invalid.</exception>
    public static ApiId Parse(string text) => TryParse(text, out var id) ? id : throw new ApiIdentityException("Text is null/empty.");

    /// <summary>Attempts to parse a culture identifier.</summary>
    private static bool TryParseCulture(string text, out ApiId id)
    {
        try
        {
            var c = CultureInfo.GetCultureInfo(text, predefinedOnly: true);
            id = FromCulture(c);
            return true;
        }
        catch
        {
            id = default;
            return false;
        }
    }

    /// <summary>Attempts to parse a Guid identifier.</summary>
    private static bool TryParseGuid(string text, out ApiId id)
    {
        if (Guid.TryParse(text, out var g))
        {
            id = FromGuid(g);
            return true;
        }

        id = default;
        return false;
    }

    /// <summary>Attempts to parse an Int32 identifier.</summary>
    private static bool TryParseInt32(string text, out ApiId id)
    {
        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i32))
        {
            id = FromInt32(i32);
            return true;
        }

        id = default;
        return false;
    }

    /// <summary>Attempts to parse an Int64 identifier.</summary>
    private static bool TryParseInt64(string text, out ApiId id)
    {
        if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i64))
        {
            id = FromInt64(i64);
            return true;
        }

        id = default;
        return false;
    }

    /// <summary>Attempts to parse a string identifier.</summary>
    private static bool TryParseString(string text, out ApiId id)
    {
        id = FromString(text);
        return true;
    }

    /// <summary>Attempts to parse a Ulid identifier.</summary>
    private static bool TryParseUlid(string text, out ApiId id)
    {
        if (Ulid.TryParse(text, out var u))
        {
            id = FromUlid(u);
            return true;
        }

        id = default;
        return false;
    }
    #endregion

    #region Formatting
    /// <summary>
    ///     Returns a canonical string representation of the identifier or null if empty.
    /// </summary>
    public override string? ToString() => this.Kind switch
    {
        ApiIdKind.None => null,
        ApiIdKind.String => (string?)_ref,
        ApiIdKind.Int32 => _val.Int32.ToString(CultureInfo.InvariantCulture),
        ApiIdKind.Int64 => _val.Int64.ToString(CultureInfo.InvariantCulture),
        ApiIdKind.Guid => _val.Guid.ToString("D"),
        ApiIdKind.Ulid => _val.Ulid.ToString(),
        ApiIdKind.Culture => ((CultureInfo)_ref!).Name,
        ApiIdKind.Composite => CompositeString((ApiIdPart[])_ref!),
        _ => (string?)_ref
    };

    /// <summary>Returns a debugger-friendly display string.</summary>
    internal string ToDebuggerDisplay() => this.HasValue ? $"{this.Kind}:{this}" : "(empty)";
    #endregion

    #region Equality / Ordering
    /// <summary>
    ///     Determines whether this instance is equal to <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The other identifier.</param>
    /// <returns>True if equal.</returns>
    public bool Equals(ApiId other)
    {
        if (this.Kind != other.Kind)
        {
            return false;
        }

        return this.Kind switch
        {
            ApiIdKind.None => true,
            ApiIdKind.String => string.Equals((string?)_ref, (string?)other._ref, StringComparison.Ordinal),
            ApiIdKind.Int32 => _val.Int32 == other._val.Int32,
            ApiIdKind.Int64 => _val.Int64 == other._val.Int64,
            ApiIdKind.Guid => _val.Guid == other._val.Guid,
            ApiIdKind.Ulid => _val.Ulid == other._val.Ulid,
            ApiIdKind.Culture => string.Equals(((CultureInfo)_ref!).Name, ((CultureInfo)other._ref!).Name, StringComparison.OrdinalIgnoreCase),
            ApiIdKind.Composite => PartsEqual((ApiIdPart[])_ref!, (ApiIdPart[])other._ref!),
            _ => false
        };
    }
    /// <summary>
    ///     Compares two composite part arrays for structural equality.
    /// </summary>
    private static bool PartsEqual(ApiIdPart[] a, ApiIdPart[] b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a.Length != b.Length)
        {
            return false;
        }

        for (var i = 0; i < a.Length; i++)
        {
            if (!string.Equals(a[i].Name, b[i].Name, StringComparison.Ordinal))
            {
                return false;
            }

            if (!a[i].Value.Equals(b[i].Value))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>Determines whether this instance equals the specified object.</summary>
    public override bool Equals(object? obj) => obj is ApiId other && this.Equals(other);

    /// <summary>Returns a hash code for this identifier.</summary>
    public override int GetHashCode()
    {
        return this.Kind switch
        {
            ApiIdKind.None => 0,
            ApiIdKind.String => HashCode.Combine((int)this.Kind, (string?)_ref),
            ApiIdKind.Int32 => HashCode.Combine((int)this.Kind, _val.Int32),
            ApiIdKind.Int64 => HashCode.Combine((int)this.Kind, _val.Int64),
            ApiIdKind.Guid => HashCode.Combine((int)this.Kind, _val.Guid),
            ApiIdKind.Ulid => HashCode.Combine((int)this.Kind, _val.Ulid),
            ApiIdKind.Culture => HashCode.Combine((int)this.Kind, ((CultureInfo?)_ref)?.Name?.ToUpperInvariant()),
            ApiIdKind.Composite => GetCompositeHash((ApiIdPart[])_ref!),
            _ => (int)this.Kind
        };
    }

    /// <summary>Computes a hash code for a composite parts array.</summary>
    private static int GetCompositeHash(ApiIdPart[] parts)
    {
        var h = new HashCode();
        h.Add((int)ApiIdKind.Composite);
        foreach (var p in parts)
        {
            h.Add(p.Name, StringComparer.Ordinal);
            h.Add(p.Value);
        }
        return h.ToHashCode();
    }

    /// <summary>Equality operator for <see cref="ApiId"/>.</summary>
    public static bool operator ==(ApiId left, ApiId right) => left.Equals(right);

    /// <summary>Inequality operator for <see cref="ApiId"/>.</summary>
    public static bool operator !=(ApiId left, ApiId right) => !left.Equals(right);

    /// <summary>
    ///     Compares this instance with <paramref name="other"/> for ordering.
    /// </summary>
    /// <param name="other">The other identifier.</param>
    /// <returns>Negative, zero, or positive depending on ordering.</returns>
    public int CompareTo(ApiId other)
    {
        var kindCmp = this.Kind.CompareTo(other.Kind);
        if (kindCmp != 0)
        {
            return Math.Sign(kindCmp);
        }

        return this.Kind switch
        {
            ApiIdKind.None => 0,
            ApiIdKind.String => Math.Sign(string.Compare((string?)_ref, (string?)other._ref, StringComparison.Ordinal)),
            ApiIdKind.Int32 => _val.Int32.CompareTo(other._val.Int32),
            ApiIdKind.Int64 => _val.Int64.CompareTo(other._val.Int64),
            ApiIdKind.Guid => _val.Guid.CompareTo(other._val.Guid),
            ApiIdKind.Ulid => _val.Ulid.CompareTo(other._val.Ulid),
            ApiIdKind.Culture => Math.Sign(string.Compare(((CultureInfo)_ref!).Name, ((CultureInfo)other._ref!).Name, StringComparison.OrdinalIgnoreCase)),
            ApiIdKind.Composite => CompareParts((ApiIdPart[])_ref!, (ApiIdPart[])other._ref!),
            _ => 0
        };
    }

    /// <summary>Compares two composite part arrays for ordering.</summary>
    private static int CompareParts(ApiIdPart[] a, ApiIdPart[] b)
    {
        var lenCmp = a.Length.CompareTo(b.Length);
        if (lenCmp != 0)
        {
            return lenCmp;
        }

        for (var i = 0; i < a.Length; i++)
        {
            var nameCmp = Math.Sign(string.Compare(a[i].Name, b[i].Name, StringComparison.Ordinal));
            if (nameCmp != 0)
            {
                return nameCmp;
            }

            var valCmp = a[i].Value.CompareTo(b[i].Value);
            if (valCmp != 0)
            {
                return valCmp;
            }
        }
        return 0;
    }
    #endregion

    #region Conversions
    /// <summary>Implicit conversion from <see cref="string"/> to <see cref="ApiId"/>.</summary>
    public static implicit operator ApiId(string value) => FromString(value);

    /// <summary>Implicit conversion from <see cref="int"/> to <see cref="ApiId"/>.</summary>
    public static implicit operator ApiId(int value) => FromInt32(value);

    /// <summary>Implicit conversion from <see cref="long"/> to <see cref="ApiId"/>.</summary>
    public static implicit operator ApiId(long value) => FromInt64(value);

    /// <summary>Implicit conversion from <see cref="Guid"/> to <see cref="ApiId"/>.</summary>
    public static implicit operator ApiId(Guid value) => FromGuid(value);

    /// <summary>Implicit conversion from <see cref="Ulid"/> to <see cref="ApiId"/>.</summary>
    public static implicit operator ApiId(Ulid value) => FromUlid(value);

    /// <summary>Implicit conversion from <see cref="CultureInfo"/> to <see cref="ApiId"/>.</summary>
    public static implicit operator ApiId(CultureInfo value) => FromCulture(value);

    /// <summary>Explicit conversion to <see cref="string"/> returns null if not a string identifier.</summary>
    public static explicit operator string?(ApiId id) => id.Kind == ApiIdKind.String ? (string?)id._ref : null;

    /// <summary>Explicit conversion to <see cref="int"/>; throws if kind mismatch.</summary>
    public static explicit operator int(ApiId id) => id.AsInt32OrThrow();

    /// <summary>Explicit conversion to <see cref="long"/>; throws if kind mismatch.</summary>
    public static explicit operator long(ApiId id) => id.AsInt64OrThrow();

    /// <summary>Explicit conversion to <see cref="Guid"/>; throws if kind mismatch.</summary>
    public static explicit operator Guid(ApiId id) => id.AsGuidOrThrow();

    /// <summary>Explicit conversion to <see cref="Ulid"/>; throws if kind mismatch.</summary>
    public static explicit operator Ulid(ApiId id) => id.AsUlidOrThrow();

    /// <summary>Explicit conversion to <see cref="CultureInfo"/> returns null if not a culture identifier.</summary>
    public static explicit operator CultureInfo?(ApiId id) => id.Kind == ApiIdKind.Culture ? (CultureInfo?)id._ref : null;
    #endregion
}
