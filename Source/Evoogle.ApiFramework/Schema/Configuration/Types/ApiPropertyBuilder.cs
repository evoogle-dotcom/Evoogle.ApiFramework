// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Types.Internal;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Extensions;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Types;

/// <summary>
///     Builds <see cref="ApiProperty"/> definitions from CLR property/field metadata and optional nullability overrides.
/// </summary>
public class ApiPropertyBuilder : ExtensionBuilder<ApiPropertyBuilder>
{
    #region Fields
    private readonly ApiPropertyState _state;
    private readonly string _clrName;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of <see cref="ApiPropertyBuilder"/> with an explicit API name
    ///     and the CLR property or field name.
    /// </summary>
    /// <param name="apiName">The API name of the property.</param>
    /// <param name="clrName">The CLR property or field name.</param>
    public ApiPropertyBuilder(string apiName, string clrName)
        : this(apiName, clrName, ApiConfigurationSource.Explicit, false)
    {
    }

    internal ApiPropertyBuilder
    (
        string apiName,
        string clrName,
        ApiConfigurationSource apiNameSource,
        bool isConventionDiscovered
    )
    {
        _state = new ApiPropertyState(ValidateName(apiName, nameof(apiName)), apiNameSource);
        _clrName = ValidateName(clrName, nameof(clrName));
        this.IsConventionDiscovered = isConventionDiscovered;
    }
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the API name currently configured for the property.
    /// </summary>
    internal string ApiName => _state.ApiName;

    /// <summary>
    ///     Gets the CLR property or field name this builder represents.
    /// </summary>
    internal string ClrName => _clrName;

    internal bool IsConventionDiscovered { get; }

    internal bool IsConventionOnly => this.IsConventionDiscovered &&
        _state.ApiNameSource == ApiConfigurationSource.Convention &&
        _state.NullabilitySource == ApiConfigurationSource.Convention &&
        this.BuildExtensions() is null;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiPropertyBuilder AddPropertyExtension(Type extensionType, object extension) => this.AddExtension(extensionType, extension);
    #endregion

    #region With Methods
    /// <summary>
    ///     Configures nullability for the property at explicit precedence.
    /// </summary>
    /// <param name="apiNullability">The property value nullability.</param>
    /// <returns>The current builder instance.</returns>
    public ApiPropertyBuilder WithNullability(ApiNullability apiNullability)
        => this.SetNullability(apiNullability, ApiConfigurationSource.Explicit);

    /// <summary>
    ///     Sets the API name for the property at explicit precedence.
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
    ///     Sets the API name at convention precedence.
    ///     Has no effect if a higher-precedence value has already been applied.
    /// </summary>
    internal ApiPropertyBuilder SetApiNameConvention(string apiName)
        => this.SetApiName(apiName, ApiConfigurationSource.Convention);

    /// <summary>
    ///     Sets the API name at data-annotation precedence.
    ///     Has no effect if an explicit value has already been applied.
    /// </summary>
    internal ApiPropertyBuilder SetApiNameDataAnnotation(string apiName)
        => this.SetApiName(apiName, ApiConfigurationSource.DataAnnotation);

    /// <summary>
    ///     Sets nullability at convention precedence.
    ///     Has no effect if a higher-precedence value has already been applied.
    /// </summary>
    internal ApiPropertyBuilder SetNullabilityConvention(ApiNullability apiNullability)
        => this.SetNullability(apiNullability, ApiConfigurationSource.Convention);

    /// <summary>
    ///     Sets nullability at data-annotation precedence.
    ///     Has no effect if an explicit value has already been applied.
    /// </summary>
    internal ApiPropertyBuilder SetNullabilityDataAnnotation(ApiNullability apiNullability)
        => this.SetNullability(apiNullability, ApiConfigurationSource.DataAnnotation);
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

        throw new ApiSchemaConfigurationException
        (
            $"Cannot build {nameof(ApiProperty)} '{_state.ApiName}' because CLR type " +
            $"'{clrObjectType.SafeToName()}' has no public property or field named '{_clrName}'."
        );
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

    private ApiNullability BuildNullability(ApiNullability inferredNullability)
        => _state.ApiNullability ?? inferredNullability;

    private ApiPropertyBuilder SetApiName(string apiName, ApiConfigurationSource source)
    {
        if (source >= _state.ApiNameSource)
        {
            _state.ApiName = apiName;
            _state.ApiNameSource = source;
        }

        return this;
    }

    private ApiPropertyBuilder SetNullability(ApiNullability apiNullability, ApiConfigurationSource source)
    {
        if (source >= _state.NullabilitySource)
        {
            _state.ApiNullability = apiNullability;
            _state.NullabilitySource = source;
        }

        return this;
    }

    private ApiProperty BuildFromNullabilityInfo(MemberNullableInfo clrNullabilityInfo, ClrMemberKind clrMemberKind)
    {
        var apiTypeExpression = ApiTypeExpressionBuilder.Build(clrNullabilityInfo);
        var apiInitialNullability = clrNullabilityInfo.Nullability == MemberNullability.NonNullable ? ApiNullability.NonNullable : ApiNullability.Nullable;
        var apiNullability = this.BuildNullability(apiInitialNullability);

        return this.CreateAndBuildExtensions(_state.ApiName, apiTypeExpression, apiNullability, _clrName, clrMemberKind);
    }

    private ApiProperty CreateAndBuildExtensions(string apiName, ApiTypeExpression apiTypeExpression, ApiNullability apiNullability, string clrName, ClrMemberKind clrMemberKind)
    {
        var clrValueMember = new ClrMemberReference(clrMemberKind, clrName);
        var apiProperty = new ApiProperty(apiName, apiTypeExpression, apiNullability, clrValueMember);

        var extensions = this.BuildExtensions();
        if (extensions is not null)
        {
            apiProperty.AttachExtensions(extensions);
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
