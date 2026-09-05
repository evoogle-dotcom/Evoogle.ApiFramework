// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Configuration.Trace.Internal;

internal static class ApiSchemaBuildTraceExtensions
{
    internal static ApiSchemaBuildConfigurationSource ToTraceSource
    (
        this ApiConfigurationSource source
    )
    {
        return source switch
        {
            ApiConfigurationSource.Convention => ApiSchemaBuildConfigurationSource.Convention,
            ApiConfigurationSource.DataAnnotation =>
                ApiSchemaBuildConfigurationSource.DataAnnotation,
            ApiConfigurationSource.Explicit => ApiSchemaBuildConfigurationSource.Explicit,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null),
        };
    }
}

/// <summary>
///     Assigns build-order sequence numbers and forwards schema-build trace events to an optional sink.
/// </summary>
internal sealed class ApiSchemaBuildTraceDispatcher
{
    #region Fields
    private readonly ILogger? _logger;
    private IApiSchemaBuildTraceSink? _sink;
    private long _sequence;
    #endregion

    #region Constructors
    internal ApiSchemaBuildTraceDispatcher
    (
        IApiSchemaBuildTraceSink? sink,
        ILogger? logger
    )
    {
        _sink = sink;
        _logger = logger;
    }
    #endregion

    #region Methods
    internal void Record(ApiSchemaBuildTraceEvent traceEvent)
    {
        ArgumentNullException.ThrowIfNull(traceEvent);

        traceEvent.Sequence = ++_sequence;

        if (_sink == null)
        {
            return;
        }

        try
        {
            _sink.Record(traceEvent);
        }
        catch (Exception exception)
        {
            _logger?.LogWarning
            (
                exception,
                "The API schema build trace sink failed and has been disabled."
            );
            _sink = null;
        }
    }
    #endregion
}
