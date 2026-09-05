// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiEnumTypeEnumValueDiscoveryConvention : IApiEnumTypeConvention
{
    #region Properties
    /// <inheritdoc />
    public ApiConventionPhase Phase => ApiConventionPhase.Configuration;
    #endregion

    #region IApiEnumTypeConvention
    /// <inheritdoc />
    public void Apply(ApiEnumTypeBuilder builder)
    {
        if (!builder.ClrType.IsEnum)
        {
            return;
        }

        var clrType = builder.ClrType;
        var members = Enum.GetNames(clrType)
            .Select(name => (Name: name, Ordinal: Convert.ToInt32(Enum.Parse(clrType, name))))
            .OrderBy(m => m.Ordinal);

        foreach (var (clrName, clrOrdinal) in members)
        {
            builder.AddValueIfAbsent(clrName, clrOrdinal);
        }
    }
    #endregion
}
