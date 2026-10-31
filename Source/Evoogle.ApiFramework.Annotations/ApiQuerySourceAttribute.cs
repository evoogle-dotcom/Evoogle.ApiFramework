// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework;

/// <summary>Declares a delegate signature as an API query source without creating an execution binding.</summary>
[AttributeUsage(AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
public sealed class ApiQuerySourceAttribute : ApiNamedElementAttribute
{
    #region Properties
    /// <summary>
    ///     Gets the query source API name. When <see langword="null"/>, the delegate type name is
    ///     processed by the active query-source naming conventions.
    /// </summary>
    public new string? ApiName
    {
        get => base.ApiName;
        init => base.ApiName = value;
    }

    /// <summary>Gets an explicit output nullability override.</summary>
    public ApiNullability ApiOutputNullability { get; init; }

    /// <summary>Gets an explicit output-composition override.</summary>
    public ApiQueryComposition ApiOutputComposition { get; init; }
    #endregion
}
