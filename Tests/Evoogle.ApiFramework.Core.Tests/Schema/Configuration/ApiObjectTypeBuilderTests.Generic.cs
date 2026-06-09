// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;
using Evoogle.XUnit.Json;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public partial class ApiObjectTypeBuilderTests
{
    #region Test Classes
    private class BuildTestGeneric : BuildTestCore
    {
        #region User Supplied Properties
        [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiObjectType>))]
        public required Expression<Func<ApiObjectType>> ApiTypeActualBuildExpression { get; init; } = null!;
        #endregion

        #region Constructors
        [SetsRequiredMembers]
        public BuildTestGeneric()
        {
            this.Name = nameof(BuildTestGeneric);
        }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            var apiTypeActualBuildLambda = this.ApiTypeActualBuildExpression.Compile();
            var apiTypeActual = apiTypeActualBuildLambda();

            this.ApiTypeActual = apiTypeActual;
            this.WriteLine($"ApiTypeActual:   {this.ApiTypeActual.SafeToString()}");
        }
        #endregion
    }

    [method: SetsRequiredMembers]
    private class BuildTestGeneric<TExtension>() : BuildTestGeneric
    {
        #region BuildTestCore Methods
        protected override IEnumerable<Type> GetExtensionTypes()
        {
            yield return typeof(TExtension);
        }
        #endregion
    }

    [method: SetsRequiredMembers]
    private class BuildTestGeneric<TExtension1, TExtension2>() : BuildTestGeneric
    {
        #region BuildTestCore Methods
        protected override IEnumerable<Type> GetExtensionTypes()
        {
            yield return typeof(TExtension1);
            yield return typeof(TExtension2);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildGenericTheoryData =>
    [
        // Simple API Schema
        // - Empty
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(Empty)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Empty),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromSimpleApiSchemaTheEmptyType(),
        },
        new BuildTestGeneric<GraphQlExtension>
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(Empty)}' API object type with '{nameof(GraphQlExtension)}'",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Empty),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromSimpleApiSchemaTheEmptyType<GraphQlExtension>(),
        },

        // Simple API Schema
        // - ScalarsOnly
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(ScalarsOnly)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromSimpleApiSchemaTheScalarsOnlyType(),
        },
        new BuildTestGeneric<GraphQlExtension>
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(ScalarsOnly)}' API object type with '{nameof(GraphQlExtension)}'",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromSimpleApiSchemaTheScalarsOnlyType<GraphQlExtension>()
        },
        new BuildTestGeneric<JsonApiExtension>
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(ScalarsOnly)}' API object type with '{nameof(JsonApiExtension)}'",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromSimpleApiSchemaTheScalarsOnlyType<JsonApiExtension>()
        },
        new BuildTestGeneric<GraphQlExtension, JsonApiExtension>
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(ScalarsOnly)}' API object type with '{nameof(GraphQlExtension)}' and '{nameof(JsonApiExtension)}'",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromSimpleApiSchemaTheScalarsOnlyType<GraphQlExtension, JsonApiExtension>()
        },
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(ScalarsOnly)}' API object type with explicitly add required/optional properties",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromSimpleApiSchemaTheScalarsOnlyTypeWithAddRequiredOrOptionalProperty()
        },

        // Simple API Schema
        // - Company
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(Company)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromSimpleApiSchemaTheCompanyType(),
        },
        new BuildTestGeneric<GraphQlExtension>
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(Company)}' API object type with '{nameof(GraphQlExtension)}'",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromSimpleApiSchemaTheCompanyType<GraphQlExtension>(),
        },
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Simple}' API schema the '{nameof(Company)}' API object type with explicitly configuring required/optional properties",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromSimpleApiSchemaTheCompanyTypeWithConfigureRequiredOrOptionalProperty(),
        },

        // Key API Schema
        // - KeyOneScalarPart
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Key}' API schema the '{nameof(KeyOneScalarPart)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromKeyApiSchemaTheKeyOneScalarPartType(),
        },

        // Key API Schema
        // - KeyTwoScalarPartComposite
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Key}' API schema the '{nameof(KeyTwoScalarPartComposite)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromKeyApiSchemaTheKeyTwoScalarPartCompositeType(),
        },

        // Key API Schema
        // - KeyThreeScalarPartComposite
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Key}' API schema the '{nameof(KeyThreeScalarPartComposite)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyThreeScalarPartComposite),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromKeyApiSchemaTheKeyThreeScalarPartCompositeType(),
        },

        // Key API Schema
        // - KeyNestedComposite
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Key}' API schema the '{nameof(KeyNestedComposite)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyNestedComposite),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromKeyApiSchemaTheKeyNestedCompositeType(),
        },

        // Key API Schema
        // - KeyOwnedComposite
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Key}' API schema the '{nameof(KeyOwnedComposite)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyOwnedComposite),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromKeyApiSchemaTheKeyOwnedCompositeType(),
        },

        // Key API Schema
        // - KeyOwnedDependent
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Key}' API schema the '{nameof(KeyOwnedDependent)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Key,
            ApiObjectTypeName = nameof(KeyOwnedDependent),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromKeyApiSchemaTheKeyOwnedDependentType(),
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildGenericTheoryData))]
    public void BuildGeneric(IXUnitTest test) => test.Execute(this);

    [Fact]
    public void AddKeyGenericSupportsFourExpressionParts()
    {
        var ctx = new ApiSchemaBuilderContext();
        var apiObjectType = new ApiObjectTypeBuilder<RelationshipOrderLine>(ctx)
            .WithName(nameof(RelationshipOrderLine))
            .AddProperty(p => p.OrderId)
            .AddProperty(p => p.LineNumber)
            .AddProperty(p => p.ProductSku)
            .AddProperty(p => p.ProductRevision)
            .AddKey
            (
                "PK_RelationshipOrderLine_FourPart",
                p => p.OrderId,
                p => p.LineNumber,
                p => p.ProductSku,
                p => p.ProductRevision
            )
            .Build();

        apiObjectType.ApiKeyTypes["PK_RelationshipOrderLine_FourPart"].ApiKeyPaths.Should().HaveCount(4);
    }
    #endregion
}
