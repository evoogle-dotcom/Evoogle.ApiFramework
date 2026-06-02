// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Globalization;

namespace Evoogle.ApiFramework.Key;

/// <summary>
///     Defines the discriminated union kinds for <see cref="ApiKey"/> values.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiKeyKind"/> serves as the type tag for the <see cref="ApiKey"/> discriminated union,
///         distinguishing between scalar representations (e.g., <see cref="Int32"/>, <see cref="Guid"/>, <see cref="String"/>)
///         and the <see cref="Composite"/> kind which contains multiple component parts.
///     </para>
///     <para>
///         The enum is used throughout the API framework for type-safe operations, serialization, and runtime type checking.
///     </para>
/// </remarks>
public enum ApiKeyKind : byte
{
    #region Values
    /// <summary>
    ///     Represents an empty key with no value.
    /// </summary>
    /// <remarks>
    ///     Equivalent to <c>default(<see cref="ApiKey"/>)</c> or <see cref="ApiKey.Empty"/>.
    ///     Used to indicate the absence of a key value.
    /// </remarks>
    Empty,

    /// <summary>
    ///     Represents a composite key composed of multiple scalar <see cref="ApiKey"/> parts.
    /// </summary>
    /// <remarks>
    ///     Composite keys can be either named (e.g., "CustomerId=42|OrderNumber=1001") or ordered/positional (e.g., "42|1001").
    ///     All parts within a composite must be scalar values; nested composites are not allowed.
    /// </remarks>
    Composite,

    /// <summary>
    ///     Represents a <see cref="CultureInfo"/> key stored by culture name.
    /// </summary>
    /// <remarks>
    ///     Culture keys are compared case-insensitively and stored by their <see cref="CultureInfo.Name"/> property.
    /// </remarks>
    Culture,

    /// <summary>
    ///     Represents a <see cref="Guid"/> (Globally Unique Identifier) value.
    /// </summary>
    /// <remarks>
    ///     Stored in the value union without allocation.
    ///     Provides globally unique identification suitable for distributed systems.
    /// </remarks>
    Guid,

    /// <summary>
    ///     Represents a 32-bit signed integer (<see cref="int"/>) key.
    /// </summary>
    /// <remarks>
    ///     Stored in the value union without boxing.
    ///     Suitable for database primary keys and sequential keys.
    /// </remarks>
    Int32,

    /// <summary>
    ///     Represents a 64-bit signed integer (<see cref="long"/>) key.
    /// </summary>
    /// <remarks>
    ///     Stored in the value union without boxing.
    ///     Suitable for high-volume systems requiring large key spaces.
    /// </remarks>
    Int64,

    /// <summary>
    ///     Represents a <see cref="string"/> key.
    /// </summary>
    /// <remarks>
    ///     String keys use ordinal (case-sensitive) comparison for equality and ordering.
    ///     Null strings are treated as <see cref="Empty"/>.
    /// </remarks>
    String,

    /// <summary>
    ///     Represents a <see cref="Ulid"/> (Universally Unique Lexicographically Sortable Identifier) value.
    /// </summary>
    /// <remarks>
    ///     Stored in the value union without allocation.
    ///     Provides time-ordered unique identification with better database indexing properties than <see cref="Guid"/>.
    /// </remarks>
    Ulid,
    #endregion
}
