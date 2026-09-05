// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Provides CLR metadata and builder context for an <see cref="IApiPropertyConvention"/>.
/// </summary>
/// <param name="clrMemberKind">The kind of CLR member backing the property, or <see langword="null"/> when no CLR member could be resolved.</param>
/// <param name="clrMemberInfo">The CLR property or field backing the property, or <see langword="null"/> when no CLR member could be resolved.</param>
/// <param name="clrMemberNullableInfo">The reflected nullability information for <paramref name="clrMemberInfo"/>, or <see langword="null"/> when it cannot be determined.</param>
/// <param name="clrDeclaringType">The CLR type whose property convention is running.</param>
/// <param name="apiObjectTypeBuilder">The object type builder that contains the property.</param>
/// <param name="apiSchemaBuilder">The schema builder running the convention pipeline.</param>
public class ApiPropertyConventionContext
(
    ClrMemberKind? clrMemberKind,
    MemberInfo? clrMemberInfo,
    MemberNullableInfo? clrMemberNullableInfo,
    Type clrDeclaringType,
    ApiObjectTypeBuilder apiObjectTypeBuilder,
    ApiSchemaBuilder apiSchemaBuilder
)
{
    #region Properties
    /// <summary>Gets the kind of CLR member backing the property, or <see langword="null"/> when no CLR member could be resolved.</summary>
    public ClrMemberKind? ClrMemberKind { get; } = clrMemberKind;

    /// <summary>Gets the CLR property or field backing the property, or <see langword="null"/> when no CLR member could be resolved.</summary>
    public MemberInfo? ClrMemberInfo { get; } = clrMemberInfo;

    /// <summary>Gets the reflected nullability information for <see cref="ClrMemberInfo"/>, or <see langword="null"/> when no CLR member or nullability information is available.</summary>
    public MemberNullableInfo? ClrMemberNullableInfo { get; } = clrMemberNullableInfo;

    /// <summary>Gets the CLR type whose property convention is running.</summary>
    public Type ClrDeclaringType { get; } = clrDeclaringType;

    /// <summary>
    ///     Gets the object type builder that contains the property. A convention may use this
    ///     builder to add sibling properties, which receive property conventions in a later pass.
    /// </summary>
    public ApiObjectTypeBuilder ApiObjectTypeBuilder { get; } = apiObjectTypeBuilder;

    /// <summary>Gets the schema builder running the convention pipeline.</summary>
    public ApiSchemaBuilder ApiSchemaBuilder { get; } = apiSchemaBuilder;
    #endregion
}
