// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
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
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the API name configured for the type.
    /// </summary>
    internal string ApiName => _apiName;

    /// <summary>
    ///     Gets the CLR type represented by this builder.
    /// </summary>
    internal Type ClrType { get; } = ValidateClrType(clrType);

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

        _apiName = apiName;
        return (TBuilder)this;
    }
    #endregion

    #region Implementation Methods
    private static Type ValidateClrType(Type clrType)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        return clrType;
    }
    #endregion
}
