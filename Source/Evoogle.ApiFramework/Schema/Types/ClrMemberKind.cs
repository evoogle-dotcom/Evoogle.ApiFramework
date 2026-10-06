// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>
///     Specifies the concrete kind of CLR member identified by a <see cref="ClrMemberReference"/>.
/// </summary>
public enum ClrMemberKind
{
    #region Values
    /// <summary>
    ///     Binds only a CLR property (<see cref="PropertyInfo"/>).
    /// </summary>
    Property,

    /// <summary>
    ///     Binds only a CLR field (<see cref="FieldInfo"/>).
    /// </summary>
    Field
    #endregion
}
