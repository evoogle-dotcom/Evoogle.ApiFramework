// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema.Configuration;

public partial class ApiObjectTypeBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private class BuildTestCore : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
        public required string ApiObjectTypeName { get; init; } = null!;
        #endregion

        #region Calculated Properties
        protected ApiType? ApiTypeExpected { get; set; }
        protected ApiType? ApiTypeActual { get; set; }
        #endregion

        #region Constructors
        [SetsRequiredMembers]
        public BuildTestCore()
        {
            this.Name = nameof(BuildTestCore);
            this.ExcludeMembers = ApiSchemaExcludeMembers.Standard;
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind) ?? throw new InvalidOperationException($"{nameof(ApiSchema)} creation failed.");
            var apiType = apiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName) as ApiType ?? throw new InvalidOperationException($"{nameof(ApiObjectType)} '{this.ApiObjectTypeName}' not found in ApiSchema.");

            var apiTypeExpected = (ApiObjectType)apiType.DeepCopy()!; // Needs to be ApiType here to allow deep copy by JSON serialization/deserialization to work properly

            var apiExtensionTypes = this.GetExtensionTypes().SafeToList();
            var apiExtensionTypesCount = apiExtensionTypes.Count;
            if (apiExtensionTypesCount > 0)
            {
                apiTypeExpected.Extensions ??= [];

                foreach (var apiExtensionType in apiExtensionTypes)
                {
                    var extensionInstance = Activator.CreateInstance(apiExtensionType);
                    apiTypeExpected.Extensions[apiExtensionType] = extensionInstance!;
                }
            }

            this.ApiTypeExpected = apiTypeExpected;

            this.WriteLine($"ApiSchema: {apiSchema.ApiName.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"ApiTypeExpected: {this.ApiTypeExpected.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ApiTypeActual.Should().NotBeNull();

            this.ApiTypeActual.Should().BeOfType<ApiObjectType>();
            this.ApiTypeExpected.Should().BeOfType<ApiObjectType>();

            this.AssertBeEquivalentTo(this.ApiTypeActual, this.ApiTypeExpected);
        }
        #endregion

        #region BuildTestCore Methods
        protected virtual IEnumerable<Type> GetExtensionTypes() => [];
        #endregion
    }

    private class BuildTest : BuildTestCore
    {
        #region User Supplied Properties
        public Type? ApiExtensionType1 { get; init; }
        public Type? ApiExtensionType2 { get; init; }
        #endregion

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
            var apiObjectType = (ApiObjectType)this.ApiTypeExpected!;

            var apiName = apiObjectType.ApiName;
            var clrType = apiObjectType.ClrType;

            var context = new ApiSchemaBuilderContext();
            var builder = new ApiObjectTypeBuilder(clrType, context)
                .WithName(apiName);

            var apiProperties = apiObjectType.ApiProperties;
            foreach (var apiProperty in apiProperties ?? [])
            {
                var apiPropertyName = apiProperty.ApiName;
                var clrPropertyName = apiProperty.ClrName;
                builder.AddProperty(apiPropertyName, clrPropertyName);
            }

            builder.ConfigureOptions(apiObjectType);
            builder.ConfigureKeyTypes(apiObjectType);
            builder.ConfigureExtensions(apiObjectType);

            this.ApiTypeActual = builder.Build();
            this.WriteLine($"ApiTypeActual:   {this.ApiTypeActual.SafeToString()}");
        }
        #endregion

        #region BuildTestCore Methods
        protected override IEnumerable<Type> GetExtensionTypes()
        {
            if (this.ApiExtensionType1 != null)
            {
                yield return this.ApiExtensionType1;
            }

            if (this.ApiExtensionType2 != null)
            {
                yield return this.ApiExtensionType2;
            }
        }
        #endregion
    }

    private class BuildWithAddRequiredOrOptionalPropertyTest : BuildTest
    {
        #region Constructors
        [SetsRequiredMembers]
        public BuildWithAddRequiredOrOptionalPropertyTest()
        {
            this.Name = nameof(BuildTest);
        }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            var apiObjectType = (ApiObjectType)this.ApiTypeExpected!;

            var apiName = apiObjectType.ApiName;
            var clrType = apiObjectType.ClrType;

            var context = new ApiSchemaBuilderContext();
            var builder = new ApiObjectTypeBuilder(clrType, context)
                .WithName(apiName);

            var apiProperties = apiObjectType.ApiProperties;
            foreach (var apiProperty in apiProperties ?? [])
            {
                var apiPropertyName = apiProperty.ApiName;
                var clrPropertyName = apiProperty.ClrName;
                var isRequired = apiProperty.IsRequired;

                if (apiPropertyName == clrPropertyName)
                {
                    var name = apiPropertyName;
                    if (isRequired)
                    {
                        builder.AddRequiredProperty(name);
                    }
                    else
                    {
                        builder.AddOptionalProperty(name);
                    }
                }
                else
                {
                    if (isRequired)
                    {
                        builder.AddRequiredProperty(apiPropertyName, clrPropertyName);
                    }
                    else
                    {
                        builder.AddOptionalProperty(apiPropertyName, clrPropertyName);
                    }
                }
            }

            builder.ConfigureOptions(apiObjectType);
            builder.ConfigureKeyTypes(apiObjectType);
            builder.ConfigureExtensions(apiObjectType);

            this.ApiTypeActual = builder.Build();
            this.WriteLine($"ApiTypeActual:   {this.ApiTypeActual.SafeToString()}");
        }
        #endregion
    }

    private class BuildWithConfigureAsRequiredOrOptionalPropertyTest : BuildTest
    {
        #region Constructors
        [SetsRequiredMembers]
        public BuildWithConfigureAsRequiredOrOptionalPropertyTest()
        {
            this.Name = nameof(BuildTest);
        }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            var apiObjectType = (ApiObjectType)this.ApiTypeExpected!;

            var apiName = apiObjectType.ApiName;
            var clrType = apiObjectType.ClrType;

            var context = new ApiSchemaBuilderContext();
            var builder = new ApiObjectTypeBuilder(clrType, context)
                .WithName(apiName);

            var apiProperties = apiObjectType.ApiProperties;
            foreach (var apiProperty in apiProperties ?? [])
            {
                var apiPropertyName = apiProperty.ApiName;
                var clrPropertyName = apiProperty.ClrName;
                var isRequired = apiProperty.IsRequired;

                if (apiPropertyName == clrPropertyName)
                {
                    var name = apiPropertyName;
                    if (isRequired)
                    {
                        builder.AddProperty(name, x => x.AsRequired());
                    }
                    else
                    {
                        builder.AddProperty(name, x => x.AsOptional());
                    }
                }
                else
                {
                    if (isRequired)
                    {
                        builder.AddProperty(apiPropertyName, clrPropertyName, x => x.AsRequired());
                    }
                    else
                    {
                        builder.AddProperty(apiPropertyName, clrPropertyName, x => x.AsOptional());
                    }
                }
            }

            builder.ConfigureOptions(apiObjectType);
            builder.ConfigureKeyTypes(apiObjectType);
            builder.ConfigureExtensions(apiObjectType);

            this.ApiTypeActual = builder.Build();
            this.WriteLine($"ApiTypeActual:   {this.ApiTypeActual.SafeToString()}");
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        // Simple API Schema
        // - Empty
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(Empty)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Empty),
        },
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(Empty)}' API object type with '{nameof(GraphQlExtension)}'",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Empty),
            ApiExtensionType1 = typeof(GraphQlExtension),
        },

        // Simple API Schema
        // - ScalarsOnly
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(ScalarsOnly)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
        },
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(ScalarsOnly)}' API object type with '{nameof(GraphQlExtension)}'",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiExtensionType1 = typeof(GraphQlExtension),
        },
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(ScalarsOnly)}' API object type with '{nameof(JsonApiExtension)}'",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiExtensionType1 = typeof(JsonApiExtension),
        },
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(ScalarsOnly)}' API object type with '{nameof(GraphQlExtension)}' and '{nameof(JsonApiExtension)}'",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiExtensionType1 = typeof(GraphQlExtension),
            ApiExtensionType2 = typeof(JsonApiExtension),
        },
        new BuildWithAddRequiredOrOptionalPropertyTest
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(ScalarsOnly)}' API object type with explicitly add required/optional properties",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
        },

        // Simple API Schema
        // - Company
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(Company)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
        },
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(Company)}' API object type with '{nameof(GraphQlExtension)}'",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            ApiExtensionType1 = typeof(GraphQlExtension),
        },
        new BuildWithConfigureAsRequiredOrOptionalPropertyTest
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(Company)}' API object type with explicitly configuring required/optional properties",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
        },

        // Identity API Schema
        // - IdentityScalar
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Key}' API schema the '{nameof(KeyOneScalarPart)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyOneScalarPart),
        },

        // Identity API Schema
        // - IdentityTwoScalarPartComposite
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Key}' API schema the '{nameof(KeyTwoScalarPartComposite)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
        },

        // Identity API Schema
        // - IdentityThreeScalarPartComposite
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Key}' API schema the '{nameof(KeyThreeScalarPartComposite)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyThreeScalarPartComposite),
        },

        // Identity API Schema
        // - IdentityNestedComposite
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Key}' API schema the '{nameof(KeyNestedComposite)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyNestedComposite),
        },

        // Identity API Schema
        // - IdentityOwnedComposite
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Key}' API schema the '{nameof(KeyOwnedComposite)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyOwnedComposite),
        },

        // Identity API Schema
        // - IdentityOwnedDependent
        new BuildTest
        {
            Name = $"Build from '{ApiSchemaKind.Key}' API schema the '{nameof(KeyOwnedDependent)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyOwnedDependent),
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}

