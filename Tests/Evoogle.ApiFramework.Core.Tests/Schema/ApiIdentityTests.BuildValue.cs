// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiIdentityTests
{
    #region Test Types
    private class BuildValueFromInstanceTest : XUnitTest
    {
        #region User Supplied Properties
        public required string ApiObjectTypeName { get; init; }
        public string? ApiIdentityName { get; init; }
        public required object ClrInstance { get; init; }
        public object? ClrOwnerInstance { get; init; }
        public ApiIdentityPartNullHandling NullHandling { get; init; }
        public ApiIdentityValue? ExpectedValue { get; init; }
        public Type? ExpectedExceptionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiIdentity? ApiIdentity { get; set; }
        private ApiIdentityValue? ActualValue { get; set; }
        private Type? ActualExceptionType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ApiIdentity = this.ApiIdentityName is not null
                ? GetIdentityByName(this.ApiObjectTypeName, this.ApiIdentityName)
                : GetPrimaryIdentity(this.ApiObjectTypeName);

            this.WriteLine($"ApiObjectType: {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiIdentity:   {this.ApiIdentity.ApiName.SafeToString()}");
            this.WriteLine($"ClrInstance:   {this.ClrInstance.SafeToString()}");
            this.WriteLine($"ClrOwner:      {this.ClrOwnerInstance.SafeToString()}");
            this.WriteLine($"NullHandling:  {this.NullHandling.SafeToString()}");
            this.WriteLine();

            if (this.ExpectedValue is not null)
            {
                this.WriteLine($"Expected Value: {this.ExpectedValue.SafeToString()}");
            }
            else if (this.ExpectedExceptionType is not null)
            {
                this.WriteLine($"Expected Exception: {this.ExpectedExceptionType.SafeToName()}");
            }
        }

        protected override void Act()
        {
            try
            {
                var context = new ApiIdentityValueBuildContext
                {
                    ClrInstance = this.ClrInstance,
                    ClrOwnerInstance = this.ClrOwnerInstance,
                    NullHandling = this.NullHandling
                };
                this.ActualValue = this.ApiIdentity!.BuildValue(context);
                this.WriteLine($"Actual Value:   {this.ActualValue.SafeToString()}");
            }
            catch (Exception ex)
            {
                this.ActualExceptionType = ex.GetType();
                this.WriteLine($"Actual Exception:   {this.ActualExceptionType.SafeToName()} - {ex.Message}");
            }
        }

        protected override void Assert()
        {
            if (this.ExpectedValue is not null)
            {
                this.ActualExceptionType.Should().BeNull();
                this.ActualValue.Should().NotBeNull();
                this.ActualValue.Should().BeEquivalentTo(this.ExpectedValue, options => options
                    .Excluding(ctx => ctx.Path.EndsWith(nameof(ApiIdentityValue.ApiScalarValue), StringComparison.Ordinal))
                    .Excluding(ctx => ctx.Path.EndsWith(nameof(ApiIdentityValue.ApiObjectValue), StringComparison.Ordinal)));
            }
            else if (this.ExpectedExceptionType is not null)
            {
                this.ActualExceptionType.Should().NotBeNull();
                this.ActualExceptionType.Should().Be(this.ExpectedExceptionType);
            }
        }
        #endregion
    }

    private class BuildValueFromValuesTest : XUnitTest
    {
        #region User Supplied Properties
        public required string ApiObjectTypeName { get; init; }
        public string? ApiIdentityName { get; init; }
        public required IReadOnlyDictionary<string, object?> Values { get; init; }
        public IReadOnlyDictionary<string, object?>? OwnerValues { get; init; }
        public ApiIdentityPartNullHandling NullHandling { get; init; }
        public ApiIdentityValue? ExpectedValue { get; init; }
        public Type? ExpectedExceptionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiIdentity? ApiIdentity { get; set; }
        private ApiIdentityValue? ActualValue { get; set; }
        private Type? ActualExceptionType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ApiIdentity = this.ApiIdentityName is not null
                ? GetIdentityByName(this.ApiObjectTypeName, this.ApiIdentityName)
                : GetPrimaryIdentity(this.ApiObjectTypeName);

            static string FormatValue(object? value) =>
                value is IReadOnlyDictionary<string, object?> nested
                    ? $"{{{string.Join(',', nested.Select(kvp => $"{kvp.Key}={FormatValue(kvp.Value)}"))}}}"
                    : value.SafeToString();

            var valuesDisplay = this.Values is not null
                ? $"[{string.Join(',', this.Values.Select(kvp => $"{kvp.Key.SafeToString()}={FormatValue(kvp.Value)}"))}]"
                : "<null>";

            var ownerValuesDisplay = this.OwnerValues is not null
                ? $"[{string.Join(',', this.OwnerValues.Select(kvp => $"{kvp.Key.SafeToString()}={FormatValue(kvp.Value)}"))}]"
                : "<null>";

            this.WriteLine($"ApiObjectType: {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiIdentity:   {this.ApiIdentity.ApiName.SafeToString()}");
            this.WriteLine($"Values:        {valuesDisplay}");
            this.WriteLine($"OwnerValues:   {ownerValuesDisplay}");
            this.WriteLine($"NullHandling:  {this.NullHandling.SafeToString()}");
            this.WriteLine();

            if (this.ExpectedValue is not null)
            {
                this.WriteLine($"Expected Value: {this.ExpectedValue.SafeToString()}");
            }
            else if (this.ExpectedExceptionType is not null)
            {
                this.WriteLine($"Expected Exception: {this.ExpectedExceptionType.SafeToName()}");
            }
        }

        protected override void Act()
        {
            try
            {
                var context = new ApiIdentityValueBuildFromValuesContext
                {
                    Values = this.Values,
                    OwnerValues = this.OwnerValues,
                    NullHandling = this.NullHandling
                };
                this.ActualValue = this.ApiIdentity!.BuildValue(context);
                this.WriteLine($"Actual Value:   {this.ActualValue.SafeToString()}");
            }
            catch (Exception ex)
            {
                this.ActualExceptionType = ex.GetType();
                this.WriteLine($"Actual Exception:   {this.ActualExceptionType.SafeToName()} - {ex.Message}");
            }
        }

        protected override void Assert()
        {
            if (this.ExpectedValue is not null)
            {
                this.ActualExceptionType.Should().BeNull();
                this.ActualValue.Should().NotBeNull();
                this.ActualValue.Should().BeEquivalentTo(this.ExpectedValue, options => options
                    .Excluding(ctx => ctx.Path.EndsWith(nameof(ApiIdentityValue.ApiScalarValue), StringComparison.Ordinal))
                    .Excluding(ctx => ctx.Path.EndsWith(nameof(ApiIdentityValue.ApiObjectValue), StringComparison.Ordinal)));
            }
            else if (this.ExpectedExceptionType is not null)
            {
                this.ActualExceptionType.Should().NotBeNull();
                this.ActualExceptionType.Should().Be(this.ExpectedExceptionType);
            }
        }
        #endregion
    }

    private class BuildValueNullContextTest : XUnitTest
    {
        #region User Supplied Properties
        public required string ApiObjectTypeName { get; init; }
        public required bool UseInstanceOverload { get; init; }
        #endregion

        #region Calculated Properties
        private ApiIdentity? ApiIdentity { get; set; }
        private Type? ActualExceptionType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ApiIdentity = GetPrimaryIdentity(this.ApiObjectTypeName);

            this.WriteLine($"ApiObjectType:       {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"UseInstanceOverload: {this.UseInstanceOverload}");
        }

        protected override void Act()
        {
            try
            {
                if (this.UseInstanceOverload)
                {
                    this.ApiIdentity!.BuildValue((ApiIdentityValueBuildContext)null!);
                }
                else
                {
                    this.ApiIdentity!.BuildValue((ApiIdentityValueBuildFromValuesContext)null!);
                }
            }
            catch (Exception ex)
            {
                this.ActualExceptionType = ex.GetType();
                this.WriteLine($"Actual Exception: {this.ActualExceptionType.SafeToName()}");
            }
        }

        protected override void Assert()
        {
            this.ActualExceptionType.Should().NotBeNull();
            this.ActualExceptionType.Should().Be(typeof(ArgumentNullException));
        }
        #endregion
    }
    #endregion

    #region BuildValueFromInstance Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildValueFromInstanceTheoryData =>
    [
        // Scalar identity — single part (int)
        new BuildValueFromInstanceTest
        {
            Name = $"{ScalarInstance} with primary identity",
            ApiObjectTypeName = nameof(IdentityScalar),
            ApiIdentityName = "PK_IdentityScalar",
            ClrInstance = ScalarInstance,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(42))
            ])
        },

        // Scalar identity — alternate key (string)
        new BuildValueFromInstanceTest
        {
            Name = $"{ScalarInstance} with alternate identity",
            ApiObjectTypeName = nameof(IdentityScalar),
            ApiIdentityName = "AK_IdentityScalar",
            ClrInstance = ScalarInstance,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityScalarPartValue("Name", ApiId.FromString("TestName"))
            ])
        },

        // Composite identity — two scalar parts (int + string)
        new BuildValueFromInstanceTest
        {
            Name = $"{TwoPartInstance}",
            ApiObjectTypeName = nameof(IdentityTwoScalarPartComposite),
            ClrInstance = TwoPartInstance,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityScalarPartValue("Id1", ApiId.FromInt32(1)),
                new ApiIdentityScalarPartValue("Id2", ApiId.FromString("abc"))
            ])
        },

        // Composite identity — three scalar parts (int + string + Guid)
        new BuildValueFromInstanceTest
        {
            Name = $"{ThreePartInstance}",
            ApiObjectTypeName = nameof(IdentityThreeScalarPartComposite),
            ClrInstance = ThreePartInstance,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityScalarPartValue("Id1", ApiId.FromInt32(10)),
                new ApiIdentityScalarPartValue("Id2", ApiId.FromString("xyz")),
                new ApiIdentityScalarPartValue("Id3", ApiId.FromGuid(Guid.Parse("11111111-1111-1111-1111-111111111111")))
            ])
        },

        // Nested identity — composite with nested object part
        new BuildValueFromInstanceTest
        {
            Name = $"{NestedCompositeInstance}",
            ApiObjectTypeName = nameof(IdentityNestedComposite),
            ClrInstance = NestedCompositeInstance,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityObjectPartValue("NestedPart",
                    ApiIdentityValue.Composite(
                    [
                        new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(5))
                    ])),
                new ApiIdentityScalarPartValue("Name", ApiId.FromString("Nested"))
            ])
        },

        // Owner identity — owned composite with owner instance
        new BuildValueFromInstanceTest
        {
            Name = $"{OwnedCompositeInstance} with owner {OwnerInstance}",
            ApiObjectTypeName = nameof(IdentityOwnedComposite),
            ClrInstance = OwnedCompositeInstance,
            ClrOwnerInstance = OwnerInstance,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityObjectPartValue("IdentityOwner",
                    ApiIdentityValue.Composite(
                    [
                        new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(99))
                    ])),
                new ApiIdentityScalarPartValue("LineNumber", ApiId.FromInt32(3))
            ])
        },

        // Owner identity — owned dependent with owner only
        new BuildValueFromInstanceTest
        {
            Name = $"{OwnedDependentInstance} with owner {OwnerInstance}",
            ApiObjectTypeName = nameof(IdentityOwnedDependent),
            ClrInstance = OwnedDependentInstance,
            ClrOwnerInstance = OwnerInstance,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityObjectPartValue("IdentityOwner",
                    ApiIdentityValue.Composite(
                    [
                        new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(99))
                    ]))
            ])
        },

        // Null handling — UseDefaultOnNull: scalar with null value
        new BuildValueFromInstanceTest
        {
            Name = $"{TwoPartNullId2Instance} returns empty with {ApiIdentityPartNullHandling.UseDefaultOnNull}",
            ApiObjectTypeName = nameof(IdentityTwoScalarPartComposite),
            ClrInstance = TwoPartNullId2Instance,
            NullHandling = ApiIdentityPartNullHandling.UseDefaultOnNull,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityScalarPartValue("Id1", ApiId.FromInt32(1)),
                new ApiIdentityScalarPartValue("Id2", ApiId.Empty)
            ])
        },

        // Null handling — UseDefaultOnNull: nested with null object
        new BuildValueFromInstanceTest
        {
            Name = $"{NestedCompositeNullNestedPartInstance} returns skeleton with {ApiIdentityPartNullHandling.UseDefaultOnNull}",
            ApiObjectTypeName = nameof(IdentityNestedComposite),
            ClrInstance = NestedCompositeNullNestedPartInstance,
            NullHandling = ApiIdentityPartNullHandling.UseDefaultOnNull,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityObjectPartValue("NestedPart", apiObjectValue: null, apiStructure:
                [
                    new ApiIdentityScalarPartValue("Id", ApiId.Empty)
                ]),
                new ApiIdentityScalarPartValue("Name", ApiId.FromString("NoNested"))
            ])
        },

        // Null handling — UseDefaultOnNull: owner with null owner instance
        new BuildValueFromInstanceTest
        {
            Name = $"{OwnedCompositeInstance} with null owner returns skeleton with {ApiIdentityPartNullHandling.UseDefaultOnNull}",
            ApiObjectTypeName = nameof(IdentityOwnedComposite),
            ClrInstance = OwnedCompositeInstance,
            ClrOwnerInstance = null,
            NullHandling = ApiIdentityPartNullHandling.UseDefaultOnNull,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityObjectPartValue("IdentityOwner", apiObjectValue: null, apiStructure:
                [
                    new ApiIdentityScalarPartValue("Id", ApiId.Empty)
                ]),
                new ApiIdentityScalarPartValue("LineNumber", ApiId.FromInt32(3))
            ])
        },

        // Null handling — ThrowOnNull: scalar with null value
        new BuildValueFromInstanceTest
        {
            Name = $"{TwoPartNullId2Instance} throws with {ApiIdentityPartNullHandling.ThrowOnNull}",
            ApiObjectTypeName = nameof(IdentityTwoScalarPartComposite),
            ClrInstance = TwoPartNullId2Instance,
            NullHandling = ApiIdentityPartNullHandling.ThrowOnNull,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },

        // Null handling — ThrowOnNull: nested with null object
        new BuildValueFromInstanceTest
        {
            Name = $"{NestedCompositeNullNestedPartInstance} throws with {ApiIdentityPartNullHandling.ThrowOnNull}",
            ApiObjectTypeName = nameof(IdentityNestedComposite),
            ClrInstance = NestedCompositeNullNestedPartInstance,
            NullHandling = ApiIdentityPartNullHandling.ThrowOnNull,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },

        // Null handling — ThrowOnNull: owner with null owner instance
        new BuildValueFromInstanceTest
        {
            Name = $"{OwnedCompositeInstance} with null owner throws with {ApiIdentityPartNullHandling.ThrowOnNull}",
            ApiObjectTypeName = nameof(IdentityOwnedComposite),
            ClrInstance = OwnedCompositeInstance,
            ClrOwnerInstance = null,
            NullHandling = ApiIdentityPartNullHandling.ThrowOnNull,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },
    ];
    #endregion

    #region BuildValueFromValues Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildValueFromValuesTheoryData =>
    [
        // Scalar identity from dictionary (int)
        new BuildValueFromValuesTest
        {
            Name = $"{nameof(IdentityScalar)} with primary identity",
            ApiObjectTypeName = nameof(IdentityScalar),
            ApiIdentityName = "PK_IdentityScalar",
            Values = new Dictionary<string, object?> { ["Id"] = 42 },
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(42))
            ])
        },

        // Alternate key from dictionary (string)
        new BuildValueFromValuesTest
        {
            Name = $"{nameof(IdentityScalar)} with alternate identity",
            ApiObjectTypeName = nameof(IdentityScalar),
            ApiIdentityName = "AK_IdentityScalar",
            Values = new Dictionary<string, object?> { ["Name"] = "TestName" },
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityScalarPartValue("Name", ApiId.FromString("TestName"))
            ])
        },

        // Composite identity from dictionary — two parts
        new BuildValueFromValuesTest
        {
            Name = $"{nameof(IdentityTwoScalarPartComposite)}",
            ApiObjectTypeName = nameof(IdentityTwoScalarPartComposite),
            Values = new Dictionary<string, object?> { ["Id1"] = 1, ["Id2"] = "abc" },
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityScalarPartValue("Id1", ApiId.FromInt32(1)),
                new ApiIdentityScalarPartValue("Id2", ApiId.FromString("abc"))
            ])
        },

        // Composite identity from dictionary — three parts
        new BuildValueFromValuesTest
        {
            Name = $"{nameof(IdentityThreeScalarPartComposite)}",
            ApiObjectTypeName = nameof(IdentityThreeScalarPartComposite),
            Values = new Dictionary<string, object?>
            {
                ["Id1"] = 10,
                ["Id2"] = "xyz",
                ["Id3"] = Guid.Parse("11111111-1111-1111-1111-111111111111")
            },
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityScalarPartValue("Id1", ApiId.FromInt32(10)),
                new ApiIdentityScalarPartValue("Id2", ApiId.FromString("xyz")),
                new ApiIdentityScalarPartValue("Id3", ApiId.FromGuid(Guid.Parse("11111111-1111-1111-1111-111111111111")))
            ])
        },

        // Nested identity from dictionary — nested dict
        new BuildValueFromValuesTest
        {
            Name = $"{nameof(IdentityNestedComposite)} with nested dictionary",
            ApiObjectTypeName = nameof(IdentityNestedComposite),
            Values = new Dictionary<string, object?>
            {
                ["NestedPart"] = new Dictionary<string, object?> { ["Id"] = 5 },
                ["Name"] = "Nested"
            },
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityObjectPartValue("NestedPart",
                    ApiIdentityValue.Composite(
                    [
                        new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(5))
                    ])),
                new ApiIdentityScalarPartValue("Name", ApiId.FromString("Nested"))
            ])
        },

        // Nested identity from dictionary — CLR object fallback
        new BuildValueFromValuesTest
        {
            Name = $"{nameof(IdentityNestedComposite)} with CLR instance fallback",
            ApiObjectTypeName = nameof(IdentityNestedComposite),
            Values = new Dictionary<string, object?>
            {
                ["NestedPart"] = NestedPartInstance,
                ["Name"] = "Nested"
            },
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityObjectPartValue("NestedPart",
                    ApiIdentityValue.Composite(
                    [
                        new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(5))
                    ])),
                new ApiIdentityScalarPartValue("Name", ApiId.FromString("Nested"))
            ])
        },

        // Owner identity from dictionary — with owner values
        new BuildValueFromValuesTest
        {
            Name = $"{nameof(IdentityOwnedComposite)}",
            ApiObjectTypeName = nameof(IdentityOwnedComposite),
            Values = new Dictionary<string, object?> { ["LineNumber"] = 3 },
            OwnerValues = new Dictionary<string, object?> { ["Id"] = 99 },
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityObjectPartValue("IdentityOwner",
                    ApiIdentityValue.Composite(
                    [
                        new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(99))
                    ])),
                new ApiIdentityScalarPartValue("LineNumber", ApiId.FromInt32(3))
            ])
        },

        // Owner-only identity from dictionary
        new BuildValueFromValuesTest
        {
            Name = $"{nameof(IdentityOwnedDependent)}",
            ApiObjectTypeName = nameof(IdentityOwnedDependent),
            Values = new Dictionary<string, object?>(),
            OwnerValues = new Dictionary<string, object?> { ["Id"] = 99 },
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityObjectPartValue("IdentityOwner",
                    ApiIdentityValue.Composite(
                    [
                        new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(99))
                    ]))
            ])
        },

        // Null handling — UseDefaultOnNull: missing key in dictionary
        new BuildValueFromValuesTest
        {
            Name = $"Missing key in dictionary returns empty with {ApiIdentityPartNullHandling.UseDefaultOnNull}",
            ApiObjectTypeName = nameof(IdentityTwoScalarPartComposite),
            Values = new Dictionary<string, object?> { ["Id1"] = 1 },
            NullHandling = ApiIdentityPartNullHandling.UseDefaultOnNull,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityScalarPartValue("Id1", ApiId.FromInt32(1)),
                new ApiIdentityScalarPartValue("Id2", ApiId.Empty)
            ])
        },

        // Null handling — UseDefaultOnNull: null value in dictionary
        new BuildValueFromValuesTest
        {
            Name = $"Null value in dictionary returns empty with {ApiIdentityPartNullHandling.UseDefaultOnNull}",
            ApiObjectTypeName = nameof(IdentityTwoScalarPartComposite),
            Values = new Dictionary<string, object?> { ["Id1"] = 1, ["Id2"] = null },
            NullHandling = ApiIdentityPartNullHandling.UseDefaultOnNull,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityScalarPartValue("Id1", ApiId.FromInt32(1)),
                new ApiIdentityScalarPartValue("Id2", ApiId.Empty)
            ])
        },

        // Null handling — UseDefaultOnNull: null nested value
        new BuildValueFromValuesTest
        {
            Name = $"Null nested value in dictionary returns skeleton with {ApiIdentityPartNullHandling.UseDefaultOnNull}",
            ApiObjectTypeName = nameof(IdentityNestedComposite),
            Values = new Dictionary<string, object?> { ["NestedPart"] = null, ["Name"] = "NoNested" },
            NullHandling = ApiIdentityPartNullHandling.UseDefaultOnNull,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityObjectPartValue("NestedPart", apiObjectValue: null, apiStructure:
                [
                    new ApiIdentityScalarPartValue("Id", ApiId.Empty)
                ]),
                new ApiIdentityScalarPartValue("Name", ApiId.FromString("NoNested"))
            ])
        },

        // Null handling — UseDefaultOnNull: null owner values
        new BuildValueFromValuesTest
        {
            Name = $"Null owner values returns skeleton with {ApiIdentityPartNullHandling.UseDefaultOnNull}",
            ApiObjectTypeName = nameof(IdentityOwnedComposite),
            Values = new Dictionary<string, object?> { ["LineNumber"] = 3 },
            OwnerValues = null,
            NullHandling = ApiIdentityPartNullHandling.UseDefaultOnNull,
            ExpectedValue = ApiIdentityValue.Composite(
            [
                new ApiIdentityObjectPartValue("IdentityOwner", apiObjectValue: null, apiStructure:
                [
                    new ApiIdentityScalarPartValue("Id", ApiId.Empty)
                ]),
                new ApiIdentityScalarPartValue("LineNumber", ApiId.FromInt32(3))
            ])
        },

        // Null handling — ThrowOnNull: missing key
        new BuildValueFromValuesTest
        {
            Name = $"Missing key in dictionary throws with {ApiIdentityPartNullHandling.ThrowOnNull}",
            ApiObjectTypeName = nameof(IdentityTwoScalarPartComposite),
            Values = new Dictionary<string, object?> { ["Id1"] = 1 },
            NullHandling = ApiIdentityPartNullHandling.ThrowOnNull,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },

        // Null handling — ThrowOnNull: null value
        new BuildValueFromValuesTest
        {
            Name = $"Null value in dictionary throws with {ApiIdentityPartNullHandling.ThrowOnNull}",
            ApiObjectTypeName = nameof(IdentityTwoScalarPartComposite),
            Values = new Dictionary<string, object?> { ["Id1"] = 1, ["Id2"] = null },
            NullHandling = ApiIdentityPartNullHandling.ThrowOnNull,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },

        // Null handling — ThrowOnNull: null nested value
        new BuildValueFromValuesTest
        {
            Name = $"Null nested value in dictionary throws with {ApiIdentityPartNullHandling.ThrowOnNull}",
            ApiObjectTypeName = nameof(IdentityNestedComposite),
            Values = new Dictionary<string, object?> { ["NestedPart"] = null, ["Name"] = "Nested" },
            NullHandling = ApiIdentityPartNullHandling.ThrowOnNull,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },

        // Null handling — ThrowOnNull: null owner values
        new BuildValueFromValuesTest
        {
            Name = $"Null owner values throws with {ApiIdentityPartNullHandling.ThrowOnNull}",
            ApiObjectTypeName = nameof(IdentityOwnedComposite),
            Values = new Dictionary<string, object?> { ["LineNumber"] = 3 },
            OwnerValues = null,
            NullHandling = ApiIdentityPartNullHandling.ThrowOnNull,
            ExpectedExceptionType = typeof(ApiIdentityException)
        },
    ];
    #endregion

    #region BuildValueNullContext Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildValueNullContextTheoryData =>
    [
        new BuildValueNullContextTest
        {
            Name = "Null context throws ArgumentNullException for instance overload",
            ApiObjectTypeName = nameof(IdentityScalar),
            UseInstanceOverload = true
        },
        new BuildValueNullContextTest
        {
            Name = "Null context throws ArgumentNullException for values overload",
            ApiObjectTypeName = nameof(IdentityScalar),
            UseInstanceOverload = false
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildValueFromInstanceTheoryData))]
    public void BuildValueFromInstance(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(BuildValueFromValuesTheoryData))]
    public void BuildValueFromValues(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(BuildValueNullContextTheoryData))]
    public void BuildValueNullContext(IXUnitTest test) => test.Execute(this);
    #endregion
}
