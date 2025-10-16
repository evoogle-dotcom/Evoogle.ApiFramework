// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Identity.Internal;
using Evoogle.ApiFramework.Identity.Json;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     ApiId is a compact, tagged-discriminated union for identifiers.
///     Scalars are allocation-free; composites allocate an array of parts.
///     Uses a safe value-type union for primitives and a single reference slot for reference arms.
/// </summary>
[DebuggerDisplay("{ToDebuggerDisplay(),nq}")]
[JsonConverter(typeof(ApiIdJsonConverter))]
public readonly struct ApiId : IEquatable<ApiId>, IComparable<ApiId>
{
    #region Fields
    public static readonly ApiId Empty = default;

    // Union layout:
    //  - _val: explicit-layout union for value-type arms (int, long, Guid, Ulid)
    //  - _ref: single reference slot for reference arms (string, CultureInfo, ApiIdPart[])
    private readonly ApiIdValueUnion _val;
    private readonly object? _ref;
    #endregion

    #region Properties
    public ApiIdKind Kind { get; }

    /// <summary>Optional original text (diagnostics/round-tripping only; not used for equality).</summary>
    public string? OriginalString { get; }
    #endregion

    #region Computed Properties
    public bool HasValue => this.Kind != ApiIdKind.None;

    public bool IsString => this.Kind == ApiIdKind.String;
    public bool IsInt32 => this.Kind == ApiIdKind.Int32;
    public bool IsInt64 => this.Kind == ApiIdKind.Int64;
    public bool IsGuid => this.Kind == ApiIdKind.Guid;
    public bool IsUlid => this.Kind == ApiIdKind.Ulid;
    public bool IsCulture => this.Kind == ApiIdKind.Culture;
    public bool IsComposite => this.Kind == ApiIdKind.Composite;
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

            return parts[0].Name is not null;
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

            return parts[0].Name is null;
        }
    }

    public int PartCount => this.IsComposite ? ((ApiIdPart[])_ref!).Length : (this.HasValue ? 1 : 0);
    public ReadOnlySpan<ApiIdPart> Parts => this.IsComposite ? ((ApiIdPart[])_ref!).AsSpan() : ReadOnlySpan<ApiIdPart>.Empty;
    public ApiId this[int index] => this.IsComposite ? ((ApiIdPart[])_ref!)[index].Value : throw new InvalidOperationException("Not composite.");
    #endregion

    #region Constructors
    internal ApiId(ApiIdKind kind, in ApiIdValueUnion val, object? reference, string? original)
    {
        _val = val;
        _ref = reference;

        this.Kind = kind;
        this.OriginalString = original;
    }
    #endregion

    #region Factory Methods
    public static ApiId FromCulture(CultureInfo value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        return new ApiId(ApiIdKind.Culture, default, value, value.Name);
    }

    public static ApiId FromCulture(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        return new ApiId(ApiIdKind.Culture, default, CultureInfo.GetCultureInfo(name), name);
    }

    public static ApiId FromString(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        return new ApiId(ApiIdKind.String, default, value, value);
    }

    public static ApiId FromInt32(int value) => new(ApiIdKind.Int32, ApiIdValueUnion.FromInt32(value), null, value.ToString(CultureInfo.InvariantCulture));
    public static ApiId FromInt64(long value) => new(ApiIdKind.Int64, ApiIdValueUnion.FromInt64(value), null, value.ToString(CultureInfo.InvariantCulture));
    public static ApiId FromGuid(Guid value) => new(ApiIdKind.Guid, ApiIdValueUnion.FromGuid(value), null, value.ToString("D"));
    public static ApiId FromUlid(Ulid value) => new(ApiIdKind.Ulid, ApiIdValueUnion.FromUlid(value), null, value.ToString());

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

    public static ApiId Composite(params ApiId[] orderedParts)
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

    public static ApiId Composite(IEnumerable<ApiIdPart> namedParts)
    {
        if (namedParts is null || !namedParts.Any())
        {
            return Empty;
        }

        var clone = namedParts.Select(p => new ApiIdPart(p.Name, p.Value)).ToArray();

        ValidateCompositeParts(clone);

        return new ApiId(ApiIdKind.Composite, default, clone, CompositeString(clone));
    }

    public static ApiId Composite(params ApiIdPart[] namedParts)
    {
        if (namedParts is null || namedParts.Length == 0)
        {
            return Empty;
        }

        var clone = (ApiIdPart[])namedParts.Clone();

        ValidateCompositeParts(clone);

        return new ApiId(ApiIdKind.Composite, default, clone, CompositeString(clone));
    }

    public static string? CompositeString(IEnumerable<ApiIdPart>? parts)
    {
        if (parts is null || !parts.Any())
        {
            return null;
        }

        return string.Join("|", parts);
    }       

    private static void ValidateCompositeParts(ApiIdPart[] parts)
    {
        var anyNamed = false;
        var anyUnnamed = false;
        foreach (var p in parts)
        {
            if (p.Value.Kind == ApiIdKind.Composite)
            {
                throw new InvalidOperationException($"Nested composite parts are not allowed in {nameof(ApiId)}.");
            }

            if (p.Name is null)
            {
                anyUnnamed = true;
            }
            else
            {
                anyNamed = true;
            }

            if (anyNamed && anyUnnamed)
            {
                throw new InvalidOperationException($"Cannot mix named and unnamed parts in the same composite {nameof(ApiId)}.");
            }
        }
    }
    #endregion

    #region AsOrThrow Methods
    public string AsStringOrThrow() => this.Kind == ApiIdKind.String ? (string)_ref! : throw new InvalidOperationException($"Kind {this.Kind} is not a string.");
    public int AsInt32OrThrow() => this.Kind == ApiIdKind.Int32 ? _val.Int32 : throw new InvalidOperationException($"Kind {this.Kind} is not an Int32.");
    public long AsInt64OrThrow() => this.Kind == ApiIdKind.Int64 ? _val.Int64 : throw new InvalidOperationException($"Kind {this.Kind} is not an Int64.");
    public Guid AsGuidOrThrow() => this.Kind == ApiIdKind.Guid ? _val.Guid : throw new InvalidOperationException($"Kind {this.Kind} is not a Guid.");
    public Ulid AsUlidOrThrow() => this.Kind == ApiIdKind.Ulid ? _val.Ulid : throw new InvalidOperationException($"Kind {this.Kind} is not a Ulid.");
    public CultureInfo AsCultureOrThrow() => this.Kind == ApiIdKind.Culture ? (CultureInfo)_ref! : throw new InvalidOperationException($"Kind {this.Kind} is not a Culture.");
    #endregion

    #region TryGet Methods
    public bool TryGet(out string? value) { value = this.Kind == ApiIdKind.String ? (string?)_ref : null; return this.Kind == ApiIdKind.String; }
    public bool TryGet(out int value) { value = _val.Int32; return this.Kind == ApiIdKind.Int32; }
    public bool TryGet(out long value) { value = _val.Int64; return this.Kind == ApiIdKind.Int64; }
    public bool TryGet(out Guid value) { value = _val.Guid; return this.Kind == ApiIdKind.Guid; }
    public bool TryGet(out Ulid value) { value = _val.Ulid; return this.Kind == ApiIdKind.Ulid; }
    public bool TryGet(out CultureInfo? value) { value = this.Kind == ApiIdKind.Culture ? (CultureInfo?)_ref : null; return this.Kind == ApiIdKind.Culture; }

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

    public static ApiId Parse(ApiIdKind kind, string text) => TryParse(kind, text, out var id) ? id : throw new FormatException($"Text '{text}' is not a valid {kind} id.");

    public static ApiId Parse(string text) => TryParse(text, out var id) ? id : throw new FormatException("Text is null/empty.");

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

    private static bool TryParseString(string text, out ApiId id)
    {
        id = FromString(text);
        return true;
    }

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

    private string ToDebuggerDisplay() => this.HasValue ? $"{this.Kind}:{this}" : "(empty)";
    #endregion

    #region Equality / Ordering
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

    public override bool Equals(object? obj) => obj is ApiId other && this.Equals(other);
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

    public static bool operator ==(ApiId left, ApiId right) => left.Equals(right);
    public static bool operator !=(ApiId left, ApiId right) => !left.Equals(right);

    public int CompareTo(ApiId other)
    {
        var kindCmp = this.Kind.CompareTo(other.Kind);
        if (kindCmp != 0)
        {
            return kindCmp;
        }

        return this.Kind switch
        {
            ApiIdKind.None => 0,
            ApiIdKind.String => string.Compare((string?)_ref, (string?)other._ref, StringComparison.Ordinal),
            ApiIdKind.Int32 => _val.Int32.CompareTo(other._val.Int32),
            ApiIdKind.Int64 => _val.Int64.CompareTo(other._val.Int64),
            ApiIdKind.Guid => _val.Guid.CompareTo(other._val.Guid),
            ApiIdKind.Ulid => _val.Ulid.CompareTo(other._val.Ulid),
            ApiIdKind.Culture => string.Compare(((CultureInfo)_ref!).Name, ((CultureInfo)other._ref!).Name, StringComparison.OrdinalIgnoreCase),
            ApiIdKind.Composite => CompareParts((ApiIdPart[])_ref!, (ApiIdPart[])other._ref!),
            _ => 0
        };
    }
    private static int CompareParts(ApiIdPart[] a, ApiIdPart[] b)
    {
        var lenCmp = a.Length.CompareTo(b.Length);
        if (lenCmp != 0)
        {
            return lenCmp;
        }

        for (var i = 0; i < a.Length; i++)
        {
            var nameCmp = string.Compare(a[i].Name, b[i].Name, StringComparison.Ordinal);
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
    public static implicit operator ApiId(string value) => FromString(value);
    public static implicit operator ApiId(int value) => FromInt32(value);
    public static implicit operator ApiId(long value) => FromInt64(value);
    public static implicit operator ApiId(Guid value) => FromGuid(value);
    public static implicit operator ApiId(Ulid value) => FromUlid(value);
    public static implicit operator ApiId(CultureInfo value) => FromCulture(value);

    public static explicit operator string?(ApiId id) => id.Kind == ApiIdKind.String ? (string?)id._ref : null;
    public static explicit operator int(ApiId id) => id.AsInt32OrThrow();
    public static explicit operator long(ApiId id) => id.AsInt64OrThrow();
    public static explicit operator Guid(ApiId id) => id.AsGuidOrThrow();
    public static explicit operator Ulid(ApiId id) => id.AsUlidOrThrow();
    public static explicit operator CultureInfo?(ApiId id) => id.Kind == ApiIdKind.Culture ? (CultureInfo?)id._ref : null;
    #endregion
}
