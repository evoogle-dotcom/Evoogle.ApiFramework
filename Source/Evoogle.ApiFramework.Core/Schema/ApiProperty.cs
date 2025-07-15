// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extension;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents structural metadata of an API property belonging to an <see cref="ApiObjectType"/>.
///     Each property corresponds to a named data element in an API contract.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiProperty"/> captures type-level structure for fields such as primitives,
///         objects, collections, and complex types. Properties may be referenced by one or more
///         <see cref="ApiRelationship"/> instances to convey semantic meaning (e.g., parent-child, references).
///     </para>
/// </remarks>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiProperty"/> class.
/// </remarks>
/// <param name="apiName">The API name of the property.</param>
/// <param name="apiTypeExpression">The API type expression of the property.</param>
/// <param name="apiTypeModifiers">Modifiers applied to the property (e.g., Required).</param>
/// <param name="clrName">The CLR property name corresponding to this API property.</param>
public sealed class ApiProperty(string apiName, ApiTypeExpression apiTypeExpression, ApiTypeModifiers apiTypeModifiers, string clrName) : ExtensibleBase
{
    #region Properties
    /// <summary>Gets the API name of the property (used in API requests/responses).</summary>
    public string ApiName { get; } = apiName;

    /// <summary>Gets the API type of the property.</summary>
    public ApiType ApiType => this.ApiTypeExpression.ApiType;

    /// <summary>Gets the modifiers applied to this property (e.g., Required).</summary>
    public ApiTypeModifiers ApiTypeModifiers { get; } = apiTypeModifiers;

    /// <summary>Gets the CLR name of the property (matching the C# property name).</summary>
    public string ClrName { get; } = clrName;

    /// <summary>
    ///     Gets the API type expression to the API type of this property.
    ///     May point to a named type or inline type (e.g., collection).
    /// </summary>
    internal ApiTypeExpression ApiTypeExpression { get; } = apiTypeExpression;
    #endregion

    #region ApiProperty Methods
    internal void Initialize(ApiSchema apiSchema, string apiValidationPath, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        this.InitializeApiName(apiSchema, apiValidationPath, ref results);
        this.InitializeClrName(apiSchema, apiValidationPath, ref results);
        this.InitializeApiTypeExpression(apiSchema, apiValidationPath, ref results);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiTypeExpression = this.ApiTypeExpression.SafeToString();
        var apiTypeModifiers = this.ApiTypeModifiers.SafeToString();
        var clrName = this.ClrName.SafeToString();

        return $"{nameof(ApiProperty)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiTypeExpression)}={apiTypeExpression}, {nameof(this.ApiTypeModifiers)}={apiTypeModifiers}, {nameof(this.ClrName)}={clrName}}}";
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiName(ApiSchema _, string apiValidationPath, ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiName)} cannot be null or whitespace.", [nameof(this.ApiName)]));
        }
    }

    private void InitializeApiTypeExpression(ApiSchema apiSchema, string apiParentValidationPath, ref List<ValidationResult>? results)
    {
        if (this.ApiTypeExpression is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiParentValidationPath}.{nameof(this.ApiTypeExpression)} cannot be null.", [nameof(this.ApiTypeExpression)]));
            return;
        }

        var apiChildValidationPath = $"{apiParentValidationPath}.{nameof(this.ApiTypeExpression)}";
        this.ApiTypeExpression.Initialize(apiSchema, apiChildValidationPath, ref results);
    }

    private void InitializeClrName(ApiSchema _, string apiValidationPath, ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ClrName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} cannot be null or whitespace.", [nameof(this.ClrName)]));
        }
    }
    #endregion
}
