// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema.Configuration;

public partial class ApiSchemaBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private class BuildTestCore : XUnitTest
    {
        #region Fields
        protected static readonly JsonSerializerOptions _defaultToJsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
        };
        #endregion

        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        #endregion

        #region Calculated Properties
        protected ApiSchema? ApiSchemaExpected { get; set; }
        protected ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region Constructors
        [SetsRequiredMembers]
        public BuildTestCore()
        {
            this.Name = nameof(BuildTestCore);
            this.ExcludeMembers = ApiSchemaExcludeMembers.SchemaInitialized;
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind) ?? throw new InvalidOperationException($"{nameof(ApiSchema)} creation failed.");
            var apiSchemaExpected = apiSchema.DeepCopy()!;

            var apiExtensionTypes = this.GetExtensionTypes().SafeToList();
            var apiExtensionTypesCount = apiExtensionTypes.Count;
            if (apiExtensionTypesCount > 0)
            {
                apiSchemaExpected.Extensions ??= [];

                foreach (var apiExtensionType in apiExtensionTypes)
                {
                    var extensionInstance = Activator.CreateInstance(apiExtensionType);
                    apiSchemaExpected.Extensions[apiExtensionType] = extensionInstance!;
                }
            }

            this.ApiSchemaExpected = apiSchemaExpected;

            this.WriteLine($"ApiSchemaKind: {this.ApiSchemaKind.SafeToString()}");
            this.WriteLine();
            this.WriteLine("ApiSchemaExpected:");
            this.WriteLine($"{this.ApiSchemaExpected.SafeToJson(_defaultToJsonOptions)}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();
            this.AssertBeEquivalentTo(this.ApiSchemaActual, this.ApiSchemaExpected);
        }
        #endregion

        #region BuildTestCore Methods
        protected virtual IEnumerable<Type> GetExtensionTypes() => [];
        #endregion
    }

    private class BuildTest : BuildTestCore
    {
        #region Constructors
        [SetsRequiredMembers]
        public BuildTest()
        {
            this.Name = nameof(BuildTest);
        }
        #endregion

        #region XUnitTest Methods
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
                    x.ConfigureIdentities(apiObjectType);
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

                            x.WithPrincipalEnd(clrPrincipalType, p => p.ConfigureExtensions(apiPrincipalEnd));
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

                            x.WithPrincipalEnd(clrPrincipalType, p => p.ConfigureExtensions(apiPrincipalEnd));
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

                        x.WithPrincipalEndA(clrPrincipalTypeA, p => p.ConfigureExtensions(apiPrincipalEndA));
                        x.WithPrincipalEndB(clrPrincipalTypeB, p => p.ConfigureExtensions(apiPrincipalEndB));
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
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = $"Build '{ApiSchemaKind.Simple}' API schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
        },

        new BuildTest
        {
            Name = $"Build '{ApiSchemaKind.Key}' API schema",
            ApiSchemaKind = ApiSchemaKind.Key,
        },

        new BuildTest
        {
            Name = $"Build '{ApiSchemaKind.Relationship}' API schema",
            ApiSchemaKind = ApiSchemaKind.Relationship,
        },

        new BuildTest
        {
            Name = $"Build '{ApiSchemaKind.Commerce}' API schema",
            ApiSchemaKind = ApiSchemaKind.Commerce,
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}
