// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>
///     Receives structured events while an API schema is being built.
/// </summary>
public interface IApiSchemaBuildTraceSink
{
    /// <summary>Records one schema-build trace event.</summary>
    /// <param name="traceEvent">The event to record.</param>
    void Record(ApiSchemaBuildTraceEvent traceEvent);
}
