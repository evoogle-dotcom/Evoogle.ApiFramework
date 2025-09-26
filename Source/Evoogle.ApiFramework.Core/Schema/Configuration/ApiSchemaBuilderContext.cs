// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Logging;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Maintains shared state for <see cref="ApiSchemaBuilder"/> while schema components are being configured.
///     The context caches builder instances and exposes a consolidated logger to provide consistent diagnostics.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during schema construction.</param>
public sealed class ApiSchemaBuilderContext(ILogger<ApiSchemaBuilder>? logger = null)
{
    #region Fields
    private readonly Dictionary<Type, ApiScalarTypeBuilder> _apiScalarTypeBuilders = [];
    private readonly Dictionary<Type, ApiEnumTypeBuilder> _apiEnumTypeBuilders = [];
    private readonly Dictionary<Type, ApiObjectTypeBuilder> _apiObjectTypeBuilders = [];
    #endregion

    #region Properties
    internal IEnumerable<ApiScalarTypeBuilder> ApiScalarTypeBuilders => _apiScalarTypeBuilders.Values;
    internal IEnumerable<ApiEnumTypeBuilder> ApiEnumTypeBuilders => _apiEnumTypeBuilders.Values;
    internal IEnumerable<ApiObjectTypeBuilder> ApiObjectTypeBuilders => _apiObjectTypeBuilders.Values;

    internal ILogger<ApiSchemaBuilder> Logger { get; } = new MultiplexingLogger<ApiSchemaBuilder>(logger, MultiplexingLoggerMode.All);
    #endregion

    #region Methods
    /// <summary>
    ///     Gets existing or adds new <see cref="ApiEnumTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR enum type.</param>
    /// <returns>The corresponding <see cref="ApiEnumTypeBuilder"/>.</returns>
    internal ApiEnumTypeBuilder GetOrAddEnumTypeBuilder(Type clrType) =>
        GetOrAddBuilder(clrType, _apiEnumTypeBuilders, static (t, ctx) => new ApiEnumTypeBuilder(t, ctx), this);

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiObjectTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR object type.</param>
    /// <returns>The corresponding <see cref="ApiObjectTypeBuilder"/>.</returns>
    internal ApiObjectTypeBuilder GetOrAddObjectTypeBuilder(Type clrType) =>
        GetOrAddBuilder(clrType, _apiObjectTypeBuilders, static (t, ctx) => new ApiObjectTypeBuilder(t, ctx), this);

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiScalarTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR scalar type.</param>
    /// <returns>The corresponding <see cref="ApiScalarTypeBuilder"/>.</returns>
    internal ApiScalarTypeBuilder GetOrAddScalarTypeBuilder(Type clrType) =>
        GetOrAddBuilder(clrType, _apiScalarTypeBuilders, static (t, ctx) => new ApiScalarTypeBuilder(t, ctx), this);

    private static TBuilder GetOrAddBuilder<TBuilder>
    (
        Type clrType,
        Dictionary<Type, TBuilder> builders,
        Func<Type, ApiSchemaBuilderContext, TBuilder> factory,
        ApiSchemaBuilderContext context
    )
    {
        ArgumentNullException.ThrowIfNull(clrType);

        if (!builders.TryGetValue(clrType, out var builder))
        {
            builder = factory(clrType, context);
            builders[clrType] = builder;
        }

        return builder;
    }
    #endregion
}
