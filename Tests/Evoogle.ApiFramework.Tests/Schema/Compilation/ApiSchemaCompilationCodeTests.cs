// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Json;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Compilation;

public class ApiSchemaCompilationCodeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class SerializationTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaCompilationCode Code { get; init; }
        public required string ExpectedValue { get; init; }
        #endregion

        #region Calculated Properties
        private string? ActualJson { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new EnumJsonConverter<ApiSchemaCompilationCode>());

            this.ActualJson = JsonSerializer.Serialize(this.Code, options);
        }

        protected override void Assert() => this.ActualJson.Should().Be($"\"{this.ExpectedValue}\"");
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryData<IXUnitTest> SerializationTheoryData => new()
    {
        CreateSerializationTest
        (
            ApiSchemaCompilationCode.ApiKeyDefinitionNullOrEmptyPaths,
            "API_KEY_DEFINITION_NULL_OR_EMPTY_PATHS"
        ),
        CreateSerializationTest
        (
            ApiSchemaCompilationCode.ApiNamedKeyDefinitionInvalidApiName,
            "API_NAMED_KEY_DEFINITION_INVALID_API_NAME"
        ),
        CreateSerializationTest
        (
            ApiSchemaCompilationCode.ApiObjectTypeDuplicateKeyApiName,
            "API_OBJECT_TYPE_DUPLICATE_KEY_API_NAME"
        ),
        CreateSerializationTest
        (
            ApiSchemaCompilationCode.ApiVersionDefinitionNullClrType,
            "API_VERSION_DEFINITION_NULL_CLR_TYPE"
        ),
        CreateSerializationTest
        (
            ApiSchemaCompilationCode.ApiVersionDefinitionNullableClrType,
            "API_VERSION_DEFINITION_NULLABLE_CLR_TYPE"
        ),
        CreateSerializationTest
        (
            ApiSchemaCompilationCode.ApiVersionDefinitionUnresolvedScalarType,
            "API_VERSION_DEFINITION_UNRESOLVED_SCALAR_TYPE"
        ),
        CreateSerializationTest
        (
            ApiSchemaCompilationCode.ApiVersionDefinitionUnresolvedProperty,
            "API_VERSION_DEFINITION_UNRESOLVED_PROPERTY"
        ),
        CreateSerializationTest
        (
            ApiSchemaCompilationCode.ApiVersionDefinitionNonScalarProperty,
            "API_VERSION_DEFINITION_NON_SCALAR_PROPERTY"
        ),
        CreateSerializationTest
        (
            ApiSchemaCompilationCode.ApiVersionDefinitionClrTypeMismatch,
            "API_VERSION_DEFINITION_CLR_TYPE_MISMATCH"
        ),
        CreateSerializationTest
        (
            ApiSchemaCompilationCode.ApiVersionDefinitionOptionalProperty,
            "API_VERSION_DEFINITION_OPTIONAL_PROPERTY"
        ),
        CreateSerializationTest
        (
            ApiSchemaCompilationCode.ApiRelationshipEndUnresolvedKey,
            "API_RELATIONSHIP_END_UNRESOLVED_KEY"
        )
    };
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(SerializationTheoryData))]
    public void Serialization(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Implementation Methods
    private static SerializationTest CreateSerializationTest
    (
        ApiSchemaCompilationCode code,
        string expectedValue
    ) => new()
    {
        Name = $"{code} Serializes As {expectedValue}",
        Code = code,
        ExpectedValue = expectedValue
    };
    #endregion
}
