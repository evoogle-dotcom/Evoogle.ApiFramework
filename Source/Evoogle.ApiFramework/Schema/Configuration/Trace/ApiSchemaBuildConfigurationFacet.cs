// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>
///     Identifies the configurable facet reported by a build trace event.
/// </summary>
public enum ApiSchemaBuildConfigurationFacet
{
    /// <summary>Reports an API name change attempt.</summary>
    ApiName = 0,

    /// <summary>Reports an API type-modifier change attempt.</summary>
    Modifiers = 1,

    /// <summary>Reports an options change attempt.</summary>
    Options = 2,

    /// <summary>Reports a key-path change attempt.</summary>
    KeyPath = 3,

    /// <summary>Reports a relationship key or behavior change attempt.</summary>
    Relationship = 4,

    /// <summary>Reports an extension change attempt.</summary>
    Extension = 5,

    /// <summary>Reports a configuration facet not represented by another member.</summary>
    Other = 6,
}
