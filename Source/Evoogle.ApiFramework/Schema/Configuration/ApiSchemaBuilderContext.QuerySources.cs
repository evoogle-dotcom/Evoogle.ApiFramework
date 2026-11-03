// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Schema.Configuration.Conventions;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Query;

namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed partial class ApiSchemaBuilderContext
{
    #region Fields
    private readonly Dictionary<object, ApiQuerySourceBuilder> _apiQuerySourceBuilders = [];
    private readonly HashSet<Type> _discoveredQuerySourceClrTypes = [];
    #endregion

    #region Properties
    internal IEnumerable<ApiQuerySourceBuilder> ApiQuerySourceBuilders => _apiQuerySourceBuilders.Values;
    #endregion

    #region Registration Methods
    internal ApiQuerySourceBuilder GetOrAddDelegateQuerySourceBuilder(Type clrDelegateType)
    {
        if (!_apiQuerySourceBuilders.TryGetValue(clrDelegateType, out var builder))
        {
            builder = new ApiQuerySourceBuilder(clrDelegateType.Name);
            builder.InitializeDelegate(clrDelegateType);
            _apiQuerySourceBuilders.Add(clrDelegateType, builder);
        }
        return builder;
    }

    /// <summary>Declares an independent query source using the supplied initial naming candidate.</summary>
    /// <param name="apiName">The schema-unique API name of the query source.</param>
    /// <returns>The corresponding <see cref="ApiQuerySourceBuilder"/>.</returns>
    internal ApiQuerySourceBuilder DeclareQuerySourceBuilder(string apiName)
        => this.DeclareQuerySourceBuilder(apiName, this.CurrentConfigurationSource);

    /// <summary>Declares an independent query source at the supplied configuration source.</summary>
    internal ApiQuerySourceBuilder DeclareQuerySourceBuilder(string apiName, ApiConfigurationSource source)
    {
        var builder = new ApiQuerySourceBuilder(apiName);
        builder.SetRegistrationSource(source);
        _apiQuerySourceBuilders.Add(new object(), builder);
        return builder;
    }

    internal ApiQuerySourceBuilder DeclareQuerySourceBuilder
    (
        object identity,
        string apiName,
        ApiConfigurationSource source
    )
    {
        var builder = new ApiQuerySourceBuilder(apiName);
        builder.SetRegistrationSource(source);
        _apiQuerySourceBuilders.Add(identity, builder);
        return builder;
    }

    internal ApiQuerySourceBuilder GetQuerySourceBuilder(object identity)
        => _apiQuerySourceBuilders[identity];
    #endregion

    #region Annotation Methods
    internal bool TryBeginQuerySourceAnnotation(Type clrDelegateType)
        => _discoveredQuerySourceClrTypes.Add(clrDelegateType);

    internal bool HasQuerySourceAnnotation(Type clrDelegateType)
        => _discoveredQuerySourceClrTypes.Contains(clrDelegateType);
    #endregion

    #region Convention Context Methods
    internal ApiQuerySourceConventionContext GetQuerySourceConventionContext
    (
        ApiQuerySourceBuilder builder,
        ApiSchemaBuilder schemaBuilder
    ) => new
    (
        builder.ClrDelegateType,
        builder.ClrDelegateType?.GetMethod(nameof(Action.Invoke)) is { } invoke ? Reflection.ParameterReflection.GetNullabilityInfo(invoke.ReturnParameter) : null,
        schemaBuilder
    );

    internal static ApiQuerySourceParameterConventionContext GetQuerySourceParameterConventionContext
    (
        ApiQuerySourceParameterBuilder builder,
        ApiQuerySourceBuilder querySourceBuilder,
        ApiQuerySourceConventionContext querySourceContext
    ) => new
    (
        builder.ClrParameterInfo,
        builder.ClrParameterInfo is { } parameter ? Reflection.ParameterReflection.GetNullabilityInfo(parameter) : null,
        querySourceBuilder,
        querySourceContext
    );
    #endregion

    #region Build Copy Methods
    private void CopyQuerySourceStateTo(ApiSchemaBuilderContext copy)
    {
        foreach (var (identity, builder) in _apiQuerySourceBuilders)
        {
            copy._apiQuerySourceBuilders.Add(identity, builder.CreateBuildSnapshot());
        }
    }
    #endregion

    #region Resolution Methods
    internal void CollectQuerySourceConfigurationIssues(string? schemaName)
    {
        foreach (var querySource in this.ApiQuerySourceBuilders)
        {
            querySource.CollectConfigurationIssues(this, schemaName);
        }
    }
    #endregion
}
