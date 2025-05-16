// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extension;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API property belonging to an API object type.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiProperty"/> class.
/// </remarks>
/// <param name="apiName">The API name of the property.</param>
/// <param name="apiType">The API type of the property.</param>
/// <param name="apiTypeModifiers">Modifiers applied to the property (e.g., Required).</param>
/// <param name="clrName">The CLR property name corresponding to this API property.</param>
/// <exception cref="ArgumentNullException">
///     Thrown if <paramref name="apiName"/>, <paramref name="apiType"/>, or <paramref name="clrName"/> is null.
/// </exception>
public sealed class ApiProperty(string apiName, ApiType apiType, ApiTypeModifiers apiTypeModifiers, string clrName) : ExtensibleBase
{
    #region ApiProperty Properties
    /// <summary>Gets the API name of the property (used in API requests/responses).</summary>    
    public string ApiName { get; } = apiName ?? throw new ArgumentNullException(nameof(apiName));

    /// <summary>Gets the API type of the property.</summary>
    public ApiType ApiType { get; } = apiType ?? throw new ArgumentNullException(nameof(apiType));

    /// <summary>Gets the modifiers applied to this property (e.g., Required).</summary>
    public ApiTypeModifiers ApiTypeModifiers { get; } = apiTypeModifiers;

    /// <summary>Gets the CLR name of the property (matching the C# property name).</summary>
    public string ClrName { get; } = clrName ?? throw new ArgumentNullException(nameof(clrName));
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiType = this.ApiType.SafeToString();
        var apiTypeModifiers = this.ApiTypeModifiers.SafeToString();
        var clrName = this.ClrName.SafeToString();

        return $"{nameof(ApiProperty)} {{{nameof(ApiName)}={apiName}, {nameof(ApiType)}={apiType}, {nameof(ApiTypeModifiers)}={apiTypeModifiers}, {nameof(ClrName)}={clrName}}}";
    }
    #endregion
}