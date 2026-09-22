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
///     Fluent builder used to configure the association of an <see cref="ApiRelationshipManyToMany"/>.
/// </summary>
/// <remarks>
///     Set the foreign key role keys with <see cref="WithForeignKeyA"/> and <see cref="WithForeignKeyB"/>.
///     When neither side is configured the relationship is treated as purely navigational.
/// </remarks>
/// <param name="apiObjectTypeReference">The association object-type reference.</param>
public class ApiRelationshipAssociationBuilder(ApiTypeReference apiObjectTypeReference)
    : ExtensionBuilder<ApiRelationshipAssociationBuilder>
{
    #region Fields
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    private readonly ApiTypeReference _apiObjectTypeReference = apiObjectTypeReference ??
        throw new ArgumentNullException(nameof(apiObjectTypeReference));
    private readonly ApiRelationshipAssociationState _state = new();
    #endregion

    #region Properties
    /// <summary>Gets the object-type reference represented by this association.</summary>
    internal ApiTypeReference ApiObjectTypeReference => _apiObjectTypeReference;

    /// <summary>Gets the source associated with the active fluent configuration callback.</summary>
    internal ApiConfigurationSource CurrentConfigurationSource =>
        _configurationSourceScope.CurrentSource;
    #endregion

    #region Constructors
    /// <summary>Creates an association builder from a CLR type.</summary>
    /// <param name="clrObjectType">The association CLR object type.</param>
    public ApiRelationshipAssociationBuilder(Type clrObjectType)
        : this
        (
            new ApiTypeReference
                (clrObjectType ?? throw new ArgumentNullException(nameof(clrObjectType)))
        )
    {
    }
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder AddRelationshipAssociationExtension(Type extensionType, object extension) => this.AddExtension(extensionType, extension);
    #endregion

    #region WithForeignKey Methods
    /// <summary>
    ///     Sets the A-side foreign key role's <see cref="ApiKeyDefinition"/>, optionally configuring it further.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder WithForeignKeyA(Action<ApiKeyDefinitionBuilder>? configure = null)
    {
        var source = this.CurrentConfigurationSource;
        if (_state.ForeignKeyBuilderASource == null || source >= _state.ForeignKeyBuilderASource.Value)
        {
            _state.ForeignKeyBuilderA ??= this.CreateForeignKeyBuilder();
            configure?.Invoke(_state.ForeignKeyBuilderA);
            _state.ForeignKeyBuilderASource = source;
        }

        return this;
    }

    /// <summary>
    ///     Sets the B-side foreign key role's <see cref="ApiKeyDefinition"/>, optionally configuring it further.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder WithForeignKeyB(Action<ApiKeyDefinitionBuilder>? configure = null)
    {
        var source = this.CurrentConfigurationSource;
        if (_state.ForeignKeyBuilderBSource == null || source >= _state.ForeignKeyBuilderBSource.Value)
        {
            _state.ForeignKeyBuilderB ??= this.CreateForeignKeyBuilder();
            configure?.Invoke(_state.ForeignKeyBuilderB);
            _state.ForeignKeyBuilderBSource = source;
        }

        return this;
    }

    private ApiKeyDefinitionBuilder CreateForeignKeyBuilder()
    {
        if (this.ApiObjectTypeReference.ClrType is null)
        {
            return new ApiKeyDefinitionBuilder();
        }

        return ApiBuilderFactory.CreateClosedGeneric<ApiKeyDefinitionBuilder>
        (
            typeof(ApiKeyDefinitionBuilder<>),
            this.ApiObjectTypeReference.ClrType,
            (object?)null
        );
    }

    /// <summary>
    ///     Allows subclasses to set a pre-constructed A-side key builder for the foreign key role.
    /// </summary>
    protected void SetForeignKeyBuilderACore(ApiKeyDefinitionBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var source = this.CurrentConfigurationSource;
        if (_state.ForeignKeyBuilderASource == null || source >= _state.ForeignKeyBuilderASource.Value)
        {
            _state.ForeignKeyBuilderA = builder;
            _state.ForeignKeyBuilderASource = source;
        }
    }

    /// <summary>
    ///     Allows subclasses to set a pre-constructed B-side key builder for the foreign key role.
    /// </summary>
    protected void SetForeignKeyBuilderBCore(ApiKeyDefinitionBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var source = this.CurrentConfigurationSource;
        if (_state.ForeignKeyBuilderBSource == null || source >= _state.ForeignKeyBuilderBSource.Value)
        {
            _state.ForeignKeyBuilderB = builder;
            _state.ForeignKeyBuilderBSource = source;
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
            builder._state.ForeignKeyBuilderA != null &&
            builder._state.ForeignKeyBuilderASource != null &&
            (
                _state.ForeignKeyBuilderASource == null ||
                builder._state.ForeignKeyBuilderASource.Value >=
                    _state.ForeignKeyBuilderASource.Value
            )
        )
        {
            _state.ForeignKeyBuilderA = builder._state.ForeignKeyBuilderA;
            _state.ForeignKeyBuilderASource = builder._state.ForeignKeyBuilderASource;
        }

        if
        (
            builder._state.ForeignKeyBuilderB != null &&
            builder._state.ForeignKeyBuilderBSource != null &&
            (
                _state.ForeignKeyBuilderBSource == null ||
                builder._state.ForeignKeyBuilderBSource.Value >=
                    _state.ForeignKeyBuilderBSource.Value
            )
        )
        {
            _state.ForeignKeyBuilderB = builder._state.ForeignKeyBuilderB;
            _state.ForeignKeyBuilderBSource = builder._state.ForeignKeyBuilderBSource;
        }
    }
    #endregion

    #region Configuration Source Methods
    /// <summary>Runs a fluent callback at the supplied configuration-source precedence.</summary>
    internal void ApplyConfiguration(ApiConfigurationSource source, Action configure) => _configurationSourceScope.Apply(source, configure);
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiRelationshipAssociation"/> configured by this builder.
    /// </summary>
    internal ApiRelationshipAssociation Build()
    {
        var fkA = _state.ForeignKeyBuilderA?.Build();
        var fkB = _state.ForeignKeyBuilderB?.Build();

        var apiRelationshipAssociation = fkA != null && fkB != null
            ? new ApiRelationshipAssociation(_apiObjectTypeReference, fkA, fkB)
            : new ApiRelationshipAssociation(_apiObjectTypeReference);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiRelationshipAssociation.AttachExtensions(extensions);
        }

        return apiRelationshipAssociation;
    }
    #endregion
}
