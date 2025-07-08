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

    private readonly Dictionary<string, ApiScalarTypeBuilder> _scalarBuilders = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, ApiEnumTypeBuilder> _enumBuilders = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, ApiObjectTypeBuilder> _objectTypeBuilders = new(StringComparer.OrdinalIgnoreCase);

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

    public ApiSchemaBuilder AddEnum(string apiName, Type clrType, Action<ApiEnumTypeBuilder>? configure = null)
    {
        if (!_enumBuilders.TryGetValue(apiName, out var builder))
        {
            builder = new ApiEnumTypeBuilder(apiName, clrType);
            _enumBuilders[apiName] = builder;
        }

        configure?.Invoke(builder);
        _context.RegisterApiType(clrType, builder);
        return this;
    }

    public ApiSchemaBuilder AddObject(string apiName, Action<ApiObjectTypeBuilder> configure)
    {
        if (!_objectTypeBuilders.TryGetValue(apiName, out var builder))
        {
            builder = new ApiObjectTypeBuilder(apiName, _context);
            _objectTypeBuilders[apiName] = builder;
        }

        configure(builder);
        return this;
    }

    public ApiSchemaBuilder AddScalar(string apiName, Type clrType, Action<ApiScalarTypeBuilder>? configure = null)
    {
        if (!_scalarBuilders.TryGetValue(apiName, out var builder))
        {
            builder = new ApiScalarTypeBuilder(apiName, clrType);
            _scalarBuilders[apiName] = builder;
        }

        configure?.Invoke(builder);
        _context.RegisterApiType(clrType, builder); // Register builder now, resolve later in Build()
        return this;
    }

    public ApiSchema Build()
    {
        var scalarTypes = _scalarBuilders.Values.Select(b => b.Build()).ToList();
        var enumTypes = _enumBuilders.Values.Select(b => b.Build()).ToList();
        var objectTypes = _objectTypeBuilders.Values.Select(b => b.Build()).ToList();

        return new ApiSchema(_name, scalarTypes, enumTypes, objectTypes)
        {
            Version = _version
        };
    }
}
