// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Base class for fluent builders of API named types.
/// </summary>
/// <typeparam name="TBuilder">The concrete builder type.</typeparam>
/// <param name="clrType">The CLR type represented by the API named type.</param>
/// <param name="context">The shared builder context.</param>
public abstract class ApiNamedTypeBuilder<TBuilder>(Type clrType, ApiSchemaBuilderContext context) : ExtensionBuilder<TBuilder>
    where TBuilder : ApiNamedTypeBuilder<TBuilder>
{
    #region Fields
    private string _apiName = ValidateClrType(clrType).SafeToName();
    private ApiConfigurationSource _apiNameSource = ApiConfigurationSource.Convention;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the API name configured for the type.
    /// </summary>
    public string ApiName => _apiName;

    /// <summary>
    ///     Gets the CLR type represented by this builder.
    /// </summary>
    public Type ClrType { get; } = ValidateClrType(clrType);

    /// <summary>
    ///     Gets the shared builder context.
    /// </summary>
    protected ApiSchemaBuilderContext Context { get; } = context ?? throw new ArgumentNullException(nameof(context));
    #endregion

    #region With Methods
    /// <summary>
    ///     Sets the API name for the type being built.
    /// </summary>
    /// <param name="apiName">The API name to use.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder WithName(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        return this.SetApiName(apiName, ApiConfigurationSource.Explicit);
    }
    #endregion

    #region Internal Convention/Annotation Methods
    /// <summary>
    ///     Sets the API name at <see cref="ApiConfigurationSource.Convention"/> precedence.
    ///     Has no effect if a higher-precedence value has already been applied.
    /// </summary>
    internal TBuilder SetApiNameConvention(string apiName)
        => this.SetApiName(apiName, ApiConfigurationSource.Convention);

    /// <summary>
    ///     Sets the API name at <see cref="ApiConfigurationSource.DataAnnotation"/> precedence.
    ///     Has no effect if an explicit value has already been applied.
    /// </summary>
    internal TBuilder SetApiNameDataAnnotation(string apiName)
        => this.SetApiName(apiName, ApiConfigurationSource.DataAnnotation);
    #endregion

    #region Implementation Methods
    private TBuilder SetApiName(string apiName, ApiConfigurationSource source)
    {
        if (source >= _apiNameSource)
        {
            _apiName = apiName;
            _apiNameSource = source;
        }

        return (TBuilder)this;
    }

    private static Type ValidateClrType(Type clrType)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        return clrType;
    }
    #endregion
}
