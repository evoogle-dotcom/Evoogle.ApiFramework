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

        // Identity API Schema
        // - IdentityScalar
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Identity}' API schema the '{nameof(IdentityScalar)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(IdentityScalar),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromIdentityApiSchemaTheIdentityScalarType(),
        },

        // Identity API Schema
        // - IdentityTwoScalarPartComposite
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Identity}' API schema the '{nameof(IdentityTwoScalarPartComposite)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(IdentityTwoScalarPartComposite),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromIdentityApiSchemaTheIdentityTwoScalarPartCompositeType(),
        },

        // Identity API Schema
        // - IdentityThreeScalarPartComposite
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Identity}' API schema the '{nameof(IdentityThreeScalarPartComposite)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(IdentityThreeScalarPartComposite),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromIdentityApiSchemaTheIdentityThreeScalarPartCompositeType(),
        },

        // Identity API Schema
        // - IdentityNestedComposite
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Identity}' API schema the '{nameof(IdentityNestedComposite)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(IdentityNestedComposite),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromIdentityApiSchemaTheIdentityNestedCompositeType(),
        },

        // Identity API Schema
        // - IdentityOwnedComposite
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Identity}' API schema the '{nameof(IdentityOwnedComposite)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(IdentityOwnedComposite),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromIdentityApiSchemaTheIdentityOwnedCompositeType(),
        },

        // Identity API Schema
        // - IdentityOwnedDependent
        new BuildTestGeneric
        {
            Name = $"Build from '{ApiSchemaKind.Identity}' API schema the '{nameof(IdentityOwnedDependent)}' API object type",
            ApiSchemaKind = ApiSchemaKind.Identity,
            ApiObjectTypeName = nameof(IdentityOwnedDependent),
            ApiTypeActualBuildExpression = static () => ApiObjectTypeBuilderTestsGenericTestFactory.BuildFromIdentityApiSchemaTheIdentityOwnedDependentType(),
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildGenericTheoryData))]
    public void BuildGeneric(IXUnitTest test) => test.Execute(this);
    #endregion
}
