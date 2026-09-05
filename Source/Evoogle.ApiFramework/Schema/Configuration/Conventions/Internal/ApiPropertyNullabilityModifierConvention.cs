// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiPropertyNullabilityModifierConvention : IApiPropertyConvention
{
    #region Properties
    /// <inheritdoc />
    public ApiConventionPhase Phase => ApiConventionPhase.Configuration;
    #endregion

    #region IApiPropertyConvention
    /// <inheritdoc />
    public void Apply(ApiPropertyBuilder builder, ApiPropertyConventionContext context)
    {
        if (context.ClrMemberKind is null || context.ClrMemberNullableInfo == null)
        {
            return;
        }

        switch (context.ClrMemberNullableInfo.Nullability)
        {
            case MemberNullability.NonNullable:
                builder.SetModifiersConvention(static m => m.Required());
                break;
            case MemberNullability.Nullable:
                builder.SetModifiersConvention(static m => m.Optional());
                break;
            default:
                // MemberNullability.Unknown — no inference possible; leave as-is.
                break;
        }
    }
    #endregion
}
