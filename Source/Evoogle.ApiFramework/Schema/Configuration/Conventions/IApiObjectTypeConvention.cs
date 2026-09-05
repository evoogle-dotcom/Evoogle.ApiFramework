// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Convention applied to each <see cref="ApiObjectTypeBuilder"/> after its CLR type is registered.
/// </summary>
/// <remarks>
///     Runs after <see cref="IApiSchemaConvention"/> and before <see cref="IApiPropertyConvention"/>.
///     May add properties, keys, or configure options on the object type.
/// </remarks>
public interface IApiObjectTypeConvention : IApiConvention
{
    #region Methods
    /// <summary>
    ///     Applies the convention to the supplied object type builder.
    /// </summary>
    /// <param name="builder">The object type builder to apply the convention to.</param>
    void Apply(ApiObjectTypeBuilder builder);
    #endregion
}
