// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Relationships.Internal;
using Evoogle.ApiFramework.Schema.Relationships;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Relationships;

/// <summary>
///     Fluent builder used to configure the principal end of an <see cref="ApiRelationship"/>.
/// </summary>
/// <remarks>
///     The principal end provides the principal key for the relationship. For key-bound relationships, compilation
///     infers the best compatible principal key from the corresponding foreign key when no name is supplied. Call
///     <see cref="WithPrincipalKey"/> to specify the named principal key explicitly. Delete behavior is configured on the
///     relationship builder, not on individual ends.
/// </remarks>
/// <param name="apiObjectTypeReference">The principal object-type reference.</param>
public sealed class ApiRelationshipPrincipalEndBuilder(ApiTypeReference apiObjectTypeReference)
    : ExtensionBuilder<ApiRelationshipPrincipalEndBuilder>
{
    #region Fields
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    private readonly ApiTypeReference _apiObjectTypeReference = apiObjectTypeReference ??
        throw new ArgumentNullException(nameof(apiObjectTypeReference));
    private readonly ApiRelationshipPrincipalEndState _state = new();
    private ApiRelationshipTraversal? _traversal;
    private ApiConfigurationSource? _traversalSource;
    #endregion

    #region Properties
    /// <summary>Gets the object-type reference represented by this principal end.</summary>
    internal ApiTypeReference ApiObjectTypeReference => _apiObjectTypeReference;
    #endregion

    #region Constructors
    /// <summary>Creates a principal-end builder from a CLR type.</summary>
    /// <param name="clrObjectType">The principal CLR object type.</param>
    public ApiRelationshipPrincipalEndBuilder(Type clrObjectType)
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
    public ApiRelationshipPrincipalEndBuilder AddRelationshipPrincipalEndExtension(Type extensionType, object extension) => this.AddExtension(extensionType, extension);
    #endregion

    #region WithTraversal Methods
    /// <summary>Sets the optional traversal from this end to the opposite relationship end.</summary>
    /// <param name="apiName">The API name exposed on the source object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipPrincipalEndBuilder WithTraversal(string apiName)
        => this.WithTraversalCore(apiName, clrMemberReference: null);

    /// <summary>Sets the traversal and its CLR navigation member reference.</summary>
    /// <param name="apiName">The API name exposed on the source object type.</param>
    /// <param name="clrMemberReference">The CLR navigation member reference.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipPrincipalEndBuilder WithTraversal
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
    public ApiRelationshipPrincipalEndBuilder WithTraversal
    (
        string apiName,
        string clrMemberName,
        ClrMemberKind clrMemberKind = ClrMemberKind.Property
    ) => this.WithTraversal
    (
        apiName,
        new ApiClrMemberReference(clrMemberKind, clrMemberName)
    );

    private ApiRelationshipPrincipalEndBuilder WithTraversalCore
    (
        string apiName,
        ApiClrMemberReference? clrMemberReference
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        var source = _configurationSourceScope.CurrentSource;
        if (_traversalSource is null || source >= _traversalSource.Value)
        {
            _traversal = new ApiRelationshipTraversal(apiName, clrMemberReference);
            _traversalSource = source;
        }
        return this;
    }
    #endregion

    #region With Methods
    /// <summary>
    ///     Selects a named key on the principal object type to use as the relationship's principal key,
    ///     overriding automatic compatibility-based key inference.
    /// </summary>
    /// <param name="apiPrincipalKeyName">The name of the principal key to use for the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipPrincipalEndBuilder WithPrincipalKey(string apiPrincipalKeyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiPrincipalKeyName, nameof(apiPrincipalKeyName));

        var source = _configurationSourceScope.CurrentSource;
        if (_state.PrincipalKeyNameSource == null || source >= _state.PrincipalKeyNameSource.Value)
        {
            _state.PrincipalKeyName = apiPrincipalKeyName;
            _state.PrincipalKeyNameSource = source;
        }

        return this;
    }
    #endregion

    #region Configuration Source Methods
    /// <summary>Runs a fluent callback at the supplied configuration-source precedence.</summary>
    internal void ApplyConfiguration(ApiConfigurationSource source, Action configure) => _configurationSourceScope.Apply(source, configure);
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiRelationshipPrincipalEnd"/> configured by this builder.
    /// </summary>
    internal ApiRelationshipPrincipalEnd Build()
    {
        var end = new ApiRelationshipPrincipalEnd
        (
            _apiObjectTypeReference,
            _traversal,
            _state.PrincipalKeyName
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
