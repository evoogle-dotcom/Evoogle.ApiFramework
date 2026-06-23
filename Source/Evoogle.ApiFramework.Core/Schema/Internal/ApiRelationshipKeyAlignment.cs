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
    public static ApiRelationshipKeyBinding? ResolvePrincipalForeignKeyBinding
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

        var keyPathCount = foreignKeyType.ApiKeyPaths.Length;
        var runCountCheck = true;
        ApiKeyType? principalKeyType = null;
        var principalKeyResolutionSource = ApiRelationshipPrincipalKeyResolutionSource.Inferred;

        var principalObjectType = principalEnd.ApiResolvedObjectType;
        if (principalObjectType is null)
        {
            // The principal end already recorded the unresolved object type issue.
            return null;
        }

        if (principalEnd.ApiPrincipalKeyTypeName is not null)
        {
            principalKeyResolutionSource = ApiRelationshipPrincipalKeyResolutionSource.Explicit;
            if (!principalObjectType.TryGetKeyTypeByApiName(principalEnd.ApiPrincipalKeyTypeName, out principalKeyType))
            {
                AddUnresolvedExplicitPrincipalKeyIssue(context, principalEnd, principalObjectType);
                return null;
            }
        }
        else
        {
            var matchingShapeKeys = principalObjectType.ApiKeyTypes
                .Where(keyType => ApiRelationshipKeyCompatibility.CountKeyLeaves(keyType) == keyPathCount)
                .Select(static keyType => new KeyValuePair<string, ApiKeyType>(keyType.ApiName!, keyType))
                .ToList();
            var matchingKeys = matchingShapeKeys
                .Where(kvp => ApiRelationshipKeyCompatibility.AreKeyTypesCompatible(kvp.Value, foreignKeyType))
                .ToList();

            if (matchingKeys.Count > 1)
            {
                AddAmbiguousPrincipalKeyIssue(context, relationshipPath, principalObjectType, matchingKeys, principalEndQualifier, explicitKeyTarget);
                runCountCheck = false;
            }
            else if (matchingKeys.Count == 1)
            {
                principalKeyType = matchingKeys[0].Value;
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

        if (principalKeyType is null)
        {
            // Inference failed and the relevant issue was already recorded.
            return null;
        }

        if (!runCountCheck)
        {
            return null;
        }

        var keyTypePathCount = ApiRelationshipKeyCompatibility.CountKeyLeaves(principalKeyType);
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
            return null;
        }

        if (ApiRelationshipKeyCompatibility.TryAreKeyTypesCompatible(principalKeyType, foreignKeyType, out var isCompatible) && !isCompatible)
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

            return null;
        }

        return new ApiRelationshipKeyBinding
        (
            principalEnd,
            principalKeyType,
            foreignKeyType,
            principalKeyResolutionSource
        );
    }
    #endregion

    #region Implementation Methods
    private static void AddUnresolvedExplicitPrincipalKeyIssue
    (
        ApiInitializationContext context,
        ApiRelationshipPrincipalEnd principalEnd,
        ApiObjectType principalObjectType
    )
    {
        var availableKeyTypes = string.Join(", ", principalObjectType.GetKeyTypeApiNames().Select(static k => $"'{k}'"));
        var remediation = !string.IsNullOrEmpty(availableKeyTypes)
            ? $"Use one of the available key types: {availableKeyTypes}"
            : $"Define a key type on '{principalObjectType.ApiName}' or remove {nameof(ApiRelationshipPrincipalEnd.ApiPrincipalKeyTypeName)}";

        var path = principalEnd.ApiPath;
        var severity = ApiInitializationSeverity.Error;
        var code = ApiInitializationCode.API_RELATIONSHIP_END_UNRESOLVED_KEY_TYPE;
        var description = $"Referenced principal key type '{principalEnd.ApiPrincipalKeyTypeName}' could not be found on object type '{principalObjectType.ApiName}'";

        context.AddIssue(path, severity, code, description, remediation);
    }

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
        var description = $"Cannot automatically determine the referenced principal key type{qualifier}: {matchingKeys.Count} key types on '{principalObjectType.ApiName}' are compatible with the foreign key type: {keyTypeNames}";
        var remediation = $"Set {explicitKeyTarget} to specify the principal key type explicitly; available key types: {keyTypeNames}";

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

    private static void AddInferredCountMismatchIssue
    (
        ApiInitializationContext context,
        string relationshipPath,
        string foreignKeyPath,
        ApiObjectType principalObjectType,
        int keyPathCount,
        ApiInitializationCode countMismatchCode,
        string? principalEndQualifier,
        string explicitKeyTarget
    )
    {
        var keyTypeNames = string.Join(", ", principalObjectType.ApiKeyTypes.Select(static keyType => $"'{keyType.ApiName}'"));
        var qualifier = string.IsNullOrWhiteSpace(principalEndQualifier) ? null : $" {principalEndQualifier}";
        var severity = ApiInitializationSeverity.Error;
        var description = $"Cannot automatically determine the referenced principal key type{qualifier}: {foreignKeyPath}.{nameof(ApiKeyType.ApiKeyPaths)} has {keyPathCount} key path(s), but no key type on '{principalObjectType.ApiName}' has {keyPathCount} key path(s)";
        var remediation = principalObjectType.ApiKeyTypes.Length > 0
            ? $"Set {explicitKeyTarget} explicitly or align the foreign key shape with one of these key types: {keyTypeNames}"
            : $"Define a key type on '{principalObjectType.ApiName}' or set {explicitKeyTarget} explicitly";

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
        var principalKeyTypes = ApiRelationshipKeyCompatibility.DescribeKeyLeafTypes(principalKeyType);
        var foreignKeyTypes = ApiRelationshipKeyCompatibility.DescribeKeyLeafTypes(foreignKeyType);
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
        var canCompare = matchingShapeKeys.Any(kvp => ApiRelationshipKeyCompatibility.TryAreKeyTypesCompatible(kvp.Value, foreignKeyType, out _));
        if (!canCompare)
        {
            return;
        }

        var keyTypeNames = string.Join(", ", matchingShapeKeys.Select(static kvp => $"'{kvp.Key}'"));
        var foreignKeyTypes = ApiRelationshipKeyCompatibility.DescribeKeyLeafTypes(foreignKeyType);
        var qualifier = string.IsNullOrWhiteSpace(principalEndQualifier) ? null : $" {principalEndQualifier}";
        var severity = ApiInitializationSeverity.Error;
        var code = ApiInitializationCode.API_RELATIONSHIP_INCOMPATIBLE_PRINCIPAL_FOREIGN_KEY;
        var description = $"Cannot automatically determine the referenced principal key type{qualifier}: no key type on '{principalObjectType.ApiName}' with {keyPathCount} key path(s) is compatible with foreign key leaf type(s) [{foreignKeyTypes}]";
        var remediation = $"Set {explicitKeyTarget} explicitly or align the {inferredForeignKeyLabel} leaf type(s) with one of these key types: {keyTypeNames}";

        context.AddIssue(relationshipPath, severity, code, description, remediation);
    }
    #endregion
}
