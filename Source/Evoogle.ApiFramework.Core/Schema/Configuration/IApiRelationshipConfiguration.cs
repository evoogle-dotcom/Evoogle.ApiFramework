// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Identifies an API schema relationship configuration by its API name.
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
