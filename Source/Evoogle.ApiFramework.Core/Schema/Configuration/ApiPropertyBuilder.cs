// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Builds <see cref="ApiProperty"/> definitions from CLR property/field metadata and optional modifiers.
/// </summary>
/// <param name="apiName">The API name of the property.</param>
/// <param name="clrName">The CLR property name.</param>
public class ApiPropertyBuilder(string apiName, string clrName) : ExtensionBuilder<ApiPropertyBuilder>
{
    #region Fields
    private string _apiName = apiName;
    private readonly string _clrName = clrName;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets or sets a delegate that configures additional type modifiers for the property.
    /// </summary>
    internal Action<ApiTypeModifiersBuilder>? Modifiers { get; set; }
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiPropertyBuilder AddPropertyExtension(Type type, object value)
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
    public ApiPropertyBuilder AddPropertyExtension<T>(T value) where T : notnull
        => this.AddPropertyExtension(typeof(T), value);
    #endregion

    #region As Methods
    /// <summary>
    ///     Marks this property as required, overriding any nullability-inferred modifier.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiPropertyBuilder AsRequired()
    {
        return this.WithModifiers(m => m.Required());
    }

    /// <summary>
    ///     Marks this property as optional, overriding any nullability-inferred modifier.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiPropertyBuilder AsOptional()
    {
        return this.WithModifiers(m => m.Optional());
    }
    #endregion

    #region With Methods
    /// <summary>
    ///     Configures type modifiers for the property.
    /// </summary>
    /// <param name="configure">Callback to configure type modifiers.</param>
    /// <returns>The current builder instance.</returns>
    public ApiPropertyBuilder WithModifiers(Action<ApiTypeModifiersBuilder> configure)
    {
        this.Modifiers = configure;
        return this;
    }

    /// <summary>
    ///    Sets the API name for the property being built.
    /// </summary>
    /// <param name="apiName">The API name to use.</param>
    /// <returns>The current builder instance.</returns>
    public ApiPropertyBuilder WithName(string apiName)
    {
        _apiName = apiName;
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds an <see cref="ApiProperty"/> for the specified CLR object type.
    /// </summary>
    /// <param name="clrObjectType">The CLR type declaring the property/field.</param>
    /// <returns>The constructed <see cref="ApiProperty"/> instance.</returns>
    internal ApiProperty Build(Type clrObjectType)
    {
        var clrPropertyInfo = TypeReflection.GetProperty(clrObjectType, _clrName);
        if (clrPropertyInfo != null)
        {
            return this.BuildFromPropertyInfo(clrPropertyInfo);
        }

        var clrFieldInfo = TypeReflection.GetField(clrObjectType, _clrName);
        if (clrFieldInfo != null)
        {
            return this.BuildFromFieldInfo(clrFieldInfo);
        }

        return this.BuildUnknownProperty();
    }

    private ApiProperty BuildFromPropertyInfo(PropertyInfo clrPropertyInfo)
    {
        var clrPropertyNullabilityInfo = PropertyReflection.GetNullabilityInfo(clrPropertyInfo);
        return this.BuildFromNullabilityInfo(clrPropertyNullabilityInfo, ClrMemberKind.Property);
    }

    private ApiProperty BuildFromFieldInfo(FieldInfo clrFieldInfo)
    {
        var clrFieldNullabilityInfo = FieldReflection.GetNullabilityInfo(clrFieldInfo);
        return this.BuildFromNullabilityInfo(clrFieldNullabilityInfo, ClrMemberKind.Field);
    }

    private ApiTypeModifiers BuildModifiers(ApiTypeModifiers apiInitialTypeModifiers)
    {
        if (this.Modifiers == null)
        {
            return apiInitialTypeModifiers;
        }

        var modifierBuilder = new ApiTypeModifiersBuilder(apiInitialTypeModifiers);
        this.Modifiers.Invoke(modifierBuilder);
        return modifierBuilder.Build();
    }

    private ApiProperty BuildFromNullabilityInfo(MemberNullableInfo clrNullabilityInfo, ClrMemberKind clrMemberKind)
    {
        var apiTypeExpression = ApiTypeExpressionBuilder.Build(clrNullabilityInfo);
        var apiInitialTypeModifiers = clrNullabilityInfo.Nullability == MemberNullability.NonNullable ? ApiTypeModifiers.Required : ApiTypeModifiers.None;
        var apiTypeModifiers = this.BuildModifiers(apiInitialTypeModifiers);

        return this.CreateAndBuildExtensions(_apiName, apiTypeExpression, apiTypeModifiers, _clrName, clrMemberKind);
    }

    private ApiProperty BuildUnknownProperty()
    {
        var apiTypeModifiers = this.BuildModifiers(ApiTypeModifiers.None);

        return this.CreateAndBuildExtensions(_apiName, default!, apiTypeModifiers, _clrName, ClrMemberKind.Unknown);
    }

    private ApiProperty CreateAndBuildExtensions(string apiName, ApiTypeExpression apiTypeExpression, ApiTypeModifiers apiTypeModifiers, string clrName, ClrMemberKind clrMemberKind)
    {
        var apiProperty = new ApiProperty(apiName, apiTypeExpression, apiTypeModifiers, clrName, clrMemberKind);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiProperty.Extensions = extensions;
        }

        return apiProperty;
    }
    #endregion
}
