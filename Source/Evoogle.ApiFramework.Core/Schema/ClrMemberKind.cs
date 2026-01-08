// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Specifies the kind of CLR member that an <see cref="ApiProperty"/> represents.
/// </summary>
public enum ClrMemberKind
{
    #region Values
    /// <summary>
    ///     The CLR member kind is unknown or has not been determined.
    /// </summary>
    Unknown,

    /// <summary>
    ///     The CLR member is a property (PropertyInfo).
    /// </summary>
    Property,

    /// <summary>
    ///     The CLR member is a field (FieldInfo).
    /// </summary>
    Field
    #endregion
}
