// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Runtime.InteropServices;

namespace Evoogle.ApiFramework.Identity.Internal;

/// <summary>
///     Explicit-layout union for storing value-type arms of <see cref="ApiId"/> without boxing.
/// </summary>
/// <remarks>
///     <para>
///         <strong>Infrastructure Component:</strong> This type is an internal implementation detail of <see cref="ApiId"/>
///         and is not intended for direct use in application code. It may change or be removed in future releases.
///     </para>
///     <para>
///         Uses <see cref="StructLayoutAttribute"/> with <see cref="LayoutKind.Explicit"/> to overlay all value-type fields
///         in the same memory location, enabling zero-allocation storage of primitives within <see cref="ApiId"/>.
///     </para>
///     <para>
///         Supports: <see cref="int"/>, <see cref="long"/>, <see cref="Guid"/>, and <see cref="Ulid"/>.
///         Reference types (<see cref="string"/>, <see cref="System.Globalization.CultureInfo"/>, composite arrays)
///         are stored separately in <see cref="ApiId"/>'s reference field.
///     </para>
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
internal readonly struct ApiIdValueUnion
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
    private ApiIdValueUnion(int i) { this = default; Int32 = i; }
    private ApiIdValueUnion(long l) { this = default; Int64 = l; }
    private ApiIdValueUnion(Guid g) { this = default; Guid = g; }
    private ApiIdValueUnion(Ulid u) { this = default; Ulid = u; }
    #endregion

    #region Factory Methods
    public static ApiIdValueUnion FromInt32(int i) => new(i);
    public static ApiIdValueUnion FromInt64(long l) => new(l);
    public static ApiIdValueUnion FromGuid(Guid g) => new(g);
    public static ApiIdValueUnion FromUlid(Ulid u) => new(u);
    #endregion
}
