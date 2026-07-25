// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Convention applied to each <see cref="ApiEnumValueBuilder"/> after enum-type conventions
///     have run.
/// </summary>
public interface IApiEnumValueConvention : IApiConvention
{
    #region Methods
    /// <summary>
    ///     Applies the convention to the supplied enum-value builder.
    /// </summary>
    /// <param name="builder">The enum-value builder to apply the convention to.</param>
    /// <param name="context">Contextual CLR metadata for the enumeration value.</param>
    void Apply(ApiEnumValueBuilder builder, ApiEnumValueConventionContext context);
    #endregion
}
