// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Key;
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;
using Evoogle.XUnit.Json;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiKeyTypeTests
{
    #region Test Fields
    private static readonly IReadOnlyDictionary<string, ApiKeyPartNameFormatterDelegate> _customPartNameFormats = new Dictionary<string, ApiKeyPartNameFormatterDelegate>
    {
        ["Custom"] = static c => $"{c.ApiKeyType.ApiName ?? "(anonymous)"}[{c.PartIndex}]"
    };
    #endregion

    #region Test Types
    private class MaterializeKeyFromInstanceTest : XUnitTest
    {
        #region User Supplied Properties
        public required string ApiObjectTypeName { get; init; }
        public string? ApiKeyTypeName { get; init; }
        public object? SelfObject { get; init; }
        public object? OwnerObject { get; init; }
        public ApiKeyNullHandling NullHandling { get; init; }
        public ApiKeyPartNameFormat? PartNameFormat { get; init; }
        public string? CustomPartNameFormatterName { get; init; }
        public ApiKey? ExpectedValue { get; init; }
        public Type? ExpectedExceptionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiKeyType? ApiKeyType { get; set; }
        private ApiKeyPartNameFormatterDelegate? PartNameFormatter { get; set; }
        private ApiKey? ActualValue { get; set; }
        private Type? ActualExceptionType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ApiKeyType = this.ApiKeyTypeName is not null
                ? GetKeyTypeByName(this.ApiObjectTypeName, this.ApiKeyTypeName)
                : GetPrimaryKeyType(this.ApiObjectTypeName);

            if (this.CustomPartNameFormatterName is not null)
            {
                if (!_customPartNameFormats.TryGetValue(this.CustomPartNameFormatterName, out var formatter))
                {
                    throw new InvalidOperationException($"Custom part name formatter with name '{this.CustomPartNameFormatterName}' not found.");
                }

                this.PartNameFormatter = formatter;
            }

            this.WriteLine($"ApiObjectType:     {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiKeyType:        {this.ApiKeyType?.ToString().SafeToString()}");
            this.WriteLine($"SelfObject:        {this.SelfObject.SafeToString()}");
            this.WriteLine($"OwnerObject:       {this.OwnerObject.SafeToString()}");
            this.WriteLine($"NullHandling:      {this.NullHandling.SafeToString()}");
            this.WriteLine($"PartNameFormat:   {this.PartNameFormat.SafeToString()}");
            this.WriteLine($"CustomFormatter:     {this.CustomPartNameFormatterName.SafeToString()}");
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
                var context = new ApiKeyMaterializationContext
                {
                    NullHandling = this.NullHandling,
                    PartNameFormat = this.PartNameFormat.GetValueOrDefault(ApiKeyPartNameFormat.None),
                    PartNameFormatter = this.PartNameFormatter
                };

                if (this.SelfObject is not null)
                {
                    context.WithObject(this.SelfObject);
                }

                if (this.OwnerObject is not null)
                {
                    context.WithObject(this.OwnerObject);
                }

                this.ActualValue = this.ApiKeyType!.MaterializeKey(context);
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
                this.ActualValue.Should().BeEquivalentTo(this.ExpectedValue);
            }
            else if (this.ExpectedExceptionType is not null)
            {
                this.ActualExceptionType.Should().NotBeNull();
                this.ActualExceptionType.Should().Be(this.ExpectedExceptionType);
                this.ActualValue.Should().BeNull();
            }
        }
        #endregion
    }

    private class MaterializeKeyFromValuesTest : XUnitTest
    {
        #region User Supplied Properties
        public required string ApiObjectTypeName { get; init; }
        public string? ApiKeyTypeName { get; init; }

        [JsonConverter(typeof(ExpressionActionJsonConverter<ApiKeyMaterializationContext>))]
        public required Expression<Action<ApiKeyMaterializationContext>> ConfigureValuesExpression { get; init; }

        public ApiKeyNullHandling NullHandling { get; init; }
        public ApiKeyPartNameFormat? PartNameFormat { get; init; }
        public string? CustomPartNameFormatterName { get; init; }
        public ApiKey? ExpectedValue { get; init; }
        public Type? ExpectedExceptionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiKeyType? ApiKeyType { get; set; }
        private ApiKeyPartNameFormatterDelegate? PartNameFormatter { get; set; }
        private ApiKey? ActualValue { get; set; }
        private Type? ActualExceptionType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ApiKeyType = this.ApiKeyTypeName is not null
                ? GetKeyTypeByName(this.ApiObjectTypeName, this.ApiKeyTypeName)
                : GetPrimaryKeyType(this.ApiObjectTypeName);

            if (this.CustomPartNameFormatterName is not null)
            {
                if (!_customPartNameFormats.TryGetValue(this.CustomPartNameFormatterName, out var formatter))
                {
                    throw new InvalidOperationException($"Custom part name formatter with name '{this.CustomPartNameFormatterName}' not found.");
                }

                this.PartNameFormatter = formatter;
            }

            this.WriteLine($"ApiObjectType:   {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiKeyType:      {this.ApiKeyType?.ToString().SafeToString()}");
            this.WriteLine($"NullHandling:    {this.NullHandling.SafeToString()}");
            this.WriteLine($"PartNameFormat: {this.PartNameFormat.SafeToString()}");
            this.WriteLine($"CustomFormatter:   {this.CustomPartNameFormatterName.SafeToString()}");
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
                var context = new ApiKeyMaterializationContext
                {
                    NullHandling = this.NullHandling,
                    PartNameFormat = this.PartNameFormat.GetValueOrDefault(ApiKeyPartNameFormat.None),
                    PartNameFormatter = this.PartNameFormatter
                };

                this.ConfigureValuesExpression.Compile()(context);

                this.ActualValue = this.ApiKeyType!.MaterializeKeyFromValues(context);
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
                this.ActualValue.Should().BeEquivalentTo(this.ExpectedValue);
            }
            else if (this.ExpectedExceptionType is not null)
            {
                this.ActualExceptionType.Should().NotBeNull();
                this.ActualExceptionType.Should().Be(this.ExpectedExceptionType);
                this.ActualValue.Should().BeNull();
            }
        }
        #endregion
    }
    #endregion

    #region MaterializeKeyFromInstance Theory Data
    public static TheoryDataRow<IXUnitTest>[] MaterializeKeyFromInstanceTheoryData =>
    [
        // KeyOneScalarPartInstance ///////////////////////////////////////////////////////////////////////////////////
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOneScalarPartInstance} with primary scalar key (int) and none name format",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "PK_KeyOneScalarPart",
            SelfObject = KeyOneScalarPartInstance,
            PartNameFormat = ApiKeyPartNameFormat.None,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(null, ApiKey.FromInt32(KeyOneScalarPartInstance.Id)))
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOneScalarPartInstance} with primary scalar key (int) and CLR path only name format",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "PK_KeyOneScalarPart",
            SelfObject = KeyOneScalarPartInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOneScalarPart.Id), ApiKey.FromInt32(KeyOneScalarPartInstance.Id)))
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOneScalarPartInstance} with primary scalar key (int) and CLR root and path name format",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "PK_KeyOneScalarPart",
            SelfObject = KeyOneScalarPartInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOneScalarPart) + "." + nameof(KeyOneScalarPart.Id), ApiKey.FromInt32(KeyOneScalarPartInstance.Id)))
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOneScalarPartInstance} with alternate scalar key (string) and none name format",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "AK_KeyOneScalarPart",
            SelfObject = KeyOneScalarPartInstance,
            PartNameFormat = ApiKeyPartNameFormat.None,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(null, ApiKey.FromString(KeyOneScalarPartInstance.Name)))
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOneScalarPartInstance} with alternate scalar key (string) and CLR path only name format",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "AK_KeyOneScalarPart",
            SelfObject = KeyOneScalarPartInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOneScalarPart.Name), ApiKey.FromString(KeyOneScalarPartInstance.Name)))
        },

        // KeyOneScalarPartInstance - alternate scalar key (string) and CLR root and path name format
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOneScalarPartInstance} with alternate scalar key (string) and CLR root and path name format",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "AK_KeyOneScalarPart",
            SelfObject = KeyOneScalarPartInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOneScalarPart) + "." + nameof(KeyOneScalarPart.Name), ApiKey.FromString(KeyOneScalarPartInstance.Name)))
        },

        // KeyTwoScalarPartCompositeInstance //////////////////////////////////////////////////////////////////////////
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyTwoScalarPartCompositeInstance} with primary composite key (int + string) and none name format",
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            SelfObject = KeyTwoScalarPartCompositeInstance,
            PartNameFormat = ApiKeyPartNameFormat.None,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(null, ApiKey.FromInt32(KeyTwoScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(null, ApiKey.FromString(KeyTwoScalarPartCompositeInstance.Id2!))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyTwoScalarPartCompositeInstance} with primary composite key (int + string) and CLR path only name format",
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            SelfObject = KeyTwoScalarPartCompositeInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite.Id1), ApiKey.FromInt32(KeyTwoScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite.Id2), ApiKey.FromString(KeyTwoScalarPartCompositeInstance.Id2!))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyTwoScalarPartCompositeInstance} with primary composite key (int + string) and CLR root and path name format",
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            SelfObject = KeyTwoScalarPartCompositeInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite) + "." + nameof(KeyTwoScalarPartComposite.Id1), ApiKey.FromInt32(KeyTwoScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite) + "." + nameof(KeyTwoScalarPartComposite.Id2), ApiKey.FromString(KeyTwoScalarPartCompositeInstance.Id2!))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyTwoScalarPartCompositeInstance} with primary composite key (int + string) and custom name formatter",
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            SelfObject = KeyTwoScalarPartCompositeInstance,
            CustomPartNameFormatterName = "Custom",
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create("PK_KeyTwoScalarPartComposite[0]", ApiKey.FromInt32(KeyTwoScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create("PK_KeyTwoScalarPartComposite[1]", ApiKey.FromString(KeyTwoScalarPartCompositeInstance.Id2!))
            )
        },

        // KeyThreeScalarPartCompositeInstance ////////////////////////////////////////////////////////////////////////
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyThreeScalarPartCompositeInstance} with primary composite key (int + string + Guid) and none name format",
            ApiObjectTypeName = nameof(KeyThreeScalarPartComposite),
            SelfObject = KeyThreeScalarPartCompositeInstance,
            PartNameFormat = ApiKeyPartNameFormat.None,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(null, ApiKey.FromInt32(KeyThreeScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(null, ApiKey.FromString(KeyThreeScalarPartCompositeInstance.Id2!)),
                ApiKeyPart.Create(null, ApiKey.FromGuid(KeyThreeScalarPartCompositeInstance.Id3))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyThreeScalarPartCompositeInstance} with primary composite key (int + string + Guid) and CLR path only name format",
            ApiObjectTypeName = nameof(KeyThreeScalarPartComposite),
            SelfObject = KeyThreeScalarPartCompositeInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyThreeScalarPartComposite.Id1), ApiKey.FromInt32(KeyThreeScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(nameof(KeyThreeScalarPartComposite.Id2), ApiKey.FromString(KeyThreeScalarPartCompositeInstance.Id2!)),
                ApiKeyPart.Create(nameof(KeyThreeScalarPartComposite.Id3), ApiKey.FromGuid(KeyThreeScalarPartCompositeInstance.Id3))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyThreeScalarPartCompositeInstance} with primary composite key (int + string + Guid) and CLR root and path name format",
            ApiObjectTypeName = nameof(KeyThreeScalarPartComposite),
            SelfObject = KeyThreeScalarPartCompositeInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyThreeScalarPartComposite) + "." + nameof(KeyThreeScalarPartComposite.Id1), ApiKey.FromInt32(KeyThreeScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(nameof(KeyThreeScalarPartComposite) + "." + nameof(KeyThreeScalarPartComposite.Id2), ApiKey.FromString(KeyThreeScalarPartCompositeInstance.Id2!)),
                ApiKeyPart.Create(nameof(KeyThreeScalarPartComposite) + "." + nameof(KeyThreeScalarPartComposite.Id3), ApiKey.FromGuid(KeyThreeScalarPartCompositeInstance.Id3))
            )
        },

        // KeyNestedCompositeInstance /////////////////////////////////////////////////////////////////////////////////
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyNestedCompositeInstance} with primary composite key (int + string) and none name format",
            ApiObjectTypeName = nameof(KeyNestedComposite),
            SelfObject = KeyNestedCompositeInstance,
            PartNameFormat = ApiKeyPartNameFormat.None,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(null, ApiKey.FromInt32(KeyNestedCompositeInstance.NestedPart.Id)),
                ApiKeyPart.Create(null, ApiKey.FromString(KeyNestedCompositeInstance.Name!))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyNestedCompositeInstance} with primary composite key (int + string) and CLR path only name format",
            ApiObjectTypeName = nameof(KeyNestedComposite),
            SelfObject = KeyNestedCompositeInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyNestedComposite.NestedPart) + "." + nameof(KeyNested.Id), ApiKey.FromInt32(KeyNestedCompositeInstance.NestedPart.Id)),
                ApiKeyPart.Create(nameof(KeyNestedComposite.Name), ApiKey.FromString(KeyNestedCompositeInstance.Name!))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyNestedCompositeInstance} with primary composite key (int + string) and CLR root and path name format",
            ApiObjectTypeName = nameof(KeyNestedComposite),
            SelfObject = KeyNestedCompositeInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyNestedComposite) + "." + nameof(KeyNestedComposite.NestedPart) + "." + nameof(KeyNested.Id), ApiKey.FromInt32(KeyNestedCompositeInstance.NestedPart.Id)),
                ApiKeyPart.Create(nameof(KeyNestedComposite) + "." + nameof(KeyNestedComposite.Name), ApiKey.FromString(KeyNestedCompositeInstance.Name!))
            )
        },

        // KeyOwnedCompositeInstance //////////////////////////////////////////////////////////////////////////////////
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedCompositeInstance} with primary composite key (int + int) and none name format",
            ApiObjectTypeName = nameof(KeyOwnedComposite),
            SelfObject = KeyOwnedCompositeInstance,
            OwnerObject = KeyOwnerInstance,
            PartNameFormat = ApiKeyPartNameFormat.None,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(null, ApiKey.FromInt32(KeyOwnerInstance.Id)),
                ApiKeyPart.Create(null, ApiKey.FromInt32(KeyOwnedCompositeInstance.LineNumber))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedCompositeInstance} with primary composite key (int + int) and CLR path only name format",
            ApiObjectTypeName = nameof(KeyOwnedComposite),
            SelfObject = KeyOwnedCompositeInstance,
            OwnerObject = KeyOwnerInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyOwner.Id), ApiKey.FromInt32(KeyOwnerInstance.Id)),
                ApiKeyPart.Create(nameof(KeyOwnedComposite.LineNumber), ApiKey.FromInt32(KeyOwnedCompositeInstance.LineNumber))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedCompositeInstance} with primary composite key (int + int) and CLR root and path name format",
            ApiObjectTypeName = nameof(KeyOwnedComposite),
            SelfObject = KeyOwnedCompositeInstance,
            OwnerObject = KeyOwnerInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyOwner) + "." + nameof(KeyOwner.Id), ApiKey.FromInt32(KeyOwnerInstance.Id)),
                ApiKeyPart.Create(nameof(KeyOwnedComposite) + "." + nameof(KeyOwnedComposite.LineNumber), ApiKey.FromInt32(KeyOwnedCompositeInstance.LineNumber))
            )
        },

        // KeyOwnedDependentInstance //////////////////////////////////////////////////////////////////////////////////
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedDependentInstance} with primary scalar key (int) and none name format",
            ApiObjectTypeName = nameof(KeyOwnedDependent),
            SelfObject = KeyOwnedDependentInstance,
            OwnerObject = KeyOwnerInstance,
            PartNameFormat = ApiKeyPartNameFormat.None,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(null, ApiKey.FromInt32(KeyOwnerInstance.Id))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedDependentInstance} with primary scalar key (int) and CLR path only name format",
            ApiObjectTypeName = nameof(KeyOwnedDependent),
            SelfObject = KeyOwnedDependentInstance,
            OwnerObject = KeyOwnerInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyOwner.Id), ApiKey.FromInt32(KeyOwnerInstance.Id))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedDependentInstance} with primary scalar key (int) and CLR root and path name format",
            ApiObjectTypeName = nameof(KeyOwnedDependent),
            SelfObject = KeyOwnedDependentInstance,
            OwnerObject = KeyOwnerInstance,
            PartNameFormat = ApiKeyPartNameFormat.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyOwner) + "." + nameof(KeyOwner.Id), ApiKey.FromInt32(KeyOwnerInstance.Id))
            )
        },

        // Null handling — UseDefaultOnNull
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyTwoScalarPartCompositeWithNullId2PropertyInstance} returns empty key part when {nameof(ApiKeyNullHandling)}={ApiKeyNullHandling.UseDefaultOnNull}",
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            SelfObject = KeyTwoScalarPartCompositeWithNullId2PropertyInstance,
            NullHandling = ApiKeyNullHandling.UseDefaultOnNull,
            PartNameFormat = ApiKeyPartNameFormat.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite) + "." + nameof(KeyTwoScalarPartComposite.Id1), ApiKey.FromInt32(KeyTwoScalarPartCompositeWithNullId2PropertyInstance.Id1)),
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite) + "." + nameof(KeyTwoScalarPartComposite.Id2), ApiKey.Empty)
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyNestedCompositeWithNullNestedPartPropertyInstance} returns empty key part when {nameof(ApiKeyNullHandling)}={ApiKeyNullHandling.UseDefaultOnNull}",
            ApiObjectTypeName = nameof(KeyNestedComposite),
            SelfObject = KeyNestedCompositeWithNullNestedPartPropertyInstance,
            NullHandling = ApiKeyNullHandling.UseDefaultOnNull,
            PartNameFormat = ApiKeyPartNameFormat.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyNestedComposite) + "." + nameof(KeyNestedComposite.NestedPart) + "." + nameof(KeyNested.Id), ApiKey.Empty),
                ApiKeyPart.Create(nameof(KeyNestedComposite) + "." + nameof(KeyNestedComposite.Name), ApiKey.FromString(KeyNestedCompositeWithNullNestedPartPropertyInstance.Name!))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedCompositeInstance} returns empty key part when {nameof(ApiKeyNullHandling)}={ApiKeyNullHandling.UseDefaultOnNull} and null owner",
            ApiObjectTypeName = nameof(KeyOwnedComposite),
            SelfObject = KeyOwnedCompositeInstance,
            OwnerObject = null,
            NullHandling = ApiKeyNullHandling.UseDefaultOnNull,
            PartNameFormat = ApiKeyPartNameFormat.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyOwner) + "." + nameof(KeyOwner.Id), ApiKey.Empty),
                ApiKeyPart.Create(nameof(KeyOwnedComposite) + "." + nameof(KeyOwnedComposite.LineNumber), ApiKey.FromInt32(KeyOwnedCompositeInstance.LineNumber))
            )
        },

        // Null handling - ThrowOnNull
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyTwoScalarPartCompositeWithNullId2PropertyInstance} throws when {nameof(ApiKeyNullHandling)}={ApiKeyNullHandling.ThrowOnNull}",
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            SelfObject = KeyTwoScalarPartCompositeWithNullId2PropertyInstance,
            NullHandling = ApiKeyNullHandling.ThrowOnNull,
            ExpectedExceptionType = typeof(ApiKeyException)
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyNestedCompositeWithNullNestedPartPropertyInstance} throws when {nameof(ApiKeyNullHandling)}={ApiKeyNullHandling.ThrowOnNull}",
            ApiObjectTypeName = nameof(KeyNestedComposite),
            SelfObject = KeyNestedCompositeWithNullNestedPartPropertyInstance,
            NullHandling = ApiKeyNullHandling.ThrowOnNull,
            ExpectedExceptionType = typeof(ApiKeyException)
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedCompositeInstance} throws when {nameof(ApiKeyNullHandling)}={ApiKeyNullHandling.ThrowOnNull} and null owner",
            ApiObjectTypeName = nameof(KeyOwnedComposite),
            SelfObject = KeyOwnedCompositeInstance,
            OwnerObject = null,
            NullHandling = ApiKeyNullHandling.ThrowOnNull,
            ExpectedExceptionType = typeof(ApiKeyException)
        },
    ];
    #endregion

    #region MaterializeKeyFromValues Theory Data
    public static TheoryDataRow<IXUnitTest>[] MaterializeKeyFromValuesTheoryData =>
    [
        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyOneScalarPart)} values with primary scalar key (int) and CLR path only name format",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "PK_KeyOneScalarPart",
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureTextIntTerminalScalar(a),
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOneScalarPart.Id), ApiKey.FromInt32(1234)))
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyOneScalarPart)} values with alternate scalar key (string) and CLR path only name format",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "AK_KeyOneScalarPart",
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureTextStringTerminalScalar(a),
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOneScalarPart.Name), ApiKey.FromString("1234")))
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyTwoScalarPartComposite)} values with primary composite key (int + string) and CLR path only name format",
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureCompositeApiKeyValues(a),
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite.Id1), ApiKey.FromInt32(KeyTwoScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite.Id2), ApiKey.FromString(KeyTwoScalarPartCompositeInstance.Id2!))
            )
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyThreeScalarPartComposite)} values with primary composite key (int + string + Guid) and CLR path only name format",
            ApiObjectTypeName = nameof(KeyThreeScalarPartComposite),
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureTypedConvenienceValues(a),
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyThreeScalarPartComposite.Id1), ApiKey.FromInt32(KeyThreeScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(nameof(KeyThreeScalarPartComposite.Id2), ApiKey.FromString(KeyThreeScalarPartCompositeInstance.Id2!)),
                ApiKeyPart.Create(nameof(KeyThreeScalarPartComposite.Id3), ApiKey.FromGuid(KeyThreeScalarPartCompositeInstance.Id3))
            )
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyNestedComposite)} values with primary composite key (int + string) and CLR path only name format",
            ApiObjectTypeName = nameof(KeyNestedComposite),
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureNestedClrPathValues(a),
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyNestedComposite.NestedPart) + "." + nameof(KeyNested.Id), ApiKey.FromInt32(KeyNestedCompositeInstance.NestedPart.Id)),
                ApiKeyPart.Create(nameof(KeyNestedComposite.Name), ApiKey.FromString(KeyNestedCompositeInstance.Name))
            )
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyOwnedComposite)} values with primary composite key (int + int) and CLR root and path name format",
            ApiObjectTypeName = nameof(KeyOwnedComposite),
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureOwnerAndDependentRootValues(a),
            PartNameFormat = ApiKeyPartNameFormat.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyOwner) + "." + nameof(KeyOwner.Id), ApiKey.FromInt32(KeyOwnerInstance.Id)),
                ApiKeyPart.Create(nameof(KeyOwnedComposite) + "." + nameof(KeyOwnedComposite.LineNumber), ApiKey.FromInt32(KeyOwnedCompositeInstance.LineNumber))
            )
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyOwnedDependent)} values with primary scalar key (int) and CLR path only name format",
            ApiObjectTypeName = nameof(KeyOwnedDependent),
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureOwnerOnlyDependentKey(a),
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOwner.Id), ApiKey.FromInt32(KeyOwnerInstance.Id)))
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyTwoScalarPartComposite)} values with primary composite key (int + string) and custom name formatter",
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureCustomPartNameFormatterValues(a),
            CustomPartNameFormatterName = "Custom",
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create("PK_KeyTwoScalarPartComposite[0]", ApiKey.FromInt32(KeyTwoScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create("PK_KeyTwoScalarPartComposite[1]", ApiKey.FromString(KeyTwoScalarPartCompositeInstance.Id2!))
            )
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyTwoScalarPartComposite)} values return empty key part when {nameof(ApiKeyNullHandling)}={ApiKeyNullHandling.UseDefaultOnNull}",
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureMissingCompositeValue(a),
            NullHandling = ApiKeyNullHandling.UseDefaultOnNull,
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite.Id1), ApiKey.FromInt32(KeyTwoScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite.Id2), ApiKey.Empty)
            )
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyOneScalarPart)} values return empty key part when text is null and {nameof(ApiKeyNullHandling)}={ApiKeyNullHandling.UseDefaultOnNull}",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "PK_KeyOneScalarPart",
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureNullText(a),
            NullHandling = ApiKeyNullHandling.UseDefaultOnNull,
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOneScalarPart.Id), ApiKey.Empty))
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyOneScalarPart)} values return empty key part when text is whitespace and {nameof(ApiKeyNullHandling)}={ApiKeyNullHandling.UseDefaultOnNull}",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "PK_KeyOneScalarPart",
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureWhitespaceText(a),
            NullHandling = ApiKeyNullHandling.UseDefaultOnNull,
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOneScalarPart.Id), ApiKey.Empty))
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyOneScalarPart)} values return empty key part when ApiKey is empty and {nameof(ApiKeyNullHandling)}={ApiKeyNullHandling.UseDefaultOnNull}",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "PK_KeyOneScalarPart",
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureEmptyApiKey(a),
            NullHandling = ApiKeyNullHandling.UseDefaultOnNull,
            PartNameFormat = ApiKeyPartNameFormat.ClrPathOnly,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOneScalarPart.Id), ApiKey.Empty))
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyTwoScalarPartComposite)} values throw when {nameof(ApiKeyNullHandling)}={ApiKeyNullHandling.ThrowOnNull}",
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureMissingCompositeValue(a),
            NullHandling = ApiKeyNullHandling.ThrowOnNull,
            ExpectedExceptionType = typeof(ApiKeyException)
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyOneScalarPart)} values throw when primary scalar key (int) text cannot parse",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "PK_KeyOneScalarPart",
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureInvalidTextParse(a),
            NullHandling = ApiKeyNullHandling.UseDefaultOnNull,
            ExpectedExceptionType = typeof(ApiKeyException)
        },

        new MaterializeKeyFromValuesTest
        {
            Name = $"{nameof(KeyOneScalarPart)} values throw when primary scalar key (int) value has mismatched ApiKey kind",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "PK_KeyOneScalarPart",
            ConfigureValuesExpression = static a => ApiKeyTypeMaterializeKeyFromValuesTestFactory.ConfigureMismatchedApiKeyKind(a),
            ExpectedExceptionType = typeof(ApiKeyException)
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(MaterializeKeyFromInstanceTheoryData))]
    public void MaterializeKeyFromInstance(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(MaterializeKeyFromValuesTheoryData))]
    public void MaterializeKeyFromValues(IXUnitTest test) => test.Execute(this);
    #endregion
}
