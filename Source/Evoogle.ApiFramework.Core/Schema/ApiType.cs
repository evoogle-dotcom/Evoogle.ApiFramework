// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extension;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the abstract base class for all API types in the schema (e.g., Scalar, Enum, Object, Collection).
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiType"/> class.
/// </remarks>
[JsonConverter(typeof(ApiTypeJsonConverter))]
public abstract class ApiType : ExtensibleBase
{
    #region ApiType Properties
    /// <summary>Gets the kind of API type represented by this instance.</summary>    
    public abstract ApiTypeKind Kind { get; }

    /// <summary>Gets the CLR type associated with the API type.</summary>
    public abstract Type ClrType { get; }

    /// <summary>Gets runtime API type name of the API type.</summary>
    protected abstract string ApiTypeName { get; }

    /// <summary>Gets the validation path for the API type, used for error messages and validation results.</summary>
    protected abstract string ValidationPath { get; }
    #endregion

    #region ApiType Methods
    internal virtual void Initialize(ApiSchema apiSchema, ref List<ValidationResult>? results)
    { }
    #endregion
}
