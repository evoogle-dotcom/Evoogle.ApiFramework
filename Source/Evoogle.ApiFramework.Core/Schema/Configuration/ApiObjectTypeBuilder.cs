// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

using System;
using System.Collections.Generic;

using Evoogle.ApiFramework.Schema;

public sealed class ApiObjectTypeBuilder(Type clrType, ApiSchemaBuilderContext context)
    : ApiNamedTypeBuilder<ApiObjectTypeBuilder>(clrType, context)
{
    #region Fields
    private readonly List<ApiProperty> _properties = [];
    private readonly List<ApiRelationship> _relationships = [];
    #endregion

    #region Builder Methods
    public ApiObjectTypeBuilder AddProperty(string apiName, string clrName, Type clrType, Action<ApiTypeModifiersBuilder>? modifiers = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentException.ThrowIfNullOrWhiteSpace(clrName, nameof(clrName));
        ArgumentNullException.ThrowIfNull(clrType, nameof(clrType));

        var typeExpression = ApiTypeExpressionBuilder.FromClrType(clrType, this.Context);

        var modifierBuilder = new ApiTypeModifiersBuilder();
        modifiers?.Invoke(modifierBuilder);

        var property = new ApiProperty(apiName, typeExpression, modifierBuilder.Build(), clrName);
        _properties.Add(property);

        return this;
    }

    public ApiObjectTypeBuilder AddRelationship(string apiName, string? apiPropertyName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        var relationship = new ApiRelationship(apiName, apiPropertyName);
        _relationships.Add(relationship);

        return this;
    }

    public ApiObjectType Build()
    {
        var objectType = new ApiObjectType
        (
            apiName: this.ApiName,
            apiProperties: _properties,
            apiRelationships: _relationships,
            clrObjectType: this.ClrType
        );

        return objectType;
    }
    #endregion
}
