// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Reflection;

using Evoogle.ApiFramework.Schema.Configuration.Relationships;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>Provides source CLR metadata and ownership for a relationship traversal convention.</summary>
/// <param name="clrDeclaringType">The source endpoint CLR type, or null for an API-only reference.</param>
/// <param name="clrMemberInfo">The navigation property or field, or null when no CLR member is resolved.</param>
/// <param name="apiObjectTypeReference">The traversal source object-type reference.</param>
/// <param name="apiRelationshipBuilder">The owning relationship builder.</param>
/// <param name="apiSchemaBuilder">The schema builder running the convention pipeline.</param>
public sealed class ApiRelationshipTraversalConventionContext
(
    Type? clrDeclaringType,
    MemberInfo? clrMemberInfo,
    ApiTypeReference apiObjectTypeReference,
    ApiRelationshipBuilder apiRelationshipBuilder,
    ApiSchemaBuilder apiSchemaBuilder
)
{
    #region Properties
    /// <summary>Gets the source endpoint CLR type, or null when unavailable.</summary>
    public Type? ClrDeclaringType { get; } = clrDeclaringType;

    /// <summary>Gets the navigation property or field, or null when unavailable.</summary>
    public MemberInfo? ClrMemberInfo { get; } = clrMemberInfo;

    /// <summary>Gets the traversal source object-type reference.</summary>
    public ApiTypeReference ApiObjectTypeReference { get; } = apiObjectTypeReference;

    /// <summary>Gets the relationship containing the traversal.</summary>
    public ApiRelationshipBuilder ApiRelationshipBuilder { get; } = apiRelationshipBuilder;

    /// <summary>Gets the schema builder running the convention pipeline.</summary>
    public ApiSchemaBuilder ApiSchemaBuilder { get; } = apiSchemaBuilder;
    #endregion
}
