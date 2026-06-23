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
    public static ApiNamedType GetTypeByApiName(this ApiSchema apiSchema, string apiName)
    {
        if (apiSchema.TryGetTypeByApiName(apiName, out var result))
        {
            return result;
        }

        var availableTypesByApiName = string.Join(", ", apiSchema.ApiNamedTypes.OrderBy(t => t.ApiName).Select(t => t.ApiName));
        var errorMessage =
            $"{nameof(ApiNamedType)} with {nameof(ApiNamedType.ApiName)} '{apiName}' not found in {apiSchema.SafeToString()}. " +
            $"Available {nameof(ApiNamedType)} by {nameof(ApiNamedType.ApiName)} are: {availableTypesByApiName}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiNamedType"/> by its CLR type.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="clrType">The CLR type of the target API type.</param>
    /// <returns>The matching <see cref="ApiNamedType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if no type with the specified CLR type exists in the schema.</exception>
    public static ApiNamedType GetTypeByClrType(this ApiSchema apiSchema, Type clrType)
    {
        if (apiSchema.TryGetTypeByClrType(clrType, out var result))
        {
            return result;
        }

        var errorMessage =
            $"{nameof(ApiNamedType)} with {nameof(ApiNamedType.ClrType)} '{clrType.SafeToName()}' not found in {apiSchema.SafeToString()}. " +
            $"Ensure the {nameof(ApiNamedType)} is registered in the {nameof(ApiSchema)}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiNamedType"/> by its CLR type using a generic type parameter.
    /// </summary>
    /// <typeparam name="TClr">The CLR type to search for in the schema.</typeparam>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <returns>The matching <see cref="ApiNamedType"/> for the specified CLR type.</returns>
    /// <exception cref="ApiSchemaException">Thrown if no type with the specified CLR type exists in the schema.</exception>
    /// <remarks>
    ///     This is a convenience overload that uses the generic type parameter to determine the CLR type,
    ///     avoiding the need to explicitly pass <c>typeof(TClr)</c>.
    /// </remarks>
    public static ApiNamedType GetTypeByClrType<TClr>(this ApiSchema apiSchema)
    {
        var clrType = typeof(TClr);
        return apiSchema.GetTypeByClrType(clrType);
    }

    /// <summary>
    ///     Gets an <see cref="ApiEnumType"/> by its API name.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="apiName">The API name of the enum type.</param>
    /// <returns>The matching <see cref="ApiEnumType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the enum type is not found in the schema.</exception>
    public static ApiEnumType GetEnumTypeByApiName(this ApiSchema apiSchema, string apiName)
    {
        if (apiSchema.TryGetEnumTypeByApiName(apiName, out var result))
        {
            return result;
        }

        var availableEnumTypesByApiName = string.Join(", ", apiSchema.ApiEnumTypes.OrderBy(t => t.ApiName).Select(t => t.ApiName));
        var errorMessage =
            $"{nameof(ApiEnumType)} with {nameof(ApiEnumType.ApiName)} '{apiName.SafeToString()}' not found in {apiSchema.SafeToString()}. " +
            $"Available {nameof(ApiEnumType)} by {nameof(ApiEnumType.ApiName)} are: {availableEnumTypesByApiName}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiEnumType"/> by its CLR type.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="clrType">The CLR type of the enum.</param>
    /// <returns>The matching <see cref="ApiEnumType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the enum type is not found in the schema.</exception>
    public static ApiEnumType GetEnumTypeByClrType(this ApiSchema apiSchema, Type clrType)
    {
        if (apiSchema.TryGetEnumTypeByClrType(clrType, out var result))
        {
            return result;
        }

        var errorMessage =
            $"{nameof(ApiEnumType)} with {nameof(ApiEnumType.ClrType)} '{clrType.SafeToName()}' not found in {apiSchema.SafeToString()}. " +
            $"Ensure the {nameof(ApiEnumType)} is registered in the {nameof(ApiSchema)}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiEnumType"/> by its CLR type using a generic type parameter.
    /// </summary>
    /// <typeparam name="TEnum">The CLR enum type to search for in the schema.</typeparam>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <returns>The matching <see cref="ApiEnumType"/> for the specified CLR enum type.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the enum type is not found in the schema.</exception>
    /// <remarks>
    ///     This is a convenience overload that uses the generic type parameter to determine the CLR enum type,
    ///     avoiding the need to explicitly pass <c>typeof(TEnum)</c>.
    /// </remarks>
    public static ApiEnumType GetEnumTypeByClrType<TEnum>(this ApiSchema apiSchema)
    {
        var clrEnumType = typeof(TEnum);
        return apiSchema.GetEnumTypeByClrType(clrEnumType);
    }

    /// <summary>
    ///     Gets an <see cref="ApiObjectType"/> by its API name.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="apiName">The API name of the object type.</param>
    /// <returns>The matching <see cref="ApiObjectType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the object type is not found in the schema.</exception>
    public static ApiObjectType GetObjectTypeByApiName(this ApiSchema apiSchema, string apiName)
    {
        if (apiSchema.TryGetObjectTypeByApiName(apiName, out var result))
        {
            return result;
        }

        var availableObjectTypesByApiName = string.Join(", ", apiSchema.ApiObjectTypes.OrderBy(t => t.ApiName).Select(t => t.ApiName));
        var errorMessage =
            $"{nameof(ApiObjectType)} with {nameof(ApiObjectType.ApiName)} '{apiName.SafeToString()}' not found in {apiSchema.SafeToString()}. " +
            $"Available {nameof(ApiObjectType)} by {nameof(ApiObjectType.ApiName)} are: {availableObjectTypesByApiName}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiObjectType"/> by its CLR type.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="clrType">The CLR type of the object.</param>
    /// <returns>The matching <see cref="ApiObjectType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the object type is not found in the schema.</exception>
    public static ApiObjectType GetObjectTypeByClrType(this ApiSchema apiSchema, Type clrType)
    {
        if (apiSchema.TryGetObjectTypeByClrType(clrType, out var result))
        {
            return result;
        }

        var errorMessage =
            $"{nameof(ApiObjectType)} with {nameof(ApiObjectType.ClrType)} '{clrType.SafeToName()}' not found in {apiSchema.SafeToString()}. " +
            $"Ensure the {nameof(ApiObjectType)} is registered in the {nameof(ApiSchema)}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiObjectType"/> by its CLR type using a generic type parameter.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type to search for in the schema.</typeparam>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <returns>The matching <see cref="ApiObjectType"/> for the specified CLR object type.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the object type is not found in the schema.</exception>
    /// <remarks>
    ///     This is a convenience overload that uses the generic type parameter to determine the CLR object type,
    ///     avoiding the need to explicitly pass <c>typeof(TObject)</c>.
    /// </remarks>
    public static ApiObjectType GetObjectTypeByClrType<TObject>(this ApiSchema apiSchema)
    {
        var clrObjectType = typeof(TObject);
        return apiSchema.GetObjectTypeByClrType(clrObjectType);
    }

    /// <summary>
    ///     Gets an <see cref="ApiScalarType"/> by its API name.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="apiName">The API name of the scalar type.</param>
    /// <returns>The matching <see cref="ApiScalarType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the scalar type is not found in the schema.</exception>
    public static ApiScalarType GetScalarTypeByApiName(this ApiSchema apiSchema, string apiName)
    {
        if (apiSchema.TryGetScalarTypeByApiName(apiName, out var result))
        {
            return result;
        }

        var availableScalarTypesByApiName = string.Join(", ", apiSchema.ApiScalarTypes.OrderBy(t => t.ApiName).Select(t => t.ApiName));
        var errorMessage =
            $"{nameof(ApiScalarType)} with {nameof(ApiNamedType.ApiName)} '{apiName}' not found in {apiSchema.SafeToString()}. " +
            $"Available {nameof(ApiScalarType)} by {nameof(ApiScalarType.ApiName)} are: {availableScalarTypesByApiName}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiScalarType"/> by its CLR type.
    /// </summary>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <param name="clrType">The CLR type of the scalar.</param>
    /// <returns>The matching <see cref="ApiScalarType"/>.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the scalar type is not found in the schema.</exception>
    public static ApiScalarType GetScalarTypeByClrType(this ApiSchema apiSchema, Type clrType)
    {
        if (apiSchema.TryGetScalarTypeByClrType(clrType, out var result))
        {
            return result;
        }

        var errorMessage =
            $"{nameof(ApiScalarType)} with {nameof(ApiScalarType.ClrType)} '{clrType.SafeToName()}' not found in {apiSchema.SafeToString()}. " +
            $"Ensure the {nameof(ApiScalarType)} is registered in the {nameof(ApiSchema)}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiScalarType"/> by its CLR type using a generic type parameter.
    /// </summary>
    /// <typeparam name="TScalar">The CLR scalar type to search for in the schema.</typeparam>
    /// <param name="apiSchema">The API schema to search.</param>
    /// <returns>The matching <see cref="ApiScalarType"/> for the specified CLR scalar type.</returns>
    /// <exception cref="ApiSchemaException">Thrown if the scalar type is not found in the schema.</exception>
    /// <remarks>
    ///     This is a convenience overload that uses the generic type parameter to determine the CLR scalar type,
    ///     avoiding the need to explicitly pass <c>typeof(TScalar)</c>.
    /// </remarks>
    public static ApiScalarType GetScalarTypeByClrType<TScalar>(this ApiSchema apiSchema)
    {
        var clrScalarType = typeof(TScalar);
        return apiSchema.GetScalarTypeByClrType(clrScalarType);
    }
    #endregion
}
