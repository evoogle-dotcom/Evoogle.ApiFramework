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
public class ApiPropertyBuilder : ExtensionBuilder<ApiPropertyBuilder>
{
    #region Fields
    private string _apiName;
    private ApiConfigurationSource _apiNameSource;
    private readonly string _clrName;
    private Action<ApiTypeModifiersBuilder>? _modifiers;
    private ApiConfigurationSource? _modifiersSource;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of <see cref="ApiPropertyBuilder"/> with an explicit API name
    ///     and the CLR property or field name.
    /// </summary>
    /// <param name="apiName">The API name of the property.</param>
    /// <param name="clrName">The CLR property or field name.</param>
    public ApiPropertyBuilder(string apiName, string clrName)
        : this(apiName, clrName, ApiConfigurationSource.Explicit)
    {
    }

    internal ApiPropertyBuilder(string apiName, string clrName, ApiConfigurationSource apiNameSource)
    {
        _apiName = ValidateName(apiName, nameof(apiName));
        _clrName = ValidateName(clrName, nameof(clrName));
        _apiNameSource = apiNameSource;
    }
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the API name currently configured for the property.
    /// </summary>
    internal string ApiName => _apiName;

    /// <summary>
    ///     Gets the CLR property or field name this builder represents.
    /// </summary>
    internal string ClrName => _clrName;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiPropertyBuilder AddPropertyExtension(Type extensionType, object extension)
    {
        return this.AddExtension(extensionType, extension);
    }
    #endregion

    #region With Methods
    /// <summary>
    ///     Configures type modifiers for the property at <see cref="ApiConfigurationSource.Explicit"/> precedence.
    /// </summary>
    /// <param name="configure">Callback to configure type modifiers.</param>
    /// <returns>The current builder instance.</returns>
    public ApiPropertyBuilder WithModifiers(Action<ApiTypeModifiersBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        return this.SetModifiers(configure, ApiConfigurationSource.Explicit);
    }

    /// <summary>
    ///     Sets the API name for the property at <see cref="ApiConfigurationSource.Explicit"/> precedence.
    /// </summary>
    /// <param name="apiName">The API name to use.</param>
    /// <returns>The current builder instance.</returns>
    public ApiPropertyBuilder WithName(string apiName)
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
    internal ApiPropertyBuilder SetApiNameConvention(string apiName)
        => this.SetApiName(apiName, ApiConfigurationSource.Convention);

    /// <summary>
    ///     Sets the API name at <see cref="ApiConfigurationSource.DataAnnotation"/> precedence.
    ///     Has no effect if an explicit value has already been applied.
    /// </summary>
    internal ApiPropertyBuilder SetApiNameDataAnnotation(string apiName)
        => this.SetApiName(apiName, ApiConfigurationSource.DataAnnotation);

    /// <summary>
    ///     Sets the type modifier delegate at <see cref="ApiConfigurationSource.Convention"/> precedence.
    ///     Has no effect if a higher-precedence value has already been applied.
    /// </summary>
    internal ApiPropertyBuilder SetModifiersConvention(Action<ApiTypeModifiersBuilder> configure)
        => this.SetModifiers(configure, ApiConfigurationSource.Convention);

    /// <summary>
    ///     Sets the type modifier delegate at <see cref="ApiConfigurationSource.DataAnnotation"/> precedence.
    ///     Has no effect if an explicit value has already been applied.
    /// </summary>
    internal ApiPropertyBuilder SetModifiersDataAnnotation(Action<ApiTypeModifiersBuilder> configure)
        => this.SetModifiers(configure, ApiConfigurationSource.DataAnnotation);
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

    private ApiTypeModifiers BuildModifiers(ApiTypeModifiers apiNullabilityModifiers)
    {
        // No tier has set modifiers: fall back to the nullability-derived default.
        if (_modifiers == null)
        {
            return apiNullabilityModifiers;
        }

        // A tier set modifiers; start from None so the stored delegate has full control.
        var modifierBuilder = new ApiTypeModifiersBuilder(ApiTypeModifiers.None);
        _modifiers.Invoke(modifierBuilder);
        return modifierBuilder.Build();
    }

    private ApiPropertyBuilder SetApiName(string apiName, ApiConfigurationSource source)
    {
        if (source >= _apiNameSource)
        {
            _apiName = apiName;
            _apiNameSource = source;
        }

        return this;
    }

    private ApiPropertyBuilder SetModifiers(Action<ApiTypeModifiersBuilder> configure, ApiConfigurationSource source)
    {
        if (_modifiersSource == null || source >= _modifiersSource.Value)
        {
            _modifiers = configure;
            _modifiersSource = source;
        }

        return this;
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

    private static string ValidateName(string name, string paramName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, paramName);
        return name;
    }
    #endregion
}
