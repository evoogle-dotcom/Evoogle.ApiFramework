// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations.Internal;

/// <summary>Holds the ordered annotation readers and centrally applies their results.</summary>
internal sealed class ApiAnnotationReaderSet
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
    internal IReadOnlyList<IApiAnnotationReader> Readers => _readers;

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
                    typeReader,
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
                    typeReader,
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
                    typeReader,
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
                this.AddReaderIssue
                (
                    enumValueReader,
                    enumTypeBuilder.ClrType,
                    enumValueBuilder.ClrName,
                    exception
                );
                continue;
            }

            if (results == null)
            {
                this.AddInvalidContributionIssue
                (
                    enumValueReader,
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
                        enumValueReader,
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
                        enumValueReader,
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
                this.AddReaderIssue(propertyReader, clrType, clrName, exception);
                continue;
            }

            if (results == null)
            {
                this.AddInvalidContributionIssue
                (
                    propertyReader,
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
                        propertyReader,
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
                    this.AddInvalidContributionIssue
                    (
                        propertyReader,
                        clrType,
                        clrName,
                        exception.Message
                    );
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
        var discoveredTypes = new Dictionary
        <Type, (ApiTypeKind ApiKind, IApiAnnotationReader Reader)>();

        foreach (var reader in _readers)
        {
            if (reader is not IApiTypeDiscoveryAnnotationReader discoveryReader)
            {
                continue;
            }

            ApiAnnotationReaderResult<ApiTypeDiscoveryAnnotationResult>? readerResult;
            try
            {
                readerResult = discoveryReader.ReadTypeDiscoveryAnnotations(assembly, filter);
            }
            catch (Exception exception)
            {
                this.AddReaderIssue
                (
                    discoveryReader,
                    assembly.GetName().Name ?? assembly.FullName ?? assembly.ToString(),
                    exception
                );
                continue;
            }

            var assemblyApiPath = assembly.GetName().Name ??
                assembly.FullName ??
                assembly.ToString();
            if (readerResult == null)
            {
                this.AddInvalidContributionIssue
                (
                    discoveryReader,
                    assemblyApiPath,
                    "A type-discovery annotation reader returned null instead of a result list."
                );
                continue;
            }

            this.ApplyReaderDiagnostics(discoveryReader, assemblyApiPath, readerResult.Diagnostics);

            var readerResults = readerResult.Contributions;
            if (readerResults == null)
            {
                this.AddInvalidContributionIssue
                (
                    discoveryReader,
                    assemblyApiPath,
                    "A type-discovery annotation reader returned null contributions."
                );
                continue;
            }

            foreach (var result in readerResults)
            {
                if (result == null)
                {
                    this.AddInvalidContributionIssue
                    (
                        discoveryReader,
                        assemblyApiPath,
                        "A type-discovery annotation reader returned a null result."
                    );
                    continue;
                }

                var resultType = result.ClrType;
                if (resultType == null)
                {
                    this.AddInvalidContributionIssue
                    (
                        discoveryReader,
                        assemblyApiPath,
                        "A type-discovery contribution returned a null CLR type."
                    );
                    continue;
                }

                if (!eligibleTypes.Contains(resultType))
                {
                    this.AddInvalidContributionIssue
                    (
                        discoveryReader,
                        resultType.FullName ?? assembly.FullName ?? assembly.ToString(),
                        "A type-discovery contribution did not satisfy the assembly scan eligibility rules."
                    );
                    continue;
                }

                if (result.ApiKind is not (ApiTypeKind.Object or ApiTypeKind.Scalar or ApiTypeKind.Enum))
                {
                    this.AddInvalidContributionIssue
                    (
                        discoveryReader,
                        resultType.FullName ?? resultType.Name,
                        $"The API type kind '{result.ApiKind}' cannot be discovered from a CLR type."
                    );
                    continue;
                }

                if
                (
                    discoveredTypes.TryGetValue(resultType, out var existingDiscovery) &&
                    existingDiscovery.ApiKind != result.ApiKind
                )
                {
                    this.ApplyReaderDiagnostics
                    (
                        discoveryReader,
                        resultType.FullName ?? resultType.Name,
                        [new
                        (
                            ApiInitializationCode.ApiAnnotationTypeDiscoveryConflict,
                            resultType.FullName ?? resultType.Name,
                            "The CLR type was already discovered as an " +
                            $"API {existingDiscovery.ApiKind} " +
                            "by reader '" +
                            $"{existingDiscovery.Reader.GetType().FullName ?? existingDiscovery.Reader.GetType().Name}' " +
                            $"and cannot also be discovered as an API {result.ApiKind}.",
                            "Ensure all annotation readers agree on the API type kind for each " +
                            "CLR type."
                        )]
                    );
                    continue;
                }

                discoveredTypes.TryAdd(resultType, (result.ApiKind, discoveryReader));
                results.Add(result);
            }
        }

        return results;
    }

    #endregion

    #region Key Methods
    private void ApplyKeyAnnotations(ApiObjectTypeBuilder builder)
    {
        var contributions = new Dictionary
        <
            string,
            (int ReaderIndex, IApiAnnotationReader Reader, List<ApiKeyAnnotationResult> Results)
        >();
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
                this.AddReaderIssue(keyReader, builder.ClrType, exception);
                readerIndex++;
                continue;
            }

            if (results == null)
            {
                this.AddInvalidContributionIssue
                (
                    keyReader,
                    builder.ClrType,
                    "A key annotation reader returned null instead of a result list."
                );
                readerIndex++;
                continue;
            }

            foreach (var result in results)
            {
                if (result == null || !this.IsValidKeyResult(keyReader, builder.ClrType, result))
                {
                    if (result == null)
                    {
                        this.AddInvalidContributionIssue
                        (
                            keyReader,
                            builder.ClrType,
                            "A key annotation reader returned a null result."
                        );
                    }

                    continue;
                }

                if (!contributions.TryGetValue(result.ApiName, out var existing) ||
                    existing.ReaderIndex != readerIndex)
                {
                    existing = (readerIndex, keyReader, []);
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
                    contribution.Reader,
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
        IApiAnnotationReader reader,
        Func<Type, IReadOnlyList<ApiTypeAnnotationResult>> read,
        string targetKind
    )
        where TBuilder : ApiNamedTypeBuilder<TBuilder>
    {
        this.ApplyTypeDiagnostics(reader, clrType);

        IReadOnlyList<ApiTypeAnnotationResult>? results;
        try
        {
            results = read(clrType);
        }
        catch (Exception exception)
        {
            this.AddReaderIssue(reader, clrType, exception);
            return;
        }

        if (results == null)
        {
            this.AddInvalidContributionIssue
            (
                reader,
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
                    reader,
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
                this.AddInvalidContributionIssue(reader, clrType, exception.Message);
            }
        }
    }

    private void ApplyTypeDiagnostics(IApiAnnotationReader reader, Type clrType)
    {
        if (reader is not IApiTypeAnnotationDiagnosticReader diagnosticReader)
        {
            return;
        }

        IReadOnlyList<ApiAnnotationReaderDiagnostic>? diagnostics;
        try
        {
            diagnostics = diagnosticReader.ReadTypeAnnotationDiagnostics(clrType);
        }
        catch (Exception exception)
        {
            this.AddReaderIssue(diagnosticReader, clrType, exception);
            return;
        }

        this.ApplyReaderDiagnostics
        (
            diagnosticReader,
            clrType.FullName ?? clrType.Name,
            diagnostics
        );
    }

    private bool IsValidKeyResult
    (
        IApiAnnotationReader reader,
        Type clrType,
        ApiKeyAnnotationResult result
    )
    {
        if (string.IsNullOrWhiteSpace(result.ApiName) ||
            result.ClrRootType == null ||
            result.ClrPropertyNames == null ||
            result.ClrPropertyNames.Count == 0 ||
            result.ClrPropertyNames.Any(string.IsNullOrWhiteSpace))
        {
            this.AddInvalidContributionIssue
            (
                reader,
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
            this.AddReaderIssue(reader, clrType, exception);
            return;
        }

        if (results == null)
        {
            this.AddInvalidContributionIssue
            (
                reader,
                clrType,
                "A one-to-many reader returned null instead of a result list."
            );
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
                this.AddInvalidContributionIssue(reader, clrType, exception.Message);
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
            this.AddReaderIssue(reader, clrType, exception);
            return;
        }

        if (results == null)
        {
            this.AddInvalidContributionIssue
            (
                reader,
                clrType,
                "A one-to-one reader returned null instead of a result list."
            );
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
                this.AddInvalidContributionIssue(reader, clrType, exception.Message);
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
            this.AddReaderIssue(reader, clrType, exception);
            return;
        }

        if (results == null)
        {
            this.AddInvalidContributionIssue
            (
                reader,
                clrType,
                "A many-to-many reader returned null instead of a result list."
            );
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
                this.AddInvalidContributionIssue(reader, clrType, exception.Message);
            }
        }
    }

    #endregion

    #region Issue Methods
    private void ApplyReaderDiagnostics
    (
        IApiAnnotationReader reader,
        string defaultApiPath,
        IReadOnlyList<ApiAnnotationReaderDiagnostic>? diagnostics
    )
    {
        if (diagnostics == null)
        {
            this.AddInvalidContributionIssue
            (
                reader,
                defaultApiPath,
                "An annotation reader returned null diagnostics."
            );
            return;
        }

        foreach (var diagnostic in diagnostics)
        {
            if (diagnostic == null)
            {
                this.AddInvalidContributionIssue
                (
                    reader,
                    defaultApiPath,
                    "An annotation reader returned a null diagnostic."
                );
                continue;
            }

            if (!Enum.IsDefined(typeof(ApiInitializationCode), diagnostic.Code))
            {
                this.AddInvalidContributionIssue
                (
                    reader,
                    defaultApiPath,
                    "An annotation reader returned a diagnostic with an undefined " +
                    "initialization code."
                );
                continue;
            }

            if (string.IsNullOrWhiteSpace(diagnostic.ApiPath) ||
                string.IsNullOrWhiteSpace(diagnostic.Description))
            {
                this.AddInvalidContributionIssue
                (
                    reader,
                    defaultApiPath,
                    "An annotation reader returned a diagnostic without a path or description."
                );
                continue;
            }

            _issues.Add
            (
                new ApiInitializationIssue
                (
                    diagnostic.ApiPath,
                    diagnostic.Severity,
                    diagnostic.Code,
                    diagnostic.Description,
                    diagnostic.Remediation,
                    reader.GetType(),
                    diagnostic.Exception
                )
            );
        }
    }

    private void AddReaderIssue(IApiAnnotationReader reader, Type clrType, Exception exception)
        => this.AddReaderIssue(reader, clrType.FullName ?? clrType.Name, exception);

    private void AddReaderIssue
    (
        IApiAnnotationReader reader,
        Type clrType,
        string memberName,
        Exception exception
    )
        => this.AddReaderIssue
        (
            reader,
            $"{clrType.FullName ?? clrType.Name}.{memberName}",
            exception
        );

    private void AddReaderIssue(IApiAnnotationReader reader, string apiPath, Exception exception)
    {
        _issues.Add
        (
            new ApiInitializationIssue
            (
                apiPath,
                ApiInitializationSeverity.Error,
                ApiInitializationCode.ApiAnnotationReaderExecutionFailed,
                exception.Message,
                "Correct the annotation reader implementation or its input metadata.",
                reader.GetType(),
                exception
            )
        );
    }

    private void AddInvalidContributionIssue
    (
        IApiAnnotationReader reader,
        Type clrType,
        string description
    )
        => this.AddInvalidContributionIssue
        (
            reader,
            clrType.FullName ?? clrType.Name,
            description,
            ApiInitializationCode.ApiAnnotationInvalidContribution
        );

    private void AddInvalidContributionIssue
    (
        IApiAnnotationReader reader,
        Type clrType,
        string memberName,
        string description
    )
        => this.AddInvalidContributionIssue
        (
            reader,
            $"{clrType.FullName ?? clrType.Name}.{memberName}",
            description,
            ApiInitializationCode.ApiAnnotationInvalidContribution
        );

    private void AddInvalidContributionIssue
    (
        IApiAnnotationReader reader,
        Type clrType,
        string memberName,
        string description,
        ApiInitializationCode code
    )
        => this.AddInvalidContributionIssue
        (
            reader,
            $"{clrType.FullName ?? clrType.Name}.{memberName}",
            description,
            code
        );

    private void AddInvalidContributionIssue
    (
        IApiAnnotationReader reader,
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
                "Return a valid declarative annotation result for the supplied target.",
                reader.GetType()
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
