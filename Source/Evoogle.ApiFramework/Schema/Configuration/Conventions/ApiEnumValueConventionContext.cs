// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Provides CLR metadata and builder context for an <see cref="IApiEnumValueConvention"/>.
/// </summary>
/// <param name="clrMemberInfo">
///     The CLR field backing the enumeration value, or <see langword="null"/> when it cannot be
///     resolved.
/// </param>
/// <param name="clrEnumType">The CLR enumeration type that declares the value.</param>
/// <param name="apiEnumTypeBuilder">The enum type builder that contains the value.</param>
/// <param name="apiSchemaBuilder">The schema builder running the convention pipeline.</param>
public sealed class ApiEnumValueConventionContext
(
    FieldInfo? clrMemberInfo,
    Type clrEnumType,
    ApiEnumTypeBuilder apiEnumTypeBuilder,
    ApiSchemaBuilder apiSchemaBuilder
)
{
    #region Properties
    /// <summary>
    ///     Gets the CLR field backing the enumeration value, or <see langword="null"/> when it
    ///     cannot be resolved.
    /// </summary>
    public FieldInfo? ClrMemberInfo { get; } = clrMemberInfo;

    /// <summary>Gets the CLR enumeration type that declares the value.</summary>
    public Type ClrEnumType { get; } =
        clrEnumType ?? throw new ArgumentNullException(nameof(clrEnumType));

    /// <summary>Gets the enum type builder that contains the value.</summary>
    public ApiEnumTypeBuilder ApiEnumTypeBuilder { get; } =
        apiEnumTypeBuilder ?? throw new ArgumentNullException(nameof(apiEnumTypeBuilder));

    /// <summary>Gets the schema builder running the convention pipeline.</summary>
    public ApiSchemaBuilder ApiSchemaBuilder { get; } =
        apiSchemaBuilder ?? throw new ArgumentNullException(nameof(apiSchemaBuilder));
    #endregion
}
