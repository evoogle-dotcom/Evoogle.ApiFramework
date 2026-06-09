// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
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
    private ApiKeyTypeBuilder? _foreignKeyTypeBuilderA;
    private ApiKeyTypeBuilder? _foreignKeyTypeBuilderB;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder AddRelationshipAssociationExtension(Type type, object value)
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
    public ApiRelationshipAssociationBuilder AddRelationshipAssociationExtension<T>(T value) where T : notnull
        => this.AddRelationshipAssociationExtension(typeof(T), value);
    #endregion

    #region WithForeignKey Methods
    /// <summary>
    ///     Sets the A-side foreign key role's <see cref="ApiKeyType"/>, optionally configuring it further.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder WithForeignKeyA(Action<ApiKeyTypeBuilder>? configure = null)
    {
        _foreignKeyTypeBuilderA = new ApiKeyTypeBuilder();
        configure?.Invoke(_foreignKeyTypeBuilderA);
        return this;
    }

    /// <summary>
    ///     Sets the B-side foreign key role's <see cref="ApiKeyType"/>, optionally configuring it further.
    /// </summary>
    /// <param name="configure">Optional callback to configure key paths on the key type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipAssociationBuilder WithForeignKeyB(Action<ApiKeyTypeBuilder>? configure = null)
    {
        _foreignKeyTypeBuilderB = new ApiKeyTypeBuilder();
        configure?.Invoke(_foreignKeyTypeBuilderB);
        return this;
    }

    /// <summary>
    ///     Allows subclasses to set a pre-constructed A-side key type builder for the foreign key role.
    /// </summary>
    protected void SetForeignKeyTypeBuilderACore(ApiKeyTypeBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _foreignKeyTypeBuilderA = builder;
    }

    /// <summary>
    ///     Allows subclasses to set a pre-constructed B-side key type builder for the foreign key role.
    /// </summary>
    protected void SetForeignKeyTypeBuilderBCore(ApiKeyTypeBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _foreignKeyTypeBuilderB = builder;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiRelationshipAssociation"/> configured by this builder.
    /// </summary>
    internal ApiRelationshipAssociation Build()
    {
        var fkA = _foreignKeyTypeBuilderA?.Build();
        var fkB = _foreignKeyTypeBuilderB?.Build();

        var apiRelationshipAssociation = fkA != null && fkB != null
            ? new ApiRelationshipAssociation(clrObjectType, fkA, fkB)
            : new ApiRelationshipAssociation(clrObjectType);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiRelationshipAssociation.Extensions = extensions;
        }

        return apiRelationshipAssociation;
    }
    #endregion
}
