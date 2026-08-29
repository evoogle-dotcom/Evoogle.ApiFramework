// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Humanizer;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiNamingSingularizeConvention
(
    ApiNamingConventionTargets targets = ApiNamingConventionTargets.ObjectType
)
    : ApiNamingConvention(targets)
{
    #region Properties
    /// <inheritdoc />
    public override ApiConventionPhase Phase => ApiConventionPhase.Configuration;
    #endregion

    #region IApiNamingConvention
    /// <inheritdoc />
    public override string ConvertName(string apiName, ApiNamingConventionContext context)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        return apiName.Singularize(inputIsKnownToBePlural: false);
    }
    #endregion
}
