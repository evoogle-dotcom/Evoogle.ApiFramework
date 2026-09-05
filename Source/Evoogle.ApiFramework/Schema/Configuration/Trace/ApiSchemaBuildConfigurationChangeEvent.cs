// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>Reports a configuration change attempt.</summary>
public sealed record ApiSchemaBuildConfigurationChangeEvent : ApiSchemaBuildTraceEvent
{
    /// <summary>Gets the configured facet.</summary>
    public required ApiSchemaBuildConfigurationFacet Facet { get; init; }

    /// <summary>Gets the attempted configuration source.</summary>
    public required ApiSchemaBuildConfigurationSource ConfigurationSource { get; init; }

    /// <summary>Gets the previous diagnostic value.</summary>
    public string? PreviousValue { get; init; }

    /// <summary>Gets the requested diagnostic value.</summary>
    public string? RequestedValue { get; init; }

    /// <summary>Gets the effective diagnostic value after the attempt.</summary>
    public string? EffectiveValue { get; init; }

    /// <summary>Gets whether the requested value was accepted.</summary>
    public required bool WasApplied { get; init; }

    /// <summary>Gets the reason the requested value was rejected, when applicable.</summary>
    public string? RejectionReason { get; init; }
}
