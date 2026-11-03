// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Query.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiQuerySourceState
{
    #region Constructors
    internal ApiQuerySourceState()
    {
    }

    internal ApiQuerySourceState(ApiQuerySourceState source)
    {
        this.RegistrationSource = source.RegistrationSource;
        this.InputParameterBuilders.AddRange(source.InputParameterBuilders.Select(static builder => builder.CreateBuildSnapshot()));
        this.OutputTypeFactory = source.OutputTypeFactory;
        this.ApiOutputNullability = source.ApiOutputNullability;
        this.OutputTypeSource = source.OutputTypeSource;
        this.OutputNullabilitySource = source.OutputNullabilitySource;
        this.ApiOutputComposition = source.ApiOutputComposition;
        this.OutputCompositionSource = source.OutputCompositionSource;
    }
    #endregion

    #region Properties
    internal ApiConfigurationSource RegistrationSource { get; set; } = ApiConfigurationSource.Convention;

    internal List<ApiQuerySourceParameterBuilder> InputParameterBuilders { get; } = [];

    internal Func<ApiTypeExpression>? OutputTypeFactory { get; set; }

    internal ApiNullability ApiOutputNullability { get; set; } = ApiSchemaDefaults.QuerySourceOutputNullability;

    internal ApiConfigurationSource OutputTypeSource { get; set; } = ApiConfigurationSource.Convention;

    internal ApiConfigurationSource OutputNullabilitySource { get; set; } = ApiConfigurationSource.Convention;

    internal ApiQueryComposition ApiOutputComposition { get; set; } = ApiSchemaDefaults.QuerySourceOutputComposition;

    internal ApiConfigurationSource OutputCompositionSource { get; set; } = ApiConfigurationSource.Convention;
    #endregion
}
