// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the base class for API types that are identified by a unique API name.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiNamedType"/> class.
/// </remarks>
/// <param name="apiName">The API name of the type.</param>
/// <param name="clrType">The CLR type associated with this API type.</param>
/// <exception cref="ArgumentNullException">Thrown if <paramref name="apiName"/> is null.</exception>
public abstract class ApiNamedType(string apiName, Type clrType) : ApiType(clrType)
{
    #region ApiNamedType Properties
    /// <summary>Gets the API name of the type.</summary>    
    public string ApiName { get; } = apiName ?? throw new ArgumentNullException(nameof(apiName));
    #endregion
}
