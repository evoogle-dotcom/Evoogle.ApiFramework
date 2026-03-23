// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Represents a single resolved part value within an <see cref="ApiIdentityValue"/> snapshot,
///     holding either a scalar <see cref="ApiId"/> or a nested <see cref="ApiIdentityValue"/>.
/// </summary>
public class ApiIdentityPartValue
{
    #region Properties
    /// <summary>Gets or sets the name of the identity part as declared in the schema.</summary>
    public string ApiName { get; set; } = null!;

    /// <summary>Gets or sets the kind of value stored in this part: scalar or nested object.</summary>
    public ApiIdentityPartValueKind ApiKind { get; set; }

    /// <summary>
    ///     Gets or sets the scalar <see cref="ApiId"/> value when <see cref="ApiKind"/> is <see cref="ApiIdentityPartValueKind.Scalar"/>.
    ///     <see langword="null"/> when <see cref="ApiKind"/> is <see cref="ApiIdentityPartValueKind.Object"/>.
    /// </summary>
    public ApiId? ApiScalarValue { get; set; }

    /// <summary>
    ///     Gets or sets the nested <see cref="ApiIdentityValue"/> when <see cref="ApiKind"/> is <see cref="ApiIdentityPartValueKind.Object"/>.
    ///     <see langword="null"/> when <see cref="ApiKind"/> is <see cref="ApiIdentityPartValueKind.Scalar"/>.
    /// </summary>
    public ApiIdentityValue? ApiObjectValue { get; set; }
    #endregion
}
