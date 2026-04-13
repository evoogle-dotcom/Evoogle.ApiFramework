// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiObjectTypeBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required string ApiObjectTypeName { get; init; } = null!;
        public Type? ApiExtensionType1 { get; init; }
        public Type? ApiExtensionType2 { get; init; }
        #endregion

        #region Calculated Properties
        private ApiType? ApiTypeExpected { get; set; }
        private ApiType? ApiTypeActual { get; set; }
        #endregion

        #region Constructors
        [SetsRequiredMembers]
        public BuildTest()
        {
            this.Name = nameof(BuildTest);
            this.ExcludeMembers = ApiSchemaExcludeMembers.Standard;
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind) ?? throw new InvalidOperationException($"{nameof(ApiSchema)} creation failed.");
            var apiType = apiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName) as ApiType ?? throw new InvalidOperationException($"{nameof(ApiObjectType)} '{this.ApiObjectTypeName}' not found in ApiSchema.");

            var apiTypeExpected = (ApiObjectType)apiType.DeepCopy()!; // Needs to be ApiType here to allow deep copy by JSON serialization/deserialization to work properly

            if (this.ApiExtensionType1 != null)
            {
                var extensionInstance = Activator.CreateInstance(this.ApiExtensionType1);
                apiTypeExpected.Extensions ??= [];
                apiTypeExpected.Extensions[this.ApiExtensionType1] = extensionInstance!;
            }

            if (this.ApiExtensionType2 != null)
            {
                var extensionInstance = Activator.CreateInstance(this.ApiExtensionType2);
                apiTypeExpected.Extensions ??= [];
                apiTypeExpected.Extensions[this.ApiExtensionType2] = extensionInstance!;
            }

            this.ApiTypeExpected = apiTypeExpected;

            this.WriteLine($"ApiSchema:     {apiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType: {apiTypeExpected.ApiName.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.ApiTypeExpected.SafeToString()}");
        }

        protected override void Act()
        {
            var apiObjectType = (ApiObjectType)this.ApiTypeExpected!;

            var apiName = apiObjectType.ApiName;
            var clrType = apiObjectType.ClrType;

            var context = new ApiSchemaBuilderContext();
            var builder = new ApiObjectTypeBuilder(clrType, context)
                .WithName(apiName);

            var apiIdentities = apiObjectType.ApiIdentities;
            foreach (var apiIdentity in apiIdentities ?? [])
            {
                builder.AddIdentity(apiIdentity.ApiName, identityBuilder =>
                {
                    foreach (var apiIdentityPart in apiIdentity.ApiIdentityParts)
                    {
                        var apiKind = apiIdentityPart.ApiKind;
                        switch (apiKind)
                        {
                            case ApiIdentityPartKind.Scalar:
                                {
                                    var scalarPart = (ApiIdentityScalarPart)apiIdentityPart;
                                    var clrScalarTypeHint = scalarPart.ClrScalarTypeHint;
                                    if (clrScalarTypeHint is not null)
                                    {
                                        identityBuilder.AddScalar(scalarPart.ClrPropertyName, clrScalarTypeHint);
                                    }
                                    else
                                    {
                                        identityBuilder.AddScalar(scalarPart.ClrPropertyName);
                                    }
                                    break;
                                }

                            case ApiIdentityPartKind.Nested:
                                {
                                    var nestedPart = (ApiIdentityNestedPart)apiIdentityPart;
                                    var apiIdentityName = nestedPart.ApiIdentityName;
                                    if (apiIdentityName is not null)
                                    {
                                        identityBuilder.AddNested(nestedPart.ClrPropertyName, apiIdentityName);
                                    }
                                    else
                                    {
                                        identityBuilder.AddNested(nestedPart.ClrPropertyName);
                                    }
                                    break;
                                }

                            case ApiIdentityPartKind.Owner:
                                {
                                    var ownerPart = (ApiIdentityOwnerPart)apiIdentityPart;
                                    var apiIdentityName = ownerPart.ApiIdentityName;
                                    if (apiIdentityName is not null)
                                    {
                                        identityBuilder.AddOwner(apiIdentityName);
                                    }
                                    else
                                    {
                                        identityBuilder.AddOwner(ownerPart.ApiIdentityName!);
                                    }
                                    break;
                                }

                            default:
                                throw new InvalidOperationException($"Unsupported API identity part kind: {apiKind}");
                        }
                    }
                });
            }

            var apiProperties = apiObjectType.ApiProperties;
            foreach (var apiProperty in apiProperties ?? [])
            {
                builder.AddProperty(apiProperty.ApiName, apiProperty.ClrName);
            }

            var apiOptions = apiObjectType.ApiOptions;
            if (apiOptions is not null)
            {
                builder.WithOptions(optionsBuilder =>
                {
                    if (apiOptions.ApiIdentityNullHandling.HasValue)
                    {
                        optionsBuilder.WithIdentityNullHandling(apiOptions.ApiIdentityNullHandling.Value);
                    }
                });
            }

            var extensions = apiObjectType.Extensions;
            foreach (var extension in extensions ?? [])
            {
                builder.AddObjectExtension(extension.Key, extension.Value);
            }

            this.ApiTypeActual = builder.Build();
            this.WriteLine($"Actual:   {this.ApiTypeActual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ApiTypeActual.Should().NotBeNull();

            this.ApiTypeActual.Should().BeOfType<ApiObjectType>();
            this.ApiTypeExpected.Should().BeOfType<ApiObjectType>();

            this.AssertBeEquivalentTo(this.ApiTypeActual, this.ApiTypeExpected);
        }
        #endregion
    }
    #endregion

    private class AddIdentityNullConfigureTest : XUnitTest
    {
        #region User Supplied Properties
        public required string IdentityApiName { get; init; }
        #endregion

        #region Calculated Properties
        private ApiObjectType? Actual { get; set; }
        #endregion

        #region Constructors
        public AddIdentityNullConfigureTest()
        {
            this.Name = nameof(AddIdentityNullConfigureTest);
            this.ExcludeMembers = ApiSchemaExcludeMembers.Standard;
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"IdentityApiName: {this.IdentityApiName}");
        }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext();
            this.Actual = new ApiObjectTypeBuilder(typeof(Customer), context)
                .WithName(nameof(Customer))
                .AddIdentity(this.IdentityApiName)
                .Build();
            this.WriteLine($"Actual: {this.Actual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.Actual.Should().NotBeNull();
            this.Actual!.ApiIdentities.Should().ContainSingle()
                .Which.ApiName.Should().Be(this.IdentityApiName);
        }
        #endregion
    }

    private class AddPropertySingleNameTest : XUnitTest
    {
        #region User Supplied Properties
        public string PropertyName { get; init; } = null!;
        #endregion

        #region Calculated Properties
        private ApiObjectType? ApiTypeFromSingleName { get; set; }
        private ApiObjectType? ApiTypeFromTwoNames { get; set; }
        #endregion

        #region Constructors
        [SetsRequiredMembers]
        public AddPropertySingleNameTest()
        {
            this.Name = nameof(AddPropertySingleNameTest);
            this.ExcludeMembers = ApiSchemaExcludeMembers.Standard;
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"PropertyName: {this.PropertyName}");
        }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext();

            this.ApiTypeFromSingleName = new ApiObjectTypeBuilder(typeof(ScalarsOnly), context)
                .WithName(nameof(ScalarsOnly))
                .AddProperty(this.PropertyName)
                .Build();

            this.ApiTypeFromTwoNames = new ApiObjectTypeBuilder(typeof(ScalarsOnly), context)
                .WithName(nameof(ScalarsOnly))
                .AddProperty(this.PropertyName, this.PropertyName)
                .Build();

            this.WriteLine($"Single-name result: {this.ApiTypeFromSingleName.SafeToString()}");
            this.WriteLine($"Two-name result:    {this.ApiTypeFromTwoNames.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ApiTypeFromSingleName.Should().NotBeNull();
            this.ApiTypeFromTwoNames.Should().NotBeNull();
            this.AssertBeEquivalentTo(this.ApiTypeFromSingleName, this.ApiTypeFromTwoNames);
        }
        #endregion
    }

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] AddPropertySingleNameTheoryData =>
    [
        new AddPropertySingleNameTest
        {
            Name = "AddProperty(name) is equivalent to AddProperty(name, name) for a required property",
            PropertyName = nameof(ScalarsOnly.RequiredName),
        },
        new AddPropertySingleNameTest
        {
            Name = "AddProperty(name) is equivalent to AddProperty(name, name) for an optional property",
            PropertyName = nameof(ScalarsOnly.OptionalName),
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = $"Builds {nameof(Empty)} from {ApiSchemaKind.Simple} API schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Empty),
        },
        new BuildTest
        {
            Name = $"Builds {nameof(Empty)} with GraphQl extension from {ApiSchemaKind.Simple} API schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Empty),
            ApiExtensionType1 = typeof(GraphQlExtension),
        },

        new BuildTest
        {
            Name = $"Builds {nameof(ScalarsOnly)} from {ApiSchemaKind.Simple} API schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
        },
        new BuildTest
        {
            Name = $"Builds {nameof(ScalarsOnly)} with GraphQl extension from {ApiSchemaKind.Simple} API schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiExtensionType1 = typeof(GraphQlExtension),
        },
        new BuildTest
        {
            Name = $"Builds {nameof(ScalarsOnly)} with JsonApi extension from {ApiSchemaKind.Simple} API schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiExtensionType1 = typeof(JsonApiExtension),
        },
        new BuildTest
        {
            Name = $"Builds {nameof(ScalarsOnly)} with GraphQl and JsonApi extensions from {ApiSchemaKind.Simple} API schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiExtensionType1 = typeof(GraphQlExtension),
            ApiExtensionType2 = typeof(JsonApiExtension),
        },

        new BuildTest
        {
            Name = $"Builds {nameof(Company)} from {ApiSchemaKind.Simple} API schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
        },
        new BuildTest
        {
            Name = $"Builds {nameof(Company)} with GraphQl extension from {ApiSchemaKind.Simple} API schema",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            ApiExtensionType1 = typeof(GraphQlExtension),
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] AddIdentityNullConfigureTheoryData =>
    [
        new AddIdentityNullConfigureTest
        {
            Name = "AddIdentity(name) without configure builds empty identity with correct API name",
            IdentityApiName = "PK_Customer",
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(AddPropertySingleNameTheoryData))]
    public void AddPropertySingleName(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(AddIdentityNullConfigureTheoryData))]
    public void AddIdentityNullConfigure(IXUnitTest test) => test.Execute(this);
    #endregion
}

