// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Query.Internal;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Query;

/// <summary>Configures a named schema query source without service execution bindings.</summary>
/// <remarks>
/// Configuration uses convention, data-annotation, and explicit precedence, in that order.
/// Later settings at the same precedence win. Each build creates an independent schema declaration.
/// </remarks>
public sealed class ApiQuerySourceBuilder : ExtensionBuilder<ApiQuerySourceBuilder>
{
    #region Fields
    private string _apiName;
    private readonly ApiQuerySourceState _state = new();
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    #endregion

    #region Constructors
    /// <summary>Creates a query source builder with a fixed API name.</summary>
    /// <param name="apiName">The schema-unique query source API name.</param>
    public ApiQuerySourceBuilder(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        _apiName = apiName;
    }
    #endregion

    #region Properties
    /// <summary>Gets the configured query source API name.</summary>
    public string ApiName => _apiName;

    /// <summary>Gets the configured parameter builders in declaration order.</summary>
    internal IReadOnlyList<ApiQuerySourceParameterBuilder> InputParameterBuilders => _state.InputParameterBuilders;

    /// <summary>Gets the source associated with the active configuration callback.</summary>
    internal ApiConfigurationSource CurrentConfigurationSource => _configurationSourceScope.CurrentSource;

    /// <summary>Gets the highest-precedence source that registered this query source.</summary>
    internal ApiConfigurationSource RegistrationSource => _state.RegistrationSource;

    /// <summary>Sets the API name while resolving a convention-derived declaration name.</summary>
    internal void SetApiNameConvention(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        _apiName = apiName;
    }

    #endregion

    #region Configuration Source Methods
    /// <summary>Runs a callback at the supplied configuration-source precedence.</summary>
    internal void ApplyConfiguration(ApiConfigurationSource source, Action configure)
        => _configurationSourceScope.Apply(source, configure);

    /// <summary>Records the source that registered this query source.</summary>
    internal void SetRegistrationSource(ApiConfigurationSource source)
    {
        if (source >= _state.RegistrationSource)
        {
            _state.RegistrationSource = source;
        }
    }
    #endregion

    #region AddInputParameter Methods
    /// <summary>Adds or configures a parameter referencing a registered schema type.</summary>
    /// <remarks>
    /// New parameters require non-null arguments by default. A matching API name and type reference reuse
    /// the parameter builder. A different type replaces its configuration and extensions at equal or higher
    /// precedence, preserving parameter order. Lower-precedence type changes are ignored.
    /// </remarks>
    /// <param name="apiName">The parameter API name.</param>
    /// <param name="apiTypeReference">The input type reference.</param>
    /// <param name="configure">Optional parameter configuration.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder AddInputParameter
    (
        string apiName,
        ApiTypeReference apiTypeReference,
        Action<ApiQuerySourceParameterBuilder>? configure = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        ArgumentNullException.ThrowIfNull(apiTypeReference);

        return this.AddInputParameterCore
        (
            apiName,
            apiTypeReference,
            () => new ApiTypeExpression(apiTypeReference),
            configure
        );
    }

    /// <summary>Adds or configures a parameter from an annotation-derived type expression.</summary>
    internal ApiQuerySourceParameterBuilder AddInputParameterDataAnnotation
    (
        string apiName,
        object apiTypeIdentity,
        Func<ApiTypeExpression> apiTypeExpressionFactory,
        Action<ApiQuerySourceParameterBuilder>? configure
    )
    {
        this.ApplyConfiguration
        (
            ApiConfigurationSource.DataAnnotation,
            () => this.AddInputParameterCore(apiName, apiTypeIdentity, apiTypeExpressionFactory, configure)
        );

        return _state.InputParameterBuilders.First
            (parameter => ApiNameComparer.Instance.Equals(parameter.ApiName, apiName));
    }

    private ApiQuerySourceBuilder AddInputParameterCore
    (
        string apiName,
        object apiTypeIdentity,
        Func<ApiTypeExpression> apiTypeExpressionFactory,
        Action<ApiQuerySourceParameterBuilder>? configure
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        ArgumentNullException.ThrowIfNull(apiTypeIdentity);
        ArgumentNullException.ThrowIfNull(apiTypeExpressionFactory);

        var source = this.CurrentConfigurationSource;
        var parameterIndex = _state.InputParameterBuilders.FindIndex
            (parameter => ApiNameComparer.Instance.Equals(parameter.ApiName, apiName));
        if (parameterIndex >= 0)
        {
            var existing = _state.InputParameterBuilders[parameterIndex];
            if (Equals(existing.ApiTypeIdentity, apiTypeIdentity))
            {
                existing.SetRegistrationSource(source);
                if (configure is not null)
                {
                    existing.ApplyConfiguration(source, () => configure(existing));
                }

                return this;
            }

            if (source < existing.RegistrationSource)
            {
                return this;
            }
        }

        var builder = new ApiQuerySourceParameterBuilder(apiName, apiTypeIdentity, apiTypeExpressionFactory);
        builder.SetRegistrationSource(source);
        if (configure is not null)
        {
            builder.ApplyConfiguration(source, () => configure(builder));
        }

        if (parameterIndex >= 0)
        {
            _state.InputParameterBuilders[parameterIndex] = builder;
        }
        else
        {
            _state.InputParameterBuilders.Add(builder);
        }

        return this;
    }

    /// <summary>Adds or configures a parameter using its registered CLR type.</summary>
    /// <param name="apiName">The parameter API name.</param>
    /// <param name="clrType">The registered CLR input type.</param>
    /// <param name="configure">Optional parameter configuration.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder AddInputParameter
    (
        string apiName,
        Type clrType,
        Action<ApiQuerySourceParameterBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(clrType);
        return this.AddInputParameter(apiName, new ApiTypeReference(clrType), configure);
    }

    /// <summary>Adds or configures a parameter using its registered CLR type.</summary>
    /// <typeparam name="TParameter">The registered CLR input type.</typeparam>
    /// <param name="apiName">The parameter API name.</param>
    /// <param name="configure">Optional parameter configuration.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder AddInputParameter<TParameter>
        (string apiName, Action<ApiQuerySourceParameterBuilder>? configure = null)
        => this.AddInputParameter(apiName, typeof(TParameter), configure);
    #endregion

    #region With Methods
    /// <summary>Declares the source output using a registered schema type reference.</summary>
    /// <remarks>The type and nullability are configured together at the active precedence.</remarks>
    /// <param name="apiTypeReference">The output type reference.</param>
    /// <param name="apiOutputNullability">The output nullability; nullable by default.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder WithOutput
    (
        ApiTypeReference apiTypeReference,
        ApiNullability apiOutputNullability = ApiSchemaDefaults.QuerySourceOutputNullability
    )
    {
        ArgumentNullException.ThrowIfNull(apiTypeReference);
        return this.SetOutput(() => new ApiTypeExpression(apiTypeReference), apiOutputNullability);
    }

    /// <summary>Declares output referencing a registered CLR type.</summary>
    /// <param name="clrType">The registered CLR output type.</param>
    /// <param name="apiOutputNullability">The output nullability; nullable by default.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder WithOutput
    (
        Type clrType,
        ApiNullability apiOutputNullability = ApiSchemaDefaults.QuerySourceOutputNullability
    )
    {
        ArgumentNullException.ThrowIfNull(clrType);
        return this.WithOutput(new ApiTypeReference(clrType), apiOutputNullability);
    }

    /// <summary>Declares output referencing the registered schema type for a CLR type.</summary>
    /// <typeparam name="TOutput">The registered CLR output type.</typeparam>
    /// <param name="apiOutputNullability">The output nullability; nullable by default.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder WithOutput<TOutput>
    (
        ApiNullability apiOutputNullability = ApiSchemaDefaults.QuerySourceOutputNullability
    )
        => this.WithOutput(typeof(TOutput), apiOutputNullability);

    /// <summary>Declares list output using a registered CLR item type.</summary>
    /// <remarks>The item type, item nullability, and collection nullability share one precedence.</remarks>
    /// <param name="clrItemType">The registered CLR item type.</param>
    /// <param name="apiCollectionNullability">The collection nullability; non-nullable by default.</param>
    /// <param name="apiItemNullability">The item nullability; non-nullable by default.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder WithCollectionOutput
    (
        Type clrItemType,
        ApiNullability apiCollectionNullability = ApiSchemaDefaults.QuerySourceCollectionOutputNullability,
        ApiNullability apiItemNullability = ApiSchemaDefaults.QuerySourceCollectionOutputItemNullability
    )
    {
        ArgumentNullException.ThrowIfNull(clrItemType);

        var clrCollectionType = typeof(List<>).MakeGenericType(clrItemType);
        return this.SetOutput
        (
            () => new ApiTypeExpression(new ApiCollectionType
            (
                new ApiTypeExpression(new ApiTypeReference(clrItemType)),
                apiItemNullability,
                clrCollectionType
            )),
            apiCollectionNullability
        );
    }

    /// <summary>Declares list output with independently configured collection and item nullability.</summary>
    /// <typeparam name="TItem">The registered CLR item type.</typeparam>
    /// <param name="apiCollectionNullability">The collection nullability.</param>
    /// <param name="apiItemNullability">The item nullability.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder WithCollectionOutput<TItem>
    (
        ApiNullability apiCollectionNullability = ApiSchemaDefaults.QuerySourceCollectionOutputNullability,
        ApiNullability apiItemNullability = ApiSchemaDefaults.QuerySourceCollectionOutputItemNullability
    ) => this.WithCollectionOutput(typeof(TItem), apiCollectionNullability, apiItemNullability);

    /// <summary>Declares which query composition operations are allowed over the source output.</summary>
    /// <remarks>Replaces the allowed operations at the active precedence; does not apply them to the output.</remarks>
    /// <param name="apiOutputComposition">The permitted composition flags.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder WithOutputComposition(ApiQueryComposition apiOutputComposition)
    {
        var source = this.CurrentConfigurationSource;
        if (source >= _state.OutputCompositionSource)
        {
            _state.ApiOutputComposition = apiOutputComposition;
            _state.OutputCompositionSource = source;
        }

        return this;
    }
    #endregion

    #region AddExtension Methods
    /// <summary>Adds query source extension metadata.</summary>
    /// <param name="extensionType">The extension registration type.</param>
    /// <param name="extension">The extension metadata.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder AddQuerySourceExtension(Type extensionType, object extension)
        => this.AddExtension(extensionType, extension);
    #endregion

    #region Build Methods
    /// <summary>Builds a fresh query source and its owned parameter declarations.</summary>
    internal ApiQuerySource Build()
    {
        // Build ApiQuerySource instance from all the configured components.
        var apiName = this.ApiName;
        var apiInputParameters = _state.InputParameterBuilders.Select(parameter => parameter.Build());
        var apiOutputType = _state.OutputTypeFactory?.Invoke()!;
        var apiOutputNullability = _state.ApiOutputNullability;
        var apiOutputComposition = _state.ApiOutputComposition;

        var apiQuerySource = new ApiQuerySource
        (
            apiName,
            apiInputParameters,
            apiOutputType,
            apiOutputNullability,
            apiOutputComposition
        );

        // Add any extensions that were configured.
        var extensions = this.BuildExtensions();
        if (extensions is not null)
        {
            apiQuerySource.AttachExtensions(extensions);
        }

        return apiQuerySource;
    }
    #endregion

    #region Implementation Methods
    private ApiQuerySourceBuilder SetOutput
    (
        Func<ApiTypeExpression> outputTypeFactory,
        ApiNullability apiOutputNullability
    )
    {
        this.SetOutputType(outputTypeFactory);
        this.SetOutputNullability(apiOutputNullability);

        return this;
    }

    internal void SetOutputTypeDataAnnotation(Func<ApiTypeExpression> outputTypeFactory)
        => this.ApplyConfiguration
        (
            ApiConfigurationSource.DataAnnotation,
            () => this.SetOutputType(outputTypeFactory)
        );

    internal void SetOutputNullabilityDataAnnotation(ApiNullability apiOutputNullability)
        => this.ApplyConfiguration
        (
            ApiConfigurationSource.DataAnnotation,
            () => this.SetOutputNullability(apiOutputNullability)
        );

    private void SetOutputType(Func<ApiTypeExpression> outputTypeFactory)
    {
        ArgumentNullException.ThrowIfNull(outputTypeFactory);

        var source = this.CurrentConfigurationSource;
        if (source >= _state.OutputTypeSource)
        {
            _state.OutputTypeFactory = outputTypeFactory;
            _state.OutputTypeSource = source;
        }
    }

    private void SetOutputNullability(ApiNullability apiOutputNullability)
    {
        var source = this.CurrentConfigurationSource;
        if (source >= _state.OutputNullabilitySource)
        {
            _state.ApiOutputNullability = apiOutputNullability;
            _state.OutputNullabilitySource = source;
        }
    }
    #endregion
}
