// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Configuration.Types.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiNamedTypeState
{
    #region Constructors
    internal ApiNamedTypeState(Type clrType)
    {
        ArgumentNullException.ThrowIfNull(clrType);

        this.ClrType = clrType;
        this.ApiName = clrType.SafeToName();
    }
    #endregion

    #region Properties
    internal string ApiName { get; set; }

    internal Type ClrType { get; }

    internal ApiConfigurationSource ApiNameSource { get; set; } = ApiConfigurationSource.Convention;
    #endregion
}
