// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Convention that produces API names for schema elements that support convention-based
///     naming.
/// </summary>
public interface IApiNamingConvention : IApiConvention
{
    #region Methods
    /// <summary>
    ///     Converts or replaces the current candidate API name for the supplied naming target.
    /// </summary>
    /// <param name="apiName">
    ///     The current candidate API name. Implementations may transform this value for
    ///     composable naming, or ignore it and derive a replacement name from
    ///     <paramref name="context"/>.
    /// </param>
    /// <param name="context">
    ///     The naming context for the element being configured, including CLR metadata that can
    ///     be used to derive a replacement API name.
    /// </param>
    /// <returns>The API name to apply to the naming target.</returns>
    string ConvertName(string apiName, ApiNamingConventionContext context);
    #endregion
}
