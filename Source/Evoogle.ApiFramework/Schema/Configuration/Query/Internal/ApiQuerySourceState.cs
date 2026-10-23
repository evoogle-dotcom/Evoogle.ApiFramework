// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Query.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiQuerySourceState
{
    #region Properties
    internal ApiConfigurationSource RegistrationSource { get; set; } = ApiConfigurationSource.Convention;

    internal List<ApiQuerySourceParameterBuilder> InputParameterBuilders { get; } = [];

    internal Func<ApiTypeExpression>? OutputTypeFactory { get; set; }

    internal ApiTypeModifiers ApiOutputTypeModifiers { get; set; }

    internal ApiConfigurationSource OutputSource { get; set; } = ApiConfigurationSource.Convention;

    internal ApiQueryComposition ApiOutputComposition { get; set; } = ApiQueryComposition.Projection;

    internal ApiConfigurationSource OutputCompositionSource { get; set; } = ApiConfigurationSource.Convention;
    #endregion
}
