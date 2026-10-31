// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Reflection;

using Evoogle.ApiFramework.Schema.Configuration.Query;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>Provides CLR declaration metadata for a query-source-parameter convention.</summary>
/// <param name="clrParameterInfo">The delegate parameter, or null for a fluent-only parameter.</param>
/// <param name="clrParameterNullableInfo">The reflected parameter nullability, or null when unavailable.</param>
/// <param name="apiQuerySourceBuilder">The query source containing the parameter.</param>
/// <param name="apiQuerySourceContext">The containing query-source convention context.</param>
public sealed class ApiQuerySourceParameterConventionContext
(
    ParameterInfo? clrParameterInfo,
    MemberNullableInfo? clrParameterNullableInfo,
    ApiQuerySourceBuilder apiQuerySourceBuilder,
    ApiQuerySourceConventionContext apiQuerySourceContext
)
{
    #region Properties
    /// <summary>Gets the delegate parameter, or null for a fluent-only parameter.</summary>
    public ParameterInfo? ClrParameterInfo { get; } = clrParameterInfo;

    /// <summary>Gets the reflected parameter nullability, or null when unavailable.</summary>
    public MemberNullableInfo? ClrParameterNullableInfo { get; } = clrParameterNullableInfo;

    /// <summary>Gets the query source containing the parameter.</summary>
    public ApiQuerySourceBuilder ApiQuerySourceBuilder { get; } = apiQuerySourceBuilder;

    /// <summary>Gets the containing query-source convention context.</summary>
    public ApiQuerySourceConventionContext ApiQuerySourceContext { get; } = apiQuerySourceContext;
    #endregion
}
