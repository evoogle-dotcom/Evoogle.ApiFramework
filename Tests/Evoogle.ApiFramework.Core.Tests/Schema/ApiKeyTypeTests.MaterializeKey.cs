// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Key;
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiKeyTypeTests
{
    #region Test Types
    private class MaterializeKeyFromInstanceTest : XUnitTest
    {
        #region User Supplied Properties
        public required string ApiObjectTypeName { get; init; }
        public string? ApiKeyTypeName { get; init; }
        public object? SelfObject { get; init; }
        public object? OwnerObject { get; init; }
        public ApiKeyNullHandling NullHandling { get; init; }
        public ApiKeyPartNameBuilder PartNameBuilder { get; init; }
        public ApiKey? ExpectedValue { get; init; }
        public Type? ExpectedExceptionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiKeyType? ApiKeyType { get; set; }
        private ApiKey? ActualValue { get; set; }
        private Type? ActualExceptionType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ApiKeyType = this.ApiKeyTypeName is not null
                ? GetKeyTypeByName(this.ApiObjectTypeName, this.ApiKeyTypeName)
                : GetPrimaryKeyType(this.ApiObjectTypeName);

            this.WriteLine($"ApiObjectType:     {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiKeyType:        {this.ApiKeyType?.ApiName.SafeToString()}");
            this.WriteLine($"SelfObject:        {this.SelfObject.SafeToString()}");
            this.WriteLine($"OwnerObject:       {this.OwnerObject.SafeToString()}");
            this.WriteLine($"NullHandling:      {this.NullHandling.SafeToString()}");
            this.WriteLine($"PartNameBuilder:   {this.PartNameBuilder}");
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
                    PartNameBuilder = this.PartNameBuilder
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

    // private class BuildValueFromValuesTest : XUnitTest
    // {
    //     #region User Supplied Properties
    //     public required string ApiObjectTypeName { get; init; }
    //     public string? ApiIdentityName { get; init; }
    //     public required IReadOnlyDictionary<string, object?> Values { get; init; }
    //     public IReadOnlyDictionary<string, object?>? OwnerValues { get; init; }
    //     public ApiIdentityPartNullHandling NullHandling { get; init; }
    //     public ApiIdentityValue? ExpectedValue { get; init; }
    //     public Type? ExpectedExceptionType { get; init; }
    //     #endregion

    //     #region Calculated Properties
    //     private ApiIdentity? ApiIdentity { get; set; }
    //     private ApiIdentityValue? ActualValue { get; set; }
    //     private Type? ActualExceptionType { get; set; }
    //     #endregion

    //     #region XUnitTest Methods
    //     protected override void Arrange()
    //     {
    //         this.ApiIdentity = this.ApiIdentityName is not null
    //             ? GetIdentityByName(this.ApiObjectTypeName, this.ApiIdentityName)
    //             : GetPrimaryIdentity(this.ApiObjectTypeName);

    //         static string FormatValue(object? value) =>
    //             value is IReadOnlyDictionary<string, object?> nested
    //                 ? $"{{{string.Join(',', nested.Select(kvp => $"{kvp.Key}={FormatValue(kvp.Value)}"))}}}"
    //                 : value.SafeToString();

    //         var valuesDisplay = this.Values is not null
    //             ? $"[{string.Join(',', this.Values.Select(kvp => $"{kvp.Key.SafeToString()}={FormatValue(kvp.Value)}"))}]"
    //             : "<null>";

    //         var ownerValuesDisplay = this.OwnerValues is not null
    //             ? $"[{string.Join(',', this.OwnerValues.Select(kvp => $"{kvp.Key.SafeToString()}={FormatValue(kvp.Value)}"))}]"
    //             : "<null>";

    //         this.WriteLine($"ApiObjectType: {this.ApiObjectTypeName.SafeToString()}");
    //         this.WriteLine($"ApiIdentity:   {this.ApiIdentity.ApiName.SafeToString()}");
    //         this.WriteLine($"Values:        {valuesDisplay}");
    //         this.WriteLine($"OwnerValues:   {ownerValuesDisplay}");
    //         this.WriteLine($"NullHandling:  {this.NullHandling.SafeToString()}");
    //         this.WriteLine();

    //         if (this.ExpectedValue is not null)
    //         {
    //             this.WriteLine($"Expected Value: {this.ExpectedValue.SafeToString()}");
    //         }
    //         else if (this.ExpectedExceptionType is not null)
    //         {
    //             this.WriteLine($"Expected Exception: {this.ExpectedExceptionType.SafeToName()}");
    //         }
    //     }

    //     protected override void Act()
    //     {
    //         try
    //         {
    //             var context = new ApiIdentityValueBuildFromValuesContext
    //             {
    //                 Values = this.Values,
    //                 OwnerValues = this.OwnerValues,
    //                 NullHandling = this.NullHandling
    //             };
    //             this.ActualValue = this.ApiIdentity!.BuildValue(context);
    //             this.WriteLine($"Actual Value:   {this.ActualValue.SafeToString()}");
    //         }
    //         catch (Exception ex)
    //         {
    //             this.ActualExceptionType = ex.GetType();
    //             this.WriteLine($"Actual Exception:   {this.ActualExceptionType.SafeToName()} - {ex.Message}");
    //         }
    //     }

    //     protected override void Assert()
    //     {
    //         if (this.ExpectedValue is not null)
    //         {
    //             this.ActualExceptionType.Should().BeNull();
    //             this.ActualValue.Should().NotBeNull();
    //             this.ActualValue.Should().BeEquivalentTo(this.ExpectedValue, options => options
    //                 .Excluding(ctx => ctx.Path.EndsWith(nameof(ApiIdentityValue.ApiScalarValue), StringComparison.Ordinal))
    //                 .Excluding(ctx => ctx.Path.EndsWith(nameof(ApiIdentityValue.ApiObjectValue), StringComparison.Ordinal)));
    //         }
    //         else if (this.ExpectedExceptionType is not null)
    //         {
    //             this.ActualExceptionType.Should().NotBeNull();
    //             this.ActualExceptionType.Should().Be(this.ExpectedExceptionType);
    //         }
    //     }
    //     #endregion
    // }

    // private class BuildValueNullContextTest : XUnitTest
    // {
    //     #region User Supplied Properties
    //     public required string ApiObjectTypeName { get; init; }
    //     public required bool UseInstanceOverload { get; init; }
    //     #endregion

    //     #region Calculated Properties
    //     private ApiIdentity? ApiIdentity { get; set; }
    //     private Type? ActualExceptionType { get; set; }
    //     #endregion

    //     #region XUnitTest Methods
    //     protected override void Arrange()
    //     {
    //         this.ApiIdentity = GetPrimaryIdentity(this.ApiObjectTypeName);

    //         this.WriteLine($"ApiObjectType:       {this.ApiObjectTypeName.SafeToString()}");
    //         this.WriteLine($"UseInstanceOverload: {this.UseInstanceOverload}");
    //     }

    //     protected override void Act()
    //     {
    //         try
    //         {
    //             if (this.UseInstanceOverload)
    //             {
    //                 this.ApiIdentity!.BuildValue((ApiIdentityValueBuildContext)null!);
    //             }
    //             else
    //             {
    //                 this.ApiIdentity!.BuildValue((ApiIdentityValueBuildFromValuesContext)null!);
    //             }
    //         }
    //         catch (Exception ex)
    //         {
    //             this.ActualExceptionType = ex.GetType();
    //             this.WriteLine($"Actual Exception: {this.ActualExceptionType.SafeToName()}");
    //         }
    //     }

    //     protected override void Assert()
    //     {
    //         this.ActualExceptionType.Should().NotBeNull();
    //         this.ActualExceptionType.Should().Be(typeof(ArgumentNullException));
    //     }
    //     #endregion
    // }
    #endregion

    #region MaterializeKeyFromInstance Theory Data
    public static TheoryDataRow<IXUnitTest>[] MaterializeKeyFromInstanceTheoryData =>
    [
        // KeyOneScalarPartInstance ///////////////////////////////////////////////////////////////////////////////////
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOneScalarPartInstance} with primary scalar key (int) and none name builder",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "PK_KeyOneScalarPart",
            SelfObject = KeyOneScalarPartInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.None,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(null, ApiKey.FromInt32(KeyOneScalarPartInstance.Id)))
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOneScalarPartInstance} with primary scalar key (int) and CLR path only name builder",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "PK_KeyOneScalarPart",
            SelfObject = KeyOneScalarPartInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrPathOnly,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOneScalarPart.Id), ApiKey.FromInt32(KeyOneScalarPartInstance.Id)))
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOneScalarPartInstance} with primary scalar key (int) and CLR root and path name builder",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "PK_KeyOneScalarPart",
            SelfObject = KeyOneScalarPartInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOneScalarPart) + "." + nameof(KeyOneScalarPart.Id), ApiKey.FromInt32(KeyOneScalarPartInstance.Id)))
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOneScalarPartInstance} with alternate scalar key (string) and none name builder",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "AK_KeyOneScalarPart",
            SelfObject = KeyOneScalarPartInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.None,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(null, ApiKey.FromString(KeyOneScalarPartInstance.Name)))
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOneScalarPartInstance} with alternate scalar key (string) and CLR path only name builder",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "AK_KeyOneScalarPart",
            SelfObject = KeyOneScalarPartInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrPathOnly,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOneScalarPart.Name), ApiKey.FromString(KeyOneScalarPartInstance.Name)))
        },

        // KeyOneScalarPartInstance - alternate scalar key (string) and CLR root and path name builder
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOneScalarPartInstance} with alternate scalar key (string) and CLR root and path name builder",
            ApiObjectTypeName = nameof(KeyOneScalarPart),
            ApiKeyTypeName = "AK_KeyOneScalarPart",
            SelfObject = KeyOneScalarPartInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite(ApiKeyPart.Create(nameof(KeyOneScalarPart) + "." + nameof(KeyOneScalarPart.Name), ApiKey.FromString(KeyOneScalarPartInstance.Name)))
        },

        // KeyTwoScalarPartCompositeInstance //////////////////////////////////////////////////////////////////////////
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyTwoScalarPartCompositeInstance} with primary composite key (int + string) and none name builder",
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            SelfObject = KeyTwoScalarPartCompositeInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.None,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(null, ApiKey.FromInt32(KeyTwoScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(null, ApiKey.FromString(KeyTwoScalarPartCompositeInstance.Id2!))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyTwoScalarPartCompositeInstance} with primary composite key (int + string) and CLR path only name builder",
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            SelfObject = KeyTwoScalarPartCompositeInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite.Id1), ApiKey.FromInt32(KeyTwoScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite.Id2), ApiKey.FromString(KeyTwoScalarPartCompositeInstance.Id2!))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyTwoScalarPartCompositeInstance} with primary composite key (int + string) and CLR root and path name builder",
            ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
            SelfObject = KeyTwoScalarPartCompositeInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite) + "." + nameof(KeyTwoScalarPartComposite.Id1), ApiKey.FromInt32(KeyTwoScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(nameof(KeyTwoScalarPartComposite) + "." + nameof(KeyTwoScalarPartComposite.Id2), ApiKey.FromString(KeyTwoScalarPartCompositeInstance.Id2!))
            )
        },

        // KeyThreeScalarPartCompositeInstance ////////////////////////////////////////////////////////////////////////
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyThreeScalarPartCompositeInstance} with primary composite key (int + string + Guid) and none name builder",
            ApiObjectTypeName = nameof(KeyThreeScalarPartComposite),
            SelfObject = KeyThreeScalarPartCompositeInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.None,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(null, ApiKey.FromInt32(KeyThreeScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(null, ApiKey.FromString(KeyThreeScalarPartCompositeInstance.Id2!)),
                ApiKeyPart.Create(null, ApiKey.FromGuid(KeyThreeScalarPartCompositeInstance.Id3))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyThreeScalarPartCompositeInstance} with primary composite key (int + string + Guid) and CLR path only name builder",
            ApiObjectTypeName = nameof(KeyThreeScalarPartComposite),
            SelfObject = KeyThreeScalarPartCompositeInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyThreeScalarPartComposite.Id1), ApiKey.FromInt32(KeyThreeScalarPartCompositeInstance.Id1)),
                ApiKeyPart.Create(nameof(KeyThreeScalarPartComposite.Id2), ApiKey.FromString(KeyThreeScalarPartCompositeInstance.Id2!)),
                ApiKeyPart.Create(nameof(KeyThreeScalarPartComposite.Id3), ApiKey.FromGuid(KeyThreeScalarPartCompositeInstance.Id3))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyThreeScalarPartCompositeInstance} with primary composite key (int + string + Guid) and CLR root and path name builder",
            ApiObjectTypeName = nameof(KeyThreeScalarPartComposite),
            SelfObject = KeyThreeScalarPartCompositeInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrRootAndPath,
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
            Name = $"{KeyNestedCompositeInstance} with primary composite key (int + string) and none name builder",
            ApiObjectTypeName = nameof(KeyNestedComposite),
            SelfObject = KeyNestedCompositeInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.None,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(null, ApiKey.FromInt32(KeyNestedCompositeInstance.NestedPart.Id)),
                ApiKeyPart.Create(null, ApiKey.FromString(KeyNestedCompositeInstance.Name!))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyNestedCompositeInstance} with primary composite key (int + string) and CLR path only name builder",
            ApiObjectTypeName = nameof(KeyNestedComposite),
            SelfObject = KeyNestedCompositeInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyNestedComposite.NestedPart) + "." + nameof(KeyNested.Id), ApiKey.FromInt32(KeyNestedCompositeInstance.NestedPart.Id)),
                ApiKeyPart.Create(nameof(KeyNestedComposite.Name), ApiKey.FromString(KeyNestedCompositeInstance.Name!))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyNestedCompositeInstance} with primary composite key (int + string) and CLR root and path name builder",
            ApiObjectTypeName = nameof(KeyNestedComposite),
            SelfObject = KeyNestedCompositeInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyNestedComposite) + "." + nameof(KeyNestedComposite.NestedPart) + "." + nameof(KeyNested.Id), ApiKey.FromInt32(KeyNestedCompositeInstance.NestedPart.Id)),
                ApiKeyPart.Create(nameof(KeyNestedComposite) + "." + nameof(KeyNestedComposite.Name), ApiKey.FromString(KeyNestedCompositeInstance.Name!))
            )
        },

        // KeyOwnedCompositeInstance //////////////////////////////////////////////////////////////////////////////////
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedCompositeInstance} with primary composite key (int + int) and none name builder",
            ApiObjectTypeName = nameof(KeyOwnedComposite),
            SelfObject = KeyOwnedCompositeInstance,
            OwnerObject = KeyOwnerInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.None,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(null, ApiKey.FromInt32(KeyOwnerInstance.Id)),
                ApiKeyPart.Create(null, ApiKey.FromInt32(KeyOwnedCompositeInstance.LineNumber))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedCompositeInstance} with primary composite key (int + int) and CLR path only name builder",
            ApiObjectTypeName = nameof(KeyOwnedComposite),
            SelfObject = KeyOwnedCompositeInstance,
            OwnerObject = KeyOwnerInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyOwner.Id), ApiKey.FromInt32(KeyOwnerInstance.Id)),
                ApiKeyPart.Create(nameof(KeyOwnedComposite.LineNumber), ApiKey.FromInt32(KeyOwnedCompositeInstance.LineNumber))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedCompositeInstance} with primary composite key (int + int) and CLR root and path name builder",
            ApiObjectTypeName = nameof(KeyOwnedComposite),
            SelfObject = KeyOwnedCompositeInstance,
            OwnerObject = KeyOwnerInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrRootAndPath,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyOwner) + "." + nameof(KeyOwner.Id), ApiKey.FromInt32(KeyOwnerInstance.Id)),
                ApiKeyPart.Create(nameof(KeyOwnedComposite) + "." + nameof(KeyOwnedComposite.LineNumber), ApiKey.FromInt32(KeyOwnedCompositeInstance.LineNumber))
            )
        },

        // KeyOwnedDependentInstance //////////////////////////////////////////////////////////////////////////////////
        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedDependentInstance} with primary scalar key (int) and none name builder",
            ApiObjectTypeName = nameof(KeyOwnedDependent),
            SelfObject = KeyOwnedDependentInstance,
            OwnerObject = KeyOwnerInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.None,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(null, ApiKey.FromInt32(KeyOwnerInstance.Id))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedDependentInstance} with primary scalar key (int) and CLR path only name builder",
            ApiObjectTypeName = nameof(KeyOwnedDependent),
            SelfObject = KeyOwnedDependentInstance,
            OwnerObject = KeyOwnerInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrPathOnly,
            ExpectedValue = ApiKey.Composite
            (
                ApiKeyPart.Create(nameof(KeyOwner.Id), ApiKey.FromInt32(KeyOwnerInstance.Id))
            )
        },

        new MaterializeKeyFromInstanceTest
        {
            Name = $"{KeyOwnedDependentInstance} with primary scalar key (int) and CLR root and path name builder",
            ApiObjectTypeName = nameof(KeyOwnedDependent),
            SelfObject = KeyOwnedDependentInstance,
            OwnerObject = KeyOwnerInstance,
            PartNameBuilder = ApiKeyPartNameBuilder.ClrRootAndPath,
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
            PartNameBuilder = ApiKeyPartNameBuilder.ClrRootAndPath,
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
            PartNameBuilder = ApiKeyPartNameBuilder.ClrRootAndPath,
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
            PartNameBuilder = ApiKeyPartNameBuilder.ClrRootAndPath,
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

        // // Composite key — custom part names
        // new MaterializeKeyFromInstanceTest
        // {
        //     Name = $"{TwoPartInstance} with custom part names",
        //     ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
        //     SelfObject = TwoPartInstance,
        //     PartNameBuilder = static c => $"{c.ApiKeyType.ApiName}[{c.PartIndex}]",
        //     ExpectedValue = ApiKey.Composite
        //     (
        //         ApiKeyPart.Create("PK_KeyTwoScalarPartComposite[0]", ApiKey.FromInt32(TwoPartInstance.Id1)),
        //         ApiKeyPart.Create("PK_KeyTwoScalarPartComposite[1]", ApiKey.FromString(TwoPartInstance.Id2!))
        //     )
        // },

    ];
    #endregion

    #region BuildValueFromValues Theory Data
    // public static TheoryDataRow<IXUnitTest>[] BuildValueFromValuesTheoryData =>
    // [
    //     // Scalar key from dictionary (int)
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"{nameof(KeyOneScalarPart)} with primary key",
    //         ApiObjectTypeName = nameof(KeyOneScalarPart),
    //         ApiIdentityName = "PK_IdentityScalar",
    //         Values = new Dictionary<string, object?> { ["Id"] = 42 },
    //         ExpectedValue = ApiIdentityValue.Composite(
    //         [
    //             new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(42))
    //         ])
    //     },

    //     // Alternate key from dictionary (string)
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"{nameof(KeyOneScalarPart)} with alternate key",
    //         ApiObjectTypeName = nameof(KeyOneScalarPart),
    //         ApiIdentityName = "AK_IdentityScalar",
    //         Values = new Dictionary<string, object?> { ["Name"] = "TestName" },
    //         ExpectedValue = ApiIdentityValue.Composite(
    //         [
    //             new ApiIdentityScalarPartValue("Name", ApiId.FromString("TestName"))
    //         ])
    //     },

    //     // Composite key from dictionary — two parts
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"{nameof(KeyTwoScalarPartComposite)}",
    //         ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
    //         Values = new Dictionary<string, object?> { ["Id1"] = 1, ["Id2"] = "abc" },
    //         ExpectedValue = ApiIdentityValue.Composite(
    //         [
    //             new ApiIdentityScalarPartValue("Id1", ApiId.FromInt32(1)),
    //             new ApiIdentityScalarPartValue("Id2", ApiId.FromString("abc"))
    //         ])
    //     },

    //     // Composite key from dictionary — three parts
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"{nameof(KeyThreeScalarPartComposite)}",
    //         ApiObjectTypeName = nameof(KeyThreeScalarPartComposite),
    //         Values = new Dictionary<string, object?>
    //         {
    //             ["Id1"] = 10,
    //             ["Id2"] = "xyz",
    //             ["Id3"] = Guid.Parse("11111111-1111-1111-1111-111111111111")
    //         },
    //         ExpectedValue = ApiIdentityValue.Composite(
    //         [
    //             new ApiIdentityScalarPartValue("Id1", ApiId.FromInt32(10)),
    //             new ApiIdentityScalarPartValue("Id2", ApiId.FromString("xyz")),
    //             new ApiIdentityScalarPartValue("Id3", ApiId.FromGuid(Guid.Parse("11111111-1111-1111-1111-111111111111")))
    //         ])
    //     },

    //     // Nested key from dictionary — nested dict
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"{nameof(KeyNestedComposite)} with nested dictionary",
    //         ApiObjectTypeName = nameof(KeyNestedComposite),
    //         Values = new Dictionary<string, object?>
    //         {
    //             ["NestedPart"] = new Dictionary<string, object?> { ["Id"] = 5 },
    //             ["Name"] = "Nested"
    //         },
    //         ExpectedValue = ApiIdentityValue.Composite(
    //         [
    //             new ApiIdentityObjectPartValue("NestedPart",
    //                 ApiIdentityValue.Composite(
    //                 [
    //                     new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(5))
    //                 ])),
    //             new ApiIdentityScalarPartValue("Name", ApiId.FromString("Nested"))
    //         ])
    //     },

    //     // Nested key from dictionary — CLR object fallback
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"{nameof(KeyNestedComposite)} with CLR instance fallback",
    //         ApiObjectTypeName = nameof(KeyNestedComposite),
    //         Values = new Dictionary<string, object?>
    //         {
    //             ["NestedPart"] = NestedPartInstance,
    //             ["Name"] = "Nested"
    //         },
    //         ExpectedValue = ApiIdentityValue.Composite(
    //         [
    //             new ApiIdentityObjectPartValue("NestedPart",
    //                 ApiIdentityValue.Composite(
    //                 [
    //                     new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(5))
    //                 ])),
    //             new ApiIdentityScalarPartValue("Name", ApiId.FromString("Nested"))
    //         ])
    //     },

    //     // Owner key from dictionary — with owner values
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"{nameof(KeyOwnedComposite)}",
    //         ApiObjectTypeName = nameof(KeyOwnedComposite),
    //         Values = new Dictionary<string, object?> { ["LineNumber"] = 3 },
    //         OwnerValues = new Dictionary<string, object?> { ["Id"] = 99 },
    //         ExpectedValue = ApiIdentityValue.Composite(
    //         [
    //             new ApiIdentityObjectPartValue("IdentityOwner",
    //                 ApiIdentityValue.Composite(
    //                 [
    //                     new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(99))
    //                 ])),
    //             new ApiIdentityScalarPartValue("LineNumber", ApiId.FromInt32(3))
    //         ])
    //     },

    //     // Owner-only key from dictionary
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"{nameof(KeyOwnedDependent)}",
    //         ApiObjectTypeName = nameof(KeyOwnedDependent),
    //         Values = new Dictionary<string, object?>(),
    //         OwnerValues = new Dictionary<string, object?> { ["Id"] = 99 },
    //         ExpectedValue = ApiIdentityValue.Composite(
    //         [
    //             new ApiIdentityObjectPartValue("IdentityOwner",
    //                 ApiIdentityValue.Composite(
    //                 [
    //                     new ApiIdentityScalarPartValue("Id", ApiId.FromInt32(99))
    //                 ]))
    //         ])
    //     },

    //     // Null handling — UseDefaultOnNull: missing key in dictionary
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"Missing key in dictionary returns empty with {ApiIdentityPartNullHandling.UseDefaultOnNull}",
    //         ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
    //         Values = new Dictionary<string, object?> { ["Id1"] = 1 },
    //         NullHandling = ApiIdentityPartNullHandling.UseDefaultOnNull,
    //         ExpectedValue = ApiIdentityValue.Composite(
    //         [
    //             new ApiIdentityScalarPartValue("Id1", ApiId.FromInt32(1)),
    //             new ApiIdentityScalarPartValue("Id2", ApiId.Empty)
    //         ])
    //     },

    //     // Null handling — UseDefaultOnNull: null value in dictionary
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"Null value in dictionary returns empty with {ApiIdentityPartNullHandling.UseDefaultOnNull}",
    //         ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
    //         Values = new Dictionary<string, object?> { ["Id1"] = 1, ["Id2"] = null },
    //         NullHandling = ApiIdentityPartNullHandling.UseDefaultOnNull,
    //         ExpectedValue = ApiIdentityValue.Composite(
    //         [
    //             new ApiIdentityScalarPartValue("Id1", ApiId.FromInt32(1)),
    //             new ApiIdentityScalarPartValue("Id2", ApiId.Empty)
    //         ])
    //     },

    //     // Null handling — UseDefaultOnNull: null nested value
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"Null nested value in dictionary returns skeleton with {ApiIdentityPartNullHandling.UseDefaultOnNull}",
    //         ApiObjectTypeName = nameof(KeyNestedComposite),
    //         Values = new Dictionary<string, object?> { ["NestedPart"] = null, ["Name"] = "NoNested" },
    //         NullHandling = ApiIdentityPartNullHandling.UseDefaultOnNull,
    //         ExpectedValue = ApiIdentityValue.Composite(
    //         [
    //             new ApiIdentityObjectPartValue("NestedPart", apiObjectValue: null, apiStructure:
    //             [
    //                 new ApiIdentityScalarPartValue("Id", ApiId.Empty)
    //             ]),
    //             new ApiIdentityScalarPartValue("Name", ApiId.FromString("NoNested"))
    //         ])
    //     },

    //     // Null handling — UseDefaultOnNull: null owner values
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"Null owner values returns skeleton with {ApiIdentityPartNullHandling.UseDefaultOnNull}",
    //         ApiObjectTypeName = nameof(KeyOwnedComposite),
    //         Values = new Dictionary<string, object?> { ["LineNumber"] = 3 },
    //         OwnerValues = null,
    //         NullHandling = ApiIdentityPartNullHandling.UseDefaultOnNull,
    //         ExpectedValue = ApiIdentityValue.Composite(
    //         [
    //             new ApiIdentityObjectPartValue("IdentityOwner", apiObjectValue: null, apiStructure:
    //             [
    //                 new ApiIdentityScalarPartValue("Id", ApiId.Empty)
    //             ]),
    //             new ApiIdentityScalarPartValue("LineNumber", ApiId.FromInt32(3))
    //         ])
    //     },

    //     // Null handling — ThrowOnNull: missing key
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"Missing key in dictionary throws with {ApiIdentityPartNullHandling.ThrowOnNull}",
    //         ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
    //         Values = new Dictionary<string, object?> { ["Id1"] = 1 },
    //         NullHandling = ApiIdentityPartNullHandling.ThrowOnNull,
    //         ExpectedExceptionType = typeof(ApiIdentityException)
    //     },

    //     // Null handling — ThrowOnNull: null value
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"Null value in dictionary throws with {ApiIdentityPartNullHandling.ThrowOnNull}",
    //         ApiObjectTypeName = nameof(KeyTwoScalarPartComposite),
    //         Values = new Dictionary<string, object?> { ["Id1"] = 1, ["Id2"] = null },
    //         NullHandling = ApiIdentityPartNullHandling.ThrowOnNull,
    //         ExpectedExceptionType = typeof(ApiIdentityException)
    //     },

    //     // Null handling — ThrowOnNull: null nested value
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"Null nested value in dictionary throws with {ApiIdentityPartNullHandling.ThrowOnNull}",
    //         ApiObjectTypeName = nameof(KeyNestedComposite),
    //         Values = new Dictionary<string, object?> { ["NestedPart"] = null, ["Name"] = "Nested" },
    //         NullHandling = ApiIdentityPartNullHandling.ThrowOnNull,
    //         ExpectedExceptionType = typeof(ApiIdentityException)
    //     },

    //     // Null handling — ThrowOnNull: null owner values
    //     new BuildValueFromValuesTest
    //     {
    //         Name = $"Null owner values throws with {ApiIdentityPartNullHandling.ThrowOnNull}",
    //         ApiObjectTypeName = nameof(KeyOwnedComposite),
    //         Values = new Dictionary<string, object?> { ["LineNumber"] = 3 },
    //         OwnerValues = null,
    //         NullHandling = ApiIdentityPartNullHandling.ThrowOnNull,
    //         ExpectedExceptionType = typeof(ApiIdentityException)
    //     },
    // ];
    #endregion

    #region BuildValueNullContext Theory Data
    // public static TheoryDataRow<IXUnitTest>[] BuildValueNullContextTheoryData =>
    // [
    //     new BuildValueNullContextTest
    //     {
    //         Name = "Null context throws ArgumentNullException for instance overload",
    //         ApiObjectTypeName = nameof(KeyOneScalarPart),
    //         UseInstanceOverload = true
    //     },
    //     new BuildValueNullContextTest
    //     {
    //         Name = "Null context throws ArgumentNullException for values overload",
    //         ApiObjectTypeName = nameof(KeyOneScalarPart),
    //         UseInstanceOverload = false
    //     },
    // ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(MaterializeKeyFromInstanceTheoryData))]
    public void MaterializeKeyFromInstance(IXUnitTest test) => test.Execute(this);

    // [Theory]
    // [MemberData(nameof(BuildValueFromValuesTheoryData))]
    // public void BuildValueFromValues(IXUnitTest test) => test.Execute(this);

    // [Theory]
    // [MemberData(nameof(BuildValueNullContextTheoryData))]
    // public void BuildValueNullContext(IXUnitTest test) => test.Execute(this);
    #endregion
}
