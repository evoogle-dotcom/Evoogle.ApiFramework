// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Runtime.InteropServices;

namespace Evoogle.ApiFramework.Version.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
internal readonly struct ApiVersionValueUnion
{
    #region Fields
    [FieldOffset(0)] public readonly long SignedInteger;
    [FieldOffset(0)] public readonly ulong UnsignedInteger;
    [FieldOffset(0)] public readonly Guid Guid;
    [FieldOffset(0)] public readonly Ulid Ulid;
    [FieldOffset(0)] public readonly DateTime DateTime;
    [FieldOffset(0)] public readonly DateTimeOffset DateTimeOffset;
    #endregion

    #region Constructors
    private ApiVersionValueUnion(long value) { this = default; SignedInteger = value; }
    private ApiVersionValueUnion(ulong value) { this = default; UnsignedInteger = value; }
    private ApiVersionValueUnion(Guid value) { this = default; Guid = value; }
    private ApiVersionValueUnion(Ulid value) { this = default; Ulid = value; }
    private ApiVersionValueUnion(DateTime value) { this = default; DateTime = value; }
    private ApiVersionValueUnion(DateTimeOffset value) { this = default; DateTimeOffset = value; }
    #endregion

    #region Factory Methods
    public static ApiVersionValueUnion FromSignedInteger(long value) => new(value);
    public static ApiVersionValueUnion FromUnsignedInteger(ulong value) => new(value);
    public static ApiVersionValueUnion FromGuid(Guid value) => new(value);
    public static ApiVersionValueUnion FromUlid(Ulid value) => new(value);
    public static ApiVersionValueUnion FromDateTime(DateTime value) => new(value);
    public static ApiVersionValueUnion FromDateTimeOffset(DateTimeOffset value) => new(value);
    #endregion
}
