// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Specifies the authoritative concrete kind of CLR member that an <see cref="ApiProperty"/>
///     binds by <see cref="ApiProperty.ClrName"/>.
/// </summary>
public enum ClrMemberKind
{
    #region Values
    /// <summary>
    ///     Binds only a CLR property (<see cref="System.Reflection.PropertyInfo"/>).
    /// </summary>
    Property,

    /// <summary>
    ///     Binds only a CLR field (<see cref="System.Reflection.FieldInfo"/>).
    /// </summary>
    Field
    #endregion
}
