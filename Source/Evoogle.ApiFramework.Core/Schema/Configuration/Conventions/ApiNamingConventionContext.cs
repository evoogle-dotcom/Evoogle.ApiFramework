// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Provides CLR and builder metadata that an <see cref="IApiNamingConvention"/> can use
///     when transforming or deriving an API name.
/// </summary>
public sealed class ApiNamingConventionContext
{
    #region Constructors
    /// <summary>
    ///     Creates a naming convention context for a schema type or property.
    /// </summary>
    /// <param name="target">The schema element kind whose API name is being produced.</param>
    /// <param name="clrType">The CLR type associated with the naming target.</param>
    /// <param name="clrName">
    ///     The CLR member name associated with the naming target, or <see langword="null"/> when
    ///     the target is not a member.
    /// </param>
    /// <param name="apiPropertyConventionContext">
    ///     The property convention context when <paramref name="target"/> is
    ///     <see cref="ApiNamingConventionTarget.Property"/>, or <see langword="null"/> otherwise.
    /// </param>
    public ApiNamingConventionContext
    (
        ApiNamingConventionTarget target,
        Type clrType,
        string? clrName = null,
        ApiPropertyConventionContext? apiPropertyConventionContext = null
    )
        : this(target, clrType, clrName, apiPropertyConventionContext, null)
    {
    }

    /// <summary>
    ///     Creates a naming convention context that can carry property or enum-value
    ///     convention metadata.
    /// </summary>
    /// <param name="target">The schema element kind whose API name is being produced.</param>
    /// <param name="clrType">The CLR type associated with the naming target.</param>
    /// <param name="clrName">
    ///     The CLR member name associated with the naming target, or <see langword="null"/> when
    ///     the target is not a member.
    /// </param>
    /// <param name="apiPropertyConventionContext">
    ///     The property convention context when <paramref name="target"/> is
    ///     <see cref="ApiNamingConventionTarget.Property"/>, or <see langword="null"/> otherwise.
    /// </param>
    /// <param name="apiEnumValueConventionContext">
    ///     The enum-value convention context when <paramref name="target"/> is
    ///     <see cref="ApiNamingConventionTarget.EnumValue"/>, or <see langword="null"/> otherwise.
    /// </param>
    public ApiNamingConventionContext
    (
        ApiNamingConventionTarget target,
        Type clrType,
        string? clrName,
        ApiPropertyConventionContext? apiPropertyConventionContext,
        ApiEnumValueConventionContext? apiEnumValueConventionContext
    )
    {
        this.Target = target;
        this.ClrType = clrType ?? throw new ArgumentNullException(nameof(clrType));
        this.ClrName = clrName;
        this.ApiPropertyConventionContext = apiPropertyConventionContext;
        this.ApiEnumValueConventionContext = apiEnumValueConventionContext;
    }
    #endregion

    #region Properties
    /// <summary>Gets the schema element kind whose API name is being produced.</summary>
    public ApiNamingConventionTarget Target { get; }

    /// <summary>Gets the CLR type associated with the naming target.</summary>
    public Type ClrType { get; }

    /// <summary>
    ///     Gets the CLR member name associated with the naming target. Naming conventions can use
    ///     this value to derive member API names from CLR metadata. This value is
    ///     <see langword="null"/> when the target is not a member.
    /// </summary>
    public string? ClrName { get; }

    /// <summary>
    ///     Gets the property convention context when <see cref="Target"/> is
    ///     <see cref="ApiNamingConventionTarget.Property"/>, or <see langword="null"/> otherwise.
    /// </summary>
    public ApiPropertyConventionContext? ApiPropertyConventionContext { get; }

    /// <summary>
    ///     Gets the enum-value convention context when <see cref="Target"/> is
    ///     <see cref="ApiNamingConventionTarget.EnumValue"/>, or <see langword="null"/> otherwise.
    /// </summary>
    public ApiEnumValueConventionContext? ApiEnumValueConventionContext { get; }
    #endregion
}
