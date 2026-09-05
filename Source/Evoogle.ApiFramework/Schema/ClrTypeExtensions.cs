// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Extension methods for .NET <see cref="Type"/> class.
/// </summary>
public static class TypeExtensions
{
    #region Methods
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
