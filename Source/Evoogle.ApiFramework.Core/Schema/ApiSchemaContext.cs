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
///     Provides runtime context and services for all API schema elements.
/// </summary>
/// <remarks>
///     This context is established once during schema initialization and provides
///     shared services like type coercion, logging, and configuration to all schema elements.
/// </remarks>
public sealed class ApiSchemaContext
{
    #region Properties
    /// <summary>
    ///     Gets the API schema associated with this context.
    /// </summary>
    public required ApiSchema ApiSchema { get; init; }

    /// <summary>
    ///     Gets the API schema options used for configuring schema behavior.
    /// </summary>
    public ApiSchemaOptions ApiSchemaOptions => this.ApiSchema.ApiOptions;

    /// <summary>
    ///     Gets the type coercion service for converting between types.
    /// </summary>
    public TypeCoercion TypeCoercion { get; init; } = new();

    /// <summary>
    ///     Gets the default type coercion context for coercion operations.
    /// </summary>
    public TypeCoercionContext TypeCoercionContext { get; init; } = TypeCoercionContext.Default;

    /// <summary>
    ///     Gets the optional logger factory for diagnostic logging during schema operations.
    /// </summary>
    public ILoggerFactory? LoggerFactory { get; init; }

    /// <summary>
    ///     Gets the multiplexing logger mode for diagnostic output.
    /// </summary>
    public MultiplexingLoggerMode LoggerMode { get; init; } = MultiplexingLoggerMode.All;

    /// <summary>
    ///     Gets the multiplexing logger for schema operations.
    /// </summary>
    /// <remarks>
    ///     This logger always returns a non-null instance and can output to multiple targets
    ///     based on the configured <see cref="LoggerMode"/>.
    /// </remarks>
    public ILogger Logger { get; private set; } = default!;
    #endregion

    #region Methods
    /// <summary>
    ///     Creates a logger for the specified type.
    /// </summary>
    internal ILogger<TCategory>? CreateLogger<TCategory>() => this.LoggerFactory?.CreateLogger<TCategory>();

    /// <summary>
    ///     Initializes the logger for this context.
    /// </summary>
    /// <remarks>
    ///     This method is called internally during schema initialization.
    /// </remarks>
    internal void InitializeLogger()
    {
        var innerLogger = this.LoggerFactory?.CreateLogger<ApiSchema>();
        this.Logger = new MultiplexingLogger(innerLogger, this.LoggerMode);
    }
    #endregion
}
