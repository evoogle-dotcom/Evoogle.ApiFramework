// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Coercion;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Provides runtime context and services for all API schema operations.
/// </summary>
/// <remarks>
///     This context is established once during schema initialization and provides
///     shared services like type coercion, logging, and configuration to all schema elements.
/// </remarks>
public sealed class ApiSchemaContext
{
    #region Fields
    private static readonly ApiSchemaContext _default = new();
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the default <see cref="ApiSchemaContext"/> with standard configuration.
    /// </summary>
    public static ApiSchemaContext Default => _default;

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
    #endregion

    #region Methods
    /// <summary>
    ///     Creates a logger for the specified type.
    /// </summary>
    internal ILogger<T>? CreateLogger<T>() => this.LoggerFactory?.CreateLogger<T>();
    #endregion
}
