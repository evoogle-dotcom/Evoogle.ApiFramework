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
///     Set the foreign key role's <see cref="ApiKeyType"/> with <see cref="WithForeignKey"/>.
///     When no key type is configured the relationship is treated as purely navigational.
/// </remarks>
/// <param name="clrObjectType">The CLR type of the dependent <see cref="ApiObjectType"/>.</param>
public class ApiRelationshipDependentEndBuilder(Type clrObjectType) : ExtensionBuilder<ApiRelationshipDependentEndBuilder>
{
    #region Fields
    private readonly Type _clrObjectType = clrObjectType ?? throw new ArgumentNullException(nameof(clrObjectType));
    private ApiKeyTypeBuilder? _foreignKeyTypeBuilder;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipDependentEndBuilder AddRelationshipDependentEndExtension(Type type, object extension)
    {
        return this.AddExtension(type, extension);
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
        _foreignKeyTypeBuilder = new ApiKeyTypeBuilder();
        configure?.Invoke(_foreignKeyTypeBuilder);
        return this;
    }

    /// <summary>
    ///     Allows subclasses to set a pre-constructed key type builder for the foreign key role.
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
