// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Runtime.InteropServices;

namespace Evoogle.ApiFramework.Identity.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
internal readonly struct ApiIdValueUnion
{
    #region Fields
    [FieldOffset(0)] public readonly int Int32;
    [FieldOffset(0)] public readonly long Int64;
    [FieldOffset(0)] public readonly Guid Guid;
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
