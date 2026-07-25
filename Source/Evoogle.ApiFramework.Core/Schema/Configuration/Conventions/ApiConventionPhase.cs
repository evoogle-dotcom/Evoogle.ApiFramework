// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Identifies when a convention participates in the schema configuration pipeline.
/// </summary>
public enum ApiConventionPhase
{
    /// <summary>
    ///     Discovers schema structure, such as registered types or object properties, before
    ///     annotations and configuration conventions are applied.
    /// </summary>
    Discovery = 0,

    /// <summary>
    ///     Configures discovered type, enum-value, or property builders after their annotations
    ///     have been applied.
    /// </summary>
    Configuration = 1,

    /// <summary>
    ///     Infers or validates relationships after all types, properties, and relationship
    ///     annotations have settled.
    /// </summary>
    Relationship = 2,
}
