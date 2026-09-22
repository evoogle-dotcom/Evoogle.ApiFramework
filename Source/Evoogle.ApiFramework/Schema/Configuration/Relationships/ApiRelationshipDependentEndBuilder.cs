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
/// <param name="apiObjectTypeReference">The dependent object-type reference.</param>
public class ApiRelationshipDependentEndBuilder(ApiTypeReference apiObjectTypeReference)
    : ExtensionBuilder<ApiRelationshipDependentEndBuilder>
{
    #region Fields
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    private readonly ApiTypeReference _apiObjectTypeReference = apiObjectTypeReference ??
        throw new ArgumentNullException(nameof(apiObjectTypeReference));
    private readonly ApiRelationshipDependentEndState _state = new();
    private ApiRelationshipTraversal? _traversal;
    private ApiConfigurationSource? _traversalSource;
    #endregion

    #region Properties
    /// <summary>Gets the object-type reference represented by this dependent end.</summary>
    internal ApiTypeReference ApiObjectTypeReference => _apiObjectTypeReference;

    /// <summary>Gets the source associated with the active fluent configuration callback.</summary>
    internal ApiConfigurationSource CurrentConfigurationSource =>
        _configurationSourceScope.CurrentSource;
    #endregion

    #region Constructors
    /// <summary>Creates a dependent-end builder from a CLR type.</summary>
    /// <param name="clrObjectType">The dependent CLR object type.</param>
    public ApiRelationshipDependentEndBuilder(Type clrObjectType)
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
    public ApiRelationshipDependentEndBuilder AddRelationshipDependentEndExtension(Type extensionType, object extension) => this.AddExtension(extensionType, extension);
    #endregion

    #region WithTraversal Methods
    /// <summary>Sets the optional traversal from this end to the opposite relationship end.</summary>
    /// <param name="apiName">The API name exposed on the source object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder WithTraversal(string apiName)
        => this.WithTraversalCore(apiName, clrMemberReference: null);

    /// <summary>Sets the traversal and its CLR navigation member reference.</summary>
    /// <param name="apiName">The API name exposed on the source object type.</param>
    /// <param name="clrMemberReference">The CLR navigation member reference.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder WithTraversal
    (
        string apiName,
        ApiClrMemberReference clrMemberReference
    )
    {
        ArgumentNullException.ThrowIfNull(clrMemberReference);
        return this.WithTraversalCore(apiName, clrMemberReference);
    }

    /// <summary>Sets the traversal and its CLR navigation member identity.</summary>
    /// <param name="apiName">The API name exposed on the source object type.</param>
    /// <param name="clrMemberName">The CLR navigation member name.</param>
    /// <param name="clrMemberKind">The CLR navigation member kind.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder WithTraversal
    (
        string apiName,
        string clrMemberName,
        ClrMemberKind clrMemberKind = ClrMemberKind.Property
    ) => this.WithTraversal
    (
        apiName,
        new ApiClrMemberReference(clrMemberKind, clrMemberName)
    );

    private ApiRelationshipDependentEndBuilder WithTraversalCore
    (
        string apiName,
        ApiClrMemberReference? clrMemberReference
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        var source = this.CurrentConfigurationSource;
        if (_traversalSource is null || source >= _traversalSource.Value)
        {
            _traversal = new ApiRelationshipTraversal(apiName, clrMemberReference);
            _traversalSource = source;
        }
        return this;
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

        if (builder._traversal is not null && builder._traversalSource is not null &&
            (_traversalSource is null || builder._traversalSource.Value >= _traversalSource.Value))
        {
            _traversal = builder._traversal;
            _traversalSource = builder._traversalSource;
        }

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
    internal void ApplyConfiguration(ApiConfigurationSource source, Action configure) => _configurationSourceScope.Apply(source, configure);
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiRelationshipDependentEnd"/> configured by this builder.
    /// </summary>
    internal ApiRelationshipDependentEnd Build()
    {
        var apiForeignKey = _state.ForeignKeyBuilder?.Build();

        var end = new ApiRelationshipDependentEnd
        (
            _apiObjectTypeReference,
            _traversal,
            apiForeignKey
        );

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            end.AttachExtensions(extensions);
        }

        return end;
    }
    #endregion
}
