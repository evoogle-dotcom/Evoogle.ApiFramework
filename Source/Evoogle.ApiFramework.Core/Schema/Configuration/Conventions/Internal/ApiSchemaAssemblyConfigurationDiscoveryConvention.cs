// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Schema.Configuration.Internal;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiSchemaAssemblyConfigurationDiscoveryConvention : IApiSchemaConvention
{
    #region Properties
    /// <inheritdoc />
    public ApiConventionPhase Phase => ApiConventionPhase.Discovery;
    #endregion

    #region Fields
    private readonly Assembly _assembly;
    private readonly Func<Type, bool>? _filter;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new <see cref="ApiSchemaAssemblyConfigurationDiscoveryConvention"/> that scans the
    ///     specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for API configurations.</param>
    /// <param name="filter">Optional predicate to limit which eligible configuration types are considered.</param>
    internal ApiSchemaAssemblyConfigurationDiscoveryConvention
    (
        Assembly assembly,
        Func<Type, bool>? filter = null
    )
    {
        ArgumentNullException.ThrowIfNull(assembly);

        _assembly = assembly;
        _filter = filter;
    }
    #endregion

    #region IApiSchemaConvention Methods
    /// <inheritdoc />
    public void Apply(ApiSchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var configurationTypes = _assembly.GetExportedTypes()
            .Where(IsEligibleConfigurationType)
            .Where(type => _filter == null || _filter(type))
            .OrderBy(GetTypeName, StringComparer.Ordinal);

        foreach (var configurationType in configurationTypes)
        {
            if (!typeof(IApiConfiguration).IsAssignableFrom(configurationType))
            {
                continue;
            }

            if (!HasConfigurationRole(configurationType))
            {
                builder.Logger.LogWarning
                (
                    "API configuration type '{ConfigurationType}' implements IApiConfiguration but no supported " +
                    "configuration role.",
                    GetTypeName(configurationType)
                );
                continue;
            }

            this.ApplyConfigurationType(builder, configurationType);
        }
    }
    #endregion

    #region Helper Methods
    private static bool IsEligibleConfigurationType(Type type)
    {
        return type.IsClass && !type.IsAbstract && !type.ContainsGenericParameters;
    }

    private static string GetTypeName(Type type)
    {
        return type.FullName ?? type.Name;
    }

    private void ApplyConfigurationType(ApiSchemaBuilder builder, Type configurationType)
    {
        var configurationTypeName = GetTypeName(configurationType);
        object configuration;

        try
        {
            configuration = Activator.CreateInstance(configurationType)
                ?? throw new InvalidOperationException
                (
                    $"Activator.CreateInstance returned null for configuration type '{configurationTypeName}'."
                );
        }
        catch (Exception exception)
        {
            builder.Context.AddConfigurationIssue
            (
                this.CreateIssue
                (
                    configurationType,
                    ApiInitializationCode.ApiConfigurationActivationFailed,
                    $"The API configuration type '{configurationTypeName}' could not be activated.",
                    "Provide a public parameterless constructor.",
                    exception
                )
            );
            return;
        }

        if (configuration is IApiObjectTypeConfiguration objectConfiguration)
        {
            this.ApplyConfigurationRole
            (
                builder,
                configurationType,
                "object type",
                () => builder.AddObject(objectConfiguration)
            );
        }

        if (configuration is IApiScalarTypeConfiguration scalarConfiguration)
        {
            this.ApplyConfigurationRole
            (
                builder,
                configurationType,
                "scalar type",
                () => builder.AddScalar(scalarConfiguration)
            );
        }

        if (configuration is IApiEnumTypeConfiguration enumConfiguration)
        {
            this.ApplyConfigurationRole
            (
                builder,
                configurationType,
                "enum type",
                () => builder.AddEnum(enumConfiguration)
            );
        }

        if (configuration is IApiRelationshipOneToOneConfiguration oneToOneConfiguration)
        {
            this.ApplyConfigurationRole
            (
                builder,
                configurationType,
                "one-to-one relationship",
                () => builder.AddOneToOneRelationship(oneToOneConfiguration)
            );
        }

        if (configuration is IApiRelationshipOneToManyConfiguration oneToManyConfiguration)
        {
            this.ApplyConfigurationRole
            (
                builder,
                configurationType,
                "one-to-many relationship",
                () => builder.AddOneToManyRelationship(oneToManyConfiguration)
            );
        }

        if (configuration is IApiRelationshipManyToManyConfiguration manyToManyConfiguration)
        {
            this.ApplyConfigurationRole
            (
                builder,
                configurationType,
                "many-to-many relationship",
                () => builder.AddManyToManyRelationship(manyToManyConfiguration)
            );
        }
    }

    private static bool HasConfigurationRole(Type configurationType)
    {
        return typeof(IApiObjectTypeConfiguration).IsAssignableFrom(configurationType) ||
            typeof(IApiScalarTypeConfiguration).IsAssignableFrom(configurationType) ||
            typeof(IApiEnumTypeConfiguration).IsAssignableFrom(configurationType) ||
            typeof(IApiRelationshipOneToOneConfiguration).IsAssignableFrom(configurationType) ||
            typeof(IApiRelationshipOneToManyConfiguration).IsAssignableFrom(configurationType) ||
            typeof(IApiRelationshipManyToManyConfiguration).IsAssignableFrom(configurationType);
    }

    private void ApplyConfigurationRole
    (
        ApiSchemaBuilder builder,
        Type configurationType,
        string configurationRole,
        Action apply
    )
    {
        try
        {
            builder.Context.ApplyConfiguration
            (
                ApiConfigurationSource.Explicit,
                apply
            );
        }
        catch (Exception exception)
        {
            var configurationTypeName = GetTypeName(configurationType);
            builder.Context.AddConfigurationIssue
            (
                this.CreateIssue
                (
                    configurationType,
                    ApiInitializationCode.ApiConfigurationExecutionFailed,
                    $"The API configuration type '{configurationTypeName}' threw while configuring its " +
                    $"{configurationRole} role.",
                    "Correct the configuration identity or Configure implementation.",
                    exception
                )
            );
        }
    }

    private ApiInitializationIssue CreateIssue
    (
        Type configurationType,
        ApiInitializationCode code,
        string description,
        string remediation,
        Exception exception
    )
    {
        return new ApiInitializationIssue
        (
            GetTypeName(configurationType),
            ApiInitializationSeverity.Warning,
            code,
            description,
            remediation,
            exception: exception
        );
    }
    #endregion
}
