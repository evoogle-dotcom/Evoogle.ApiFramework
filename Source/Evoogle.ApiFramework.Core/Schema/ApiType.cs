// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.Extension;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the abstract base class for all API types in the schema (e.g., Scalar, Enum, Object, Collection).
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiType"/> class.
/// </remarks>
/// <param name="clrType">The CLR type associated with this API type.</param>
/// <exception cref="ArgumentNullException">Thrown if <paramref name="clrType"/> is null.</exception>
[JsonConverter(typeof(ApiTypeJsonConverter))]
public abstract class ApiType(Type clrType) : ExtensibleBase
{
    #region ApiType Properties
    /// <summary>Gets the kind of API type represented by this instance.</summary>    
    public abstract ApiTypeKind Kind { get; }

    /// <summary>Gets the CLR type associated with this API type.</summary>
    public Type ClrType { get; } = clrType ?? throw new ArgumentNullException(nameof(clrType));
    #endregion
}
