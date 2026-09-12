// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Key;
using Evoogle.ApiFramework.Schema.Configuration.Relationships.Internal;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Relationships;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Relationships;

/// <summary>
///     Fluent builder used to configure the dependent end of an <see cref="ApiRelationship"/>.
/// </summary>
/// <remarks>
///     Set the foreign key role's <see cref="ApiKeyDefinition"/> with <see cref="WithForeignKey"/>.
///     When no key is configured the relationship is treated as purely navigational.
/// </remarks>
/// <param name="clrObjectType">The CLR type of the dependent <see cref="ApiObjectType"/>.</param>
public class ApiRelationshipDependentEndBuilder(Type clrObjectType) : ExtensionBuilder<ApiRelationshipDependentEndBuilder>
{
    #region Fields
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    private readonly Type _clrObjectType = clrObjectType ?? throw new ArgumentNullException(nameof(clrObjectType));
    private readonly ApiRelationshipDependentEndState _state = new();
    #endregion

    #region Properties
    /// <summary>Gets the CLR object type represented by this dependent end.</summary>
    internal Type ClrObjectType => _clrObjectType;

    /// <summary>Gets the source associated with the active fluent configuration callback.</summary>
    internal ApiConfigurationSource CurrentConfigurationSource =>
        _configurationSourceScope.CurrentSource;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder AddRelationshipDependentEndExtension(Type extensionType, object extension)
    {
        return this.AddExtension(extensionType, extension);
    }
    #endregion

    #region WithForeignKey Methods
    /// <summary>
    ///     Sets the foreign key role's <see cref="ApiKeyDefinition"/>, optionally configuring its key paths.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder WithForeignKey(Action<ApiKeyDefinitionBuilder>? configure = null)
    {
        var source = this.CurrentConfigurationSource;
        if (_state.ForeignKeyBuilderSource == null || source >= _state.ForeignKeyBuilderSource.Value)
        {
            _state.ForeignKeyBuilder ??= this.CreateForeignKeyBuilder();
            configure?.Invoke(_state.ForeignKeyBuilder);
            _state.ForeignKeyBuilderSource = source;
        }

        return this;
    }

    private ApiKeyDefinitionBuilder CreateForeignKeyBuilder()
    {
        return ApiBuilderFactory.CreateClosedGeneric<ApiKeyDefinitionBuilder>
        (
            typeof(ApiKeyDefinitionBuilder<>),
            this.ClrObjectType,
            (object?)null
        );
    }

    /// <summary>
    ///     Allows subclasses to set a pre-constructed key builder for the foreign key role.
    /// </summary>
    protected void SetForeignKeyBuilderCore(ApiKeyDefinitionBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var source = this.CurrentConfigurationSource;
        if (_state.ForeignKeyBuilderSource == null || source >= _state.ForeignKeyBuilderSource.Value)
        {
            _state.ForeignKeyBuilder = builder;
            _state.ForeignKeyBuilderSource = source;
        }
    }

    /// <summary>
    ///     Merges configured key-role facets from another builder without replacing facets that
    ///     were supplied by a higher-precedence source.
    /// </summary>
    internal void MergeConfigurationFrom(ApiRelationshipDependentEndBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if
        (
            builder._state.ForeignKeyBuilder != null &&
            builder._state.ForeignKeyBuilderSource != null &&
            (
                _state.ForeignKeyBuilderSource == null ||
                builder._state.ForeignKeyBuilderSource.Value >= _state.ForeignKeyBuilderSource.Value
            )
        )
        {
            _state.ForeignKeyBuilder = builder._state.ForeignKeyBuilder;
            _state.ForeignKeyBuilderSource = builder._state.ForeignKeyBuilderSource;
        }
    }
    #endregion

    #region Configuration Source Methods
    /// <summary>Runs a fluent callback at the supplied configuration-source precedence.</summary>
    internal void ApplyConfiguration(ApiConfigurationSource source, Action configure)
    {
        _configurationSourceScope.Apply(source, configure);
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiRelationshipDependentEnd"/> configured by this builder.
    /// </summary>
    internal ApiRelationshipDependentEnd Build()
    {
        var apiForeignKey = _state.ForeignKeyBuilder?.Build();

        var end = apiForeignKey != null
            ? new ApiRelationshipDependentEnd(_clrObjectType, apiForeignKey)
            : new ApiRelationshipDependentEnd(_clrObjectType);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            end.AttachExtensions(extensions);
        }

        return end;
    }
    #endregion
}
