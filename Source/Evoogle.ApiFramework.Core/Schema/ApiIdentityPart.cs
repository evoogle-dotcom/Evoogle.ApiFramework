// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Json;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Abstract base class for all identity part definitions within an <see cref="ApiIdentity"/>.
///     Each part describes a single contributing element of a composite or scalar object identity.
/// </summary>
[JsonConverter(typeof(ApiIdentityPartJsonConverter))]
public abstract class ApiIdentityPart : ApiSchemaElement
{
    #region ApiIdentityPart Properties
    /// <summary>Gets the kind of this identity part, which determines how its value is resolved at runtime.</summary>
    public abstract ApiIdentityPartKind ApiKind { get; }
    #endregion
}
