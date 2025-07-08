// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public static class ApiTypeExpressionBuilder
{
    public static ApiTypeExpression FromClrType(Type clrType, ApiSchemaBuilderContext context)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(context);

        // 1. Built-in scalar types
        if (IsKnownScalar(clrType, out var scalarKind, out var scalarApiName))
        {
            return new ApiTypeExpression(scalarKind, scalarApiName);
        }

        // 2. Collections (wrap element type in ApiCollectionType and ApiTypeExpression)
        if (IsCollection(clrType, out var elementType))
        {
            var elementExpr = FromClrType(elementType, context);
            var collectionType = new ApiCollectionType(
                apiItemTypeExpression: elementExpr,
                apiItemTypeModifiers: ApiTypeModifiers.None, // You could make this configurable later
                clrCollectionType: clrType);

            return new ApiTypeExpression(collectionType);
        }

        // 3. Named type previously registered (object, enum, or scalar)
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

    private static bool IsCollection(Type type, out Type elementType)
    {
        if (type.IsArray)
        {
            elementType = type.GetElementType()!;
            return true;
        }

        if (type.IsGenericType &&
            type.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            elementType = type.GetGenericArguments()[0];
            return true;
        }

        elementType = null!;
        return false;
    }
}
