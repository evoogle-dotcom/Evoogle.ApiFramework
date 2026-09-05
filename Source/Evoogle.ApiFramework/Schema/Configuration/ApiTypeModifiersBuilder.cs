// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Helper used to fluently configure <see cref="ApiTypeModifiers"/> flags.
/// </summary>
public sealed class ApiTypeModifiersBuilder(ApiTypeModifiers modifiers = ApiTypeModifiers.None)
{
    #region Fields
    private readonly ApiTypeModifiersState _state = new(modifiers);
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Marks the API type as required.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiTypeModifiersBuilder Required()
    {
        _state.Modifiers |= ApiTypeModifiers.Required;
        return this;
    }

    /// <summary>
    ///     Marks the API type as optional.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiTypeModifiersBuilder Optional()
    {
        _state.Modifiers &= ~ApiTypeModifiers.Required;
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the combined <see cref="ApiTypeModifiers"/> flags.
    /// </summary>
    /// <returns>The computed modifiers.</returns>
    internal ApiTypeModifiers Build() => _state.Modifiers;
    #endregion
}
