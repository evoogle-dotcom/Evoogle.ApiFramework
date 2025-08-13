// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.Extensions;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiPropertyBuilder(string apiName, string clrName) : ExtensionBuilder<ApiPropertyBuilder>
{
    #region Fields
    private readonly string _apiName = apiName;
    private readonly string _clrName = clrName;
    #endregion

    #region Properties
    internal Action<ApiTypeModifiersBuilder>? Modifiers { get; set; }
    #endregion

    #region Methods
    internal ApiProperty Build(Type clrObjectType)
    {
        var apiPropertyName = _apiName;
        var clrPropertyName = _clrName;
        var clrObjectTypeName = clrObjectType.SafeToName();

        var clrPropertyInfo = TypeReflection.GetProperty(clrObjectType, clrPropertyName);
        if (clrPropertyInfo == null)
        {
            var apiUnknownTypeExpression = ApiTypeExpressionBuilder.Build();

            var apiUnknownTypeModifiers = ApiTypeModifiers.None;
            if (this.Modifiers != null)
            {
                var modifierBuilder = new ApiTypeModifiersBuilder(apiUnknownTypeModifiers);
                this.Modifiers.Invoke(modifierBuilder);
                apiUnknownTypeModifiers = modifierBuilder.Build();
            }

            return this.CreateAndBuildExtensions(apiPropertyName, apiUnknownTypeExpression, apiUnknownTypeModifiers, clrPropertyName);
        }

        var clrPropertyNullabilityInfo = PropertyReflection.GetNullabilityInfo(clrPropertyInfo);

        var apiTypeExpression = ApiTypeExpressionBuilder.Build(clrPropertyNullabilityInfo);

        var apiTypeModifiers = clrPropertyNullabilityInfo.IsNullable ? ApiTypeModifiers.None : ApiTypeModifiers.Required;
        if (this.Modifiers != null)
        {
            var modifierBuilder = new ApiTypeModifiersBuilder(apiTypeModifiers);
            this.Modifiers.Invoke(modifierBuilder);
            apiTypeModifiers = modifierBuilder.Build();
        }

        return this.CreateAndBuildExtensions(apiPropertyName, apiTypeExpression, apiTypeModifiers, clrPropertyName);
    }

    private ApiProperty CreateAndBuildExtensions(string apiPropertyName, ApiTypeExpression apiTypeExpression, ApiTypeModifiers apiTypeModifiers, string clrPropertyName)
    {
        var apiProperty = new ApiProperty(apiPropertyName, apiTypeExpression, apiTypeModifiers, clrPropertyName);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiProperty.Extensions = extensions;
        }

        return apiProperty;
    }
    #endregion
}
