// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiRelationshipKeyAlignment
{
    #region ApiRelationshipKeyAlignment Methods
    public static void ValidatePrincipalForeignKeyAlignment
    (
        ApiInitializationContext context,
        string relationshipPath,
        ApiRelationshipPrincipalEnd principalEnd,
        ApiKeyType foreignKeyType,
        ApiInitializationCode countMismatchCode,
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
        ArgumentNullException.ThrowIfNull(foreignKeyType);

        var principalKeyType = principalEnd.ResolvedKeyType;
        if (principalKeyType is null)
        {
            // Principal key type failed to resolve; the principal end already recorded the error.
            return;
        }

        var keyPathCount = foreignKeyType.ApiKeyPaths.Length;
        var runCountCheck = true;

        var principalObjectType = principalEnd.ResolvedApiObjectType;
        if (principalEnd.ApiKeyTypeName is null && principalObjectType is not null)
        {
            var matchingShapeKeys = principalObjectType.ApiKeyTypes
                .Where(kvp => ApiSchemaHelpers.CountKeyLeaves(kvp.Value) == keyPathCount)
                .ToList();
            var matchingKeys = matchingShapeKeys
                .Where(kvp => ApiSchemaHelpers.AreKeyTypesCompatible(kvp.Value, foreignKeyType))
                .ToList();

            if (matchingKeys.Count > 1)
            {
                AddAmbiguousPrincipalKeyIssue(context, relationshipPath, principalObjectType, matchingKeys, principalEndQualifier, explicitKeyTarget);
                runCountCheck = false;
            }
            else if (matchingKeys.Count == 1)
            {
                principalKeyType = matchingKeys[0].Value;
                if (!ReferenceEquals(principalKeyType, principalEnd.ResolvedKeyType))
                {
                    principalEnd.OverrideResolvedKeyType(principalKeyType);
                }
            }
            else if (matchingShapeKeys.Count > 0)
            {
                AddInferredIncompatiblePrincipalKeyIssue
                (
                    context,
                    relationshipPath,
                    principalObjectType,
                    foreignKeyType,
                    matchingShapeKeys,
                    keyPathCount,
                    principalEndQualifier,
                    explicitKeyTarget,
                    inferredForeignKeyLabel
                );

                runCountCheck = false;
            }
        }

        if (!runCountCheck)
        {
            return;
        }

        var keyTypePathCount = ApiSchemaHelpers.CountKeyLeaves(principalKeyType);
        if (keyTypePathCount is not null && keyPathCount != keyTypePathCount)
        {
            AddCountMismatchIssue
            (
                context,
                relationshipPath,
                foreignKeyPath,
                keyPathCount,
                keyTypePathCount.Value,
                countMismatchCode,
                principalCountLabel,
                countMismatchRemediationTarget
            );
            return;
        }

        if (ApiSchemaHelpers.TryAreKeyTypesCompatible(principalKeyType, foreignKeyType, out var compatible) && !compatible)
        {
            AddExplicitIncompatiblePrincipalKeyIssue
            (
                context,
                relationshipPath,
                foreignKeyPath,
                foreignKeyType,
                principalKeyType,
                principalCompatibilityLabel,
                compatibilityRemediation
            );
        }
    }
    #endregion

    #region Implementation Methods
    private static void AddAmbiguousPrincipalKeyIssue
    (
        ApiInitializationContext context,
        string relationshipPath,
        ApiObjectType principalObjectType,
        List<KeyValuePair<string, ApiKeyType>> matchingKeys,
        string? principalEndQualifier,
        string explicitKeyTarget
    )
    {
        var keyTypeNames = string.Join(", ", matchingKeys.Select(static kvp => $"'{kvp.Key}'"));
        var qualifier = string.IsNullOrWhiteSpace(principalEndQualifier) ? null : $" {principalEndQualifier}";
        var severity = ApiInitializationSeverity.Error;
        var code = ApiInitializationCode.API_RELATIONSHIP_AMBIGUOUS_PRINCIPAL_KEY;
        var description = $"Cannot automatically determine the referenced key type{qualifier}: {matchingKeys.Count} key types on '{principalObjectType.ApiName}' are compatible with the foreign key type: {keyTypeNames}";
        var remediation = $"Set {explicitKeyTarget} to specify the key type explicitly; available key types: {keyTypeNames}";

        context.AddIssue(relationshipPath, severity, code, description, remediation);
    }

    private static void AddCountMismatchIssue
    (
        ApiInitializationContext context,
        string relationshipPath,
        string foreignKeyPath,
        int keyPathCount,
        int keyTypePathCount,
        ApiInitializationCode countMismatchCode,
        string principalCountLabel,
        string countMismatchRemediationTarget
    )
    {
        var severity = ApiInitializationSeverity.Error;
        var description = $"{foreignKeyPath}.{nameof(ApiKeyType.ApiKeyPaths)} has {keyPathCount} key path(s) but {principalCountLabel} has {keyTypePathCount} key path(s)";
        var remediation = $"Ensure {foreignKeyPath}.{nameof(ApiKeyType.ApiKeyPaths)} contains exactly {keyTypePathCount} key path(s) to match {countMismatchRemediationTarget}";

        context.AddIssue(relationshipPath, severity, countMismatchCode, description, remediation);
    }

    private static void AddExplicitIncompatiblePrincipalKeyIssue
    (
        ApiInitializationContext context,
        string relationshipPath,
        string foreignKeyPath,
        ApiKeyType foreignKeyType,
        ApiKeyType principalKeyType,
        string principalCompatibilityLabel,
        string compatibilityRemediation
    )
    {
        var principalKeyTypes = ApiSchemaHelpers.DescribeKeyLeafTypes(principalKeyType);
        var foreignKeyTypes = ApiSchemaHelpers.DescribeKeyLeafTypes(foreignKeyType);
        var severity = ApiInitializationSeverity.Error;
        var code = ApiInitializationCode.API_RELATIONSHIP_INCOMPATIBLE_PRINCIPAL_FOREIGN_KEY;
        var description = $"{foreignKeyPath} leaf type(s) [{foreignKeyTypes}] are not compatible with {principalCompatibilityLabel} leaf type(s) [{principalKeyTypes}]";

        context.AddIssue(relationshipPath, severity, code, description, compatibilityRemediation);
    }

    private static void AddInferredIncompatiblePrincipalKeyIssue
    (
        ApiInitializationContext context,
        string relationshipPath,
        ApiObjectType principalObjectType,
        ApiKeyType foreignKeyType,
        List<KeyValuePair<string, ApiKeyType>> matchingShapeKeys,
        int keyPathCount,
        string? principalEndQualifier,
        string explicitKeyTarget,
        string inferredForeignKeyLabel
    )
    {
        var canCompare = matchingShapeKeys.Any(kvp => ApiSchemaHelpers.TryAreKeyTypesCompatible(kvp.Value, foreignKeyType, out _));
        if (!canCompare)
        {
            return;
        }

        var keyTypeNames = string.Join(", ", matchingShapeKeys.Select(static kvp => $"'{kvp.Key}'"));
        var foreignKeyTypes = ApiSchemaHelpers.DescribeKeyLeafTypes(foreignKeyType);
        var qualifier = string.IsNullOrWhiteSpace(principalEndQualifier) ? null : $" {principalEndQualifier}";
        var severity = ApiInitializationSeverity.Error;
        var code = ApiInitializationCode.API_RELATIONSHIP_INCOMPATIBLE_PRINCIPAL_FOREIGN_KEY;
        var description = $"Cannot automatically determine the referenced key type{qualifier}: no key type on '{principalObjectType.ApiName}' with {keyPathCount} key path(s) is compatible with foreign key leaf type(s) [{foreignKeyTypes}]";
        var remediation = $"Set {explicitKeyTarget} explicitly or align the {inferredForeignKeyLabel} leaf type(s) with one of these key types: {keyTypeNames}";

        context.AddIssue(relationshipPath, severity, code, description, remediation);
    }
    #endregion
}
