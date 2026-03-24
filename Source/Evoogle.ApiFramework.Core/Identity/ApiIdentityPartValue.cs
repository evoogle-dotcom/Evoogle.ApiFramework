// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Identity.Json;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Abstract base class for a single resolved part value within an <see cref="ApiIdentityValue"/>,
///     holding either a scalar <see cref="ApiId"/> or a nested <see cref="ApiIdentityValue"/>.
/// </summary>
/// <param name="apiName">The name of the identity part as declared in the schema.</param>
[JsonConverter(typeof(ApiIdentityPartValueJsonConverter))]
public abstract class ApiIdentityPartValue(string apiName)
{
    #region Properties
    /// <summary>Gets the name of the identity part as declared in the schema.</summary>
    public string ApiName { get; } = apiName;

    /// <summary>Gets the kind of value stored in this part: scalar or nested object.</summary>
    public abstract ApiIdentityPartValueKind ApiKind { get; }
    #endregion
}
