// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Query.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiQuerySourceParameterState
{
    #region Properties
    internal ApiConfigurationSource RegistrationSource { get; set; } = ApiConfigurationSource.Convention;

    internal ApiTypeModifiers ApiTypeModifiers { get; set; } = ApiTypeModifiers.Required;

    internal ApiConfigurationSource ModifiersSource { get; set; } = ApiConfigurationSource.Convention;

    internal bool ApiRequiresArgument { get; set; } = true;

    internal ApiConfigurationSource ApiRequiresArgumentSource { get; set; } = ApiConfigurationSource.Convention;
    #endregion
}
