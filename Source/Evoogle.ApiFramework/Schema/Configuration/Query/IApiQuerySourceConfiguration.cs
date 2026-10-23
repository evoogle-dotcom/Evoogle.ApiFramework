// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Query;

/// <summary>
///     Identifies and configures an API schema query source by its API name.
/// </summary>
/// <remarks>
///     Register an instance with <see cref="ApiSchemaBuilder.AddQuerySource(IApiQuerySourceConfiguration)"/>
///     or discover public implementations with a public parameterless constructor through assembly configuration discovery.
///     Discovered implementations run at explicit precedence.
/// </remarks>
public interface IApiQuerySourceConfiguration : IApiConfiguration
{
    #region Properties
    /// <summary>Gets the schema-unique API name of the query source.</summary>
    string ApiName { get; }
    #endregion

    #region Methods
    /// <summary>Applies configuration to the supplied query source builder.</summary>
    /// <param name="builder">The query source builder to configure.</param>
    void Configure(ApiQuerySourceBuilder builder);
    #endregion
}
