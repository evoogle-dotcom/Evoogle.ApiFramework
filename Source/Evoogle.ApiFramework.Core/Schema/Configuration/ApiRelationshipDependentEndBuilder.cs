// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure the dependent end of an <see cref="ApiRelationship"/>.
/// </summary>
/// <remarks>
///     Set the foreign key role's <see cref="ApiKeyType"/> with <see cref="WithForeignKey"/>.
///     When no key type is configured the relationship is treated as purely navigational.
/// </remarks>
/// <param name="clrObjectType">The CLR type of the dependent <see cref="ApiObjectType"/>.</param>
public class ApiRelationshipDependentEndBuilder(Type clrObjectType) : ExtensionBuilder<ApiRelationshipDependentEndBuilder>
{
    #region Fields
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    private readonly Type _clrObjectType = clrObjectType ?? throw new ArgumentNullException(nameof(clrObjectType));
    private ApiKeyTypeBuilder? _foreignKeyTypeBuilder;
    private ApiConfigurationSource? _foreignKeyTypeBuilderSource;
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
    ///     Sets the foreign key role's <see cref="ApiKeyType"/>, optionally configuring its key paths.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder WithForeignKey(Action<ApiKeyTypeBuilder>? configure = null)
    {
        var source = this.CurrentConfigurationSource;
        if (_foreignKeyTypeBuilderSource == null || source >= _foreignKeyTypeBuilderSource.Value)
        {
            _foreignKeyTypeBuilder = new ApiKeyTypeBuilder();
            configure?.Invoke(_foreignKeyTypeBuilder);
            _foreignKeyTypeBuilderSource = source;
        }

        return this;
    }

    /// <summary>
    ///     Allows subclasses to set a pre-constructed key type builder for the foreign key role.
    /// </summary>
    protected void SetForeignKeyTypeBuilderCore(ApiKeyTypeBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var source = this.CurrentConfigurationSource;
        if (_foreignKeyTypeBuilderSource == null || source >= _foreignKeyTypeBuilderSource.Value)
        {
            _foreignKeyTypeBuilder = builder;
            _foreignKeyTypeBuilderSource = source;
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
            builder._foreignKeyTypeBuilder != null &&
            builder._foreignKeyTypeBuilderSource != null &&
            (
                _foreignKeyTypeBuilderSource == null ||
                builder._foreignKeyTypeBuilderSource.Value >= _foreignKeyTypeBuilderSource.Value
            )
        )
        {
            _foreignKeyTypeBuilder = builder._foreignKeyTypeBuilder;
            _foreignKeyTypeBuilderSource = builder._foreignKeyTypeBuilderSource;
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
        var apiForeignKeyType = _foreignKeyTypeBuilder?.Build();

        var end = apiForeignKeyType != null
            ? new ApiRelationshipDependentEnd(_clrObjectType, apiForeignKeyType)
            : new ApiRelationshipDependentEnd(_clrObjectType);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            end.Extensions = extensions;
        }

        return end;
    }
    #endregion
}
