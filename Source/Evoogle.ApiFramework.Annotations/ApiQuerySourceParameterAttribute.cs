// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework;

/// <summary>Overrides schema metadata inferred for a query-source delegate parameter.</summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class ApiQuerySourceParameterAttribute : ApiNamedElementAttribute
{
    #region Properties
    /// <summary>
    ///     Gets the parameter API name. When <see langword="null"/>, the CLR parameter name is
    ///     processed by the active query-source-parameter naming conventions.
    /// </summary>
    public new string? ApiName
    {
        get => base.ApiName;
        init => base.ApiName = value;
    }

    /// <summary>Gets an explicit parameter nullability override.</summary>
    public ApiNullability ApiNullability { get; init; }

    /// <summary>Gets an explicit argument-presence override.</summary>
    public ApiPresence ApiPresence { get; init; }
    #endregion
}
