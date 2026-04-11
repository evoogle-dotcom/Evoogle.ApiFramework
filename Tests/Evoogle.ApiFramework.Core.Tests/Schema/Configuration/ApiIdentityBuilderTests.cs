// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiIdentityBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private class ApiIdentityPartConfig(ApiIdentityPartKind apiKind, string? clrPropertyName = null, string? apiIdentityName = null, Type? clrScalarTypeHint = null, Type? extensionType = null)
    {
        public ApiIdentityPartKind ApiKind { get; } = apiKind;
        public string? ClrPropertyName { get; } = clrPropertyName;
        public string? ApiIdentityName { get; } = apiIdentityName;
        public Type? ClrScalarTypeHint { get; } = clrScalarTypeHint;
        public Type? ExtensionType { get; } = extensionType;

        public override string ToString()
        {
            var apiKind = this.ApiKind.SafeToString();
            var clrPropertyName = this.ClrPropertyName.SafeToString();
            var apiIdentityName = this.ApiIdentityName.SafeToString();
            var clrScalarTypeHint = this.ClrScalarTypeHint.SafeToName();
            var extensionType = this.ExtensionType.SafeToName();

            return $"{nameof(ApiIdentityPartConfig)} {{{nameof(this.ApiKind)}={apiKind}, {nameof(this.ClrPropertyName)}={clrPropertyName}, {nameof(this.ApiIdentityName)}={apiIdentityName}, {nameof(this.ClrScalarTypeHint)}={clrScalarTypeHint}, {nameof(this.ExtensionType)}={extensionType}}}";
        }
    }

    private class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public required string ApiName { get; init; } = null!;
        public required ApiIdentityPartConfig[] ApiIdentityParts { get; init; } = null!;
        public required ApiIdentity ApiIdentityExpected { get; init; } = null!;
        public Type? ExtensionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiIdentity? ApiIdentityActual { get; set; }
        #endregion

        #region Constructors
        [SetsRequiredMembers]
        public BuildTest()
        {
            this.Name = nameof(BuildTest);
            this.ExcludeMembers = ApiSchemaExcludeMembers.Standard;
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"ApiName:          {this.ApiName.SafeToString()}");
            this.WriteLine($"ApiIdentityParts: {string.Join(",", this.ApiIdentityParts.Select(p => p.ToString()))}");
            this.WriteLine($"ExtensionType:    {this.ExtensionType.SafeToName()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.ApiIdentityExpected.SafeToString()}");
        }

        protected override void Act()
        {
            var builder = new ApiIdentityBuilder(this.ApiName);

            foreach (var part in this.ApiIdentityParts)
            {
                var apiKind = part.ApiKind;
                var clrPropertyName = part.ClrPropertyName;
                var apiIdentityName = part.ApiIdentityName;
                var clrScalarTypeHint = part.ClrScalarTypeHint;
                var extensionType = part.ExtensionType;

                builder.AddPart
                (
                    apiKind,
                    clrPropertyName,
                    apiIdentityName,
                    clrScalarTypeHint,
                    extensionType != null
                        ? extensions =>
                        {
                            var extension = Activator.CreateInstance(extensionType);
                            extensions.AddExtension(extensionType, extension!);
                        }
                : null
                );
            }

            if (this.ExtensionType != null)
            {
                var extension = Activator.CreateInstance(this.ExtensionType);
                builder.AddExtension(this.ExtensionType, extension!);
            }

            this.ApiIdentityActual = builder.Build();
            this.WriteLine($"Actual:   {this.ApiIdentityActual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ApiIdentityActual.Should().NotBeNull();
            this.AssertBeEquivalentTo(this.ApiIdentityActual, this.ApiIdentityExpected);
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
            ApiIdentityParts = [new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "Id")],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "PrimaryKey",
                apiIdentityParts: [new ApiIdentityScalarPart("Id")]
            )
        },
        new BuildTest
        {
            Name = "Builds Simple Identity With 1 Part And CLR Scalar Type",
            ApiName = "PrimaryKey",
            ApiIdentityParts = [new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "Id", clrScalarTypeHint: typeof(Guid))],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "PrimaryKey",
                apiIdentityParts: [new ApiIdentityScalarPart("Id", typeof(Guid))]
            )
        },
        new BuildTest
        {
            Name = "Builds Simple Identity With 1 Part And CLR Scalar Type And Extensions",
            ApiName = "PrimaryKey",
            ApiIdentityParts = [new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "Id", clrScalarTypeHint: typeof(Guid), extensionType: typeof(GraphQlExtension))],
            ExtensionType = typeof(JsonApiExtension),
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "PrimaryKey",
                apiIdentityParts:
                [
                    new ApiIdentityScalarPart("Id", typeof(Guid))
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
            ApiIdentityParts = [new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "TenantId"), new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "UserId")],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentityParts: [new ApiIdentityScalarPart("TenantId"), new ApiIdentityScalarPart("UserId")]
            )
        },
        new BuildTest
        {
            Name = "Builds Composite Identity With 2 Parts And CLR Scalar Types",
            ApiName = "CompositeKey",
            ApiIdentityParts = [new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "TenantId", clrScalarTypeHint: typeof(Ulid)), new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "UserId", clrScalarTypeHint: typeof(int))],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentityParts: [new ApiIdentityScalarPart("TenantId", typeof(Ulid)), new ApiIdentityScalarPart("UserId", typeof(int))]
            )
        },
        new BuildTest
        {
            Name = "Builds Composite Identity With 2 Parts And CLR Scalar Types And Extensions",
            ApiName = "CompositeKey",
            ApiIdentityParts = [new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "TenantId", clrScalarTypeHint: typeof(Ulid), extensionType: typeof(GraphQlExtension)), new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "UserId", clrScalarTypeHint: typeof(int), extensionType: typeof(GraphQlExtension))],
            ExtensionType = typeof(JsonApiExtension),
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentityParts:
                [
                    new ApiIdentityScalarPart("TenantId", typeof(Ulid))
                    {
                        Extensions = new OrderedDictionary<Type, object>
                        {
                            [typeof(GraphQlExtension)] = new GraphQlExtension()
                        }
                    },
                    new ApiIdentityScalarPart("UserId", typeof(int))
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
            ApiIdentityParts = [new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "CompanyId"), new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "OrderId"), new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "ItemId")],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentityParts: [new ApiIdentityScalarPart("CompanyId"), new ApiIdentityScalarPart("OrderId"), new ApiIdentityScalarPart("ItemId")]
            )
        },
        new BuildTest
        {
            Name = "Builds Composite Identity With 3 Parts And CLR Scalar Types",
            ApiName = "CompositeKey",
            ApiIdentityParts = [new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "CompanyId", clrScalarTypeHint: typeof(Ulid)), new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "OrderId", clrScalarTypeHint: typeof(int)), new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "ItemId", clrScalarTypeHint: typeof(string))],
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentityParts: [new ApiIdentityScalarPart("CompanyId", typeof(Ulid)), new ApiIdentityScalarPart("OrderId", typeof(int)), new ApiIdentityScalarPart("ItemId", typeof(string))]
            )
        },
        new BuildTest
        {
            Name = "Builds Composite Identity With 3 Parts And CLR Scalar Types And Extensions",
            ApiName = "CompositeKey",
            ApiIdentityParts = [new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "CompanyId", clrScalarTypeHint: typeof(Ulid), extensionType: typeof(GraphQlExtension)), new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "OrderId", clrScalarTypeHint: typeof(int), extensionType: typeof(GraphQlExtension)), new ApiIdentityPartConfig(apiKind: ApiIdentityPartKind.Scalar, clrPropertyName: "ItemId", clrScalarTypeHint: typeof(string), extensionType: typeof(GraphQlExtension))],
            ExtensionType = typeof(JsonApiExtension),
            ApiIdentityExpected = new ApiIdentity
            (
                apiName: "CompositeKey",
                apiIdentityParts:
                [
                    new ApiIdentityScalarPart("CompanyId", typeof(Ulid))
                    {
                        Extensions = new OrderedDictionary<Type, object>
                        {
                            [typeof(GraphQlExtension)] = new GraphQlExtension()
                        }
                    },
                    new ApiIdentityScalarPart("OrderId", typeof(int))
                    {
                        Extensions = new OrderedDictionary<Type, object>
                        {
                            [typeof(GraphQlExtension)] = new GraphQlExtension()
                        }
                    },
                    new ApiIdentityScalarPart("ItemId", typeof(string))
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
