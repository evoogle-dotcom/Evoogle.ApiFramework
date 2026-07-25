// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Schema.Annotations;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiObjectTypePropertyDiscoveryConvention : IApiObjectTypeConvention
{
    #region Properties
    /// <inheritdoc />
    public ApiConventionPhase Phase => ApiConventionPhase.Discovery;
    #endregion

    #region IApiObjectTypeConvention
    /// <inheritdoc />
    public void Apply(ApiObjectTypeBuilder builder)
    {
        var clrType = builder.ClrType;

        // Discover public instance properties.
        foreach (var property in clrType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.GetIndexParameters().Length > 0)
            {
                continue; // Skip indexers.
            }

            if (property.IsDefined(typeof(ApiIgnoreAttribute), inherit: true))
            {
                continue;
            }

            builder.AddPropertyIfAbsent(property.Name);
        }

        // Discover public instance fields.
        foreach (var field in clrType.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            if (field.IsDefined(typeof(ApiIgnoreAttribute), inherit: true))
            {
                continue;
            }

            builder.AddPropertyIfAbsent(field.Name);
        }
    }
    #endregion
}
