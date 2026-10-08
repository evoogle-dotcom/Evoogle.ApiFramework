// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>
///     Extension methods for .NET <see cref="Type"/> class.
/// </summary>
public static class ClrTypeExtensions
{
    #region Methods
    /// <summary>
    ///     Gets the API type kind for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR type to evaluate.</param>
    /// <returns>The API type kind for the specified CLR type.</returns>
    /// <exception cref="ApiSchemaException">Thrown if an API type kind cannot be determined.</exception>
    /// <remarks>
    ///     Use <see cref="TryGetApiTypeKind"/> if you prefer non-throwing behavior.
    /// </remarks>
    public static ApiTypeKind GetApiTypeKind(this Type clrType)
    {
        if (clrType.TryGetApiTypeKind(out var apiTypeKind))
        {
            return apiTypeKind.Value;
        }

        var errorMessage = $"An {nameof(ApiTypeKind)} could not be determined for CLR type " +
            $"'{clrType.SafeToName()}'.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Determines the API type kind for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR type to evaluate.</param>
    /// <param name="apiTypeKind">The API type kind for the specified CLR type.</param>
    /// <returns>
    ///    <see langword="true"/> if the API type kind was determined; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryGetApiTypeKind(this Type? clrType, [NotNullWhen(true)] out ApiTypeKind? apiTypeKind)
    {
        apiTypeKind = null;

        if (clrType == null)
        {
            return false;
        }

        if (clrType.IsAbstract || !clrType.IsClass && !clrType.IsValueType)
        {
            return false;
        }

        if (TypeReflection.IsEnum(clrType))
        {
            apiTypeKind = ApiTypeKind.Enum;
            return true;
        }

        if (TypeReflection.IsValueType(clrType) || TypeReflection.IsSimple(clrType))
        {
            apiTypeKind = ApiTypeKind.Scalar;
            return true;
        }

        if (TypeReflection.IsComplex(clrType))
        {
            apiTypeKind = ApiTypeKind.Object;
            return true;
        }

        return false;
    }
    #endregion
}
