// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Convention that runs last in the pipeline, after all type and property conventions have
///     completed and all types are fully settled.
/// </summary>
/// <remarks>
///     Intended for relationship inference or cross-type validation.
///     Relationship conventions may add or refine relationships, but must not register schema
///     types, properties, or enum values because structural configuration has already settled.
///
///     The framework does not provide built-in implementations of this interface in the current
///     release. Developers may implement it for custom relationship inference rules.
/// </remarks>
public interface IApiRelationshipConvention : IApiConvention
{
    /// <inheritdoc />
    ApiConventionPhase IApiConvention.Phase => ApiConventionPhase.Relationship;

    #region Methods
    /// <summary>
    ///     Applies the relationship convention to the schema being built.
    /// </summary>
    /// <param name="builder">The schema builder to apply the convention to.</param>
    void Apply(ApiSchemaBuilder builder);
    #endregion
}
