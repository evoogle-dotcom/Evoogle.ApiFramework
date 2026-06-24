// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the abstract base class for all API types in the schema (e.g., Scalar, Enum, Object, Collection).
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiType"/> class.
/// </remarks>
/// <param name="clrType">The underlying CLR type this API type maps to.</param>
[JsonConverter(typeof(ApiTypeJsonConverter))]
public abstract class ApiType(Type clrType) : ApiSchemaElement
{
    #region ApiType Properties
    /// <summary>Gets the kind of API type represented by this instance.</summary>
    public abstract ApiTypeKind ApiKind { get; }

    /// <summary>Gets the CLR type associated with the API type.</summary>
    public Type ClrType { get; } = clrType;

    /// <summary>Gets the name of the CLR type associated with the API type.</summary>
    public string ClrTypeName => this.ClrType.Name;
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeClrType(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeClrType(ApiInitializationContext context)
    {
        if (this.ClrType is null)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.ApiTypeNullClrType;
            var description = $"{nameof(this.ClrType)} must not be null";
            var remediation = $"Specify a valid {nameof(this.ClrType)}";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion
}
