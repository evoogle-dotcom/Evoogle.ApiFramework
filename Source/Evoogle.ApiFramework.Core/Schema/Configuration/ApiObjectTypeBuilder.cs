// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

using System;
using System.Collections.Generic;

using Evoogle.ApiFramework.Schema;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiObjectType"/>.
/// </summary>
/// <param name="clrType">The CLR type represented by the API object type.</param>
/// <param name="context">The shared builder context.</param>
public sealed class ApiObjectTypeBuilder(Type clrType, ApiSchemaBuilderContext context)
    : ApiNamedTypeBuilder<ApiObjectTypeBuilder>(clrType, context)
{
    #region Fields
    private readonly List<ApiProperty> _properties = [];
    private readonly List<ApiRelationship> _relationships = [];
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition to the object type.
    /// </summary>
    /// <param name="apiName">The API property name.</param>
    /// <param name="clrName">The CLR property name.</param>
    /// <param name="clrType">The CLR type of the property.</param>
    /// <param name="modifiers">Optional callback to configure type modifiers.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddProperty(string apiName, string clrName, Type clrType, Action<ApiTypeModifiersBuilder>? modifiers = null)
    {
        var apiTypeExpression = ApiTypeExpressionBuilder.FromClrType(clrType, this.Context);

        var modifierBuilder = new ApiTypeModifiersBuilder();
        modifiers?.Invoke(modifierBuilder);

        var property = new ApiProperty(apiName, apiTypeExpression, modifierBuilder.Build(), clrName);
        _properties.Add(property);

        return this;
    }

    /// <summary>
    ///     Adds a relationship definition to the object type.
    /// </summary>
    /// <param name="apiName">The API name of the relationship.</param>
    /// <param name="apiPropertyName">Optional API property name backing the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddRelationship(string apiName, string? apiPropertyName = null)
    {
        var relationship = new ApiRelationship(apiName, apiPropertyName);
        _relationships.Add(relationship);

        return this;
    }

    /// <summary>
    ///     Builds the <see cref="ApiObjectType"/> using the configured properties and relationships.
    /// </summary>
    /// <returns>The constructed <see cref="ApiObjectType"/>.</returns>
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
