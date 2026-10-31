// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>Provides CLR declaration metadata for a query-source convention.</summary>
/// <param name="clrDelegateType">The annotated delegate type, or null for a fluent-only source.</param>
/// <param name="clrOutputNullableInfo">The reflected delegate output nullability, or null when unavailable.</param>
/// <param name="apiSchemaBuilder">The schema builder running the convention pipeline.</param>
public sealed class ApiQuerySourceConventionContext
(
    Type? clrDelegateType,
    MemberNullableInfo? clrOutputNullableInfo,
    ApiSchemaBuilder apiSchemaBuilder
)
{
    #region Properties
    /// <summary>Gets the annotated delegate type, or null for a fluent-only source.</summary>
    public Type? ClrDelegateType { get; } = clrDelegateType;

    /// <summary>Gets the reflected delegate output nullability, or null when unavailable.</summary>
    public MemberNullableInfo? ClrOutputNullableInfo { get; } = clrOutputNullableInfo;

    /// <summary>Gets the schema builder running the convention pipeline.</summary>
    public ApiSchemaBuilder ApiSchemaBuilder { get; } = apiSchemaBuilder;
    #endregion
}
