// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Globalization;
using System.Runtime.InteropServices;

namespace Evoogle.ApiFramework.Key.Internal;

/// <summary>
///     Explicit-layout union for storing value-type arms of <see cref="ApiKey"/> without boxing.
/// </summary>
/// <remarks>
///     <para>
///         <strong>Infrastructure Component:</strong> This type is an internal implementation detail of <see cref="ApiKey"/>
///         and is not intended for direct use in application code. It may change or be removed in future releases.
///     </para>
///     <para>
///         Uses <see cref="StructLayoutAttribute"/> with <see cref="LayoutKind.Explicit"/> to overlay all value-type fields
///         in the same memory location, enabling zero-allocation storage of primitives within <see cref="ApiKey"/>.
///     </para>
///     <para>
///         Supports: <see cref="int"/>, <see cref="long"/>, <see cref="Guid"/>, and <see cref="Ulid"/>.
///         Reference types (<see cref="string"/>, <see cref="CultureInfo"/>, composite arrays) are stored separately in <see cref="ApiKey"/>'s reference field.
///     </para>
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
internal readonly struct ApiKeyValueUnion
{
    #region Fields
    /// <summary>32-bit signed integer value, overlaid at offset 0.</summary>
    [FieldOffset(0)] public readonly int Int32;

    /// <summary>64-bit signed integer value, overlaid at offset 0.</summary>
    [FieldOffset(0)] public readonly long Int64;

    /// <summary><see cref="System.Guid"/> value (16 bytes), overlaid at offset 0.</summary>
    [FieldOffset(0)] public readonly Guid Guid;

    /// <summary><see cref="System.Ulid"/> value (16 bytes), overlaid at offset 0.</summary>
    [FieldOffset(0)] public readonly Ulid Ulid;
    #endregion

    #region Constructors
    private ApiKeyValueUnion(int int32) { this = default; Int32 = int32; }
    private ApiKeyValueUnion(long int64) { this = default; Int64 = int64; }
    private ApiKeyValueUnion(Guid guid) { this = default; Guid = guid; }
    private ApiKeyValueUnion(Ulid ulid) { this = default; Ulid = ulid; }
    #endregion

    #region Factory Methods
    public static ApiKeyValueUnion FromInt32(int int32) => new(int32);
    public static ApiKeyValueUnion FromInt64(long int64) => new(int64);
    public static ApiKeyValueUnion FromGuid(Guid guid) => new(guid);
    public static ApiKeyValueUnion FromUlid(Ulid ulid) => new(ulid);
    #endregion
}
