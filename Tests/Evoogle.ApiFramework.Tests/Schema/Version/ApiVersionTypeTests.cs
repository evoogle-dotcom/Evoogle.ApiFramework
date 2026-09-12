// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.TestData;
using Evoogle.ApiFramework.Version;
using Evoogle.Extension;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Version;

public class ApiVersionTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class VersionedObject
    {
        public required int Version { get; init; }
    }

    private sealed class MismatchedVersionedObject
    {
        public required long Version { get; init; }
    }

    private sealed record CustomVersion(string Value);

    private enum InvalidDefinition
    {
        InvalidMemberName,
        MissingProperty,
        OptionalProperty,
        TypeMismatch,
        NullType,
        NullableType,
        UnregisteredScalar
    }

    private enum InvalidMaterializationCase
    {
        NullValue,
        WrongValueType,
        RepositoryBackedFromObject
    }

    private sealed class MaterializationTest : XUnitTest
    {
        #region User Supplied Properties
        public required bool IsPropertyBacked { get; init; }
        #endregion

        #region Calculated Properties
        private ApiVersion Actual { get; set; }
        private ApiVersionType? VersionType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.VersionType = CompileVersionType
            (
                this.IsPropertyBacked
                    ? new ApiVersionType(nameof(VersionedObject.Version))
                    : new ApiVersionType(typeof(int))
            );
        }

        protected override void Act()
        {
            this.Actual = this.IsPropertyBacked
                ? this.VersionType!.MaterializeVersion(new VersionedObject { Version = 42 })
                : this.VersionType!.MaterializeVersionFromValue(42);
        }

        protected override void Assert()
        {
            this.Actual.GetValue<int>().Should().Be(42);
            this.VersionType!.ApiScalarType.ClrType.Should().Be(typeof(int));
            this.VersionType.IsPropertyBacked.Should().Be(this.IsPropertyBacked);
            this.VersionType.IsRepositoryBacked.Should().Be(!this.IsPropertyBacked);
            this.VersionType.ApiProperty.Should().Be
            (
                this.IsPropertyBacked
                    ? this.VersionType.Parent.As<ApiObjectType>().ApiProperties.Single()
                    : null
            );
        }
        #endregion
    }

    private sealed class InvalidDefinitionTest : XUnitTest
    {
        #region User Supplied Properties
        public required InvalidDefinition Definition { get; init; }
        public required ApiSchemaCompilationCode ExpectedCode { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaCompilationResult? Result { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            var memberName = this.Definition switch
            {
                InvalidDefinition.InvalidMemberName => string.Empty,
                InvalidDefinition.MissingProperty => "Missing",
                _ => nameof(VersionedObject.Version)
            };
            var versionType = this.Definition switch
            {
                InvalidDefinition.NullType => new ApiVersionType((Type)null!),
                InvalidDefinition.NullableType => new ApiVersionType(typeof(int?)),
                InvalidDefinition.UnregisteredScalar => new ApiVersionType(typeof(decimal)),
                _ => new ApiVersionType(memberName)
            };
            var modifiers = this.Definition == InvalidDefinition.OptionalProperty
                ? ApiTypeModifiers.None
                : ApiTypeModifiers.Required;

            var property = new ApiProperty
            (
                nameof(VersionedObject.Version),
                new ApiTypeExpression(typeof(int)),
                modifiers,
                nameof(VersionedObject.Version),
                ClrMemberKind.Property
            );
            var objectType = new ApiObjectType
            (
                nameof(VersionedObject),
                apiOptions: null,
                apiProperties: [property],
                apiKeyTypes: null,
                versionType,
                this.Definition == InvalidDefinition.TypeMismatch
                    ? typeof(MismatchedVersionedObject)
                    : typeof(VersionedObject)
            );
            var schema = CreateSchema(objectType);

            this.Result = ApiSchemaCompiler.Compile(schema);
        }

        protected override void Assert()
        {
            this.Result!.Errors.Should().Contain
            (
                issue => issue.Code == this.ExpectedCode
            );
        }
        #endregion
    }

    private sealed class InvalidMaterializationTest : XUnitTest
    {
        #region User Supplied Properties
        public required InvalidMaterializationCase Materialization { get; init; }
        #endregion

        #region Calculated Properties
        private Exception? Exception { get; set; }
        private ApiVersionType? VersionType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.VersionType = CompileVersionType
            (
                this.Materialization == InvalidMaterializationCase.RepositoryBackedFromObject
                    ? new ApiVersionType(typeof(int))
                    : new ApiVersionType(nameof(VersionedObject.Version))
            );
        }

        protected override void Act()
        {
            try
            {
                _ = this.Materialization switch
                {
                    InvalidMaterializationCase.NullValue =>
                        this.VersionType!.MaterializeVersionFromValue(null),
                    InvalidMaterializationCase.WrongValueType =>
                        this.VersionType!.MaterializeVersionFromValue(42L),
                    InvalidMaterializationCase.RepositoryBackedFromObject =>
                        this.VersionType!.MaterializeVersion(new VersionedObject { Version = 42 }),
                    _ => throw new InvalidOperationException()
                };
            }
            catch (Exception exception)
            {
                this.Exception = exception;
            }
        }

        protected override void Assert()
        {
            this.Exception.Should().BeOfType<ApiSchemaMaterializationException>();
        }
        #endregion
    }

    private sealed class JsonTest : XUnitTest
    {
        #region Calculated Properties
        private ApiObjectType? Actual { get; set; }
        private string? Json { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            var expected = CreateObjectType
            (
                new ApiVersionType(nameof(VersionedObject.Version))
            );
            expected.ApiVersionType!.AttachExtension(new GraphQlExtension());
            var schema = CreateSchema(expected);
            ApiSchemaCompiler.Compile(schema).ThrowIfInvalid();

            this.Json = JsonSerializer.Serialize(schema);
            var actualSchema = JsonSerializer.Deserialize<ApiSchema>(this.Json);
            this.Actual = actualSchema!.GetObjectTypeByApiName(nameof(VersionedObject));
        }

        protected override void Assert()
        {
            this.Json.Should().Contain("\"ApiVersionType\":");

            using var document = JsonDocument.Parse(this.Json!);
            var versionJson = document.RootElement
                .GetProperty(nameof(ApiSchema.ApiObjectTypes))[0]
                .GetProperty(nameof(ApiObjectType.ApiVersionType));
            versionJson.TryGetProperty(nameof(ApiVersionType.ClrMemberName), out var memberName)
                .Should().BeTrue();
            memberName.GetString().Should().Be(nameof(VersionedObject.Version));
            versionJson.TryGetProperty(nameof(ApiVersionType.ClrType), out _).Should().BeFalse();

            this.Actual!.ApiVersionType!.ClrType.Should().Be(typeof(int));
            this.Actual.ApiVersionType.ClrMemberName.Should().Be
            (
                nameof(VersionedObject.Version)
            );
            this.Actual.ApiVersionType.Extensions.Should().ContainKey(typeof(GraphQlExtension));
        }
        #endregion
    }

    private sealed class CustomReferenceMaterializationTest : XUnitTest
    {
        #region Calculated Properties
        private ApiVersion Actual { get; set; }
        private CustomVersion? Source { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Source = new CustomVersion("third-party-v1");
        }

        protected override void Act()
        {
            var objectType = CreateObjectType
            (
                new ApiVersionType(typeof(CustomVersion))
            );
            var schema = new ApiSchema
            (
                "CustomVersionTests",
                apiVersion: null,
                apiOptions: null,
                apiNamedTypes:
                [
                    new ApiScalarType("Int32", typeof(int)),
                    new ApiScalarType("CustomVersion", typeof(CustomVersion)),
                    objectType
                ],
                apiRelationships: null
            );
            ApiSchemaCompiler.Compile(schema).ThrowIfInvalid();

            this.Actual = objectType.ApiVersionType!.MaterializeVersionFromValue(this.Source);
        }

        protected override void Assert()
        {
            this.Actual.GetValue<CustomVersion>().Should().BeSameAs(this.Source);
        }
        #endregion
    }

    private sealed class DuplicateJsonMemberTest : XUnitTest
    {
        #region Calculated Properties
        private Exception? Exception { get; set; }
        private string? Json { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var objectType = CreateObjectType
            (
                new ApiVersionType(nameof(VersionedObject.Version))
            );
            var json = JsonSerializer.Serialize<ApiType>(objectType);
            using var document = JsonDocument.Parse(json);
            var versionJson = document.RootElement
                .GetProperty(nameof(ApiObjectType.ApiVersionType))
                .GetRawText();
            var marker = $"\"{nameof(ApiObjectType.ApiVersionType)}\":";
            this.Json = json.Replace(marker, $"{marker}{versionJson},{marker}");
        }

        protected override void Act()
        {
            try
            {
                _ = JsonSerializer.Deserialize<ApiType>(this.Json!);
            }
            catch (Exception exception)
            {
                this.Exception = exception;
            }
        }

        protected override void Assert()
        {
            this.Exception.Should().BeOfType<JsonException>();
        }
        #endregion
    }

    private sealed class InvalidVersionShapeJsonTest : XUnitTest
    {
        #region User Supplied Properties
        public required string Json { get; init; }
        #endregion

        #region Calculated Properties
        private Exception? Exception { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            try
            {
                _ = JsonSerializer.Deserialize<ApiVersionType>(this.Json);
            }
            catch (Exception exception)
            {
                this.Exception = exception;
            }
        }

        protected override void Assert()
        {
            this.Exception.Should().BeOfType<JsonException>();
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] MaterializationTheoryData =>
    [
        new MaterializationTest
        {
            Name = "Materialize Property Backed Version",
            IsPropertyBacked = true
        },
        new MaterializationTest
        {
            Name = "Materialize Repository Backed Version",
            IsPropertyBacked = false
        },
        new CustomReferenceMaterializationTest
        {
            Name = "Materialize Custom Immutable Reference Version"
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] InvalidDefinitionTheoryData =>
    [
        new InvalidDefinitionTest
        {
            Name = "Reject Invalid Version Member Name",
            Definition = InvalidDefinition.InvalidMemberName,
            ExpectedCode = ApiSchemaCompilationCode.ApiVersionTypeInvalidClrMemberName
        },
        new InvalidDefinitionTest
        {
            Name = "Reject Missing Version Property",
            Definition = InvalidDefinition.MissingProperty,
            ExpectedCode = ApiSchemaCompilationCode.ApiVersionTypeUnresolvedProperty
        },
        new InvalidDefinitionTest
        {
            Name = "Reject Optional Version Property",
            Definition = InvalidDefinition.OptionalProperty,
            ExpectedCode = ApiSchemaCompilationCode.ApiVersionTypeOptionalProperty
        },
        new InvalidDefinitionTest
        {
            Name = "Reject Version Property Type Mismatch",
            Definition = InvalidDefinition.TypeMismatch,
            ExpectedCode = ApiSchemaCompilationCode.ApiVersionTypeClrTypeMismatch
        },
        new InvalidDefinitionTest
        {
            Name = "Reject Null Repository Version CLR Type",
            Definition = InvalidDefinition.NullType,
            ExpectedCode = ApiSchemaCompilationCode.ApiVersionTypeNullClrType
        },
        new InvalidDefinitionTest
        {
            Name = "Reject Nullable Version CLR Type",
            Definition = InvalidDefinition.NullableType,
            ExpectedCode = ApiSchemaCompilationCode.ApiVersionTypeNullableClrType
        },
        new InvalidDefinitionTest
        {
            Name = "Reject Unregistered Version Scalar",
            Definition = InvalidDefinition.UnregisteredScalar,
            ExpectedCode = ApiSchemaCompilationCode.ApiVersionTypeUnresolvedScalarType
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] InvalidMaterializationTheoryData =>
    [
        .. Enum.GetValues<InvalidMaterializationCase>().Select
        (
            materialization => new TheoryDataRow<IXUnitTest>
            (
                new InvalidMaterializationTest
                {
                    Name = $"Reject {materialization}",
                    Materialization = materialization
                }
            )
        )
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonTheoryData =>
    [
        new JsonTest
        {
            Name = "Round Trip Version Metadata JSON"
        },
        new DuplicateJsonMemberTest
        {
            Name = "Reject Duplicate Version Metadata JSON Member"
        },
        new InvalidVersionShapeJsonTest
        {
            Name = "Reject Version Metadata JSON With Both Source Properties",
            Json = @"{
                ""ClrType"": ""System.Int32, System.Private.CoreLib"",
                ""ClrMemberName"": ""Version""
            }"
        },
        new InvalidVersionShapeJsonTest
        {
            Name = "Reject Version Metadata JSON Without A Source Property",
            Json = "{}"
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(MaterializationTheoryData))]
    public void Materialization(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(InvalidDefinitionTheoryData))]
    public void InvalidDefinitions(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(InvalidMaterializationTheoryData))]
    public void InvalidMaterialization(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonTheoryData))]
    public void Json(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Factory Methods
    private static ApiVersionType CompileVersionType(ApiVersionType versionType)
    {
        var objectType = CreateObjectType(versionType);
        var schema = CreateSchema(objectType);

        ApiSchemaCompiler.Compile(schema).ThrowIfInvalid();
        return objectType.ApiVersionType!;
    }

    private static ApiObjectType CreateObjectType(ApiVersionType versionType)
    {
        var property = new ApiProperty
        (
            nameof(VersionedObject.Version),
            new ApiTypeExpression(typeof(int)),
            ApiTypeModifiers.Required,
            nameof(VersionedObject.Version),
            ClrMemberKind.Property
        );

        return new ApiObjectType
        (
            nameof(VersionedObject),
            apiOptions: null,
            apiProperties: [property],
            apiKeyTypes: null,
            versionType,
            typeof(VersionedObject)
        );
    }

    private static ApiSchema CreateSchema(ApiObjectType objectType)
        => new
        (
            "VersionTests",
            apiVersion: null,
            apiOptions: null,
            apiNamedTypes:
            [
                new ApiScalarType("Int32", typeof(int)),
                new ApiScalarType("Int64", typeof(long)),
                objectType
            ],
            apiRelationships: null
        );
    #endregion
}
