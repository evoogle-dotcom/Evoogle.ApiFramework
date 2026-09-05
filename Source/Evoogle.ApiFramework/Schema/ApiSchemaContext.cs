// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Coercion;
using Evoogle.Logging;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Provides immutable runtime services shared by all elements in a frozen API schema.
/// </summary>
public sealed class ApiSchemaContext
{
    #region Properties
    /// <summary>Gets the API schema associated with this context.</summary>
    public ApiSchema ApiSchema { get; }

    /// <summary>Gets the API schema options used for configuring schema behavior.</summary>
    public ApiSchemaOptions ApiSchemaOptions => this.ApiSchema.ApiOptions;

    /// <summary>Gets the type coercion service for converting between types.</summary>
    public TypeCoercion TypeCoercion { get; }

    /// <summary>Gets the immutable default type coercion context.</summary>
    public TypeCoercionContext TypeCoercionContext { get; }

    /// <summary>Gets the optional logger factory for diagnostic logging.</summary>
    public ILoggerFactory? LoggerFactory { get; }

    /// <summary>Gets the multiplexing logger mode for diagnostic output.</summary>
    public MultiplexingLoggerMode LoggerMode { get; }

    /// <summary>Gets the logger for schema operations.</summary>
    public ILogger Logger { get; }
    #endregion

    #region Constructors
    internal ApiSchemaContext
    (
        ApiSchema apiSchema,
        TypeCoercion? typeCoercion = null,
        TypeCoercionContext? typeCoercionContext = null,
        ILoggerFactory? loggerFactory = null,
        MultiplexingLoggerMode loggerMode = MultiplexingLoggerMode.All
    )
    {
        ArgumentNullException.ThrowIfNull(apiSchema);

        this.ApiSchema = apiSchema;
        this.TypeCoercion = typeCoercion ?? new TypeCoercion();
        this.TypeCoercionContext = typeCoercionContext ?? TypeCoercionContext.Default;
        this.LoggerFactory = loggerFactory;
        this.LoggerMode = loggerMode;
        this.Logger = new MultiplexingLogger(loggerFactory?.CreateLogger<ApiSchema>(), loggerMode);
    }

    internal ApiSchemaContext(ApiSchema apiSchema, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(logger);

        this.ApiSchema = apiSchema;
        this.TypeCoercion = new TypeCoercion();
        this.TypeCoercionContext = TypeCoercionContext.Default;
        this.LoggerFactory = null;
        this.LoggerMode = MultiplexingLoggerMode.All;
        this.Logger = logger;
    }
    #endregion

    #region Methods
    /// <summary>Creates a logger for the specified category when a logger factory is configured.</summary>
    internal ILogger<TCategory>? CreateLogger<TCategory>() => this.LoggerFactory?.CreateLogger<TCategory>();
    #endregion
}
