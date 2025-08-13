// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Extension methods for <see cref="ApiSchema"/> class.
/// </summary>
public static class ApiSchemaExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Gets an <see cref="ApiNamedType"/> by its API name.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="apiName">The API name of the type.</param>
    /// <returns>The matching <see cref="ApiNamedType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if no type with the specified API name exists in the schema.</exception>
    public static ApiNamedType GetApiType(this ApiSchema apiSchema, string apiName)
    {
        if (apiSchema.TryGetApiType(apiName, out var result))
            return result!;

        throw new ApiSchemaException($"{nameof(ApiNamedType)} with {nameof(ApiNamedType.ApiName)} '{apiName}' was not found in {apiSchema.SafeToString()}.");
    }

    /// <summary>
    ///     Gets an <see cref="ApiNamedType"/> by its CLR type.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="clrType">The CLR type of the target API type.</param>
    /// <returns>The matching <see cref="ApiNamedType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if no type with the specified CLR type exists in the schema.</exception>
    public static ApiNamedType GetApiType(this ApiSchema apiSchema, Type clrType)
    {
        if (apiSchema.TryGetApiType(clrType, out var result))
            return result!;

        throw new ApiSchemaException($"{nameof(ApiNamedType)} with {nameof(ApiType.ClrType)} '{clrType.FullName}' was not found in {apiSchema.SafeToString()}.");
    }

    /// <summary>
    ///     Gets an <see cref="ApiEnumType"/> by its API name.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="apiName">The API name of the enum type.</param>
    /// <returns>The matching <see cref="ApiEnumType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the enum type is not found in the schema.</exception>
    public static ApiEnumType GetApiEnumType(this ApiSchema apiSchema, string apiName)
    {
        if (apiSchema.TryGetApiEnumType(apiName, out var result))
            return result!;

        throw new ApiSchemaException($"{nameof(ApiEnumType)} with {nameof(ApiNamedType.ApiName)} '{apiName}' was not found in {apiSchema.SafeToString()}.");
    }

    /// <summary>
    ///     Gets an <see cref="ApiEnumType"/> by its CLR type.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="clrType">The CLR type of the enum.</param>
    /// <returns>The matching <see cref="ApiEnumType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the enum type is not found in the schema.</exception>
    public static ApiEnumType GetApiEnumType(this ApiSchema apiSchema, Type clrType)
    {
        if (apiSchema.TryGetApiEnumType(clrType, out var result))
            return result!;

        throw new ApiSchemaException($"{nameof(ApiEnumType)} with {nameof(ApiType.ClrType)} '{clrType.FullName}' was not found in {apiSchema.SafeToString()}.");
    }

    /// <summary>
    ///     Gets an <see cref="ApiObjectType"/> by its API name.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="apiName">The API name of the object type.</param>
    /// <returns>The matching <see cref="ApiObjectType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the object type is not found in the schema.</exception>
    public static ApiObjectType GetApiObjectType(this ApiSchema apiSchema, string apiName)
    {
        if (apiSchema.TryGetApiObjectType(apiName, out var result))
            return result!;

        throw new ApiSchemaException($"{nameof(ApiObjectType)} with {nameof(ApiNamedType.ApiName)} '{apiName}' was not found in {apiSchema.SafeToString()}.");
    }

    /// <summary>
    ///     Gets an <see cref="ApiObjectType"/> by its CLR type.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="clrType">The CLR type of the object.</param>
    /// <returns>The matching <see cref="ApiObjectType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the object type is not found in the schema.</exception>
    public static ApiObjectType GetApiObjectType(this ApiSchema apiSchema, Type clrType)
    {
        if (apiSchema.TryGetApiObjectType(clrType, out var result))
            return result!;

        throw new ApiSchemaException($"{nameof(ApiObjectType)} with {nameof(ApiType.ClrType)} '{clrType.FullName}' was not found in {apiSchema.SafeToString()}.");
    }

    /// <summary>
    ///     Gets an <see cref="ApiScalarType"/> by its API name.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="apiName">The API name of the scalar type.</param>
    /// <returns>The matching <see cref="ApiScalarType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the scalar type is not found in the schema.</exception>
    public static ApiScalarType GetApiScalarType(this ApiSchema apiSchema, string apiName)
    {
        if (apiSchema.TryGetApiScalarType(apiName, out var result))
            return result!;

        throw new ApiSchemaException($"{nameof(ApiScalarType)} with {nameof(ApiNamedType.ApiName)} '{apiName}' was not found in {apiSchema.SafeToString()}.");
    }

    /// <summary>
    ///     Gets an <see cref="ApiScalarType"/> by its CLR type.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="clrType">The CLR type of the scalar.</param>
    /// <returns>The matching <see cref="ApiScalarType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the scalar type is not found in the schema.</exception>
    public static ApiScalarType GetApiScalarType(this ApiSchema apiSchema, Type clrType)
    {
        if (apiSchema.TryGetApiScalarType(clrType, out var result))
            return result!;

        throw new ApiSchemaException($"{nameof(ApiScalarType)} with {nameof(ApiType.ClrType)} '{clrType.FullName}' was not found in {apiSchema.SafeToString()}.");
    }
    #endregion
}
