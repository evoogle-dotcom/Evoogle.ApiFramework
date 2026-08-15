// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>Indicates that a convention invocation completed.</summary>
public sealed record ApiSchemaBuildConventionCompletedEvent : ApiSchemaBuildTraceEvent
{
    /// <summary>Gets the convention implementation type.</summary>
    public required Type ConventionType { get; init; }
}
