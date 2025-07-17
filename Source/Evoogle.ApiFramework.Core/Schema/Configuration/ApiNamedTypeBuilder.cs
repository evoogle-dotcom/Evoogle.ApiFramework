// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Base class for fluent builders of API named types.
/// </summary>
/// <typeparam name="TBuilder">The concrete builder type.</typeparam>
/// <param name="clrType">The CLR type represented by the API named type.</param>
/// <param name="context">The shared builder context.</param>
public abstract class ApiNamedTypeBuilder<TBuilder>(Type clrType, ApiSchemaBuilderContext context) : IApiNamedTypeBuilder
    where TBuilder : ApiNamedTypeBuilder<TBuilder>
{
    #region Fields
    private string? _apiName;
    #endregion

    #region IApiNamedTypeBuilder Properties
    /// <summary>
    ///     Gets the API name configured for the type.
    /// </summary>
    public string ApiName => _apiName!;

    /// <summary>
    ///     Gets the CLR type represented by this builder.
    /// </summary>
    public Type ClrType { get; } = clrType;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the shared builder context for registering API types.
    /// </summary>
    protected ApiSchemaBuilderContext Context { get; } = context ?? throw new ArgumentNullException(nameof(context));
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Sets the API name for the type being built.
    /// </summary>
    /// <param name="apiName">The API name to use.</param>
    /// <returns>The current builder instance.</returns>
    public TBuilder WithApiName(string apiName)
    {
        _apiName = apiName;
        return (TBuilder)this;
    }
    #endregion
}
