// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

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
            this.WriteLine($"ApiSchemaExpected: {this.ApiSchemaExpected.SafeToString()}");
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
                builder = builder.AddScalar(clrType, x => x.WithName(apiName));
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
                        x.AddProperty(apiPropertyName, clrPropertyName);
                    }

                    x.ConfigureOptions(apiObjectType);
                    x.ConfigureIdentities(apiObjectType);
                    x.ConfigureExtensions(apiObjectType);
                });
            }

            var extensions = this.ApiSchemaExpected!.Extensions;
            foreach (var extension in extensions ?? [])
            {
                builder.AddSchemaExtension(extension.Key, extension.Value);
            }

            this.ApiSchemaActual = builder.Build();
            this.WriteLine($"ApiSchemaActual:   {this.ApiSchemaActual.SafeToString()}");
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
            Name = $"Build '{ApiSchemaKind.Commerce}' API schema",
            ApiSchemaKind = ApiSchemaKind.Commerce,
        },
        new BuildTest
        {
            Name = $"Build '{ApiSchemaKind.Identity}' API schema",
            ApiSchemaKind = ApiSchemaKind.Identity,
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}
