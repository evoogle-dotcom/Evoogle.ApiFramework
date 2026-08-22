// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure the principal end of an <see cref="ApiRelationship"/>.
/// </summary>
/// <remarks>
///     The principal end provides the principal key for the relationship. For key-bound relationships, initialization
///     infers the best compatible principal key type from the corresponding foreign key when no name is supplied. Call
///     <see cref="WithPrincipalKey"/> to specify the named principal key type explicitly. Delete behavior is configured on the
///     relationship builder, not on individual ends.
/// </remarks>
/// <param name="clrObjectType">The CLR type of the principal <see cref="ApiObjectType"/>.</param>
public sealed class ApiRelationshipPrincipalEndBuilder(Type clrObjectType) : ExtensionBuilder<ApiRelationshipPrincipalEndBuilder>
{
    #region Fields
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    private readonly Type _clrObjectType = clrObjectType ?? throw new ArgumentNullException(nameof(clrObjectType));
    private readonly ApiRelationshipPrincipalEndState _state = new();
    #endregion

    #region Properties
    /// <summary>Gets the CLR object type represented by this principal end.</summary>
    internal Type ClrObjectType => _clrObjectType;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipPrincipalEndBuilder AddRelationshipPrincipalEndExtension(Type extensionType, object extension)
    {
        return this.AddExtension(extensionType, extension);
    }
    #endregion

    #region With Methods
    /// <summary>
    ///     Selects a named key type on the principal object type to use as the relationship's principal key,
    ///     overriding automatic compatibility-based key inference.
    /// </summary>
    /// <param name="apiPrincipalKeyTypeName">The name of the principal key type to use for the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipPrincipalEndBuilder WithPrincipalKey(string apiPrincipalKeyTypeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiPrincipalKeyTypeName, nameof(apiPrincipalKeyTypeName));

        var source = _configurationSourceScope.CurrentSource;
        if (_state.PrincipalKeyTypeNameSource == null || source >= _state.PrincipalKeyTypeNameSource.Value)
        {
            _state.PrincipalKeyTypeName = apiPrincipalKeyTypeName;
            _state.PrincipalKeyTypeNameSource = source;
        }

        return this;
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
    ///     Builds the <see cref="ApiRelationshipPrincipalEnd"/> configured by this builder.
    /// </summary>
    internal ApiRelationshipPrincipalEnd Build()
    {
        var end = new ApiRelationshipPrincipalEnd
        (
            _clrObjectType,
            _state.PrincipalKeyTypeName
        );

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            end.Extensions = extensions;
        }

        return end;
    }
    #endregion
}
