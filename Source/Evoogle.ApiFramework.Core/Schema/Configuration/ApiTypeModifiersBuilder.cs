// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed class ApiTypeModifiersBuilder
{
    #region Fields
    private ApiTypeModifiers _modifiers = ApiTypeModifiers.None;
    #endregion

    #region Builder Methods
    public ApiTypeModifiersBuilder Required()
    {
        _modifiers |= ApiTypeModifiers.Required;
        return this;
    }

    public ApiTypeModifiersBuilder Nullable()
    {
        _modifiers &= ~ApiTypeModifiers.Required;
        return this;
    }

    public ApiTypeModifiers Build() => _modifiers;
    #endregion
}
