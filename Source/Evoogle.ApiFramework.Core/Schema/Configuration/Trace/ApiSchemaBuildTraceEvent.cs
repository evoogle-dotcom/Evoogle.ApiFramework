// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Conventions;

namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>
///     Represents an immutable event emitted while an API schema is being built.
/// </summary>
public abstract record ApiSchemaBuildTraceEvent
{
    /// <summary>Gets the one-based sequence number assigned within the build.</summary>
    public long Sequence { get; internal set; }

    /// <summary>Gets the optional schema target associated with the event.</summary>
    public ApiSchemaBuildTraceTarget? Target { get; init; }

    /// <summary>Gets the convention phase associated with the event, when applicable.</summary>
    public ApiConventionPhase? ConventionPhase { get; init; }
}
