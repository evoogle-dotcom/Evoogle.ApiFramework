// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed class ApiSchemaBuilder
{
    private string _name = "UnnamedSchema";
    private string? _version;

    private readonly ApiSchemaBuilderContext _context = new();

    private readonly Dictionary<Type, ApiScalarTypeBuilder> _scalarTypeBuilders = [];
    private readonly Dictionary<Type, ApiEnumTypeBuilder> _enumTypeBuilders = [];
    private readonly Dictionary<Type, ApiObjectTypeBuilder> _objectTypeBuilders = [];

    public ApiSchemaBuilder WithName(string name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        return this;
    }

    public ApiSchemaBuilder WithVersion(string version)
    {
        _version = version ?? throw new ArgumentNullException(nameof(version));
        return this;
    }

    public ApiSchemaBuilder AddEnum(Type clrType, Action<ApiEnumTypeBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(configure);

        var builder = _context.GetOrAddEnumTypeBuilder(clrType);

        configure(builder);
        return this;
    }

    public ApiSchemaBuilder AddEnum(Type clrType, IApiEnumTypeConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = _context.GetOrAddEnumTypeBuilder(clrType);

        configuration.Configure(builder);
        return this;
    }

    public ApiSchemaBuilder AddObject(Type clrType, Action<ApiObjectTypeBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(configure);

        var builder = _context.GetOrAddObjectTypeBuilder(clrType);

        configure(builder);
        return this;
    }

    public ApiSchemaBuilder AddObject(Type clrType, IApiObjectTypeConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = _context.GetOrAddObjectTypeBuilder(clrType);

        configuration.Configure(builder);
        return this;
    }

    public ApiSchemaBuilder AddScalar(Type clrType, Action<ApiScalarTypeBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(configure);

        var builder = _context.GetOrAddScalarTypeBuilder(clrType);

        configure(builder);
        return this;
    }

    public ApiSchemaBuilder AddScalar(Type clrType, IApiScalarTypeConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = _context.GetOrAddScalarTypeBuilder(clrType);

        configuration.Configure(builder);
        return this;
    }

    public ApiSchema Build()
    {
        var scalarTypes = _scalarTypeBuilders.Values.Select(b => b.Build()).ToList();
        var enumTypes = _enumTypeBuilders.Values.Select(b => b.Build()).ToList();
        var objectTypes = _objectTypeBuilders.Values.Select(b => b.Build()).ToList();

        return new ApiSchema(_name, scalarTypes, enumTypes, objectTypes)
        {
            ApiVersion = _version
        };
    }

    // private ApiEnumTypeBuilder GetOrAddEnumTypeBuilder(Type clrType)
    // {
    //     ArgumentNullException.ThrowIfNull(clrType);

    //     if (!_enumTypeBuilders.TryGetValue(clrType, out var builder))
    //     {
    //         builder = new ApiEnumTypeBuilder(clrType, _context);
    //         _enumTypeBuilders[clrType] = builder;
    //         _context.RegisterApiType(clrType, builder);
    //     }

    //     return builder;
    // }

    // private ApiObjectTypeBuilder GetOrAddObjectTypeBuilder(Type clrType)
    // {
    //     ArgumentNullException.ThrowIfNull(clrType);

    //     if (!_objectTypeBuilders.TryGetValue(clrType, out var builder))
    //     {
    //         builder = new ApiObjectTypeBuilder(clrType, _context);
    //         _objectTypeBuilders[clrType] = builder;
    //         _context.RegisterApiType(clrType, builder);
    //     }

    //     return builder;
    // }

    // private ApiScalarTypeBuilder GetOrAddScalarTypeBuilder(Type clrType)
    // {
    //     ArgumentNullException.ThrowIfNull(clrType);

    //     if (!_scalarTypeBuilders.TryGetValue(clrType, out var builder))
    //     {
    //         builder = new ApiScalarTypeBuilder(clrType, _context);
    //         _scalarTypeBuilders[clrType] = builder;
    //         _context.RegisterApiType(clrType, builder);
    //     }

    //     return builder;
    // }
}
