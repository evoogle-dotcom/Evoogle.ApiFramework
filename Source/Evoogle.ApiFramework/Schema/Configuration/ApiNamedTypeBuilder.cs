// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Trace;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Base class for fluent builders of API named types.
/// </summary>
/// <typeparam name="TBuilder">The concrete builder type.</typeparam>
/// <param name="clrType">The CLR type represented by the API named type.</param>
/// <param name="context">The shared builder context.</param>
public abstract class ApiNamedTypeBuilder<TBuilder>(Type clrType, ApiSchemaBuilderContext context) : ExtensionBuilder<TBuilder>
    where TBuilder : ApiNamedTypeBuilder<TBuilder>
{
    #region Fields
    private readonly ApiNamedTypeState _state = new(clrType);
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the API name configured for the type.
    /// </summary>
    public string ApiName => _state.ApiName;

    /// <summary>
    ///     Gets the CLR type represented by this builder.
    /// </summary>
    public Type ClrType => _state.ClrType;

    /// <summary>
    ///     Gets the shared builder context.
    /// </summary>
    protected ApiSchemaBuilderContext Context { get; } = context ?? throw new ArgumentNullException(nameof(context));

    internal ApiSchemaBuilderContext ConfigurationContext => this.Context;
    #endregion

    #region With Methods
    /// <summary>
    ///     Sets the API name for the type being built.
    /// </summary>
    /// <param name="apiName">The API name to use.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder WithName(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        return this.SetApiName(apiName, ApiConfigurationSource.Explicit);
    }
    #endregion

    #region Internal Convention/Annotation Methods
    /// <summary>
    ///     Sets the API name at convention precedence.
    ///     Has no effect if a higher-precedence value has already been applied.
    /// </summary>
    internal TBuilder SetApiNameConvention(string apiName)
        => this.SetApiName(apiName, ApiConfigurationSource.Convention);

    /// <summary>
    ///     Sets the API name at data-annotation precedence.
    ///     Has no effect if an explicit value has already been applied.
    /// </summary>
    internal TBuilder SetApiNameDataAnnotation(string apiName)
        => this.SetApiName(apiName, ApiConfigurationSource.DataAnnotation);
    #endregion

    #region Implementation Methods
    private TBuilder SetApiName(string apiName, ApiConfigurationSource source)
    {
        var previousValue = _state.ApiName;
        var wasApplied = source >= _state.ApiNameSource;

        if (source >= _state.ApiNameSource)
        {
            _state.ApiName = apiName;
            _state.ApiNameSource = source;
        }

        this.Context.TraceConfigurationChange
        (
            this.GetTraceTarget(),
            ApiSchemaBuildConfigurationFacet.ApiName,
            source,
            previousValue,
            apiName,
            _state.ApiName,
            wasApplied,
            wasApplied ? null : "A higher-precedence API name is already configured."
        );

        return (TBuilder)this;
    }

    private ApiSchemaBuildTraceTarget GetTraceTarget()
    {
        var targetKind = this switch
        {
            ApiObjectTypeBuilder => ApiSchemaBuildTargetKind.ObjectType,
            ApiEnumTypeBuilder => ApiSchemaBuildTargetKind.EnumType,
            ApiScalarTypeBuilder => ApiSchemaBuildTargetKind.ScalarType,
            _ => ApiSchemaBuildTargetKind.Schema,
        };

        return new(targetKind, this.ClrType, ApiName: _state.ApiName);
    }
    #endregion
}
