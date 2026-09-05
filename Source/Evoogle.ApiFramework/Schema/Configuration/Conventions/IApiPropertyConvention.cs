// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Convention applied to each <see cref="ApiPropertyBuilder"/> on every registered object type,
///     after <see cref="IApiObjectTypeConvention"/> instances have run.
/// </summary>
/// <remarks>
///     May configure naming, modifiers, or extensions on the property, or add properties through
///     the supplied context. Newly added properties receive property conventions in a later pass;
///     properties on newly registered object types are processed only after that type's
///     <see cref="IApiObjectTypeConvention"/> instances have run.
/// </remarks>
public interface IApiPropertyConvention : IApiConvention
{
    #region Methods
    /// <summary>
    ///     Applies the convention to the supplied property builder.
    /// </summary>
    /// <param name="builder">The property builder to apply the convention to.</param>
    /// <param name="context">Contextual CLR metadata for the property.</param>
    void Apply(ApiPropertyBuilder builder, ApiPropertyConventionContext context);
    #endregion
}
