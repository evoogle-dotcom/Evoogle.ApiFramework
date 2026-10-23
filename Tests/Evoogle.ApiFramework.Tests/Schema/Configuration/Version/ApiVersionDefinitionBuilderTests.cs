// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Version;

public class ApiVersionDefinitionBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class VersionedObject
    {
        public int Version { get; init; }
    }

    private sealed class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public required bool ReplaceWithRepositoryVersion { get; init; }

        public bool UsesApiNameReference { get; init; }
        #endregion

        #region Calculated Properties
        private ApiObjectType? Actual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario:   {this.Name}");
            this.WriteLine($"CLR Member: {nameof(VersionedObject)}.{nameof(VersionedObject.Version)}");
            this.WriteLine();
            this.WriteLine($"Uses API Name Reference: {this.UsesApiNameReference}");
            this.WriteLine($"Uses Repository Version: {this.ReplaceWithRepositoryVersion}");
            this.WriteLine();
            this.WriteLine("Expected Exception: None");
        }

        protected override void Act()
        {
            var builder = new ApiObjectTypeBuilder<VersionedObject>
            (
                new ApiSchemaBuilderContext()
            )
                .WithName(nameof(VersionedObject))
                .AddProperty(x => x.Version, property => property.AsNonNullable());

            if (this.UsesApiNameReference)
            {
                builder.WithVersion
                (
                    ApiPropertyReference.ApiRef(nameof(VersionedObject.Version)),
                    version => version.AddVersionExtension(new GraphQlExtension())
                );
            }
            else
            {
                builder.WithVersion
                (
                    x => x.Version,
                    version => version.AddVersionExtension(new GraphQlExtension())
                );
            }

            if (this.ReplaceWithRepositoryVersion)
            {
                builder.WithRepositoryVersion<long>();
            }

            var objectType = builder.Build();
            var schema = new ApiSchema
            (
                "VersionBuilderTests",
                apiVersion: null,
                apiOptions: null,
                apiNamedTypes:
                [
                    new ApiScalarType("Int32", typeof(int)),
                    new ApiScalarType("Int64", typeof(long)),
                    objectType
                ],
                apiRelationships: null,
                apiQuerySources: null
            );
            ApiSchemaCompiler.Compile(schema).ThrowIfInvalid();
            this.Actual = objectType;
            this.WriteLine();
            this.WriteLine($"Actual Version CLR Type: {objectType.ApiVersion?.ClrType}");
            this.WriteLine($"Actual Version Member:   {objectType.ApiVersion?.ApiPropertyReference}");
        }

        protected override void Assert()
        {
            this.Actual!.HasVersion.Should().BeTrue();
            this.Actual.ApiVersion!.ClrType.Should().Be
            (
                this.ReplaceWithRepositoryVersion ? typeof(long) : typeof(int)
            );
            this.Actual.ApiVersion.ApiPropertyReference?.ApiName.Should().Be
            (
                !this.ReplaceWithRepositoryVersion && this.UsesApiNameReference
                    ? nameof(VersionedObject.Version)
                    : null
            );
            this.Actual.ApiVersion.ApiPropertyReference?.ClrName.Should().Be
            (
                !this.ReplaceWithRepositoryVersion && !this.UsesApiNameReference
                    ? nameof(VersionedObject.Version)
                    : null
            );
            this.Actual.ApiVersion.Extensions.Should().HaveCount
            (
                this.ReplaceWithRepositoryVersion ? 0 : 1
            );
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = "Build Property Backed Version",
            ReplaceWithRepositoryVersion = false
        },
        new BuildTest
        {
            Name = "Build API-Name Property Backed Version",
            ReplaceWithRepositoryVersion = false,
            UsesApiNameReference = true
        },
        new BuildTest
        {
            Name = "Repeated Version Configuration Replaces Prior Definition",
            ReplaceWithRepositoryVersion = true
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}
