// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>Reports a structural registration attempt.</summary>
public sealed record ApiSchemaBuildStructuralRegistrationEvent : ApiSchemaBuildTraceEvent
{
    /// <summary>Gets the structural registration kind.</summary>
    public required ApiSchemaBuildRegistrationKind RegistrationKind { get; init; }

    /// <summary>Gets whether the registration created a new builder.</summary>
    public required bool WasRegistered { get; init; }

    /// <summary>Gets the configuration source associated with the registration.</summary>
    public required ApiSchemaBuildConfigurationSource ConfigurationSource { get; init; }

    /// <summary>Gets the optional CLR ordinal for an enum-value registration.</summary>
    public int? ClrOrdinal { get; init; }

    /// <summary>Gets the reason a registration was ignored, when applicable.</summary>
    public string? RejectionReason { get; init; }
}
