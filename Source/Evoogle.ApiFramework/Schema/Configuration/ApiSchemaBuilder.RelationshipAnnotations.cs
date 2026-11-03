// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Reflection;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Configuration.Annotations;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Key;
using Evoogle.ApiFramework.Schema.Key.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Relationships;
using Evoogle.ApiFramework.Schema.Configuration.Relationships.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed partial class ApiSchemaBuilder
{
    #region Relationship Annotation Methods
    internal void ApplyRelationshipAnnotation
    (
        Type declaringType,
        IApiRelationshipAnnotationReader reader,
        int occurrence,
        ApiOneToManyRelationshipAnnotationResult result
    )
    {
        this.ApplyRelationshipAnnotationCore
        (
            declaringType, reader, occurrence, result.ApiName, result.PrincipalType, result.DependentType,
            result.ClrNavigationMember, result.InverseNavigation,
            static name => new ApiRelationshipOneToManyBuilder(name),
            builder => builder.WithDeleteBehavior(result.DeleteBehavior).From(result.PrincipalType)
                .To(result.DependentType, dependent =>
                {
                    if (result.ForeignKey is not null)
                    {
                        dependent.WithForeignKey
                        (
                            key => AddAnnotatedForeignKeyPath(key, result.DependentType, result.ForeignKey)
                        );
                    }
                })
        );
    }

    internal void ApplyRelationshipAnnotation
    (
        Type declaringType,
        IApiRelationshipAnnotationReader reader,
        int occurrence,
        ApiOneToOneRelationshipAnnotationResult result
    )
    {
        this.ApplyRelationshipAnnotationCore
        (
            declaringType, reader, occurrence, result.ApiName, result.PrincipalType, result.DependentType,
            result.ClrNavigationMember, result.InverseNavigation,
            static name => new ApiRelationshipOneToOneBuilder(name),
            builder => builder.WithDeleteBehavior(result.DeleteBehavior).From(result.PrincipalType)
                .To(result.DependentType, dependent =>
                {
                    if (result.ForeignKey is not null)
                    {
                        dependent.WithForeignKey
                        (
                            key => AddAnnotatedForeignKeyPath(key, result.DependentType, result.ForeignKey)
                        );
                    }
                })
        );
    }

    internal void ApplyRelationshipAnnotation
    (
        Type declaringType,
        IApiRelationshipAnnotationReader reader,
        int occurrence,
        ApiManyToManyRelationshipAnnotationResult result
    )
    {
        this.ApplyRelationshipAnnotationCore
        (
            declaringType, reader, occurrence, result.ApiName, result.PrincipalTypeA, result.PrincipalTypeB,
            result.ClrNavigationMember, result.InverseNavigation,
            static name => new ApiRelationshipManyToManyBuilder(name),
            builder =>
            {
                var isInverse = result.ClrNavigationMember is not null && builder.NavigationBindings.Any(binding =>
                    binding.Locator.ClrMember.Equals(result.ClrNavigationMember) && binding.EndIndex == 1);
                builder.Between(isInverse ? result.PrincipalTypeB : result.PrincipalTypeA)
                    .And(isInverse ? result.PrincipalTypeA : result.PrincipalTypeB)
                    .WithAssociation(result.AssociationType, association =>
                    {
                        var foreignKeyA = isInverse ? result.ForeignKeyB : result.ForeignKeyA;
                        var foreignKeyB = isInverse ? result.ForeignKeyA : result.ForeignKeyB;
                        if (foreignKeyA is not null)
                        {
                            association.WithForeignKeyA
                            (
                                key => AddAnnotatedForeignKeyPath(key, result.AssociationType, foreignKeyA)
                            );
                        }
                        if (foreignKeyB is not null)
                        {
                            association.WithForeignKeyB
                            (
                                key => AddAnnotatedForeignKeyPath(key, result.AssociationType, foreignKeyB)
                            );
                        }
                    });
            }
        );
    }

    private void ApplyRelationshipAnnotationCore<TBuilder>
    (
        Type declaringType,
        IApiRelationshipAnnotationReader reader,
        int occurrence,
        string apiName,
        Type clrTypeA,
        Type clrTypeB,
        MemberInfo? clrNavigationMember,
        string? inverseNavigation,
        Func<string, TBuilder> factory,
        Action<TBuilder> configure
    ) where TBuilder : ApiRelationshipBuilder
    {
        _context.ApplyConfiguration(ApiConfigurationSource.DataAnnotation, () =>
        {
            if (clrNavigationMember is null)
            {
                if (_context.TryBeginRelationshipAnnotation(declaringType, typeof(TBuilder),
                    reader is ApiAttributeAnnotationReader ? typeof(ApiAttributeAnnotationReader) : reader, occurrence))
                {
                    var builder = _context.DeclareRelationship(new object(), apiName, factory);
                    builder.ApplyConfiguration(ApiConfigurationSource.DataAnnotation, () => configure(builder));
                }
                return;
            }
            try
            {
                var primary = ApiNavigationLocator.FromMember(clrTypeA, clrNavigationMember);
                var inverse = inverseNavigation is null ? null : ApiNavigationLocator.FromName(clrTypeB,
                    inverseNavigation);
                this.AddMemberRelationship(apiName, clrTypeA, clrTypeB, primary, inverse, factory, configure);
            }
            catch (ApiSchemaConfigurationException exception)
            {
                _context.AddRelationshipIssue(ApiSchemaCompilationCode.ApiRelationshipInvalidNavigationLocator,
                    apiName, exception.Message);
            }
        });
    }
    #endregion

    #region Annotation Foreign Key Methods
    private static void AddAnnotatedForeignKeyPath(ApiKeyDefinitionBuilder keyBuilder, Type clrRootType, string clrPath)
    {
        var parsedPath = ApiKeyPathClrPathParser.Parse(clrPath);
        if (!parsedPath.IsValid)
        {
            var description = parsedPath.ValidationMessage ?? "The foreign-key CLR path is invalid.";
            throw new ApiSchemaConfigurationException(description);
        }
        if (!keyBuilder.HasPath(clrRootType, parsedPath.ClrMemberNames))
        {
            keyBuilder.AddPath(clrRootType, clrPath);
        }
    }
    #endregion
}
