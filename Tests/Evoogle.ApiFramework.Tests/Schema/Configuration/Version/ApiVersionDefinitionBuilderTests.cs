// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Types;
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
        #endregion

        #region Calculated Properties
        private ApiObjectType? Actual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            var builder = new ApiObjectTypeBuilder<VersionedObject>
            (
                new ApiSchemaBuilderContext()
            )
                .WithName(nameof(VersionedObject))
                .AddRequiredProperty(x => x.Version)
                .WithVersion
                (
                    x => x.Version,
                    version => version.AddVersionExtension(new GraphQlExtension())
                );

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
                apiRelationships: null
            );
            ApiSchemaCompiler.Compile(schema).ThrowIfInvalid();
            this.Actual = objectType;
        }

        protected override void Assert()
        {
            this.Actual!.HasVersion.Should().BeTrue();
            this.Actual.ApiVersion!.ClrType.Should().Be
            (
                this.ReplaceWithRepositoryVersion ? typeof(long) : typeof(int)
            );
            this.Actual.ApiVersion.ClrMemberName.Should().Be
            (
                this.ReplaceWithRepositoryVersion ? null : nameof(VersionedObject.Version)
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
