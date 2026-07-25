// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Convention that runs first in the pipeline with access to the full
///     <see cref="ApiSchemaBuilder"/>.
/// </summary>
/// <remarks>
///     Intended for assembly scanning and bulk type registration.
///     May call <c>AddObject</c>, <c>AddScalar</c>, and similar methods to discover types.
/// </remarks>
public interface IApiSchemaConvention : IApiConvention
{
    /// <inheritdoc />
    ApiConventionPhase IApiConvention.Phase => ApiConventionPhase.Discovery;

    #region Methods
    /// <summary>
    ///     Applies the discovery convention to the schema being built.
    /// </summary>
    /// <param name="builder">The schema builder to apply the convention to.</param>
    void Apply(ApiSchemaBuilder builder);
    #endregion
}
