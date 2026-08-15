// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>Indicates that an API schema build failed.</summary>
public sealed record ApiSchemaBuildFailedEvent : ApiSchemaBuildTraceEvent
{
    /// <summary>Gets the failed exception type name.</summary>
    public required string ExceptionType { get; init; }

    /// <summary>Gets the failed exception message.</summary>
    public required string ExceptionMessage { get; init; }
}
