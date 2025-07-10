// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed class ApiSchemaBuilderContext
{
    #region Fields
    private readonly Dictionary<Type, IApiNamedTypeBuilder> _namedTypeBuilders = [];

    private readonly Dictionary<Type, ApiScalarTypeBuilder> _scalarTypeBuilders = [];
    private readonly Dictionary<Type, ApiEnumTypeBuilder> _enumTypeBuilders = [];
    private readonly Dictionary<Type, ApiObjectTypeBuilder> _objectTypeBuilders = [];
    #endregion

    #region Methods
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





    public bool IsRegistered(Type clrType) => _namedTypeBuilders.ContainsKey(clrType);

    public void RegisterApiType<TBuilder>(Type clrType, TBuilder builder)
        where TBuilder : IApiNamedTypeBuilder
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(builder);

        if (_namedTypeBuilders.ContainsKey(clrType))
            throw new InvalidOperationException($"CLR type '{clrType.FullName}' is already registered.");

        _namedTypeBuilders[clrType] = builder!;
    }

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

    public bool TryGetBuilder(Type clrType, [NotNullWhen(true)] out IApiNamedTypeBuilder? builder) =>
        _namedTypeBuilders.TryGetValue(clrType, out builder);
    #endregion
}
