// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Version.Internal;

namespace Evoogle.ApiFramework.Version;

/// <summary>
///     Represents an immutable, equality-oriented version token for an API object.
/// </summary>
/// <remarks>
///     <para>
///         Equality includes the exact CLR representation. For example, an <see cref="int"/>
///         version does not equal a numerically identical <see cref="long"/> version.
///     </para>
///     <para>
///         Recognized binary values are defensively copied and use structural equality. Custom
///         reference-type scalar values are retained by reference as an explicit integration
///         opt-in. Such values must be immutable for their entire use as an
///         <see cref="ApiVersion"/>
///         and must provide stable equality and hash-code behavior. The framework cannot enforce
///         this contract or make an arbitrary custom object deeply immutable.
///     </para>
/// </remarks>
[DebuggerDisplay("{ToDebuggerDisplay(),nq}")]
public readonly struct ApiVersion : IEquatable<ApiVersion>
{
    #region Constants
    /// <summary>The display text used for an empty version.</summary>
    public const string EmptyDisplayText = "<empty>";
    #endregion

    #region Fields
    /// <summary>Represents the absence of a version value.</summary>
    public static readonly ApiVersion Empty = default;

    private readonly Type? _clrType;
    private readonly object? _referenceValue;
    private readonly ApiVersionStorageKind _storageKind;
    private readonly ApiVersionValueUnion _valueUnion;
    #endregion

    #region Constructors
    private ApiVersion
    (
        Type clrType,
        ApiVersionStorageKind storageKind,
        ApiVersionValueUnion valueUnion = default,
        object? referenceValue = null
    )
    {
        _clrType = clrType;
        _storageKind = storageKind;
        _valueUnion = valueUnion;
        _referenceValue = referenceValue;
    }
    #endregion

    #region Properties
    /// <summary>Gets the exact CLR type represented by this version, or null when empty.</summary>
    public Type? ClrType => _clrType;

    /// <summary>Gets whether this version contains a value.</summary>
    public bool HasValue => _storageKind != ApiVersionStorageKind.Empty;
    #endregion

    #region Factory Methods
    internal static ApiVersion FromValue(object value, Type clrType)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(clrType);

        if (value.GetType() != clrType)
        {
            throw new ApiVersionException
            (
                $"Version value CLR type '{value.GetType()}' does not match expected type " +
                $"'{clrType}'."
            );
        }

        return value switch
        {
            sbyte typedValue => FromSignedInteger(clrType, typedValue),
            short typedValue => FromSignedInteger(clrType, typedValue),
            int typedValue => FromSignedInteger(clrType, typedValue),
            long typedValue => FromSignedInteger(clrType, typedValue),
            byte typedValue => FromUnsignedInteger(clrType, typedValue),
            ushort typedValue => FromUnsignedInteger(clrType, typedValue),
            uint typedValue => FromUnsignedInteger(clrType, typedValue),
            ulong typedValue => FromUnsignedInteger(clrType, typedValue),
            Guid typedValue => new(clrType, ApiVersionStorageKind.Guid,
                ApiVersionValueUnion.FromGuid(typedValue)),
            Ulid typedValue => new(clrType, ApiVersionStorageKind.Ulid,
                ApiVersionValueUnion.FromUlid(typedValue)),
            DateTime typedValue => new(clrType, ApiVersionStorageKind.DateTime,
                ApiVersionValueUnion.FromDateTime(typedValue)),
            DateTimeOffset typedValue => new(clrType, ApiVersionStorageKind.DateTimeOffset,
                ApiVersionValueUnion.FromDateTimeOffset(typedValue)),
            string typedValue => new(clrType, ApiVersionStorageKind.String,
                referenceValue: typedValue),
            byte[] typedValue => FromBinary(clrType, typedValue),
            Memory<byte> typedValue => FromBinary(clrType, typedValue.Span),
            ReadOnlyMemory<byte> typedValue => FromBinary(clrType, typedValue.Span),
            ImmutableArray<byte> typedValue => FromBinary(clrType, typedValue.AsSpan()),
            ArraySegment<byte> typedValue => FromBinary(clrType, typedValue.AsSpan()),
            _ => new(clrType, ApiVersionStorageKind.Object, referenceValue: value)
        };
    }

    private static ApiVersion FromBinary(Type clrType, ReadOnlySpan<byte> value)
        => new(clrType, ApiVersionStorageKind.Binary, referenceValue: value.ToArray());

    private static ApiVersion FromSignedInteger(Type clrType, long value)
        => new
        (
            clrType,
            ApiVersionStorageKind.SignedInteger,
            ApiVersionValueUnion.FromSignedInteger(value)
        );

    private static ApiVersion FromUnsignedInteger(Type clrType, ulong value)
        => new
        (
            clrType,
            ApiVersionStorageKind.UnsignedInteger,
            ApiVersionValueUnion.FromUnsignedInteger(value)
        );
    #endregion

    #region Value Methods
    /// <summary>Returns the version value using its exact CLR representation.</summary>
    /// <returns>The represented value.</returns>
    /// <exception cref="ApiVersionException">The version is empty.</exception>
    public object ToObject()
    {
        if (!this.HasValue)
        {
            throw new ApiVersionException("An empty API version does not contain a value.");
        }

        return this.ToObjectCore();
    }

    /// <summary>
    ///     Gets the version value when its exact CLR type is <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The expected exact CLR type.</typeparam>
    /// <returns>The represented value.</returns>
    /// <exception cref="ApiVersionException">
    ///     The version is empty or has a different CLR representation.
    /// </exception>
    public TValue GetValue<TValue>()
    {
        if (!this.TryGetValue<TValue>(out var value))
        {
            var actualType = this.ClrType?.ToString() ?? EmptyDisplayText;
            throw new ApiVersionException
            (
                $"API version CLR type '{actualType}' is not '{typeof(TValue)}'."
            );
        }

        return value;
    }

    /// <summary>
    ///     Attempts to get the version value when its exact CLR type is
    ///     <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The expected exact CLR type.</typeparam>
    /// <param name="value">The represented value when successful.</param>
    /// <returns>True when the version has the requested exact CLR representation.</returns>
    public bool TryGetValue<TValue>([MaybeNullWhen(false)] out TValue value)
    {
        if (!this.HasValue || _clrType != typeof(TValue))
        {
            value = default;
            return false;
        }

        value = (TValue)this.ToObjectCore();
        return true;
    }

    private object ToObjectCore()
    {
        return _storageKind switch
        {
            ApiVersionStorageKind.SignedInteger => this.ConvertSignedInteger(),
            ApiVersionStorageKind.UnsignedInteger => this.ConvertUnsignedInteger(),
            ApiVersionStorageKind.Guid => _valueUnion.Guid,
            ApiVersionStorageKind.Ulid => _valueUnion.Ulid,
            ApiVersionStorageKind.DateTime => _valueUnion.DateTime,
            ApiVersionStorageKind.DateTimeOffset => _valueUnion.DateTimeOffset,
            ApiVersionStorageKind.String => _referenceValue!,
            ApiVersionStorageKind.Binary => this.ConvertBinary(),
            ApiVersionStorageKind.Object => _referenceValue!,
            _ => throw new ApiVersionException("An empty API version does not contain a value.")
        };
    }

    private object ConvertBinary()
    {
        var bytes = (byte[])_referenceValue!;
        if (_clrType == typeof(byte[]))
        {
            return bytes.ToArray();
        }

        if (_clrType == typeof(ReadOnlyMemory<byte>))
        {
            return new ReadOnlyMemory<byte>(bytes.ToArray());
        }

        if (_clrType == typeof(Memory<byte>))
        {
            return new Memory<byte>(bytes.ToArray());
        }

        if (_clrType == typeof(ArraySegment<byte>))
        {
            return new ArraySegment<byte>(bytes.ToArray());
        }

        return ImmutableArray.Create(bytes);
    }

    private object ConvertSignedInteger()
    {
        if (_clrType == typeof(sbyte)) { return (sbyte)_valueUnion.SignedInteger; }
        if (_clrType == typeof(short)) { return (short)_valueUnion.SignedInteger; }
        if (_clrType == typeof(int)) { return (int)_valueUnion.SignedInteger; }
        return _valueUnion.SignedInteger;
    }

    private object ConvertUnsignedInteger()
    {
        if (_clrType == typeof(byte)) { return (byte)_valueUnion.UnsignedInteger; }
        if (_clrType == typeof(ushort)) { return (ushort)_valueUnion.UnsignedInteger; }
        if (_clrType == typeof(uint)) { return (uint)_valueUnion.UnsignedInteger; }
        return _valueUnion.UnsignedInteger;
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public bool Equals(ApiVersion other)
    {
        if (_clrType != other._clrType || _storageKind != other._storageKind)
        {
            return false;
        }

        return _storageKind switch
        {
            ApiVersionStorageKind.Empty => true,
            ApiVersionStorageKind.SignedInteger =>
                _valueUnion.SignedInteger == other._valueUnion.SignedInteger,
            ApiVersionStorageKind.UnsignedInteger =>
                _valueUnion.UnsignedInteger == other._valueUnion.UnsignedInteger,
            ApiVersionStorageKind.Guid => _valueUnion.Guid == other._valueUnion.Guid,
            ApiVersionStorageKind.Ulid => _valueUnion.Ulid == other._valueUnion.Ulid,
            ApiVersionStorageKind.DateTime =>
                _valueUnion.DateTime.Ticks == other._valueUnion.DateTime.Ticks &&
                _valueUnion.DateTime.Kind == other._valueUnion.DateTime.Kind,
            ApiVersionStorageKind.DateTimeOffset =>
                _valueUnion.DateTimeOffset.EqualsExact(other._valueUnion.DateTimeOffset),
            ApiVersionStorageKind.String => string.Equals
            (
                (string?)_referenceValue,
                (string?)other._referenceValue,
                StringComparison.Ordinal
            ),
            ApiVersionStorageKind.Binary =>
                ((byte[])_referenceValue!).AsSpan().SequenceEqual((byte[])other._referenceValue!),
            ApiVersionStorageKind.Object => Equals(_referenceValue, other._referenceValue),
            _ => false
        };
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ApiVersion other && this.Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (_storageKind == ApiVersionStorageKind.Binary)
        {
            var hashCode = new HashCode();
            hashCode.Add(_clrType);
            foreach (var value in (byte[])_referenceValue!)
            {
                hashCode.Add(value);
            }

            return hashCode.ToHashCode();
        }

        return _storageKind switch
        {
            ApiVersionStorageKind.Empty => 0,
            ApiVersionStorageKind.SignedInteger => HashCode.Combine
                (_clrType, _valueUnion.SignedInteger),
            ApiVersionStorageKind.UnsignedInteger => HashCode.Combine
                (_clrType, _valueUnion.UnsignedInteger),
            ApiVersionStorageKind.Guid => HashCode.Combine(_clrType, _valueUnion.Guid),
            ApiVersionStorageKind.Ulid => HashCode.Combine(_clrType, _valueUnion.Ulid),
            ApiVersionStorageKind.DateTime => HashCode.Combine
                (_clrType, _valueUnion.DateTime.Ticks, _valueUnion.DateTime.Kind),
            ApiVersionStorageKind.DateTimeOffset => HashCode.Combine
                (
                    _clrType,
                    _valueUnion.DateTimeOffset.Ticks,
                    _valueUnion.DateTimeOffset.Offset
                ),
            _ => HashCode.Combine(_clrType, _referenceValue)
        };
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (!this.HasValue)
        {
            return EmptyDisplayText;
        }

        return _storageKind == ApiVersionStorageKind.Binary
            ? Convert.ToHexString((byte[])_referenceValue!)
            : this.ToObjectCore().ToString() ?? string.Empty;
    }

    private string ToDebuggerDisplay() => this.ToString();
    #endregion

    #region Operators
    /// <summary>Determines whether two versions are equal.</summary>
    public static bool operator ==(ApiVersion left, ApiVersion right) => left.Equals(right);

    /// <summary>Determines whether two versions are unequal.</summary>
    public static bool operator !=(ApiVersion left, ApiVersion right) => !left.Equals(right);
    #endregion
}
