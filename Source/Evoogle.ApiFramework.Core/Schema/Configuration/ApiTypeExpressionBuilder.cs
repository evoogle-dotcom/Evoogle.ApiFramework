// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Factory helper for creating <see cref="ApiTypeExpression"/> instances from CLR types.
/// </summary>
public static class ApiTypeExpressionBuilder
{
    #region Factory Methods
    /// <summary>
    ///     Creates an <see cref="ApiTypeExpression"/> that represents the given CLR type.
    /// </summary>
    /// <param name="clrType">The CLR type to translate.</param>
    /// <param name="context">The builder context used to resolve registered types.</param>
    /// <returns>An <see cref="ApiTypeExpression"/> describing the CLR type.</returns>
    public static ApiTypeExpression FromClrType(Type clrType, ApiSchemaBuilderContext context)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(context);

        // 1. Handle enum types
        if (TypeReflection.IsEnum(clrType))
        {
            return new ApiTypeExpression(ApiTypeKind.Enum, clrType.Name);
        }

        // 1. Handle scalar types
        if (IsKnownScalar(clrType, out var scalarKind, out var scalarApiName))
        {
            return new ApiTypeExpression(scalarKind, scalarApiName);
        }

        // 2. Handle collection types (wrap element type in ApiCollectionType and ApiTypeExpression)
        if (TypeReflection.IsEnumerableOfT(clrType, out var enumerableType))
        {
            // Handle IEnumerable<T> or ICollection<T> or similar
            var elementExpr = FromClrType(enumerableType, context);
            var collectionType = new ApiCollectionType(
                apiItemTypeExpression: elementExpr,
                apiItemTypeModifiers: ApiTypeModifiers.None, // You could make this configurable later
                clrCollectionType: clrType);

            return new ApiTypeExpression(collectionType);
        }

        // 3. Handle named type previously registered
        if (context.TryGetBuilder(clrType, out var builder))
        {
            var kind = builder switch
            {
                ApiScalarTypeBuilder => ApiTypeKind.Scalar,
                ApiEnumTypeBuilder => ApiTypeKind.Enum,
                ApiObjectTypeBuilder => ApiTypeKind.Object,
                _ => throw new InvalidOperationException($"Unsupported builder type: {builder.GetType().Name}")
            };

            return new ApiTypeExpression(kind, builder.ApiName);
        }

        throw new InvalidOperationException($"Cannot resolve API type for CLR type '{clrType.FullName}'. Is it registered with ApiSchemaBuilder?");
    }
    #endregion

    private static bool IsKnownScalar(Type type, out ApiTypeKind kind, out string apiName)
    {
        (kind, apiName) = type switch
        {
            var t when t == typeof(string) => (ApiTypeKind.Scalar, "String"),
            var t when t == typeof(int) => (ApiTypeKind.Scalar, "Int32"),
            var t when t == typeof(Guid) => (ApiTypeKind.Scalar, "Guid"),
            var t when t == typeof(bool) => (ApiTypeKind.Scalar, "Boolean"),
            var t when t == typeof(DateTime) => (ApiTypeKind.Scalar, "DateTime"),
            _ => default
        };

        return apiName != null;
    }
}
