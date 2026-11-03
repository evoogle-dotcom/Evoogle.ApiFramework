// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Relationships;

/// <summary>
///     Supplies the initial API name for an independent relationship declaration.
/// </summary>
public interface IApiRelationshipConfiguration : IApiConfiguration
{
    #region Properties
    /// <summary>
    ///     Gets the schema-unique API name of the relationship.
    /// </summary>
    string ApiName { get; }
    #endregion
}
