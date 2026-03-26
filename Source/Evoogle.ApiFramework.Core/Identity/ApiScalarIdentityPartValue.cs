// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Represents a scalar identity part value that holds an <see cref="ApiId"/> primitive value.
/// </summary>
/// <param name="apiName">The name of the identity part as declared in the schema.</param>
/// <param name="apiScalarValue">The scalar <see cref="ApiId"/> value. Use <see cref="ApiId.Empty"/> for unresolved parts.</param>
public sealed class ApiScalarIdentityPartValue(string apiName, ApiId apiScalarValue) : ApiIdentityPartValue(apiName)
{
    #region ApiIdentityPartValue Properties
    /// <inheritdoc/>
    public override ApiIdentityPartValueKind ApiKind => ApiIdentityPartValueKind.Scalar;
    #endregion

    #region ApiScalarIdentityPartValue Properties
    /// <summary>Gets the scalar <see cref="ApiId"/> value for this identity part.</summary>
    public ApiId ApiScalarValue { get; } = apiScalarValue;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiPropertyName = this.ApiScalarValue.SafeToString();

        return $"{nameof(ApiScalarIdentityPartValue)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiScalarValue)}={apiPropertyName}}}";
    }
    #endregion
}
