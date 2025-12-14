// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Json;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the abstract base class for all API types in the schema (e.g., Scalar, Enum, Object, Collection).
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiType"/> class.
/// </remarks>
[JsonConverter(typeof(ApiTypeJsonConverter))]
public abstract class ApiType : ApiSchemaElement
{
    #region ApiType Properties
    /// <summary>Gets the kind of API type represented by this instance.</summary>    
    public abstract ApiTypeKind Kind { get; }

    /// <summary>Gets the CLR type associated with the API type.</summary>
    public abstract Type ClrType { get; }

    /// <summary>Gets runtime API type name of the API type.</summary>
    protected abstract string ApiTypeName { get; }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);
    }
    #endregion
}
