// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure the dependent end of an <see cref="ApiRelationship"/>.
/// </summary>
/// <remarks>
///     Set the FK key type with <see cref="WithForeignKeyType"/>.
///     When no FK key type is configured the relationship is treated as purely navigational.
/// </remarks>
/// <param name="clrObjectType">The CLR type of the dependent <see cref="ApiObjectType"/>.</param>
public class ApiRelationshipDependentEndBuilder(Type clrObjectType) : ExtensionBuilder<ApiRelationshipDependentEndBuilder>
{
    #region Fields
    private ApiKeyTypeBuilder? _foreignKeyTypeBuilder;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder AddRelationshipDependentEndExtension(Type type, object value)
    {
        base.AddExtension(type, value);
        return this;
    }

    /// <summary>
    ///     Adds an extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="T">The extension value type.</typeparam>
    /// <param name="value">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder AddRelationshipDependentEndExtension<T>(T value) where T : notnull
        => this.AddRelationshipDependentEndExtension(typeof(T), value);
    #endregion

    #region WithForeignKeyType Methods
    /// <summary>
    ///     Sets the FK key type with the given <paramref name="apiName"/>, optionally configuring it further.
    /// </summary>
    /// <param name="apiName">The API name of the FK key type.</param>
    /// <param name="configure">Optional callback to configure key paths on the FK key type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder WithForeignKeyType(string apiName, Action<ApiKeyTypeBuilder>? configure = null)
    {
        _foreignKeyTypeBuilder = new ApiKeyTypeBuilder(apiName);
        configure?.Invoke(_foreignKeyTypeBuilder);
        return this;
    }

    /// <summary>
    ///     Allows subclasses to set a pre-constructed FK key type builder.
    /// </summary>
    protected void SetForeignKeyTypeBuilderCore(ApiKeyTypeBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _foreignKeyTypeBuilder = builder;
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
            ? new ApiRelationshipDependentEnd(clrObjectType, apiForeignKeyType)
            : new ApiRelationshipDependentEnd(clrObjectType);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            end.Extensions = extensions;
        }

        return end;
    }
    #endregion
}
