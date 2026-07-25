// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;

using static Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

[DynamicLinqType]
public static class ApiConventionTestsFactory
{
    #region Camel Case Naming Factory Methods
    public static ApiSchema BuildWithCamelCaseNamingExpressionInferredPropertyNames()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Email))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithCamelCaseNamingRequiredAndOptionalExpressionPropertyNames()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddRequiredProperty(p => p.Id)
                .AddRequiredProperty(p => p.Name)
                .AddOptionalProperty(p => p.Email))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithCamelCaseNamingPreservesSelectorExplicitApiName()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Email, "EmailAddress"))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithCamelCaseNamingPreservesCallbackExplicitApiName()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Email, p => p.WithName("EmailAddress")))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithCamelCaseNamingPreservesStringBasedExplicitApiNames()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .WithName("Person")
                .AddProperty("Id")
                .AddProperty("Name")
                .AddProperty("EmailAddress", "Email"))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithCamelCaseNamingForEnumTypeAndValues()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<CustomEnum>(x => x
                .AddValue(CustomEnum.Active))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }
    #endregion

    #region Property Discovery Factory Methods
    public static ApiSchema BuildWithPropertyDiscoveryDiscoversPublicInstanceProperties()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>()
            .UsePropertyDiscovery()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithPropertyDiscoveryDiscoversPublicFields()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<TypeWithField>()
            .UsePropertyDiscovery()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithPropertyDiscoveryDoesNotDuplicateExplicitlyAddedProperties()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddProperty("identifier", "Id")
                .AddProperty("displayName", "Name"))
            .UsePropertyDiscovery()
            .Build();

        return apiSchema;
    }
    #endregion
}
