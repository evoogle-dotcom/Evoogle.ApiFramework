// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the base class for API types that are identified by a unique API name.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiNamedType"/> class.
/// </remarks>
/// <param name="apiName">The unique API name of the type.</param>
/// <param name="clrType">The underlying CLR type this API type maps to.</param>
public abstract class ApiNamedType(string apiName, Type clrType) : ApiType()
{
    #region ApiType Properties
    /// <inheritdoc />
    public override Type ClrType { get; } = clrType;
    #endregion

    #region ApiNamedType Properties
    /// <summary>Gets the API name of the API type.</summary>
    public string ApiName { get; } = apiName;
    #endregion

    #region ApiType Methods
    /// <inheritdoc />
    protected override string GetValidationPath() => $"{this.ApiTypeName.SafeToString()}[\"{this.ApiName.SafeToString()}\"]";

    internal override void Initialize(ApiSchema apiSchema, ApiSchemaContext apiSchemaContext, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(apiSchemaContext);

        this.InitializeApiName(ref results);
        this.InitializeClrType(ref results);
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiName(ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{this.GetValidationPath()}.{nameof(this.ApiName)} cannot be null or whitespace.", [nameof(this.ApiName)]));
        }
    }

    private void InitializeClrType(ref List<ValidationResult>? results)
    {
        if (this.ClrType is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{this.GetValidationPath()}.{nameof(this.ClrType)} cannot be null.", [nameof(this.ClrType)]));
        }
    }
    #endregion
}
