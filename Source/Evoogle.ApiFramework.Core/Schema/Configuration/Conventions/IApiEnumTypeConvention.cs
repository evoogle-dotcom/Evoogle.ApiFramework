// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Convention applied to each <see cref="ApiEnumTypeBuilder"/> after its CLR type is registered.
/// </summary>
public interface IApiEnumTypeConvention : IApiConvention
{
    #region Methods
    /// <summary>
    ///     Applies the convention to the supplied enum type builder.
    /// </summary>
    /// <param name="builder">The enum type builder to apply the convention to.</param>
    void Apply(ApiEnumTypeBuilder builder);
    #endregion
}
