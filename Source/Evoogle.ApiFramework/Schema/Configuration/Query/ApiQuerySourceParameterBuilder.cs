// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Query.Internal;
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Query;

/// <summary>Configures a schema-owned query source parameter.</summary>
/// <remarks>
/// The API name and type reference are fixed. Nullability and argument presence are configured independently,
/// using convention, data-annotation, and explicit precedence, in that order.
/// </remarks>
public sealed class ApiQuerySourceParameterBuilder : ExtensionBuilder<ApiQuerySourceParameterBuilder>
{
    #region Fields
    private string _apiName;
    private readonly object _apiTypeIdentity;
    private readonly Func<ApiTypeExpression> _apiTypeExpressionFactory;
    private readonly ApiQuerySourceParameterState _state = new();
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    #endregion

    #region Constructors
    /// <summary>Creates a required, non-null input parameter builder.</summary>
    /// <param name="apiName">The parameter API name, unique within its source.</param>
    /// <param name="apiTypeReference">The registered input type reference.</param>
    public ApiQuerySourceParameterBuilder(string apiName, ApiTypeReference apiTypeReference)
        : this(apiName, apiTypeReference, () => new ApiTypeExpression(apiTypeReference))
    {
    }

    internal ApiQuerySourceParameterBuilder
    (
        string apiName,
        object apiTypeIdentity,
        Func<ApiTypeExpression> apiTypeExpressionFactory
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        ArgumentNullException.ThrowIfNull(apiTypeIdentity);
        ArgumentNullException.ThrowIfNull(apiTypeExpressionFactory);

        _apiName = apiName;
        _apiTypeIdentity = apiTypeIdentity;
        _apiTypeExpressionFactory = apiTypeExpressionFactory;
    }

    /// <summary>Creates a required, non-null input parameter builder using a registered CLR type.</summary>
    /// <param name="apiName">The parameter API name, unique within its source.</param>
    /// <param name="clrType">The registered CLR input type.</param>
    public ApiQuerySourceParameterBuilder(string apiName, Type clrType)
        : this(apiName, new ApiTypeReference(clrType ?? throw new ArgumentNullException(nameof(clrType))))
    {
    }
    #endregion

    #region Properties
    /// <summary>Gets the configured parameter API name.</summary>
    public string ApiName => _apiName;

    /// <summary>Gets the identity used to compare the configured input type.</summary>
    internal object ApiTypeIdentity => _apiTypeIdentity;

    /// <summary>Gets the source associated with the active configuration callback.</summary>
    internal ApiConfigurationSource CurrentConfigurationSource => _configurationSourceScope.CurrentSource;

    /// <summary>Gets the highest-precedence source that registered this parameter.</summary>
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

    /// <summary>Records the source that registered this parameter.</summary>
    internal void SetRegistrationSource(ApiConfigurationSource source)
    {
        if (source >= _state.RegistrationSource)
        {
            _state.RegistrationSource = source;
        }
    }
    #endregion

    #region With Methods
    /// <summary>Configures input nullability independently of argument presence.</summary>
    /// <param name="apiNullability">The input nullability.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder WithNullability(ApiNullability apiNullability)
    {
        var source = this.CurrentConfigurationSource;
        if (source >= _state.NullabilitySource)
        {
            _state.ApiNullability = apiNullability;
            _state.NullabilitySource = source;
        }

        return this;
    }

    /// <summary>Allows a supplied argument value to be null without changing argument presence.</summary>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder AsNullable()
        => this.WithNullability(ApiNullability.Nullable);

    /// <summary>Prohibits null for a supplied argument value without changing argument presence.</summary>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder AsNonNullable()
        => this.WithNullability(ApiNullability.NonNullable);

    /// <summary>Allows the caller to omit the argument without changing value nullability.</summary>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder AsOptional()
        => this.WithPresence(ApiPresence.Optional);

    /// <summary>Requires an argument without changing value nullability.</summary>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder AsRequired()
        => this.WithPresence(ApiPresence.Required);

    /// <summary>Configures whether an argument must be supplied.</summary>
    /// <param name="apiPresence">The argument presence requirement.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder WithPresence(ApiPresence apiPresence)
    {
        var source = this.CurrentConfigurationSource;
        if (source >= _state.PresenceSource)
        {
            _state.ApiPresence = apiPresence;
            _state.PresenceSource = source;
        }

        return this;
    }
    #endregion

    #region AddExtension Methods
    /// <summary>Adds query source parameter extension metadata.</summary>
    /// <param name="extensionType">The extension registration type.</param>
    /// <param name="extension">The extension metadata.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder AddQuerySourceParameterExtension(Type extensionType, object extension)
        => this.AddExtension(extensionType, extension);
    #endregion

    #region Build Methods
    /// <summary>Builds a fresh parameter declaration and type expression.</summary>
    internal ApiQuerySourceParameter Build()
    {
        // Build ApiQuerySourceParameter instance from all the configured components.
        var apiName = this.ApiName;
        var apiTypeExpression = _apiTypeExpressionFactory();
        var apiNullability = _state.ApiNullability;
        var apiPresence = _state.ApiPresence;

        var apiQuerySourceParameter = new ApiQuerySourceParameter
        (
            apiName,
            apiTypeExpression,
            apiNullability,
            apiPresence
        );

        // Add any extensions that were configured.
        var extensions = this.BuildExtensions();
        if (extensions is not null)
        {
            apiQuerySourceParameter.AttachExtensions(extensions);
        }

        return apiQuerySourceParameter;
    }
    #endregion
}
