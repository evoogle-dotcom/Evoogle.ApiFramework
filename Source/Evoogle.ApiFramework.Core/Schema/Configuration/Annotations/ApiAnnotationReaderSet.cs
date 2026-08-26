// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

/// <summary>Holds the ordered annotation readers and centrally applies their results.</summary>
public sealed class ApiAnnotationReaderSet
{
    #region Fields
    private readonly IReadOnlyList<IApiAnnotationReader> _readers;
    private readonly List<ApiInitializationIssue> _issues = [];
    #endregion

    #region Constructors
    internal ApiAnnotationReaderSet(IReadOnlyList<IApiAnnotationReader> readers)
    {
        _readers = readers;
    }
    #endregion

    #region Properties
    internal IReadOnlyList<ApiInitializationIssue> Issues => _issues;
    #endregion

    #region Annotation Methods
    internal void ApplyObjectTypeAnnotations(ApiObjectTypeBuilder builder)
    {
        var clrType = builder.ClrType;

        foreach (var reader in _readers)
        {
            if (reader is IApiTypeAnnotationReader typeReader)
            {
                this.ApplyTypeResults
                (
                    builder,
                    clrType,
                    typeReader.ReadObjectTypeAnnotations,
                    "object type"
                );
            }
        }

        this.ApplyKeyAnnotations(builder);
    }

    internal void ApplyScalarTypeAnnotations(ApiScalarTypeBuilder builder)
    {
        var clrType = builder.ClrType;

        foreach (var reader in _readers)
        {
            if (reader is IApiTypeAnnotationReader typeReader)
            {
                this.ApplyTypeResults
                (
                    builder,
                    clrType,
                    typeReader.ReadScalarTypeAnnotations,
                    "scalar type"
                );
            }
        }
    }

    internal void ApplyEnumTypeAnnotations(ApiEnumTypeBuilder builder)
    {
        var clrType = builder.ClrType;

        foreach (var reader in _readers)
        {
            if (reader is IApiTypeAnnotationReader typeReader)
            {
                this.ApplyTypeResults
                (
                    builder,
                    clrType,
                    typeReader.ReadEnumTypeAnnotations,
                    "enum type"
                );
            }
        }
    }

    internal void ApplyEnumValueAnnotations
    (
        ApiEnumTypeBuilder enumTypeBuilder,
        ApiEnumValueBuilder enumValueBuilder
    )
    {
        var clrField = TypeReflection.GetField(enumTypeBuilder.ClrType, enumValueBuilder.ClrName);
        if (clrField == null)
        {
            return;
        }

        foreach (var reader in _readers)
        {
            if (reader is not IApiEnumValueAnnotationReader enumValueReader)
            {
                continue;
            }

            IReadOnlyList<ApiEnumValueAnnotationResult>? results;
            try
            {
                results = enumValueReader.ReadEnumValueAnnotations(clrField);
            }
            catch (Exception exception)
            {
                this.AddReaderIssue(enumTypeBuilder.ClrType, enumValueBuilder.ClrName, exception);
                continue;
            }

            if (results == null)
            {
                this.AddInvalidContributionIssue
                (
                    enumTypeBuilder.ClrType,
                    enumValueBuilder.ClrName,
                    "An enum-value annotation reader returned null instead of a result list."
                );
                continue;
            }

            foreach (var result in results)
            {
                if (result == null)
                {
                    this.AddInvalidContributionIssue
                    (
                        enumTypeBuilder.ClrType,
                        enumValueBuilder.ClrName,
                        "An enum-value annotation reader returned a null result."
                    );
                    continue;
                }

                if (result.ApiName == null)
                {
                    continue;
                }

                try
                {
                    enumTypeBuilder.ConfigurationContext.ApplyConfiguration
                    (
                        ApiConfigurationSource.DataAnnotation,
                        () => enumValueBuilder.SetApiNameDataAnnotation(result.ApiName)
                    );
                }
                catch (Exception exception)
                {
                    this.AddInvalidContributionIssue
                    (
                        enumTypeBuilder.ClrType,
                        enumValueBuilder.ClrName,
                        exception.Message
                    );
                }
            }
        }
    }

    internal void ApplyPropertyAnnotations
    (
        ApiPropertyBuilder propertyBuilder,
        ApiObjectTypeBuilder objectBuilder
    )
    {
        var clrType = objectBuilder.ClrType;
        var clrName = propertyBuilder.ClrName;
        var propertyInfo = TypeReflection.GetProperty(clrType, clrName);

        var clrMember = (MemberInfo?)propertyInfo;
        var clrMemberKind = ClrMemberKind.Property;
        var clrNullabilityInfo = propertyInfo == null
            ? null
            : PropertyReflection.GetNullabilityInfo(propertyInfo);

        if (propertyInfo == null)
        {
            var fieldInfo = TypeReflection.GetField(clrType, clrName);
            if (fieldInfo == null)
            {
                return;
            }

            clrMember = fieldInfo;
            clrMemberKind = ClrMemberKind.Field;
            clrNullabilityInfo = FieldReflection.GetNullabilityInfo(fieldInfo);
        }

        if (clrMember == null || clrNullabilityInfo == null)
        {
            return;
        }

        foreach (var reader in _readers)
        {
            if (reader is not IApiPropertyAnnotationReader propertyReader)
            {
                continue;
            }

            IReadOnlyList<ApiPropertyAnnotationResult>? results;
            try
            {
                results = propertyReader.ReadPropertyAnnotations
                (
                    clrMember,
                    clrMemberKind,
                    clrNullabilityInfo!
                );
            }
            catch (Exception exception)
            {
                this.AddReaderIssue(clrType, clrName, exception);
                continue;
            }

            if (results == null)
            {
                this.AddInvalidContributionIssue
                (
                    clrType,
                    clrName,
                    "A property annotation reader returned null instead of a result list."
                );
                continue;
            }

            foreach (var result in results)
            {
                if (result == null)
                {
                    this.AddInvalidContributionIssue
                    (
                        clrType,
                        clrName,
                        "A property annotation reader returned a null result."
                    );
                    continue;
                }

                try
                {
                    objectBuilder.ConfigurationContext.ApplyConfiguration
                    (
                        ApiConfigurationSource.DataAnnotation,
                        () =>
                        {
                            if (result.ApiName != null)
                            {
                                propertyBuilder.SetApiNameDataAnnotation(result.ApiName);
                            }

                            if (result.Modifiers is not null)
                            {
                                var modifiers = result.Modifiers.Value;
                                propertyBuilder.SetModifiersDataAnnotation
                                (
                                    builder =>
                                    {
                                        if ((modifiers & ApiTypeModifiers.Required) != 0)
                                        {
                                            builder.Required();
                                        }
                                        else
                                        {
                                            builder.Optional();
                                        }
                                    }
                                );
                            }
                        }
                    );
                }
                catch (Exception exception)
                {
                    this.AddInvalidContributionIssue(clrType, clrName, exception.Message);
                }
            }
        }
    }

    internal void ApplyRelationshipAnnotations(ApiSchemaBuilder schemaBuilder, Type clrType)
    {
        foreach (var reader in _readers)
        {
            if (reader is not IApiRelationshipAnnotationReader relationshipReader)
            {
                continue;
            }

            this.ApplyOneToManyRelationships(schemaBuilder, clrType, relationshipReader);
            this.ApplyOneToOneRelationships(schemaBuilder, clrType, relationshipReader);
            this.ApplyManyToManyRelationships(schemaBuilder, clrType, relationshipReader);
        }
    }

    #endregion

    #region Discovery Methods
    internal IReadOnlyList<ApiTypeDiscoveryAnnotationResult> ReadTypeDiscoveryAnnotations
    (
        Assembly assembly,
        Func<Type, bool>? filter
    )
    {
        var eligibleTypes = assembly.GetExportedTypes()
            .Where(IsEligibleType)
            .Where(type => filter == null || filter(type))
            .ToHashSet();
        var results = new List<ApiTypeDiscoveryAnnotationResult>();

        foreach (var reader in _readers)
        {
            if (reader is not IApiTypeDiscoveryAnnotationReader discoveryReader)
            {
                continue;
            }

            IReadOnlyList<ApiTypeDiscoveryAnnotationResult>? readerResults;
            try
            {
                readerResults = discoveryReader.ReadTypeDiscoveryAnnotations(assembly, filter);
            }
            catch (Exception exception)
            {
                this.AddReaderIssue(assembly.GetName().Name ?? assembly.FullName ?? assembly.ToString(), exception);
                continue;
            }

            if (readerResults == null)
            {
                this.AddInvalidContributionIssue
                (
                    assembly.GetName().Name ?? assembly.FullName ?? assembly.ToString(),
                    "A type-discovery annotation reader returned null instead of a result list."
                );
                continue;
            }

            foreach (var result in readerResults)
            {
                if (result == null)
                {
                    this.AddInvalidContributionIssue
                    (
                        assembly.GetName().Name ?? assembly.FullName ?? assembly.ToString(),
                        "A type-discovery annotation reader returned a null result."
                    );
                    continue;
                }

                var resultType = result.ClrType;
                if (!eligibleTypes.Contains(resultType))
                {
                    this.AddInvalidContributionIssue
                    (
                        resultType.FullName ?? assembly.FullName ?? assembly.ToString(),
                        "A type-discovery contribution did not satisfy the assembly scan eligibility rules."
                    );
                    continue;
                }

                if (result.ApiKind is not (ApiTypeKind.Object or ApiTypeKind.Scalar or ApiTypeKind.Enum))
                {
                    this.AddInvalidContributionIssue
                    (
                        resultType.FullName ?? resultType.Name,
                        $"The API type kind '{result.ApiKind}' cannot be discovered from a CLR type."
                    );
                    continue;
                }

                results.Add(result);
            }
        }

        return results;
    }

    #endregion

    #region Key Methods
    private void ApplyKeyAnnotations(ApiObjectTypeBuilder builder)
    {
        var contributions = new Dictionary<string, (int ReaderIndex, List<ApiKeyAnnotationResult> Results)>();
        var readerIndex = 0;

        foreach (var reader in _readers)
        {
            if (reader is not IApiKeyAnnotationReader keyReader)
            {
                readerIndex++;
                continue;
            }

            IReadOnlyList<ApiKeyAnnotationResult>? results;
            try
            {
                results = keyReader.ReadKeyAnnotations(builder.ClrType);
            }
            catch (Exception exception)
            {
                this.AddReaderIssue(builder.ClrType, exception);
                readerIndex++;
                continue;
            }

            if (results == null)
            {
                this.AddInvalidContributionIssue
                (
                    builder.ClrType,
                    "A key annotation reader returned null instead of a result list."
                );
                readerIndex++;
                continue;
            }

            foreach (var result in results)
            {
                if (result == null || !this.IsValidKeyResult(builder.ClrType, result))
                {
                    if (result == null)
                    {
                        this.AddInvalidContributionIssue
                        (
                            builder.ClrType,
                            "A key annotation reader returned a null result."
                        );
                    }

                    continue;
                }

                if (!contributions.TryGetValue(result.ApiName, out var existing) ||
                    existing.ReaderIndex != readerIndex)
                {
                    existing = (readerIndex, []);
                    contributions[result.ApiName] = existing;
                }

                if (existing.Results.All
                    (
                        existingResult =>
                            existingResult.Order != result.Order ||
                            existingResult.ClrRootType != result.ClrRootType ||
                            !existingResult.ClrPropertyNames.SequenceEqual(result.ClrPropertyNames)
                    ))
                {
                    existing.Results.Add(result);
                }
            }

            readerIndex++;
        }

        foreach (var (apiName, contribution) in contributions)
        {
            if (builder.HasExplicitKey(apiName))
            {
                continue;
            }

            var duplicateOrders = contribution.Results
                .GroupBy(result => result.Order)
                .Where(group => group.Count() > 1)
                .ToList();

            foreach (var duplicateOrder in duplicateOrders)
            {
                this.AddInvalidContributionIssue
                (
                    builder.ClrType,
                    $"Key:{apiName}",
                    $"Multiple annotation key paths use order {duplicateOrder.Key}. Key path orders must be unique.",
                    ApiInitializationCode.ApiAnnotationKeyOrderConflict
                );
            }

            var paths = contribution.Results
                .OrderBy(result => result.Order)
                .Select(result => (result.ClrRootType, result.ClrPropertyNames))
                .DistinctBy(path => (path.ClrRootType, string.Join("\0", path.ClrPropertyNames)))
                .ToList();

            builder.ReplaceKeyFromDataAnnotation(apiName, paths);
        }
    }

    #endregion

    #region Type and Validation Methods
    private void ApplyTypeResults<TBuilder>
    (
        TBuilder builder,
        Type clrType,
        Func<Type, IReadOnlyList<ApiTypeAnnotationResult>> read,
        string targetKind
    )
        where TBuilder : ApiNamedTypeBuilder<TBuilder>
    {
        IReadOnlyList<ApiTypeAnnotationResult>? results;
        try
        {
            results = read(clrType);
        }
        catch (Exception exception)
        {
            this.AddReaderIssue(clrType, exception);
            return;
        }

        if (results == null)
        {
            this.AddInvalidContributionIssue
            (
                clrType,
                $"A {targetKind} annotation reader returned null instead of a result list."
            );
            return;
        }

        foreach (var result in results)
        {
            if (result == null)
            {
                this.AddInvalidContributionIssue
                (
                    clrType,
                    $"A {targetKind} annotation reader returned a null result."
                );
                continue;
            }

            if (result.ApiName == null)
            {
                continue;
            }

            try
            {
                builder.ConfigurationContext.ApplyConfiguration
                (
                    ApiConfigurationSource.DataAnnotation,
                    () => builder.SetApiNameDataAnnotation(result.ApiName)
                );
            }
            catch (Exception exception)
            {
                this.AddInvalidContributionIssue(clrType, exception.Message);
            }
        }
    }

    private bool IsValidKeyResult(Type clrType, ApiKeyAnnotationResult result)
    {
        if (string.IsNullOrWhiteSpace(result.ApiName) ||
            result.ClrRootType == null ||
            result.ClrPropertyNames == null ||
            result.ClrPropertyNames.Count == 0 ||
            result.ClrPropertyNames.Any(string.IsNullOrWhiteSpace))
        {
            this.AddInvalidContributionIssue
            (
                clrType,
                $"Key:{result.ApiName}",
                "A key annotation contribution must provide a name, CLR root type, and non-empty member path."
            );
            return false;
        }

        return true;
    }

    #endregion

    #region Relationship Methods
    private void ApplyOneToManyRelationships
    (
        ApiSchemaBuilder schemaBuilder,
        Type clrType,
        IApiRelationshipAnnotationReader reader
    )
    {
        IReadOnlyList<ApiOneToManyRelationshipAnnotationResult>? results;
        try
        {
            results = reader.ReadOneToManyRelationships(clrType);
        }
        catch (Exception exception)
        {
            this.AddReaderIssue(clrType, exception);
            return;
        }

        if (results == null)
        {
            this.AddInvalidContributionIssue(clrType, "A one-to-many reader returned null instead of a result list.");
            return;
        }

        foreach (var result in results)
        {
            try
            {
                schemaBuilder.AddOneToManyRelationshipCore
                (
                    result.ApiName,
                    builder =>
                    {
                        builder
                            .WithDeleteBehavior(result.DeleteBehavior)
                            .From(result.PrincipalType)
                            .To
                            (
                                result.DependentType,
                                result.ForeignKey == null
                                    ? null
                                    : dependent => dependent.WithForeignKey
                                    (
                                        foreignKey => foreignKey.AddPath
                                        (
                                            result.DependentType,
                                            result.ForeignKey
                                        )
                                    )
                            );
                    },
                    ApiConfigurationSource.DataAnnotation
                );
            }
            catch (Exception exception)
            {
                this.AddInvalidContributionIssue(clrType, exception.Message);
            }
        }
    }

    private void ApplyOneToOneRelationships
    (
        ApiSchemaBuilder schemaBuilder,
        Type clrType,
        IApiRelationshipAnnotationReader reader
    )
    {
        IReadOnlyList<ApiOneToOneRelationshipAnnotationResult>? results;
        try
        {
            results = reader.ReadOneToOneRelationships(clrType);
        }
        catch (Exception exception)
        {
            this.AddReaderIssue(clrType, exception);
            return;
        }

        if (results == null)
        {
            this.AddInvalidContributionIssue(clrType, "A one-to-one reader returned null instead of a result list.");
            return;
        }

        foreach (var result in results)
        {
            try
            {
                schemaBuilder.AddOneToOneRelationshipCore
                (
                    result.ApiName,
                    builder =>
                    {
                        builder
                            .WithDeleteBehavior(result.DeleteBehavior)
                            .From(result.PrincipalType)
                            .To
                            (
                                result.DependentType,
                                result.ForeignKey == null
                                    ? null
                                    : dependent => dependent.WithForeignKey
                                    (
                                        foreignKey => foreignKey.AddPath
                                        (
                                            result.DependentType,
                                            result.ForeignKey
                                        )
                                    )
                            );
                    },
                    ApiConfigurationSource.DataAnnotation
                );
            }
            catch (Exception exception)
            {
                this.AddInvalidContributionIssue(clrType, exception.Message);
            }
        }
    }

    private void ApplyManyToManyRelationships
    (
        ApiSchemaBuilder schemaBuilder,
        Type clrType,
        IApiRelationshipAnnotationReader reader
    )
    {
        IReadOnlyList<ApiManyToManyRelationshipAnnotationResult>? results;
        try
        {
            results = reader.ReadManyToManyRelationships(clrType);
        }
        catch (Exception exception)
        {
            this.AddReaderIssue(clrType, exception);
            return;
        }

        if (results == null)
        {
            this.AddInvalidContributionIssue(clrType, "A many-to-many reader returned null instead of a result list.");
            return;
        }

        foreach (var result in results)
        {
            try
            {
                schemaBuilder.AddManyToManyRelationshipCore
                (
                    result.ApiName,
                    builder =>
                    {
                        builder
                            .Between(result.PrincipalTypeA)
                            .And(result.PrincipalTypeB)
                            .WithAssociation
                            (
                                result.AssociationType,
                                association =>
                                {
                                    if (result.ForeignKeyA != null)
                                    {
                                        association.WithForeignKeyA
                                        (
                                            foreignKey => foreignKey.AddPath
                                            (
                                                result.AssociationType,
                                                result.ForeignKeyA
                                            )
                                        );
                                    }

                                    if (result.ForeignKeyB != null)
                                    {
                                        association.WithForeignKeyB
                                        (
                                            foreignKey => foreignKey.AddPath
                                            (
                                                result.AssociationType,
                                                result.ForeignKeyB
                                            )
                                        );
                                    }
                                }
                            );
                    },
                    ApiConfigurationSource.DataAnnotation
                );
            }
            catch (Exception exception)
            {
                this.AddInvalidContributionIssue(clrType, exception.Message);
            }
        }
    }

    #endregion

    #region Issue Methods
    private void AddReaderIssue(Type clrType, Exception exception)
        => this.AddReaderIssue(clrType.FullName ?? clrType.Name, exception);

    private void AddReaderIssue(Type clrType, string memberName, Exception exception)
        => this.AddReaderIssue
        (
            $"{clrType.FullName ?? clrType.Name}.{memberName}",
            exception
        );

    private void AddReaderIssue(string apiPath, Exception exception)
    {
        _issues.Add
        (
            new ApiInitializationIssue
            (
                apiPath,
                ApiInitializationSeverity.Error,
                ApiInitializationCode.ApiAnnotationReaderExecutionFailed,
                exception.Message,
                "Correct the annotation reader implementation or its input metadata."
            )
        );
    }

    private void AddInvalidContributionIssue(Type clrType, string description)
        => this.AddInvalidContributionIssue
        (
            clrType.FullName ?? clrType.Name,
            description,
            ApiInitializationCode.ApiAnnotationInvalidContribution
        );

    private void AddInvalidContributionIssue(Type clrType, string memberName, string description)
        => this.AddInvalidContributionIssue
        (
            $"{clrType.FullName ?? clrType.Name}.{memberName}",
            description,
            ApiInitializationCode.ApiAnnotationInvalidContribution
        );

    private void AddInvalidContributionIssue
    (
        Type clrType,
        string memberName,
        string description,
        ApiInitializationCode code
    )
        => this.AddInvalidContributionIssue
        (
            $"{clrType.FullName ?? clrType.Name}.{memberName}",
            description,
            code
        );

    private void AddInvalidContributionIssue
    (
        string apiPath,
        string description,
        ApiInitializationCode code = ApiInitializationCode.ApiAnnotationInvalidContribution
    )
    {
        _issues.Add
        (
            new ApiInitializationIssue
            (
                apiPath,
                ApiInitializationSeverity.Error,
                code,
                description,
                "Return a valid declarative annotation result for the supplied target."
            )
        );
    }

    #endregion

    #region Helper Methods
    private static bool IsEligibleType(Type type)
    {
        return !type.IsAbstract && (type.IsClass || type.IsValueType);
    }
    #endregion
}
