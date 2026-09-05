// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.ObjectModel;

namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>
///     Contains the immutable events emitted during an API schema build.
/// </summary>
public sealed class ApiSchemaBuildTrace
{
    /// <summary>Creates a trace from the supplied events.</summary>
    /// <param name="events">The events to retain.</param>
    public ApiSchemaBuildTrace(IEnumerable<ApiSchemaBuildTraceEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events);
        this.Events = new ReadOnlyCollection<ApiSchemaBuildTraceEvent>(events.ToArray());
    }

    /// <summary>Gets the events in deterministic build order.</summary>
    public IReadOnlyList<ApiSchemaBuildTraceEvent> Events { get; }
}
