// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiIdentityBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private class ApiIdentitySourceConfig(string apiPropertyName, string? apiNestedName = null, Type? clrScalarType = null, Type? apiExtensionType = null)
    {
        public string ApiPropertyName { get; } = apiPropertyName;
        public string? ApiNestedName { get; } = apiNestedName;
        public Type? ClrScalarType { get; } = clrScalarType;
        public Type? ApiExtensionType { get; } = apiExtensionType;

        public override string ToString()
        {
            var apiPropertyName = this.ApiPropertyName.SafeToString();
            var apiNestedName = this.ApiNestedName.SafeToString();
            var clrScalarType = this.ClrScalarType.SafeToName();
            var apiExtensionType = this.ApiExtensionType.SafeToName();

            return $"{nameof(ApiIdentitySourceConfig)} {{{nameof(this.ApiPropertyName)}={apiPropertyName}, {nameof(this.ApiNestedName)}={apiNestedName}, {nameof(this.ClrScalarType)}={clrScalarType}, {nameof(this.ApiExtensionType)}={apiExtensionType}}}";
        }
    }

    private class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public required string ApiName { get; init; } = null!;
        public required ApiIdentitySourceConfig[] ApiIdentitySources { get; init; } = null!;
        public required ApiIdentity ApiIdentityExpected { get; init; } = null!;
        public Type? ApiExtensionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiIdentity? ApiIdentityActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"ApiName:            {this.ApiName.SafeToString()}");
            this.WriteLine($"ApiIdentitySources: {string.Join(",", this.ApiIdentitySources.Select(p => p.ToString()))}");
            this.WriteLine($"ApiExtensionType:   {this.ApiExtensionType.SafeToName()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.ApiIdentityExpected.SafeToString()}");
        }

        protected override void Act()
        {
            var builder = new ApiIdentityBuilder(this.ApiName);

            foreach (var source in this.ApiIdentitySources)
            {
                var apiPropertyName = source.ApiPropertyName;
                var clrScalarType = source.ClrScalarType;
                var apiNestedName = source.ApiNestedName;
                var apiExtensionType = source.ApiExtensionType;

                builder.AddSource
                (
                    apiPropertyName,
                    clrScalarType,
                    apiNestedName,
                    apiExtensionType != null
                        ? extensions =>
                        {
                            var extension = Activator.CreateInstance(apiExtensionType);
                            extensions.AddExtension(apiExtensionType, extension!);
                        }
                : null
                );
            }

            if (this.ApiExtensionType != null)
            {
                var extension = Activator.CreateInstance(this.ApiExtensionType);
                builder.AddExtension(this.ApiExtensionType, extension!);
            }

            this.ApiIdentityActual = builder.Build();
            this.WriteLine($"Actual:   {this.ApiIdentityActual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ApiIdentityActual.Should().NotBeNull();

            this.ApiIdentityActual.Should().BeEquivalentTo
            (
                this.ApiIdentityExpected,
                opt => opt
                    .Excluding(info => info.Path.Contains(nameof(ApiSchemaElement.ApiPath)))
                    // .Excluding(info => info.DeclaringType == typeof(ApiIdentitySource) && info.Name == nameof(ApiIdentitySource.ApiPropertyName))
                    // .Excluding(info => info.DeclaringType == typeof(ApiIdentitySource) && info.Name == nameof(ApiIdentitySource.ClrScalarType))
                    .WithStrictOrdering()
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
            Name = "Builds Simple Identity With 1 Source",
            ApiName = "PrimaryKey",
            ApiIdentitySources = [new ApiIdentitySourceConfig("Id")],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "PrimaryKey",
                apiIdentitySources: [new ApiIdentitySource("Id")]
            )
        },
        new BuildTest
        {
            Name = "Builds Simple Identity With 1 Source And CLR Scalar Type",
            ApiName = "PrimaryKey",
            ApiIdentitySources = [new ApiIdentitySourceConfig("Id", null, typeof(Guid))],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "PrimaryKey",
                apiIdentitySources: [new ApiIdentitySource("Id", typeof(Guid))]
            )
        },
        new BuildTest
        {
            Name = "Builds Simple Identity With 1 Source And CLR Scalar Type And Extensions",
            ApiName = "PrimaryKey",
            ApiIdentitySources = [new ApiIdentitySourceConfig("Id", null, typeof(Guid), typeof(GraphQlExtension))],
            ApiExtensionType = typeof(JsonApiExtension),
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "PrimaryKey",
                apiIdentitySources:
                [
                    new ApiIdentitySource("Id", typeof(Guid))
                    {
                        Extensions = new OrderedDictionary<Type, object>
                        {
                            [typeof(GraphQlExtension)] = new GraphQlExtension()
                        }
                    }
                ]
            )
            {
                Extensions = new OrderedDictionary<Type, object>
                {
                    [typeof(JsonApiExtension)] = new JsonApiExtension()
                }
            }
        },

        new BuildTest
        {
            Name = "Builds Composite Identity With 2 Sources",
            ApiName = "CompositeKey",
            ApiIdentitySources = [new ApiIdentitySourceConfig("TenantId"), new ApiIdentitySourceConfig("UserId")],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentitySources: [new ApiIdentitySource("TenantId"), new ApiIdentitySource("UserId")]
            )
        },
        new BuildTest
        {
            Name = "Builds Composite Identity With 2 Sources And CLR Scalar Types",
            ApiName = "CompositeKey",
            ApiIdentitySources = [new ApiIdentitySourceConfig("TenantId", null, typeof(Ulid)), new ApiIdentitySourceConfig("UserId", null, typeof(int))],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentitySources: [new ApiIdentitySource("TenantId", typeof(Ulid)), new ApiIdentitySource("UserId", typeof(int))]
            )
        },
        new BuildTest
        {
            Name = "Builds Composite Identity With 2 Sources And CLR Scalar Types And Extensions",
            ApiName = "CompositeKey",
            ApiIdentitySources = [new ApiIdentitySourceConfig("TenantId", null, typeof(Ulid), typeof(GraphQlExtension)), new ApiIdentitySourceConfig("UserId", null, typeof(int), typeof(GraphQlExtension))],
            ApiExtensionType = typeof(JsonApiExtension),
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentitySources:
                [
                    new ApiIdentitySource("TenantId", typeof(Ulid))
                    {
                        Extensions = new OrderedDictionary<Type, object>
                        {
                            [typeof(GraphQlExtension)] = new GraphQlExtension()
                        }
                    },
                    new ApiIdentitySource("UserId", typeof(int))
                    {
                        Extensions = new OrderedDictionary<Type, object>
                        {
                            [typeof(GraphQlExtension)] = new GraphQlExtension()
                        }
                    }
                ]
            )
            {
                Extensions = new OrderedDictionary<Type, object>
                {
                    [typeof(JsonApiExtension)] = new JsonApiExtension()
                }
            }
        },

        new BuildTest
        {
            Name = "Builds Composite Identity With 3 Sources",
            ApiName = "CompositeKey",
            ApiIdentitySources = [new ApiIdentitySourceConfig("CompanyId"), new ApiIdentitySourceConfig("OrderId"), new ApiIdentitySourceConfig("ItemId")],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentitySources: [new ApiIdentitySource("CompanyId"), new ApiIdentitySource("OrderId"), new ApiIdentitySource("ItemId")]
            )
        },
        new BuildTest
        {
            Name = "Builds Composite Identity With 3 Sources And CLR Scalar Types",
            ApiName = "CompositeKey",
            ApiIdentitySources = [new ApiIdentitySourceConfig("CompanyId", null, typeof(Ulid)), new ApiIdentitySourceConfig("OrderId", null, typeof(int)), new ApiIdentitySourceConfig("ItemId", null, typeof(string))],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentitySources: [new ApiIdentitySource("CompanyId", typeof(Ulid)), new ApiIdentitySource("OrderId", typeof(int)), new ApiIdentitySource("ItemId", typeof(string))]
            )
        },
        new BuildTest
        {
            Name = "Builds Composite Identity With 3 Sources And CLR Scalar Types And Extensions",
            ApiName = "CompositeKey",
            ApiIdentitySources = [new ApiIdentitySourceConfig("CompanyId", null, typeof(Ulid), typeof(GraphQlExtension)), new ApiIdentitySourceConfig("OrderId", null, typeof(int), typeof(GraphQlExtension)), new ApiIdentitySourceConfig("ItemId", null, typeof(string), typeof(GraphQlExtension))],
            ApiExtensionType = typeof(JsonApiExtension),
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentitySources:
                [
                    new ApiIdentitySource("CompanyId", typeof(Ulid))
                    {
                        Extensions = new OrderedDictionary<Type, object>
                        {
                            [typeof(GraphQlExtension)] = new GraphQlExtension()
                        }
                    },
                    new ApiIdentitySource("OrderId", typeof(int))
                    {
                        Extensions = new OrderedDictionary<Type, object>
                        {
                            [typeof(GraphQlExtension)] = new GraphQlExtension()
                        }
                    },
                    new ApiIdentitySource("ItemId", typeof(string))
                    {
                        Extensions = new OrderedDictionary<Type, object>
                        {
                            [typeof(GraphQlExtension)] = new GraphQlExtension()
                        }
                    }
                ]
            )
            {
                Extensions = new OrderedDictionary<Type, object>
                {
                    [typeof(JsonApiExtension)] = new JsonApiExtension()
                }
            }
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}
