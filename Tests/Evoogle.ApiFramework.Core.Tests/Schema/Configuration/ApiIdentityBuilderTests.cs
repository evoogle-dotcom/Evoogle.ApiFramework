// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiIdentityBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private class ApiIdentityPartConfig(string apiPropertyName, Type? clrTargetType = null, Type? apiExtensionType = null)
    {
        public string ApiPropertyName { get; } = apiPropertyName;
        public Type? ClrTargetType { get; } = clrTargetType;
        public Type? ApiExtensionType { get; } = apiExtensionType;

        public override string ToString()
        {
            var apiPropertyName = this.ApiPropertyName.SafeToString();
            var clrTargetType = this.ClrTargetType.SafeToName();
            var apiExtensionType = this.ApiExtensionType.SafeToName();

            return $"{nameof(ApiIdentityPartConfig)} {{{nameof(this.ApiPropertyName)}={apiPropertyName}, {nameof(this.ClrTargetType)}={clrTargetType}, {nameof(this.ApiExtensionType)}={apiExtensionType}}}";
        }
    }

    private class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public required string ApiName { get; init; } = null!;
        public required ApiIdentityPartConfig[] ApiIdentityParts { get; init; } = null!;
        public required ApiIdentity ApiIdentityExpected { get; init; } = null!;
        public Type? ApiExtensionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiIdentity? ApiIdentityActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"ApiName:          {this.ApiName.SafeToString()}");
            this.WriteLine($"ApiIdentityParts: {string.Join(",", this.ApiIdentityParts.Select(p => p.ToString()))}");
            this.WriteLine($"ApiExtensionType: {this.ApiExtensionType.SafeToName()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.ApiIdentityExpected.SafeToString()}");
        }

        protected override void Act()
        {
            var builder = new ApiIdentityBuilder(this.ApiName);

            foreach (var part in this.ApiIdentityParts)
            {
                builder.AddPart
                (
                    part.ApiPropertyName,
                    part.ClrTargetType,
                    part.ApiExtensionType != null
                        ? extensions =>
                        {
                            var extension = Activator.CreateInstance(part.ApiExtensionType);
                            extensions.AddExtension(part.ApiExtensionType, extension!);
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
                    .Excluding(info => info.DeclaringType == typeof(ApiIdentityPart) && info.Name == nameof(ApiIdentityPart.ApiProperty))
                    .Excluding(info => info.DeclaringType == typeof(ApiIdentityPart) && info.Name == nameof(ApiIdentityPart.ClrIdType))
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
            Name = "Builds Simple Identity With 1 Part",
            ApiName = "PrimaryKey",
            ApiIdentityParts = [new ApiIdentityPartConfig("Id")],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "PrimaryKey",
                apiIdentityParts: [new ApiIdentityPart("Id")]
            )
        },
        new BuildTest
        {
            Name = "Builds Simple Identity With 1 Part And Target Type",
            ApiName = "PrimaryKey",
            ApiIdentityParts = [new ApiIdentityPartConfig("Id", typeof(Guid))],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "PrimaryKey",
                apiIdentityParts: [new ApiIdentityPart("Id", typeof(Guid))]
            )
        },
        new BuildTest
        {
            Name = "Builds Simple Identity With 1 Part And Target Type And Extensions",
            ApiName = "PrimaryKey",
            ApiIdentityParts = [new ApiIdentityPartConfig("Id", typeof(Guid), typeof(GraphQlExtension))],
            ApiExtensionType = typeof(JsonApiExtension),
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "PrimaryKey",
                apiIdentityParts:
                [
                    new ApiIdentityPart("Id", typeof(Guid))
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
            Name = "Builds Composite Identity With 2 Parts",
            ApiName = "CompositeKey",
            ApiIdentityParts = [new ApiIdentityPartConfig("TenantId"), new ApiIdentityPartConfig("UserId")],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentityParts: [new ApiIdentityPart("TenantId"), new ApiIdentityPart("UserId")]
            )
        },
        new BuildTest
        {
            Name = "Builds Composite Identity With 2 Parts And Target Types",
            ApiName = "CompositeKey",
            ApiIdentityParts = [new ApiIdentityPartConfig("TenantId", typeof(Ulid)), new ApiIdentityPartConfig("UserId", typeof(int))],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentityParts: [new ApiIdentityPart("TenantId", typeof(Ulid)), new ApiIdentityPart("UserId", typeof(int))]
            )
        },
        new BuildTest
        {
            Name = "Builds Composite Identity With 2 Parts And Target Types And Extensions",
            ApiName = "CompositeKey",
            ApiIdentityParts = [new ApiIdentityPartConfig("TenantId", typeof(Ulid), typeof(GraphQlExtension)), new ApiIdentityPartConfig("UserId", typeof(int), typeof(GraphQlExtension))],
            ApiExtensionType = typeof(JsonApiExtension),
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentityParts:
                [
                    new ApiIdentityPart("TenantId", typeof(Ulid))
                    {
                        Extensions = new OrderedDictionary<Type, object>
                        {
                            [typeof(GraphQlExtension)] = new GraphQlExtension()
                        }
                    },
                    new ApiIdentityPart("UserId", typeof(int))
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
            Name = "Builds Composite Identity With 3 Parts",
            ApiName = "CompositeKey",
            ApiIdentityParts = [new ApiIdentityPartConfig("CompanyId"), new ApiIdentityPartConfig("OrderId"), new ApiIdentityPartConfig("ItemId")],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentityParts: [new ApiIdentityPart("CompanyId"), new ApiIdentityPart("OrderId"), new ApiIdentityPart("ItemId")]
            )
        },
        new BuildTest
        {
            Name = "Builds Composite Identity With 3 Parts And Target Types",
            ApiName = "CompositeKey",
            ApiIdentityParts = [new ApiIdentityPartConfig("CompanyId", typeof(Ulid)), new ApiIdentityPartConfig("OrderId", typeof(int)), new ApiIdentityPartConfig("ItemId", typeof(string))],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentityParts: [new ApiIdentityPart("CompanyId", typeof(Ulid)), new ApiIdentityPart("OrderId", typeof(int)), new ApiIdentityPart("ItemId", typeof(string))]
            )
        },
        new BuildTest
        {
            Name = "Builds Composite Identity With 3 Parts And Target Types And Extensions",
            ApiName = "CompositeKey",
            ApiIdentityParts = [new ApiIdentityPartConfig("CompanyId", typeof(Ulid), typeof(GraphQlExtension)), new ApiIdentityPartConfig("OrderId", typeof(int), typeof(GraphQlExtension)), new ApiIdentityPartConfig("ItemId", typeof(string), typeof(GraphQlExtension))],
            ApiExtensionType = typeof(JsonApiExtension),
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentityParts:
                [
                    new ApiIdentityPart("CompanyId", typeof(Ulid))
                    {
                        Extensions = new OrderedDictionary<Type, object>
                        {
                            [typeof(GraphQlExtension)] = new GraphQlExtension()
                        }
                    },
                    new ApiIdentityPart("OrderId", typeof(int))
                    {
                        Extensions = new OrderedDictionary<Type, object>
                        {
                            [typeof(GraphQlExtension)] = new GraphQlExtension()
                        }
                    },
                    new ApiIdentityPart("ItemId", typeof(string))
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
