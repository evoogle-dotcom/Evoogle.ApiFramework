// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Marks an enum as an API enum type and optionally overrides its API name.
/// </summary>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
public sealed class ApiEnumAttribute : ApiNamedElementAttribute
{
    #region Properties
    /// <summary>
    ///     Gets the the API name for the enum type.
    ///     When <c>null</c>, the CLR type name is used.
    /// </summary>
    public new string? ApiName
    {
        get => base.ApiName;
        init => base.ApiName = value;
    }
    #endregion
}
