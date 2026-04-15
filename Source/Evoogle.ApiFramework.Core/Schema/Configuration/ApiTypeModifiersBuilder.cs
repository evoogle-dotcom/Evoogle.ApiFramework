// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Helper used to fluently configure <see cref="ApiTypeModifiers"/> flags.
/// </summary>
public sealed class ApiTypeModifiersBuilder(ApiTypeModifiers modifiers = ApiTypeModifiers.None)
{
    #region Fields
    private ApiTypeModifiers _modifiers = modifiers;
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Marks the API type as required.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiTypeModifiersBuilder Required()
    {
        _modifiers |= ApiTypeModifiers.Required;
        return this;
    }

    /// <summary>
    ///     Marks the API type as optional.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiTypeModifiersBuilder Optional()
    {
        _modifiers &= ~ApiTypeModifiers.Required;
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the combined <see cref="ApiTypeModifiers"/> flags.
    /// </summary>
    /// <returns>The computed modifiers.</returns>
    internal ApiTypeModifiers Build() => _modifiers;
    #endregion
}
