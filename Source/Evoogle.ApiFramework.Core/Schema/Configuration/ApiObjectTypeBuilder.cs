// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

using System;
using System.Collections.Generic;

using Evoogle.ApiFramework.Schema;

public sealed class ApiObjectTypeBuilder : IApiNamedTypeBuilder
{
    public string ApiName => _apiName;
    public Type ClrType => _clrType ?? typeof(object);

    private readonly string _apiName;
    private readonly ApiSchemaBuilderContext _context;
    private readonly List<ApiProperty> _properties = new();
    private readonly List<ApiRelationship> _relationships = new();
    private Type? _clrType;

    public ApiObjectTypeBuilder(string apiName, ApiSchemaBuilderContext context)
    {
        _apiName = apiName ?? throw new ArgumentNullException(nameof(apiName));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public ApiObjectTypeBuilder WithClrType(Type clrType)
    {
        _clrType = clrType ?? throw new ArgumentNullException(nameof(clrType));
        return this;
    }

    public ApiObjectTypeBuilder AddProperty(string apiName, string clrName, Type clrType, Action<ApiTypeModifiersBuilder>? modifiers = null)
    {
        if (string.IsNullOrWhiteSpace(apiName))
            throw new ArgumentException("API property name cannot be null or whitespace.", nameof(apiName));
        if (string.IsNullOrWhiteSpace(clrName))
            throw new ArgumentException("CLR property name cannot be null or whitespace.", nameof(clrName));

        var typeExpression = ApiTypeExpressionBuilder.FromClrType(clrType, _context);

        var modifierBuilder = new ApiTypeModifiersBuilder();
        modifiers?.Invoke(modifierBuilder);

        var property = new ApiProperty(apiName, typeExpression, modifierBuilder.Build(), clrName);
        _properties.Add(property);

        return this;
    }

    public ApiObjectTypeBuilder AddRelationship(string apiName, string? apiPropertyName = null)
    {
        if (string.IsNullOrWhiteSpace(apiName))
            throw new ArgumentException("API relationship name cannot be null or whitespace.", nameof(apiName));

        var relationship = new ApiRelationship(apiName, apiPropertyName);
        _relationships.Add(relationship);

        return this;
    }

    public ApiObjectType Build()
    {
        var objectType = new ApiObjectType(
            apiName: _apiName,
            apiProperties: _properties,
            apiRelationships: _relationships,
            clrObjectType: ClrType
        );

        return objectType;
    }
}
