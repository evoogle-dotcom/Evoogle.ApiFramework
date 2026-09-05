// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>
///     Collects schema-build trace events in memory so they can be inspected after a build completes.
/// </summary>
public sealed class ApiInMemorySchemaBuildTraceSink : IApiSchemaBuildTraceSink
{
    private readonly List<ApiSchemaBuildTraceEvent> _events = [];

    /// <inheritdoc/>
    public void Record(ApiSchemaBuildTraceEvent traceEvent)
    {
        ArgumentNullException.ThrowIfNull(traceEvent);

        _events.Add(traceEvent);
    }

    /// <summary>Creates an immutable snapshot of the events collected so far.</summary>
    /// <returns>A trace containing the collected events in build order.</returns>
    public ApiSchemaBuildTrace CreateTrace()
    {
        return new(_events);
    }
}
