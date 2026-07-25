// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Configuration.Conventions;

namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Provides an immutable, validated convention schedule for one configuration-pipeline run.
/// </summary>
internal sealed class ApiConventionSchedule
{
    #region Properties
    internal IReadOnlyList<IApiSchemaConvention> SchemaDiscoveryConventions { get; }
    internal IReadOnlyList<IApiObjectTypeConvention> ObjectDiscoveryConventions { get; }
    internal IReadOnlyList<IApiObjectTypeConvention> ObjectConfigurationConventions { get; }
    internal IReadOnlyList<IApiScalarTypeConvention> ScalarConfigurationConventions { get; }
    internal IReadOnlyList<IApiEnumTypeConvention> EnumTypeConfigurationConventions { get; }
    internal IReadOnlyList<IApiEnumValueConvention> EnumValueConfigurationConventions { get; }
    internal IReadOnlyList<IApiPropertyConvention> PropertyConfigurationConventions { get; }
    internal IReadOnlyList<IApiRelationshipConvention> RelationshipConventions { get; }
    #endregion

    #region Constructors
    private ApiConventionSchedule(ApiConventionSet conventionSet)
    {
        this.SchemaDiscoveryConventions = ValidatePhase
        (
            conventionSet.DiscoveryConventions,
            ApiConventionPhase.Discovery
        );

        this.ObjectDiscoveryConventions =
        [
            .. conventionSet.ObjectTypeConventions.Where
            (
                static convention => convention.Phase == ApiConventionPhase.Discovery
            ),
        ];

        this.ObjectConfigurationConventions =
        [
            .. conventionSet.ObjectTypeConventions.Where
            (
                static convention => convention.Phase == ApiConventionPhase.Configuration
            ),
        ];

        ValidateObjectConventionPhases(conventionSet.ObjectTypeConventions);

        this.ScalarConfigurationConventions = ValidatePhase
        (
            conventionSet.ScalarTypeConventions,
            ApiConventionPhase.Configuration
        );

        this.EnumTypeConfigurationConventions = ValidatePhase
        (
            conventionSet.EnumTypeConventions,
            ApiConventionPhase.Configuration
        );

        this.EnumValueConfigurationConventions = ValidatePhase
        (
            conventionSet.EnumValueConventions,
            ApiConventionPhase.Configuration
        );

        this.PropertyConfigurationConventions = ValidatePhase
        (
            conventionSet.PropertyConventions,
            ApiConventionPhase.Configuration
        );

        this.RelationshipConventions = ValidatePhase
        (
            conventionSet.RelationshipConventions,
            ApiConventionPhase.Relationship
        );
    }
    #endregion

    #region Factory Methods
    /// <summary>Creates a validated schedule by snapshotting the supplied convention set.</summary>
    internal static ApiConventionSchedule Create(ApiConventionSet conventionSet)
    {
        ArgumentNullException.ThrowIfNull(conventionSet);
        return new ApiConventionSchedule(conventionSet);
    }
    #endregion

    #region Implementation Methods
    private static IReadOnlyList<TConvention> ValidatePhase<TConvention>
    (
        IEnumerable<TConvention> conventions,
        ApiConventionPhase expectedPhase
    )
        where TConvention : IApiConvention
    {
        var result = conventions.ToList();

        foreach (var convention in result)
        {
            if (convention.Phase != expectedPhase)
            {
                throw BuildInvalidPhaseException
                (
                    convention,
                    typeof(TConvention),
                    expectedPhase
                );
            }
        }

        return result;
    }

    private static void ValidateObjectConventionPhases
    (
        IEnumerable<IApiObjectTypeConvention> conventions
    )
    {
        foreach (var convention in conventions)
        {
            if
            (
                convention.Phase != ApiConventionPhase.Discovery &&
                convention.Phase != ApiConventionPhase.Configuration
            )
            {
                throw new ApiSchemaConfigurationException
                (
                    $"Convention '{convention.GetType().Name}' cannot run in the " +
                    $"{convention.Phase} phase as an IApiObjectTypeConvention. Object-type " +
                    "conventions must use the Discovery or Configuration phase."
                );
            }
        }
    }

    private static ApiSchemaConfigurationException BuildInvalidPhaseException
    (
        IApiConvention convention,
        Type conventionTarget,
        ApiConventionPhase expectedPhase
    )
    {
        return new ApiSchemaConfigurationException
        (
            $"Convention '{convention.GetType().Name}' cannot run in the {convention.Phase} " +
            $"phase as an {conventionTarget.Name}. {conventionTarget.Name} conventions require " +
            $"the {expectedPhase} phase."
        );
    }
    #endregion
}
