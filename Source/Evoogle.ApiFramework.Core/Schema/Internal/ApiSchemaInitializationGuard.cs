// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Runtime.CompilerServices;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiSchemaInitializationGuard
{
    #region Extension Methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue ThrowIfNotInitialized<TValue>(this object obj, TValue? value)
        where TValue : class
    {
        if (value is not null)
        {
            return value;
        }

        var objectType = obj.GetType();
        throw new ApiSchemaException($"{objectType.SafeToName()} has not been initialized. Initialize the {nameof(ApiSchema)} before accessing.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue ThrowIfNotInitialized<TValue>(this object obj, TValue? value)
        where TValue : struct
    {
        if (value.HasValue)
        {
            return value.Value;
        }

        var objectType = obj.GetType();
        throw new ApiSchemaException($"{objectType.SafeToName()} has not been initialized. Initialize the {nameof(ApiSchema)} before accessing.");
    }
    #endregion
}
