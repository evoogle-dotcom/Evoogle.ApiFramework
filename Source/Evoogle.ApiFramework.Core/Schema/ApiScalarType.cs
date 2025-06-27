// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API scalar type (e.g., string, number, boolean).
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiScalarType"/> class.
/// </remarks>
/// <param name="apiName">The API name of the scalar type.</param>
/// <param name="clrScalarType">The CLR type representing the scalar (e.g., typeof(string)).</param>
/// <exception cref="ArgumentNullException">Thrown if <paramref name="apiName"/> or <paramref name="clrScalarType"/> is null.</exception>
public sealed class ApiScalarType(string apiName, Type clrScalarType) : ApiNamedType(apiName, clrScalarType)
{
    #region ApiType Properties
    /// <inheritdoc/>
    public override ApiTypeKind Kind => ApiTypeKind.Scalar;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrType = this.ClrType.SafeToString();

        return $"{nameof(ApiScalarType)} {{{nameof(this.ApiName)}={apiName}}} [{clrType}]";
    }
    #endregion
}
