// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Schema.Compilation;

namespace Evoogle.ApiFramework.Schema.Configuration.Relationships.Internal;

internal static class ApiRelationshipNavigationValidation
{
    #region Validation Methods
    internal static void Apply
    (
        ApiRelationshipBuilder builder,
        ApiSchemaBuilderContext context
    )
    {
        foreach (var binding in builder.NavigationBindings)
        {
            var locator = binding.Locator;
            var ends = builder.EndsState;
            var principalEnd = binding.EndIndex == 0 ? ends.PrincipalEndA : ends.PrincipalEndB;
            var dependentEnd = binding.EndIndex == 1 ? ends.DependentEnd : null;
            var sourceType = principalEnd?.ApiObjectTypeReference.ClrType ??
                dependentEnd?.ApiObjectTypeReference.ClrType;
            var targetType = binding.EndIndex == 0
                ? ends.PrincipalEndB?.ApiObjectTypeReference.ClrType ?? ends
                    .DependentEnd?.ApiObjectTypeReference.ClrType
                : ends.PrincipalEndA?.ApiObjectTypeReference.ClrType;
            var isMany = builder is ApiRelationshipManyToManyBuilder ||
                (builder is ApiRelationshipOneToManyBuilder && binding.EndIndex == 0);
            if (sourceType != locator.SourceType || targetType != locator.TargetType || isMany != locator.IsMany)
            {
                context.AddRelationshipIssue(ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict,
                    builder.ConfiguredApiName, $"Navigation '{locator.SourceType.FullName}.{locator.ClrMember.Name}' " +
                    "requires its declared CLR endpoint, target type, and cardinality", builder);
                continue;
            }
            principalEnd?.BindNavigation(locator.Reference);
            dependentEnd?.BindNavigation(locator.Reference);
            if (principalEnd?.HasNavigationConflict == true || dependentEnd?.HasNavigationConflict == true)
            {
                context.AddRelationshipIssue(ApiSchemaCompilationCode.ApiRelationshipConfigurationConflict,
                    builder.ConfiguredApiName,
                    "Configuration cannot replace an authoritative CLR navigation binding", builder);
            }
        }
    }
    #endregion
}
