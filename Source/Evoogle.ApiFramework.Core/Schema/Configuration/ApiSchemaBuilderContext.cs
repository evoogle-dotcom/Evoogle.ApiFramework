// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Internal context used by <see cref="ApiSchemaBuilder"/> for tracking registered type builders.
/// </summary>
public sealed class ApiSchemaBuilderContext
{
    #region Fields
    private readonly Dictionary<Type, IApiNamedTypeBuilder> _namedTypeBuilders = [];

    private readonly Dictionary<Type, ApiScalarTypeBuilder> _scalarTypeBuilders = [];
    private readonly Dictionary<Type, ApiEnumTypeBuilder> _enumTypeBuilders = [];
    private readonly Dictionary<Type, ApiObjectTypeBuilder> _objectTypeBuilders = [];
    #endregion

    #region Methods
    /// <summary>
    ///     Retrieves an existing <see cref="ApiEnumTypeBuilder"/> for the specified CLR type or creates one.
    /// </summary>
    /// <param name="clrType">The CLR enum type.</param>
    /// <returns>The corresponding <see cref="ApiEnumTypeBuilder"/>.</returns>
    public ApiEnumTypeBuilder GetOrAddEnumTypeBuilder(Type clrType)
    {
        ArgumentNullException.ThrowIfNull(clrType);

        if (!_enumTypeBuilders.TryGetValue(clrType, out var builder))
        {
            builder = new ApiEnumTypeBuilder(clrType, this);
            _enumTypeBuilders[clrType] = builder;
            this.RegisterApiType(clrType, builder);
        }

        return builder;
    }

    /// <summary>
    ///     Retrieves an existing <see cref="ApiObjectTypeBuilder"/> for the specified CLR type or creates one.
    /// </summary>
    /// <param name="clrType">The CLR object type.</param>
    /// <returns>The corresponding <see cref="ApiObjectTypeBuilder"/>.</returns>
    public ApiObjectTypeBuilder GetOrAddObjectTypeBuilder(Type clrType)
    {
        ArgumentNullException.ThrowIfNull(clrType);

        if (!_objectTypeBuilders.TryGetValue(clrType, out var builder))
        {
            builder = new ApiObjectTypeBuilder(clrType, this);
            _objectTypeBuilders[clrType] = builder;
            this.RegisterApiType(clrType, builder);
        }

        return builder;
    }

    /// <summary>
    ///     Retrieves an existing <see cref="ApiScalarTypeBuilder"/> for the specified CLR type or creates one.
    /// </summary>
    /// <param name="clrType">The CLR scalar type.</param>
    /// <returns>The corresponding <see cref="ApiScalarTypeBuilder"/>.</returns>
    public ApiScalarTypeBuilder GetOrAddScalarTypeBuilder(Type clrType)
    {
        ArgumentNullException.ThrowIfNull(clrType);

        if (!_scalarTypeBuilders.TryGetValue(clrType, out var builder))
        {
            builder = new ApiScalarTypeBuilder(clrType, this);
            _scalarTypeBuilders[clrType] = builder;
            this.RegisterApiType(clrType, builder);
        }

        return builder;
    }

    /// <summary>
    ///     Determines whether the specified CLR type has been registered.
    /// </summary>
    /// <param name="clrType">The CLR type to check.</param>
    /// <returns><c>true</c> if the type is registered; otherwise <c>false</c>.</returns>
    public bool IsRegistered(Type clrType) => _namedTypeBuilders.ContainsKey(clrType);

    /// <summary>
    ///     Registers the builder for a CLR type. Throws if the type is already registered.
    /// </summary>
    /// <typeparam name="TBuilder">The builder type.</typeparam>
    /// <param name="clrType">The CLR type.</param>
    /// <param name="builder">The builder instance.</param>
    public void RegisterApiType<TBuilder>(Type clrType, TBuilder builder)
        where TBuilder : IApiNamedTypeBuilder
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(builder);

        if (_namedTypeBuilders.ContainsKey(clrType))
            throw new InvalidOperationException($"CLR type '{clrType.FullName}' is already registered.");

        _namedTypeBuilders[clrType] = builder!;
    }

    /// <summary>
    ///     Attempts to resolve the API name for a CLR type if it has been registered.
    /// </summary>
    /// <param name="clrType">The CLR type.</param>
    /// <param name="apiName">When this method returns, contains the API name if found.</param>
    /// <returns><c>true</c> if the type is registered; otherwise <c>false</c>.</returns>
    public bool TryGetApiName(Type clrType, out string? apiName)
    {
        if (!_namedTypeBuilders.TryGetValue(clrType, out var builder))
        {
            apiName = null;
            return false;
        }

        apiName = builder switch
        {
            ApiScalarTypeBuilder sb => sb.ApiName,
            ApiEnumTypeBuilder eb => eb.ApiName,
            ApiObjectTypeBuilder ob => ob.ApiName,
            _ => null
        };

        return apiName != null;
    }

    /// <summary>
    ///     Attempts to retrieve the registered builder for a CLR type.
    /// </summary>
    /// <param name="clrType">The CLR type.</param>
    /// <param name="builder">When this method returns, contains the builder if found.</param>
    /// <returns><c>true</c> if the builder exists; otherwise <c>false</c>.</returns>
    public bool TryGetBuilder(Type clrType, [NotNullWhen(true)] out IApiNamedTypeBuilder? builder) =>
        _namedTypeBuilders.TryGetValue(clrType, out builder);
    #endregion
}
