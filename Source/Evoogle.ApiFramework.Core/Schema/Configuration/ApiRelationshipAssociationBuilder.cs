// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure the association of an <see cref="ApiRelationshipManyToMany"/>.
/// </summary>
/// <remarks>
///     Set the foreign key role key types with <see cref="WithForeignKeyA"/> and <see cref="WithForeignKeyB"/>.
///     When neither side is configured the relationship is treated as purely navigational.
/// </remarks>
/// <param name="clrObjectType">The CLR type of the association <see cref="ApiObjectType"/>.</param>
public class ApiRelationshipAssociationBuilder(Type clrObjectType) : ExtensionBuilder<ApiRelationshipAssociationBuilder>
{
    #region Fields
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    private readonly Type _clrObjectType = clrObjectType ?? throw new ArgumentNullException(nameof(clrObjectType));
    private readonly ApiRelationshipAssociationState _state = new();
    #endregion

    #region Properties
    /// <summary>Gets the CLR object type represented by this association.</summary>
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
    public ApiRelationshipAssociationBuilder AddRelationshipAssociationExtension(Type extensionType, object extension)
    {
        return this.AddExtension(extensionType, extension);
    }
    #endregion

    #region WithForeignKey Methods
    /// <summary>
    ///     Sets the A-side foreign key role's <see cref="ApiKeyType"/>, optionally configuring it further.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder WithForeignKeyA(Action<ApiKeyTypeBuilder>? configure = null)
    {
        var source = this.CurrentConfigurationSource;
        if (_state.ForeignKeyTypeBuilderASource == null || source >= _state.ForeignKeyTypeBuilderASource.Value)
        {
            _state.ForeignKeyTypeBuilderA ??= this.CreateForeignKeyTypeBuilder();
            configure?.Invoke(_state.ForeignKeyTypeBuilderA);
            _state.ForeignKeyTypeBuilderASource = source;
        }

        return this;
    }

    /// <summary>
    ///     Sets the B-side foreign key role's <see cref="ApiKeyType"/>, optionally configuring it further.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder WithForeignKeyB(Action<ApiKeyTypeBuilder>? configure = null)
    {
        var source = this.CurrentConfigurationSource;
        if (_state.ForeignKeyTypeBuilderBSource == null || source >= _state.ForeignKeyTypeBuilderBSource.Value)
        {
            _state.ForeignKeyTypeBuilderB ??= this.CreateForeignKeyTypeBuilder();
            configure?.Invoke(_state.ForeignKeyTypeBuilderB);
            _state.ForeignKeyTypeBuilderBSource = source;
        }

        return this;
    }

    private ApiKeyTypeBuilder CreateForeignKeyTypeBuilder()
    {
        return ApiBuilderFactory.CreateClosedGeneric<ApiKeyTypeBuilder>
        (
            typeof(ApiKeyTypeBuilder<>),
            this.ClrObjectType,
            (object?)null
        );
    }

    /// <summary>
    ///     Allows subclasses to set a pre-constructed A-side key type builder for the foreign key role.
    /// </summary>
    protected void SetForeignKeyTypeBuilderACore(ApiKeyTypeBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var source = this.CurrentConfigurationSource;
        if (_state.ForeignKeyTypeBuilderASource == null || source >= _state.ForeignKeyTypeBuilderASource.Value)
        {
            _state.ForeignKeyTypeBuilderA = builder;
            _state.ForeignKeyTypeBuilderASource = source;
        }
    }

    /// <summary>
    ///     Allows subclasses to set a pre-constructed B-side key type builder for the foreign key role.
    /// </summary>
    protected void SetForeignKeyTypeBuilderBCore(ApiKeyTypeBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var source = this.CurrentConfigurationSource;
        if (_state.ForeignKeyTypeBuilderBSource == null || source >= _state.ForeignKeyTypeBuilderBSource.Value)
        {
            _state.ForeignKeyTypeBuilderB = builder;
            _state.ForeignKeyTypeBuilderBSource = source;
        }
    }

    /// <summary>
    ///     Merges configured key-role facets from another builder without replacing facets that
    ///     were supplied by a higher-precedence source.
    /// </summary>
    internal void MergeConfigurationFrom(ApiRelationshipAssociationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if
        (
            builder._state.ForeignKeyTypeBuilderA != null &&
            builder._state.ForeignKeyTypeBuilderASource != null &&
            (
                _state.ForeignKeyTypeBuilderASource == null ||
                builder._state.ForeignKeyTypeBuilderASource.Value >=
                    _state.ForeignKeyTypeBuilderASource.Value
            )
        )
        {
            _state.ForeignKeyTypeBuilderA = builder._state.ForeignKeyTypeBuilderA;
            _state.ForeignKeyTypeBuilderASource = builder._state.ForeignKeyTypeBuilderASource;
        }

        if
        (
            builder._state.ForeignKeyTypeBuilderB != null &&
            builder._state.ForeignKeyTypeBuilderBSource != null &&
            (
                _state.ForeignKeyTypeBuilderBSource == null ||
                builder._state.ForeignKeyTypeBuilderBSource.Value >=
                    _state.ForeignKeyTypeBuilderBSource.Value
            )
        )
        {
            _state.ForeignKeyTypeBuilderB = builder._state.ForeignKeyTypeBuilderB;
            _state.ForeignKeyTypeBuilderBSource = builder._state.ForeignKeyTypeBuilderBSource;
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
    ///     Builds the <see cref="ApiRelationshipAssociation"/> configured by this builder.
    /// </summary>
    internal ApiRelationshipAssociation Build()
    {
        var fkA = _state.ForeignKeyTypeBuilderA?.Build();
        var fkB = _state.ForeignKeyTypeBuilderB?.Build();

        var apiRelationshipAssociation = fkA != null && fkB != null
            ? new ApiRelationshipAssociation(_clrObjectType, fkA, fkB)
            : new ApiRelationshipAssociation(_clrObjectType);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiRelationshipAssociation.Extensions = extensions;
        }

        return apiRelationshipAssociation;
    }
    #endregion
}
