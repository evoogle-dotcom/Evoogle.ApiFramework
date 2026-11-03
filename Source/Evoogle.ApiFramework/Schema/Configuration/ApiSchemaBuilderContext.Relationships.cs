// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Reflection;

using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Relationships;
using Evoogle.ApiFramework.Schema.Configuration.Relationships.Internal;
using Evoogle.ApiFramework.Schema.Relationships;

namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed partial class ApiSchemaBuilderContext
{
    #region Fields
    private readonly Dictionary<object, ApiRelationshipBuilder> _apiRelationshipBuilders = [];
    private readonly Dictionary<(Type DeclaringType, MemberInfo Member), object> _relationshipLocators = [];
    private readonly HashSet<(Type ClrType, Type Role, object Origin, int Occurrence)> _relationshipAnnotationOccurrences = [];
    private readonly List<(ApiSchemaCompilationCode Code, string ApiName, string Description, object? RelationshipIdentity)> _relationshipFindings = [];
    #endregion

    #region Properties
    internal IEnumerable<ApiRelationshipBuilder> ApiRelationshipBuilders => _apiRelationshipBuilders.Values;
    #endregion

    #region Annotation Methods
    internal bool TryBeginRelationshipAnnotation(Type clrType, Type role, object origin, int occurrence)
        => _relationshipAnnotationOccurrences.Add((clrType, role, origin, occurrence));
    #endregion

    #region Registration Methods
    internal TBuilder DeclareRelationship<TBuilder>(object identity, string apiName, Func<string, TBuilder> factory)
        where TBuilder : ApiRelationshipBuilder
    {
        var builder = factory(apiName);
        builder.SetRegistrationSource(this.CurrentConfigurationSource);
        builder.ApplyConfiguration(this.CurrentConfigurationSource, () => builder.SetApiName(apiName));
        _apiRelationshipBuilders.Add(identity, builder);
        return builder;
    }

    internal ApiRelationshipBuilder GetRelationship(object identity)
        => _apiRelationshipBuilders[identity];

    /// <summary>
    ///     Declares an independent <see cref="ApiRelationshipOneToOneBuilder"/> with the supplied API name.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <returns>The corresponding <see cref="ApiRelationshipOneToOneBuilder"/>.</returns>
    internal ApiRelationshipOneToOneBuilder DeclareOneToOneRelationshipBuilder(string apiName)
        => this.DeclareOneToOneRelationshipBuilder(apiName, this.CurrentConfigurationSource);

    /// <summary>
    ///     Declares an independent one-to-one relationship builder at the supplied configuration source.
    /// </summary>
    internal ApiRelationshipOneToOneBuilder DeclareOneToOneRelationshipBuilder
    (
        string apiName,
        ApiConfigurationSource source
    ) => this.DeclareTypedRelationshipBuilder
    (
        apiName,
        source,
        static n => new ApiRelationshipOneToOneBuilder(n)
    );

    /// <summary>
    ///     Declares an independent <see cref="ApiRelationshipOneToManyBuilder"/> with the supplied API name.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <returns>The corresponding <see cref="ApiRelationshipOneToManyBuilder"/>.</returns>
    internal ApiRelationshipOneToManyBuilder DeclareOneToManyRelationshipBuilder(string apiName)
        => this.DeclareOneToManyRelationshipBuilder(apiName, this.CurrentConfigurationSource);

    /// <summary>
    ///     Declares an independent one-to-many relationship builder at the supplied configuration source.
    /// </summary>
    internal ApiRelationshipOneToManyBuilder DeclareOneToManyRelationshipBuilder
    (
        string apiName,
        ApiConfigurationSource source
    ) => this.DeclareTypedRelationshipBuilder
    (
        apiName,
        source,
        static n => new ApiRelationshipOneToManyBuilder(n)
    );

    /// <summary>
    ///     Declares an independent <see cref="ApiRelationshipManyToManyBuilder"/> with the supplied API name.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <returns>The corresponding <see cref="ApiRelationshipManyToManyBuilder"/>.</returns>
    internal ApiRelationshipManyToManyBuilder DeclareManyToManyRelationshipBuilder(string apiName)
        => this.DeclareManyToManyRelationshipBuilder(apiName, this.CurrentConfigurationSource);

    /// <summary>
    ///     Declares an independent many-to-many relationship builder at the supplied configuration source.
    /// </summary>
    internal ApiRelationshipManyToManyBuilder DeclareManyToManyRelationshipBuilder
    (
        string apiName,
        ApiConfigurationSource source
    ) => this.DeclareTypedRelationshipBuilder
    (
        apiName,
        source,
        static n => new ApiRelationshipManyToManyBuilder(n)
    );

    private TBuilder DeclareTypedRelationshipBuilder<TBuilder>
    (
        string apiName,
        ApiConfigurationSource source,
        Func<string, TBuilder> factory
    )
        where TBuilder : ApiRelationshipBuilder
    {
        var builder = factory(apiName);
        builder.SetRegistrationSource(source);
        builder.ApplyConfiguration(source, () => builder.SetApiName(apiName));
        _apiRelationshipBuilders.Add(new object(), builder);
        return builder;
    }
    #endregion

    #region Resolution Methods
    internal TBuilder? ResolveRelationship<TBuilder>
    (
        string apiName,
        IReadOnlyList<ApiRelationshipNavigationBinding> bindings,
        Func<string, TBuilder> factory
    ) where TBuilder : ApiRelationshipBuilder
    {
        var hasConflictingRoles = bindings.GroupBy(static binding => binding.Locator.Identity).Any
        (
            static group => group.Select(static binding => binding.EndIndex).Distinct().Count() > 1
        );

        if (hasConflictingRoles)
        {
            this.AddRelationshipIssue
            (
                code: ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict,
                apiName: apiName,
                description: "One CLR navigation member cannot bind both relationship endpoint roles"
            );
            return null;
        }

        var identities = bindings.Where(binding => _relationshipLocators.ContainsKey(binding.Locator.Identity))
            .Select(binding => _relationshipLocators[binding.Locator.Identity]).Distinct().ToArray();
        if (identities.Length > 1)
        {
            this.AddRelationshipIssue
            (
                code: ApiSchemaCompilationCode.ApiRelationshipDeclarationConflict,
                apiName: apiName,
                description: "Navigation locators already identify different relationship declarations"
            );
            return null;
        }

        var identity = identities.FirstOrDefault() ?? new object();
        TBuilder builder;
        if (_apiRelationshipBuilders.TryGetValue(identity, out var existing))
        {
            if (existing is not TBuilder typed)
            {
                if (this.CurrentConfigurationSource >= existing.RegistrationSource)
                {
                    this.AddRelationshipIssue
                    (
                        code: ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict,
                        apiName: apiName,
                        description: "A navigation locator cannot change its established relationship kind",
                        relationship: existing
                    );
                }
                return null;
            }
            builder = typed;
        }
        else
        {
            builder = this.DeclareRelationship(identity, apiName, factory);
        }

        var effectiveBindings = bindings;
        var hasInverseOrdering = builder is ApiRelationshipManyToManyBuilder && bindings.Any
        (
            binding => builder.NavigationBindings.Any
            (
                existingBinding => existingBinding.Locator.Identity == binding.Locator.Identity &&
                    existingBinding.EndIndex != binding.EndIndex
            )
        );
        if (hasInverseOrdering)
        {
            effectiveBindings = [.. bindings.Select(static binding => binding with { EndIndex = 1 - binding.EndIndex })];
        }
        foreach (var binding in effectiveBindings)
        {
            var claimedBinding = builder.NavigationBindings.FirstOrDefault(existingBinding =>
                existingBinding.Locator.Identity == binding.Locator.Identity);
            var occupiedEnd = builder.NavigationBindings.FirstOrDefault(existingBinding =>
                existingBinding.EndIndex == binding.EndIndex &&
                existingBinding.Locator.Identity != binding.Locator.Identity);
            if (occupiedEnd is not null || (claimedBinding is not null &&
                (claimedBinding.EndIndex != binding.EndIndex || claimedBinding.Locator != binding.Locator)))
            {
                this.AddRelationshipIssue
                (
                    code: ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict,
                    apiName: apiName,
                    description: "Navigation contributions must preserve the established CLR binding and endpoint role",
                    relationship: builder
                );
                return null;
            }
        }
        builder.SetRegistrationSource(this.CurrentConfigurationSource);
        builder.ApplyConfiguration(this.CurrentConfigurationSource, () => builder.SetApiName(apiName));
        foreach (var binding in effectiveBindings)
        {
            _relationshipLocators[binding.Locator.Identity] = identity;
            builder.AddNavigationBinding(binding);
        }
        return builder;
    }

    internal ApiRelationshipBuilder? FindRelationship(ApiNavigationLocator locator)
    {
        if (_relationshipLocators.TryGetValue(locator.Identity, out var identity))
        {
            return _apiRelationshipBuilders[identity];
        }
        this.AddRelationshipIssue
        (
            code: ApiSchemaCompilationCode.ApiRelationshipInvalidNavigationLocator,
            apiName: locator.ClrMember.Name,
            description: $"Navigation '{locator.SourceType.FullName}.{locator.ClrMember.Name}' has no relationship declaration"
        );
        return null;
    }

    internal void AssociateInverseNavigation(ApiNavigationLocator existing, ApiNavigationLocator inverse)
    {
        var builder = this.FindRelationship(existing);
        if (builder is null)
        {
            return;
        }
        var identity = _relationshipLocators[existing.Identity];
        if (_relationshipLocators.TryGetValue(inverse.Identity, out var claimed) && !ReferenceEquals(claimed, identity))
        {
            this.AddRelationshipIssue
            (
                code: ApiSchemaCompilationCode.ApiRelationshipDeclarationConflict,
                apiName: builder.ConfiguredApiName,
                description: "The inverse navigation already identifies another relationship declaration",
                relationship: builder
            );
            return;
        }
        var binding = builder.NavigationBindings.Single(binding => binding.Locator.Identity == existing.Identity);
        var inverseEnd = binding.EndIndex == 0 ? 1 : 0;
        if (inverse.SourceType != existing.TargetType || inverse.TargetType != existing.SourceType ||
            builder.NavigationBindings.Any(existingBinding => existingBinding.EndIndex == inverseEnd && existingBinding.Locator.Identity != inverse.Identity))
        {
            this.AddRelationshipIssue
            (
                code: ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict,
                apiName: builder.ConfiguredApiName,
                description: "The inverse navigation must preserve the opposite CLR endpoint and its existing binding",
                relationship: builder
            );
            return;
        }
        _relationshipLocators[inverse.Identity] = identity;
        builder.AddNavigationBinding(new(inverse, inverseEnd));
    }

    #endregion

    #region Configuration Issue Methods
    internal void AddRelationshipIssue
    (
        ApiSchemaCompilationCode code,
        string apiName,
        string description,
        ApiRelationshipBuilder? relationship = null
    )
    {
        var identity = relationship is null
            ? null
            : _apiRelationshipBuilders.First(registration => ReferenceEquals(registration.Value, relationship)).Key;
        _relationshipFindings.Add((code, apiName, description, identity));
    }

    internal void CollectRelationshipConfigurationIssues(string? schemaName)
    {
        var schemaPath = ApiSchemaPathFormatting.BuildPath(null, nameof(ApiSchema), schemaName);
        foreach (var (code, apiName, description, relationshipIdentity) in _relationshipFindings)
        {
            var elementTypeName = nameof(ApiRelationship);
            var elementApiName = apiName;
            if (relationshipIdentity is { } identity && _apiRelationshipBuilders.TryGetValue(identity, out var relationship))
            {
                elementApiName = relationship.ConfiguredApiName;
                elementTypeName = relationship switch
                {
                    ApiRelationshipOneToOneBuilder => nameof(ApiRelationshipOneToOne),
                    ApiRelationshipOneToManyBuilder => nameof(ApiRelationshipOneToMany),
                    ApiRelationshipManyToManyBuilder => nameof(ApiRelationshipManyToMany),
                    _ => nameof(ApiRelationship),
                };
            }

            var apiPath = ApiSchemaPathFormatting.BuildPath(schemaPath, elementTypeName, elementApiName);
            var severity = ApiSchemaCompilationSeverity.Error;
            var remediation = "Use compatible navigation locators or configure an independent registration-backed relationship";

            this.AddConfigurationIssue(new(apiPath, severity, code, description, remediation));
        }
    }
    #endregion

    #region Build Copy Methods
    private void CopyRelationshipStateTo(ApiSchemaBuilderContext copy)
    {
        foreach (var (identity, builder) in _apiRelationshipBuilders)
        {
            copy._apiRelationshipBuilders.Add(identity, builder.CreateBuildCopy());
        }
        foreach (var (locator, identity) in _relationshipLocators)
        {
            copy._relationshipLocators.Add(locator, identity);
        }
        copy._relationshipFindings.AddRange(_relationshipFindings);
    }
    #endregion
}
