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
    private readonly string _apiName = apiName;
    private readonly string _clrName = clrName;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets or sets a delegate that configures additional type modifiers for the property.
    /// </summary>
    internal Action<ApiTypeModifiersBuilder>? Modifiers { get; set; }
    #endregion

    #region Methods
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
        return this.BuildFromNullabilityInfo(clrPropertyNullabilityInfo);
    }

    private ApiProperty BuildFromFieldInfo(FieldInfo clrFieldInfo)
    {
        var clrFieldNullabilityInfo = FieldReflection.GetNullabilityInfo(clrFieldInfo);
        return this.BuildFromNullabilityInfo(clrFieldNullabilityInfo);
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

    private ApiProperty BuildFromNullabilityInfo(MemberNullableInfo clrNullabilityInfo)
    {
        var apiTypeExpression = ApiTypeExpressionBuilder.Build(clrNullabilityInfo);
        var apiInitialTypeModifiers = clrNullabilityInfo.IsNullable ? ApiTypeModifiers.None : ApiTypeModifiers.Required;
        var apiTypeModifiers = this.BuildModifiers(apiInitialTypeModifiers);

        return this.CreateAndBuildExtensions(_apiName, apiTypeExpression, apiTypeModifiers, _clrName);
    }

    private ApiProperty BuildUnknownProperty()
    {
        var apiTypeModifiers = this.BuildModifiers(ApiTypeModifiers.None);

        return this.CreateAndBuildExtensions(_apiName, default!, apiTypeModifiers, _clrName);
    }

    private ApiProperty CreateAndBuildExtensions(string apiName, ApiTypeExpression apiTypeExpression, ApiTypeModifiers apiTypeModifiers, string clrName)
    {
        var apiProperty = new ApiProperty(apiName, apiTypeExpression, apiTypeModifiers, clrName);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiProperty.Extensions = extensions;
        }

        return apiProperty;
    }
    #endregion
}
