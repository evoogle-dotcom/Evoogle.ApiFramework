// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Json;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Abstract base class for all FK key path definitions within an <see cref="ApiRelationshipDependentEnd"/>.
///     Each binding describes how one or more scalar leaves of the principal <see cref="ApiIdentity"/> map to
///     properties on the dependent object graph.
/// </summary>
[JsonConverter(typeof(ApiRelationshipKeyPathJsonConverter))]
public abstract class ApiRelationshipKeyPath : ApiSchemaElement
{
    #region ApiRelationshipKeyPath Properties
    /// <summary>Gets the kind of this key path, which determines how FK scalar values are located on the dependent object graph.</summary>
    public abstract ApiRelationshipKeyPathKind ApiKind { get; }
    #endregion
}
