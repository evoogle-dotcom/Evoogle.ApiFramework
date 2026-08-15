// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;

using Evoogle.ApiFramework.Schema.Configuration;

using static Evoogle.ApiFramework.Schema.Annotations.ApiAnnotationTests;

namespace Evoogle.ApiFramework.Schema.Annotations;

[DynamicLinqType]
public static class ApiAnnotationTestsFactory
{
    #region Camel Case Naming Factory Methods
    public static ApiSchema BuildWithApiObjectTypeAttributeOverridesApiName()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonAnnotated>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Email))
            .UseDefaultAnnotations()
            .Build();

        return apiSchema;
    }
    #endregion
}
