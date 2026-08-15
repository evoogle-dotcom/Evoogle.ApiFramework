// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.TestData;

/// <summary>Builds the actual <see cref="ApiSchema"/> by using the fluent API of <see cref="ApiSchemaBuilder"/> to build an API schema of a caller-supplied <see cref="ApiSchemaKind"/> and then compares it to the expected <see cref="ApiSchema"/> built by <see cref="ApiSchemaKindExpressionBuildTest.BuildApiSchemaExpected(ApiSchemaKind, object[])"/>.</summary>
public class ApiSchemaKindFluentRoundTripBuildTest : ApiSchemaBuildTestBase
{
    #region User Supplied Properties
    public required ApiSchemaKind ApiSchemaKind { get; init; }
    #endregion

    #region Constructors
    public ApiSchemaKindFluentRoundTripBuildTest() => this.Name = nameof(ApiSchemaKindFluentRoundTripBuildTest);
    #endregion

    #region XUnitTest Methods
    protected override void Arrange()
    {
        this.ApiSchemaExpected = ApiSchemaKindExpressionBuildTest.BuildApiSchemaExpected(this.ApiSchemaKind, []);

        this.WriteLine($"ApiSchemaKind: {this.ApiSchemaKind.SafeToString()}");
        this.WriteLine();
        this.WriteLine("ApiSchemaExpected:");
        this.WriteLine($"{this.ApiSchemaExpected.SafeToJson(_defaultToJsonOptions)}");
        this.WriteLine();
    }

    protected override void Act()
    {
        var apiSchemaName = this.ApiSchemaExpected!.ApiName;
        var builder = new ApiSchemaBuilder()
            .WithName(apiSchemaName);

        var apiSchemaVersion = this.ApiSchemaExpected!.ApiVersion;
        if (apiSchemaVersion != null)
        {
            builder = builder.WithVersion(apiSchemaVersion);
        }

        var apiScalarTypes = this.ApiSchemaExpected!.ApiScalarTypes;
        foreach (var apiScalarType in apiScalarTypes)
        {
            var apiName = apiScalarType.ApiName;
            var clrType = apiScalarType.ClrType;
            builder = builder.AddScalar(clrType, x =>
            {
                x.WithName(apiName);
                x.ConfigureExtensions(apiScalarType);
            });
        }

        var apiEnumTypes = this.ApiSchemaExpected!.ApiEnumTypes;
        foreach (var apiEnumType in apiEnumTypes)
        {
            var apiName = apiEnumType.ApiName;
            var clrType = apiEnumType.ClrType;
            builder = builder.AddEnum(clrType, x =>
            {
                x.WithName(apiName);
                foreach (var apiEnumValue in apiEnumType.ApiEnumValues)
                {
                    x.AddValue(apiEnumValue.ApiName, apiEnumValue.ClrName, apiEnumValue.ClrOrdinal);
                }
                x.ConfigureExtensions(apiEnumType);
            });
        }

        var apiObjectTypes = this.ApiSchemaExpected!.ApiObjectTypes;
        foreach (var apiObjectType in apiObjectTypes)
        {
            var apiName = apiObjectType.ApiName;
            var clrType = apiObjectType.ClrType;
            builder = builder.AddObject(clrType, x =>
            {
                x.WithName(apiName);
                foreach (var apiProperty in apiObjectType.ApiProperties.SafeCast<ApiProperty>())
                {
                    var apiPropertyName = apiProperty.ApiName;
                    var clrPropertyName = apiProperty.ClrName;
                    x.AddProperty(apiPropertyName, clrPropertyName, p => p.ConfigureExtensions(apiProperty));
                }

                x.ConfigureOptions(apiObjectType);
                x.ConfigureKeyTypes(apiObjectType);
                x.ConfigureExtensions(apiObjectType);
            });
        }

        var apiRelationships = this.ApiSchemaExpected!.ApiRelationships;
        foreach (var apiRelationship in apiRelationships.SafeCast<ApiRelationship>())
        {
            var apiName = apiRelationship.ApiName;
            var apiKind = apiRelationship.ApiKind;
            var apiDeleteBehavior = apiRelationship.ApiDeleteBehavior;

            builder = apiKind switch
            {
                ApiRelationshipKind.OneToOne => builder.AddOneToOneRelationship(apiName, x =>
                    {
                        var apiRelationshipOneToOne = (ApiRelationshipOneToOne)apiRelationship;
                        var apiPrincipalEnd = apiRelationshipOneToOne.ApiPrincipalEnd;
                        var apiDependentEnd = apiRelationshipOneToOne.ApiDependentEnd;
                        var clrPrincipalType = apiPrincipalEnd.ClrObjectType;

                        x.From(clrPrincipalType, p => p.ConfigureExtensions(apiPrincipalEnd));
                        x.ConfigureDependentEnd(apiDependentEnd);
                        x.WithDeleteBehavior(apiDeleteBehavior);
                        x.ConfigureExtensions(apiRelationshipOneToOne);
                    }),
                ApiRelationshipKind.OneToMany => builder.AddOneToManyRelationship(apiName, x =>
                    {
                        var apiRelationshipOneToMany = (ApiRelationshipOneToMany)apiRelationship;
                        var apiPrincipalEnd = apiRelationshipOneToMany.ApiPrincipalEnd;
                        var apiDependentEnd = apiRelationshipOneToMany.ApiDependentEnd;
                        var clrPrincipalType = apiPrincipalEnd.ClrObjectType;

                        x.From(clrPrincipalType, p => p.ConfigureExtensions(apiPrincipalEnd));
                        x.ConfigureDependentEnd(apiDependentEnd);
                        x.WithDeleteBehavior(apiDeleteBehavior);
                        x.ConfigureExtensions(apiRelationshipOneToMany);
                    }),
                ApiRelationshipKind.ManyToMany => builder.AddManyToManyRelationship(apiName, x =>
                {
                    var apiRelationshipManyToMany = (ApiRelationshipManyToMany)apiRelationship;
                    var apiPrincipalEndA = apiRelationshipManyToMany.ApiPrincipalEndA;
                    var apiPrincipalEndB = apiRelationshipManyToMany.ApiPrincipalEndB;
                    var apiAssociation = apiRelationshipManyToMany.ApiAssociation;
                    var clrPrincipalTypeA = apiPrincipalEndA.ClrObjectType;
                    var clrPrincipalTypeB = apiPrincipalEndB.ClrObjectType;

                    x.Between(clrPrincipalTypeA, p => p.ConfigureExtensions(apiPrincipalEndA));
                    x.And(clrPrincipalTypeB, p => p.ConfigureExtensions(apiPrincipalEndB));
                    x.ConfigureAssociation(apiAssociation);
                    x.WithDeleteBehavior(apiDeleteBehavior);
                    x.ConfigureExtensions(apiRelationshipManyToMany);
                }),
                _ => throw new InvalidOperationException($"Unsupported {nameof(ApiRelationshipKind)}: {apiKind.SafeToString()}"),
            };
        }

        builder.ConfigureExtensions(this.ApiSchemaExpected!);

        this.ApiSchemaActual = builder.Build();
        this.WriteLine("ApiSchemaActual:");
        this.WriteLine($"{this.ApiSchemaActual.SafeToJson(_defaultToJsonOptions)}");
    }
    #endregion
}
