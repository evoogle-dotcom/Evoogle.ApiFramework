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
/// The declaration name and type reference are stable.
/// API naming, nullability, and presence are configured independently,
/// using convention, data-annotation, and explicit precedence, in that order.
/// </remarks>
public sealed class ApiQuerySourceParameterBuilder : ExtensionBuilder<ApiQuerySourceParameterBuilder>
{
    #region Fields
    private string _apiName;
    private ApiConfigurationSource _apiNameSource = ApiConfigurationSource.Convention;
    private ApiNullability? _itemNullability;
    private ApiConfigurationSource _itemNullabilitySource = ApiConfigurationSource.Convention;
    #endregion

    #region Properties
    internal System.Reflection.ParameterInfo? ClrParameterInfo { get; set; }
    internal string DeclarationName { get; }
    #endregion

    #region Fields
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
        this.DeclarationName = apiName;
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

    private ApiQuerySourceParameterBuilder(ApiQuerySourceParameterBuilder source)
    {
        _apiName = source._apiName;
        this.DeclarationName = source.DeclarationName;
        _apiNameSource = source._apiNameSource;
        _itemNullability = source._itemNullability;
        _itemNullabilitySource = source._itemNullabilitySource;
        this.ClrParameterInfo = source.ClrParameterInfo;
        _apiTypeIdentity = source._apiTypeIdentity;
        _apiTypeExpressionFactory = source._apiTypeExpressionFactory;
        _state = new ApiQuerySourceParameterState(source._state);
        source.CopyExtensionsTo(this);
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
    #endregion

    #region Naming Methods
    /// <summary>Sets the API name while resolving a convention-derived declaration name.</summary>
    internal void SetApiNameConvention(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        if (_apiNameSource <= ApiConfigurationSource.Convention)
        {
            _apiName = apiName;
        }
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
    /// <summary>Creates an independent builder snapshot for one schema build.</summary>
    internal ApiQuerySourceParameterBuilder CreateBuildSnapshot()
        => new(this);

    /// <summary>Builds a fresh parameter declaration and type expression.</summary>
    internal ApiQuerySourceParameter Build()
    {
        // Build ApiQuerySourceParameter instance from all the configured components.
        var apiName = this.ApiName;
        var apiTypeExpression = ApiQuerySourceBuilder.ApplyItemNullability
            (_apiTypeExpressionFactory(), _itemNullability);
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

    #region With Methods
    /// <summary>Sets the parameter API name without changing its CLR declaration identity.</summary>
    /// <param name="apiName">The parameter API name.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder WithName(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        if (this.CurrentConfigurationSource >= _apiNameSource)
        {
            _apiName = apiName;
            _apiNameSource = this.CurrentConfigurationSource;
        }
        return this;
    }

    /// <summary>Sets nullability for items of a collection parameter.</summary>
    /// <param name="apiItemNullability">The collection-item nullability.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder WithItemNullability(ApiNullability apiItemNullability)
    {
        if (this.CurrentConfigurationSource >= _itemNullabilitySource)
        {
            _itemNullability = apiItemNullability;
            _itemNullabilitySource = this.CurrentConfigurationSource;
        }
        return this;
    }
    #endregion

    #region Properties
    internal bool HasInvalidItemNullability =>
        _itemNullability is not null && _apiTypeExpressionFactory().ApiInlineType is not ApiCollectionType;
    #endregion
}
