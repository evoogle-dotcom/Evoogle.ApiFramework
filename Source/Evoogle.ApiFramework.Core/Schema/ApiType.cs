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
[JsonConverter(typeof(ApiTypeJsonConverter))]
public abstract class ApiType : ApiSchemaElement
{
    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override sealed ApiSchemaElementKind Kind => this.ApiKind switch
    {
        ApiTypeKind.Collection => ApiSchemaElementKind.CollectionType,
        ApiTypeKind.Enum => ApiSchemaElementKind.EnumType,
        ApiTypeKind.Object => ApiSchemaElementKind.ObjectType,
        ApiTypeKind.Scalar => ApiSchemaElementKind.ScalarType,
        _ => throw new ArgumentOutOfRangeException(nameof(this.ApiKind))
    };
    #endregion

    #region ApiType Properties
    /// <summary>Gets the kind of API type represented by this instance.</summary>
    public abstract ApiTypeKind ApiKind { get; }

    /// <summary>Gets the CLR type associated with the API type.</summary>
    public Type ClrType { get; }

    /// <summary>Gets the name of the CLR type associated with the API type.</summary>
    public string ClrTypeName => this.ClrType.Name;
    #endregion

    #region Constructors
    internal ApiType(Type clrType)
    {
        this.ClrType = clrType;
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    internal override void InitializeCore(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.InitializeCore(context);

        this.InitializeClrType(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeClrType(ApiInitializationContext context)
    {
        if (this.ClrType is null)
        {
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.ApiTypeNullClrType;
            var description = $"{nameof(this.ClrType)} must not be null";
            var remediation = $"Specify a valid {nameof(this.ClrType)}";

            context.AddIssue(severity, code, description, remediation);
        }
    }
    #endregion
}
