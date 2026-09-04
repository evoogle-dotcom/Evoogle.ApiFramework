// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Marks a struct or class as an API scalar type and optionally overrides its API name.
/// </summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false)]
public sealed class ApiScalarAttribute : ApiNamedElementAttribute
{
    #region Properties
    /// <summary>
    ///     Gets the the API name for the scalar type.
    ///     When <c>null</c>, the CLR type name is used.
    /// </summary>
    public new string? ApiName
    {
        get => base.ApiName;
        init => base.ApiName = value;
    }
    #endregion
}
