// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Convention applied to each <see cref="ApiScalarTypeBuilder"/> after its CLR type is registered.
/// </summary>
public interface IApiScalarTypeConvention : IApiConvention
{
    #region Methods
    /// <summary>
    ///     Applies the convention to the supplied scalar type builder.
    /// </summary>
    /// <param name="builder">The scalar type builder to apply the convention to.</param>
    void Apply(ApiScalarTypeBuilder builder);
    #endregion
}
