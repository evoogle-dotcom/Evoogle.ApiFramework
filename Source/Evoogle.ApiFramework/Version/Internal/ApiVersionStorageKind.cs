// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Version.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal enum ApiVersionStorageKind : byte
{
    #region Storage Kinds
    Empty,
    SignedInteger,
    UnsignedInteger,
    Guid,
    Ulid,
    DateTime,
    DateTimeOffset,
    String,
    Binary,
    Object
    #endregion
}
