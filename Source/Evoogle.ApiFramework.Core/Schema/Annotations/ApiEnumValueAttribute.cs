// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Optionally overrides the API name of an enum value.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class ApiEnumValueAttribute : ApiNamedElementAttribute
{
    #region Properties
    /// <summary>
    ///     Gets the the API name for the enum value.
    ///     When <c>null</c>, the enum member name is used.
    /// </summary>
    public new string? ApiName
    {
        get => base.ApiName;
        init => base.ApiName = value;
    }
    #endregion
}
