// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Query.Internal;
using Evoogle.ApiFramework.Schema.Internal;
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
    private readonly string _apiName;
    private readonly ApiTypeReference _apiTypeReference;
    private readonly ApiQuerySourceParameterState _state = new();
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    #endregion

    #region Constructors
    /// <summary>Creates a required, non-null input parameter builder.</summary>
    /// <param name="apiName">The parameter API name, unique within its source.</param>
    /// <param name="apiTypeReference">The registered input type reference.</param>
    public ApiQuerySourceParameterBuilder(string apiName, ApiTypeReference apiTypeReference)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        ArgumentNullException.ThrowIfNull(apiTypeReference);

        _apiName = apiName;
        _apiTypeReference = apiTypeReference;
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
    /// <summary>Gets the fixed parameter API name.</summary>
    internal string ApiName => _apiName;

    /// <summary>Gets the fixed input type reference.</summary>
    internal ApiTypeReference ApiTypeReference => _apiTypeReference;

    /// <summary>Gets the source associated with the active configuration callback.</summary>
    internal ApiConfigurationSource CurrentConfigurationSource => _configurationSourceScope.CurrentSource;

    /// <summary>Gets the highest-precedence source that registered this parameter.</summary>
    internal ApiConfigurationSource RegistrationSource => _state.RegistrationSource;
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
    /// <param name="apiTypeModifiers">The input type modifiers.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder WithModifiers(ApiTypeModifiers apiTypeModifiers)
    {
        var source = this.CurrentConfigurationSource;
        if (source >= _state.ModifiersSource)
        {
            _state.ApiTypeModifiers = apiTypeModifiers;
            _state.ModifiersSource = source;
        }

        return this;
    }

    /// <summary>Allows a supplied argument value to be null without changing argument presence.</summary>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder AsNullable()
        => this.WithModifiers(ApiTypeModifiers.None);

    /// <summary>Prohibits null for a supplied argument value without changing argument presence.</summary>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder AsNonNullable()
        => this.WithModifiers(ApiTypeModifiers.Required);

    /// <summary>Allows the caller to omit the argument without changing value nullability.</summary>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder WithOptionalArgument()
        => this.WithRequiredArgument(false);

    /// <summary>Configures whether an argument must be supplied.</summary>
    /// <param name="requiresArgument">Whether argument omission is prohibited.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceParameterBuilder WithRequiredArgument
    (
        bool requiresArgument = ApiSchemaDefaults.QuerySourceParameterRequiresArgument
    )
    {
        var source = this.CurrentConfigurationSource;
        if (source >= _state.ApiRequiresArgumentSource)
        {
            _state.ApiRequiresArgument = requiresArgument;
            _state.ApiRequiresArgumentSource = source;
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
        var apiTypeExpression = new ApiTypeExpression(this.ApiTypeReference);
        var apiTypeModifiers = _state.ApiTypeModifiers;
        var apiRequiresArgument = _state.ApiRequiresArgument;

        var apiQuerySourceParameter = new ApiQuerySourceParameter
        (
            apiName,
            apiTypeExpression,
            apiTypeModifiers,
            apiRequiresArgument
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
