// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Relationships.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal static class ApiRelationshipKeyAlignment
{
    #region ApiRelationshipKeyAlignment Methods
    public static ApiRelationshipKeyBinding? ResolvePrincipalForeignKeyBinding
    (
        ApiSchemaCompilationContext context,
        string relationshipPath,
        ApiRelationshipPrincipalEnd principalEnd,
        ApiKeyDefinition foreignKey,
        ApiSchemaCompilationCode countMismatchCode,
        string foreignKeyPath,
        string principalCountLabel,
        string principalCompatibilityLabel,
        string? principalEndQualifier,
        string explicitKeyTarget,
        string inferredForeignKeyLabel,
        string countMismatchRemediationTarget,
        string compatibilityRemediation
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(principalEnd);
        ArgumentNullException.ThrowIfNull(foreignKey);

        var keyPathCount = foreignKey.ApiKeyPaths.Length;
        var runCountCheck = true;
        ApiNamedKeyDefinition? principalKey = null;
        var principalKeyResolutionSource = ApiRelationshipPrincipalKeyResolutionSource.Inferred;

        var principalObjectType = principalEnd.ApiResolvedObjectType;
        if (principalObjectType is null)
        {
            // The principal end already recorded the unresolved object type issue.
            return null;
        }

        if (principalEnd.ApiPrincipalKeyName is not null)
        {
            principalKeyResolutionSource = ApiRelationshipPrincipalKeyResolutionSource.Explicit;
            if (!principalObjectType.TryGetKeyByApiName(principalEnd.ApiPrincipalKeyName, out principalKey))
            {
                AddUnresolvedExplicitPrincipalKeyIssue(context, principalEnd, principalObjectType);
                return null;
            }
        }
        else
        {
            var matchingShapeKeys = principalObjectType.ApiKeys
                .Where(keyDefinition => ApiRelationshipKeyCompatibility.CountKeyLeaves(keyDefinition) == keyPathCount)
                .Select
                (
                    static keyDefinition =>
                        new KeyValuePair<string, ApiNamedKeyDefinition>(keyDefinition.ApiName, keyDefinition)
                )
                .ToList();
            var matchingKeys = matchingShapeKeys
                .Where(kvp => ApiRelationshipKeyCompatibility.AreKeysCompatible(kvp.Value, foreignKey))
                .ToList();

            if (matchingKeys.Count > 1)
            {
                AddAmbiguousPrincipalKeyIssue(context, relationshipPath, principalObjectType, matchingKeys, principalEndQualifier, explicitKeyTarget);
                runCountCheck = false;
            }
            else if (matchingKeys.Count == 1)
            {
                principalKey = matchingKeys[0].Value;
            }
            else if (matchingShapeKeys.Count > 0)
            {
                AddInferredIncompatiblePrincipalKeyIssue
                (
                    context,
                    relationshipPath,
                    principalObjectType,
                    foreignKey,
                    matchingShapeKeys,
                    keyPathCount,
                    principalEndQualifier,
                    explicitKeyTarget,
                    inferredForeignKeyLabel
                );

                runCountCheck = false;
            }
            else
            {
                AddInferredCountMismatchIssue
                (
                    context,
                    relationshipPath,
                    foreignKeyPath,
                    principalObjectType,
                    keyPathCount,
                    countMismatchCode,
                    principalEndQualifier,
                    explicitKeyTarget
                );

                runCountCheck = false;
            }
        }

        if (principalKey is null)
        {
            // Inference failed and the relevant issue was already recorded.
            return null;
        }

        if (!runCountCheck)
        {
            return null;
        }

        var principalKeyPathCount = ApiRelationshipKeyCompatibility.CountKeyLeaves(principalKey);
        if (principalKeyPathCount is not null && keyPathCount != principalKeyPathCount)
        {
            AddCountMismatchIssue
            (
                context,
                relationshipPath,
                foreignKeyPath,
                keyPathCount,
                principalKeyPathCount.Value,
                countMismatchCode,
                principalCountLabel,
                countMismatchRemediationTarget
            );
            return null;
        }

        if (ApiRelationshipKeyCompatibility.TryAreKeysCompatible(principalKey, foreignKey, out var isCompatible) && !isCompatible)
        {
            AddExplicitIncompatiblePrincipalKeyIssue
            (
                context,
                relationshipPath,
                foreignKeyPath,
                foreignKey,
                principalKey,
                principalCompatibilityLabel,
                compatibilityRemediation
            );

            return null;
        }

        return new ApiRelationshipKeyBinding
        (
            principalEnd,
            principalKey,
            foreignKey,
            principalKeyResolutionSource
        );
    }
    #endregion

    #region Implementation Methods
    private static void AddUnresolvedExplicitPrincipalKeyIssue
    (
        ApiSchemaCompilationContext context,
        ApiRelationshipPrincipalEnd principalEnd,
        ApiObjectType principalObjectType
    )
    {
        var availableKeys = string.Join(", ", principalObjectType.ApiKeyApiNames.Select(static k => $"'{k}'"));
        var remediation = !string.IsNullOrEmpty(availableKeys)
            ? $"Use one of the available keys: {availableKeys}"
            : $"Define a key on '{principalObjectType.ApiName}' or remove {nameof(ApiRelationshipPrincipalEnd.ApiPrincipalKeyName)}";

        var path = principalEnd.ApiPath;
        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiRelationshipEndUnresolvedKey;
        var description = $"Referenced principal key '{principalEnd.ApiPrincipalKeyName}' could not be found on object type '{principalObjectType.ApiName}'";

        context.AddIssue(path, severity, code, description, remediation);
    }

    private static void AddAmbiguousPrincipalKeyIssue
    (
        ApiSchemaCompilationContext context,
        string relationshipPath,
        ApiObjectType principalObjectType,
        List<KeyValuePair<string, ApiNamedKeyDefinition>> matchingKeys,
        string? principalEndQualifier,
        string explicitKeyTarget
    )
    {
        var keyNames = string.Join(", ", matchingKeys.Select(static kvp => $"'{kvp.Key}'"));
        var qualifier = string.IsNullOrWhiteSpace(principalEndQualifier) ? null : $" {principalEndQualifier}";
        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiRelationshipAmbiguousPrincipalKey;
        var description = $"Cannot automatically determine the referenced principal key{qualifier}: {matchingKeys.Count} keys on '{principalObjectType.ApiName}' are compatible with the foreign key: {keyNames}";
        var remediation = $"Set {explicitKeyTarget} to specify the principal key explicitly; available keys: {keyNames}";

        context.AddIssue(relationshipPath, severity, code, description, remediation);
    }

    private static void AddCountMismatchIssue
    (
        ApiSchemaCompilationContext context,
        string relationshipPath,
        string foreignKeyPath,
        int keyPathCount,
        int principalKeyPathCount,
        ApiSchemaCompilationCode countMismatchCode,
        string principalCountLabel,
        string countMismatchRemediationTarget
    )
    {
        var severity = ApiSchemaCompilationSeverity.Error;
        var description = $"{foreignKeyPath}.{nameof(ApiKeyDefinition.ApiKeyPaths)} has {keyPathCount} key path(s) but {principalCountLabel} has {principalKeyPathCount} key path(s)";
        var remediation = $"Ensure {foreignKeyPath}.{nameof(ApiKeyDefinition.ApiKeyPaths)} contains exactly {principalKeyPathCount} key path(s) to match {countMismatchRemediationTarget}";

        context.AddIssue(relationshipPath, severity, countMismatchCode, description, remediation);
    }

    private static void AddInferredCountMismatchIssue
    (
        ApiSchemaCompilationContext context,
        string relationshipPath,
        string foreignKeyPath,
        ApiObjectType principalObjectType,
        int keyPathCount,
        ApiSchemaCompilationCode countMismatchCode,
        string? principalEndQualifier,
        string explicitKeyTarget
    )
    {
        var keyNames = string.Join(", ", principalObjectType.ApiKeys.Select(static keyDefinition => $"'{keyDefinition.ApiName}'"));
        var qualifier = string.IsNullOrWhiteSpace(principalEndQualifier) ? null : $" {principalEndQualifier}";
        var severity = ApiSchemaCompilationSeverity.Error;
        var description = $"Cannot automatically determine the referenced principal key{qualifier}: {foreignKeyPath}.{nameof(ApiKeyDefinition.ApiKeyPaths)} has {keyPathCount} key path(s), but no key on '{principalObjectType.ApiName}' has {keyPathCount} key path(s)";
        var remediation = principalObjectType.ApiKeys.Length > 0
            ? $"Set {explicitKeyTarget} explicitly or align the foreign key shape with one of these keys: {keyNames}"
            : $"Define a key on '{principalObjectType.ApiName}' or set {explicitKeyTarget} explicitly";

        context.AddIssue(relationshipPath, severity, countMismatchCode, description, remediation);
    }

    private static void AddExplicitIncompatiblePrincipalKeyIssue
    (
        ApiSchemaCompilationContext context,
        string relationshipPath,
        string foreignKeyPath,
        ApiKeyDefinition foreignKey,
        ApiNamedKeyDefinition principalKey,
        string principalCompatibilityLabel,
        string compatibilityRemediation
    )
    {
        var principalKeys = ApiRelationshipKeyCompatibility.DescribeKeyLeafTypes(principalKey);
        var foreignKeys = ApiRelationshipKeyCompatibility.DescribeKeyLeafTypes(foreignKey);
        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiRelationshipIncompatiblePrincipalForeignKey;
        var description = $"{foreignKeyPath} leaf type(s) [{foreignKeys}] are not compatible with {principalCompatibilityLabel} leaf type(s) [{principalKeys}]";

        context.AddIssue(relationshipPath, severity, code, description, compatibilityRemediation);
    }

    private static void AddInferredIncompatiblePrincipalKeyIssue
    (
        ApiSchemaCompilationContext context,
        string relationshipPath,
        ApiObjectType principalObjectType,
        ApiKeyDefinition foreignKey,
        List<KeyValuePair<string, ApiNamedKeyDefinition>> matchingShapeKeys,
        int keyPathCount,
        string? principalEndQualifier,
        string explicitKeyTarget,
        string inferredForeignKeyLabel
    )
    {
        var canCompare = matchingShapeKeys.Any(kvp => ApiRelationshipKeyCompatibility.TryAreKeysCompatible(kvp.Value, foreignKey, out _));
        if (!canCompare)
        {
            return;
        }

        var keyNames = string.Join(", ", matchingShapeKeys.Select(static kvp => $"'{kvp.Key}'"));
        var foreignKeys = ApiRelationshipKeyCompatibility.DescribeKeyLeafTypes(foreignKey);
        var qualifier = string.IsNullOrWhiteSpace(principalEndQualifier) ? null : $" {principalEndQualifier}";
        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiRelationshipIncompatiblePrincipalForeignKey;
        var description = $"Cannot automatically determine the referenced principal key{qualifier}: no key on '{principalObjectType.ApiName}' with {keyPathCount} key path(s) is compatible with foreign key leaf type(s) [{foreignKeys}]";
        var remediation = $"Set {explicitKeyTarget} explicitly or align the {inferredForeignKeyLabel} leaf type(s) with one of these keys: {keyNames}";

        context.AddIssue(relationshipPath, severity, code, description, remediation);
    }
    #endregion
}
