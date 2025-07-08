// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed class ApiSchemaBuilderContext
{
    private readonly Dictionary<Type, IApiNamedTypeBuilder> _clrTypeToBuilder = new();

    /// <summary>
    /// Registers a builder for a CLR type.
    /// </summary>
    public void RegisterApiType<TBuilder>(Type clrType, TBuilder builder)
        where TBuilder : IApiNamedTypeBuilder
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(builder);

        if (_clrTypeToBuilder.ContainsKey(clrType))
            throw new InvalidOperationException($"CLR type '{clrType.FullName}' is already registered.");

        _clrTypeToBuilder[clrType] = builder!;
    }

    /// <summary>
    /// Returns whether a CLR type has been registered.
    /// </summary>
    public bool IsRegistered(Type clrType) => _clrTypeToBuilder.ContainsKey(clrType);

    /// <summary>
    /// Attempts to get the registered builder for a CLR type.
    /// </summary>
    public bool TryGetBuilder(Type clrType, out IApiNamedTypeBuilder? builder) =>
        _clrTypeToBuilder.TryGetValue(clrType, out builder);

    /// <summary>
    /// Attempts to get the API name associated with the CLR type.
    /// </summary>
    public bool TryGetApiName(Type clrType, out string? apiName)
    {
        if (!_clrTypeToBuilder.TryGetValue(clrType, out var builder))
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
}
