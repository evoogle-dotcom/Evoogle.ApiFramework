// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>Indicates that a convention pipeline phase started.</summary>
public sealed record ApiSchemaBuildPhaseStartedEvent : ApiSchemaBuildTraceEvent
{
    /// <summary>Gets the pipeline phase.</summary>
    public required ApiSchemaBuildPhase Phase { get; init; }

    /// <summary>Gets the zero-based phase iteration.</summary>
    public int Iteration { get; init; }
}
