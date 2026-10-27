// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Json.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
/// <remarks>
///     Records a null or invalid enumeration value found during JSON deserialization so that schema
///     compilation can report it as an issue.
/// </remarks>
internal sealed record JsonEnumFinding(string MemberName, Type EnumType, JsonEnumReadStatus Status, string? RawText)
{
    #region Factory Methods
    /// <summary>
    ///     Creates a finding when <paramref name="readResult"/> is invalid or, unless
    ///     <paramref name="allowsNull"/> is <see langword="true"/>, null; otherwise returns <see langword="null"/>.
    /// </summary>
    public static JsonEnumFinding? Create<TEnum>
    (
        string memberName,
        JsonEnumReadResult<TEnum> readResult,
        bool allowsNull = false
    )
        where TEnum : struct, Enum
    {
        var isDefective = readResult.Status == JsonEnumReadStatus.Invalid
            || (!allowsNull && readResult.Status == JsonEnumReadStatus.Null);

        return isDefective
            ? new JsonEnumFinding(memberName, typeof(TEnum), readResult.Status, readResult.RawText)
            : null;
    }
    #endregion

    #region Methods
    public string DescribeProblem() => this.Status == JsonEnumReadStatus.Null
        ? $"{this.MemberName} is required"
        : $"{this.MemberName} must be a valid {this.EnumType.Name} value, but '{this.RawText}' was supplied";
    #endregion
}
