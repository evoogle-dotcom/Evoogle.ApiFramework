// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extension;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API property belonging to an API object type.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiProperty"/> class.
/// </remarks>
/// <param name="apiName">The API name of the property.</param>
/// <param name="apiTypeExpression">The API type expression of the property.</param>
/// <param name="apiTypeModifiers">Modifiers applied to the property (e.g., Required).</param>
/// <param name="clrName">The CLR property name corresponding to this API property.</param>
/// <exception cref="ArgumentNullException">
///     Thrown if <paramref name="apiName"/>, <paramref name="apiTypeExpression"/>, or <paramref name="clrName"/> is null.
/// </exception>
public sealed class ApiProperty(string apiName, ApiTypeExpression apiTypeExpression, ApiTypeModifiers apiTypeModifiers, string clrName) : ExtensibleBase
{
    #region ApiProperty Properties
    /// <summary>Gets the API name of the property (used in API requests/responses).</summary>
    public string ApiName { get; } = apiName ?? throw new ArgumentNullException(nameof(apiName));

    /// <summary>
    ///     Gets the API type expression to the API type of this property.
    ///     May point to a named type or inline type (e.g., collection).
    /// </summary>
    public ApiTypeExpression ApiTypeExpression { get; } = apiTypeExpression ?? throw new ArgumentNullException(nameof(apiTypeExpression));

    /// <summary>Gets the modifiers applied to this property (e.g., Required).</summary>
    public ApiTypeModifiers ApiTypeModifiers { get; } = apiTypeModifiers;

    /// <summary>Gets the CLR name of the property (matching the C# property name).</summary>
    public string ClrName { get; } = clrName ?? throw new ArgumentNullException(nameof(clrName));

    /// <summary>
    ///     Gets the resolved API type (named or inline) associated with this property.
    /// </summary>
    public ApiType ApiType => ApiTypeExpression.ApiResolvedType ?? throw new ApiSchemaException($"{nameof(ApiTypeExpression)} has not been resolved yet.");
    #endregion

    #region ApiProperty Methods
    /// <summary>
    ///     Resolves the API named type (or inline type) from the provided schema.
    /// </summary>
    /// <param name="apiSchema">The API schema to resolve the type from.</param>
    public void Resolve(ApiSchema apiSchema, ref List<ValidationResult>? results) => ApiTypeExpression.Resolve(apiSchema, ref results);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiTypeExpression = this.ApiTypeExpression.SafeToString();
        var apiTypeModifiers = this.ApiTypeModifiers.SafeToString();
        var clrName = this.ClrName.SafeToString();

        return $"{nameof(ApiProperty)} {{{nameof(ApiName)}={apiName}, {nameof(ApiTypeExpression)}={apiTypeExpression}, {nameof(ApiTypeModifiers)}={apiTypeModifiers}, {nameof(ClrName)}={clrName}}}";
    }
    #endregion
}
