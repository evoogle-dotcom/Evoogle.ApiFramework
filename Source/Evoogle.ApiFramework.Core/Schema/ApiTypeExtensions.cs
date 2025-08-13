// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Extension methods for <see cref="ApiType"/> class.
/// </summary>
public static class ApiTypeExtensions
{
    #region Extension Methods
    /// <summary>Gets a value indicating whether this API type is unknown.</summary>
    public static bool IsUnknownType(this ApiType apiType) => apiType.Kind == ApiTypeKind.Unknown;

    /// <summary>Gets a value indicating whether this API type is an enumeration type.</summary>
    public static bool IsEnumType(this ApiType apiType) => apiType.Kind == ApiTypeKind.Enum;

    /// <summary>Gets a value indicating whether this API type is a scalar type.</summary>
    public static bool IsScalarType(this ApiType apiType) => apiType.Kind == ApiTypeKind.Scalar;

    /// <summary>Gets a value indicating whether this API type is an object type.</summary>
    public static bool IsObjectType(this ApiType apiType) => apiType.Kind == ApiTypeKind.Object;

    /// <summary>Gets a value indicating whether this API type is a collection type.</summary>
    public static bool IsCollectionType(this ApiType apiType) => apiType.Kind == ApiTypeKind.Collection;
    #endregion
}
