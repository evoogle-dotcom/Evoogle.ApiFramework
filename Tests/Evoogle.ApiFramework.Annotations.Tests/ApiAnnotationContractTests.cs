// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;
using System.Runtime.CompilerServices;

using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework;

public sealed class ApiAnnotationContractTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private class AnnotationConstructorTest : XUnitTest
    {
        #region User Supplied Properties
        public required Type AnnotationType { get; init; }
        #endregion

        #region Calculated Properties
        private ConstructorInfo[]? ActualConstructors { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"AnnotationType: {this.AnnotationType}");
        }

        protected override void Act()
        {
            this.ActualConstructors = this.AnnotationType.GetConstructors
            (
                BindingFlags.Instance | BindingFlags.Public
            );

            this.WriteLine($"Constructor count: {this.ActualConstructors.Length}");
        }

        protected override void Assert()
        {
            this.ActualConstructors.Should().ContainSingle();
            this.ActualConstructors![0].GetParameters().Should().BeEmpty();
        }
        #endregion
    }

    private class AnnotationPropertyInitTest : XUnitTest
    {
        #region User Supplied Properties
        public required Type AnnotationType { get; init; }
        public required string PropertyName { get; init; }
        #endregion

        #region Calculated Properties
        private PropertyInfo? ActualProperty { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"AnnotationType: {this.AnnotationType}");
            this.WriteLine($"PropertyName: {this.PropertyName}");
        }

        protected override void Act()
        {
            this.ActualProperty = this.AnnotationType.GetProperty(this.PropertyName);
        }

        protected override void Assert()
        {
            this.ActualProperty.Should().NotBeNull();
            this.ActualProperty!.GetMethod.Should().NotBeNull();

            var setter = this.ActualProperty.GetSetMethod(nonPublic: true);
            setter.Should().NotBeNull();
            setter!.ReturnParameter
                .GetRequiredCustomModifiers()
                .Should()
                .Contain(typeof(IsExternalInit));
        }
        #endregion
    }

    private class RequiredAnnotationPropertyTest : XUnitTest
    {
        #region User Supplied Properties
        public required Type AnnotationType { get; init; }
        public required string PropertyName { get; init; }
        #endregion

        #region Calculated Properties
        private PropertyInfo? ActualProperty { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"AnnotationType: {this.AnnotationType}");
            this.WriteLine($"PropertyName: {this.PropertyName}");
        }

        protected override void Act()
        {
            this.ActualProperty = this.AnnotationType.GetProperty(this.PropertyName);
        }

        protected override void Assert()
        {
            this.ActualProperty.Should().NotBeNull();
            this.ActualProperty!
                .GetCustomAttribute<RequiredMemberAttribute>()
                .Should()
                .NotBeNull();
        }
        #endregion
    }

    private class OptionalAnnotationPropertyTest : XUnitTest
    {
        #region User Supplied Properties
        public required Type AnnotationType { get; init; }
        public required string PropertyName { get; init; }
        #endregion

        #region Calculated Properties
        private PropertyInfo? ActualProperty { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"AnnotationType: {this.AnnotationType}");
            this.WriteLine($"PropertyName: {this.PropertyName}");
        }

        protected override void Act()
        {
            this.ActualProperty = this.AnnotationType.GetProperty(this.PropertyName);
        }

        protected override void Assert()
        {
            this.ActualProperty.Should().NotBeNull();

            var nullabilityContext = new NullabilityInfoContext();
            nullabilityContext.Create(this.ActualProperty!).ReadState
                .Should()
                .Be(NullabilityState.Nullable);
        }
        #endregion
    }

    private class RequiredAnnotationApiNameTest : XUnitTest
    {
        #region User Supplied Properties
        public required Type AnnotationType { get; init; }
        public string? ApiName { get; init; }
        #endregion

        #region Calculated Properties
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"AnnotationType: {this.AnnotationType}");
            this.WriteLine($"ApiName: {this.ApiName ?? "null"}");
        }

        protected override void Act()
        {
            try
            {
                _ = CreateAnnotationWithApiName(this.AnnotationType, this.ApiName);
            }
            catch (Exception exception)
            {
                this.ActualException = exception;
            }
        }

        protected override void Assert()
        {
            this.ActualException.Should().NotBeNull();
            this.ActualException.Should().BeAssignableTo<ArgumentException>();

            var argumentException = (ArgumentException)this.ActualException!;
            argumentException.ParamName.Should().Be("apiName");
        }
        #endregion
    }

    private class RequiredAnnotationClrTypeTest : XUnitTest
    {
        #region User Supplied Properties
        public required Type AnnotationType { get; init; }
        public required string PropertyName { get; init; }
        #endregion

        #region Calculated Properties
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"AnnotationType: {this.AnnotationType}");
            this.WriteLine($"PropertyName: {this.PropertyName}");
        }

        protected override void Act()
        {
            try
            {
                _ = CreateAnnotationWithNullClrType(this.AnnotationType, this.PropertyName);
            }
            catch (Exception exception)
            {
                this.ActualException = exception;
            }
        }

        protected override void Assert()
        {
            this.ActualException.Should().BeOfType<ArgumentNullException>();
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] AnnotationConstructorTheoryData =>
    [
        new AnnotationConstructorTest
        {
            Name = $"{nameof(ApiObjectAttribute)} has a parameterless constructor",
            AnnotationType = typeof(ApiObjectAttribute)
        },
        new AnnotationConstructorTest
        {
            Name = $"{nameof(ApiScalarAttribute)} has a parameterless constructor",
            AnnotationType = typeof(ApiScalarAttribute)
        },
        new AnnotationConstructorTest
        {
            Name = $"{nameof(ApiEnumAttribute)} has a parameterless constructor",
            AnnotationType = typeof(ApiEnumAttribute)
        },
        new AnnotationConstructorTest
        {
            Name = $"{nameof(ApiEnumValueAttribute)} has a parameterless constructor",
            AnnotationType = typeof(ApiEnumValueAttribute)
        },
        new AnnotationConstructorTest
        {
            Name = $"{nameof(ApiPropertyAttribute)} has a parameterless constructor",
            AnnotationType = typeof(ApiPropertyAttribute)
        },
        new AnnotationConstructorTest
        {
            Name = $"{nameof(ApiKeyAttribute)} has a parameterless constructor",
            AnnotationType = typeof(ApiKeyAttribute)
        },
        new AnnotationConstructorTest
        {
            Name = $"{nameof(ApiRelationshipAttribute)} has a parameterless constructor",
            AnnotationType = typeof(ApiRelationshipAttribute)
        },
        new AnnotationConstructorTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)} has a parameterless constructor",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute)
        },
        new AnnotationConstructorTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)} has a parameterless constructor",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute)
        },
        new AnnotationConstructorTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)} " +
                    "has a parameterless constructor",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute)
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] AnnotationPropertyInitTheoryData =>
    [
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiNamedElementAttribute)}.{nameof(ApiNamedElementAttribute.ApiName)} uses init",
            AnnotationType = typeof(ApiNamedElementAttribute),
            PropertyName = nameof(ApiNamedElementAttribute.ApiName)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiObjectAttribute)}.{nameof(ApiObjectAttribute.ApiName)} uses init",
            AnnotationType = typeof(ApiObjectAttribute),
            PropertyName = nameof(ApiObjectAttribute.ApiName)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiScalarAttribute)}.{nameof(ApiScalarAttribute.ApiName)} uses init",
            AnnotationType = typeof(ApiScalarAttribute),
            PropertyName = nameof(ApiScalarAttribute.ApiName)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiEnumAttribute)}.{nameof(ApiEnumAttribute.ApiName)} uses init",
            AnnotationType = typeof(ApiEnumAttribute),
            PropertyName = nameof(ApiEnumAttribute.ApiName)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiEnumValueAttribute)}.{nameof(ApiEnumValueAttribute.ApiName)} uses init",
            AnnotationType = typeof(ApiEnumValueAttribute),
            PropertyName = nameof(ApiEnumValueAttribute.ApiName)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiPropertyAttribute)}.{nameof(ApiPropertyAttribute.ApiName)} uses init",
            AnnotationType = typeof(ApiPropertyAttribute),
            PropertyName = nameof(ApiPropertyAttribute.ApiName)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiPropertyAttribute)}.{nameof(ApiPropertyAttribute.IsRequired)} uses init",
            AnnotationType = typeof(ApiPropertyAttribute),
            PropertyName = nameof(ApiPropertyAttribute.IsRequired)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiPropertyAttribute)}.{nameof(ApiPropertyAttribute.IsOptional)} uses init",
            AnnotationType = typeof(ApiPropertyAttribute),
            PropertyName = nameof(ApiPropertyAttribute.IsOptional)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiKeyAttribute)}.{nameof(ApiKeyAttribute.ApiName)} uses init",
            AnnotationType = typeof(ApiKeyAttribute),
            PropertyName = nameof(ApiKeyAttribute.ApiName)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiKeyAttribute)}.{nameof(ApiKeyAttribute.Order)} uses init",
            AnnotationType = typeof(ApiKeyAttribute),
            PropertyName = nameof(ApiKeyAttribute.Order)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiKeyAttribute)}.{nameof(ApiKeyAttribute.ClrRootType)} uses init",
            AnnotationType = typeof(ApiKeyAttribute),
            PropertyName = nameof(ApiKeyAttribute.ClrRootType)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiKeyAttribute)}.{nameof(ApiKeyAttribute.ClrPath)} uses init",
            AnnotationType = typeof(ApiKeyAttribute),
            PropertyName = nameof(ApiKeyAttribute.ClrPath)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiRelationshipAttribute)}.{nameof(ApiRelationshipAttribute.ApiName)} uses init",
            AnnotationType = typeof(ApiRelationshipAttribute),
            PropertyName = nameof(ApiRelationshipAttribute.ApiName)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiRelationshipAttribute)}.{nameof(ApiRelationshipAttribute.Kind)} uses init",
            AnnotationType = typeof(ApiRelationshipAttribute),
            PropertyName = nameof(ApiRelationshipAttribute.Kind)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiRelationshipAttribute)}.{nameof(ApiRelationshipAttribute.ForeignKey)} uses init",
            AnnotationType = typeof(ApiRelationshipAttribute),
            PropertyName = nameof(ApiRelationshipAttribute.ForeignKey)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiRelationshipAttribute)}.{nameof(ApiRelationshipAttribute.DeleteBehavior)} uses init",
            AnnotationType = typeof(ApiRelationshipAttribute),
            PropertyName = nameof(ApiRelationshipAttribute.DeleteBehavior)
        },
        new AnnotationPropertyInitTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiRelationshipDefinitionAttribute.ApiName)} uses init",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiRelationshipDefinitionAttribute.ApiName)
        },
        new AnnotationPropertyInitTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiRelationshipDefinitionAttribute.PrincipalType)} uses init",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiRelationshipDefinitionAttribute.PrincipalType)
        },
        new AnnotationPropertyInitTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiRelationshipDefinitionAttribute.DependentType)} uses init",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiRelationshipDefinitionAttribute.DependentType)
        },
        new AnnotationPropertyInitTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiRelationshipDefinitionAttribute.Kind)} uses init",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiRelationshipDefinitionAttribute.Kind)
        },
        new AnnotationPropertyInitTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiRelationshipDefinitionAttribute.ForeignKey)} uses init",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiRelationshipDefinitionAttribute.ForeignKey)
        },
        new AnnotationPropertyInitTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiRelationshipDefinitionAttribute.DeleteBehavior)} uses init",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiRelationshipDefinitionAttribute.DeleteBehavior)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)}.{nameof(ApiManyToManyRelationshipAttribute.ApiName)} uses init",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipAttribute.ApiName)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)}.{nameof(ApiManyToManyRelationshipAttribute.AssociationType)} uses init",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipAttribute.AssociationType)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)}.{nameof(ApiManyToManyRelationshipAttribute.OtherPrincipalType)} uses init",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipAttribute.OtherPrincipalType)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)}.{nameof(ApiManyToManyRelationshipAttribute.ForeignKeyA)} uses init",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipAttribute.ForeignKeyA)
        },
        new AnnotationPropertyInitTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)}.{nameof(ApiManyToManyRelationshipAttribute.ForeignKeyB)} uses init",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipAttribute.ForeignKeyB)
        },
        new AnnotationPropertyInitTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute.ApiName)} uses init",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.ApiName)
        },
        new AnnotationPropertyInitTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute.PrincipalTypeA)} uses init",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.PrincipalTypeA)
        },
        new AnnotationPropertyInitTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute.PrincipalTypeB)} uses init",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.PrincipalTypeB)
        },
        new AnnotationPropertyInitTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute.AssociationType)} uses init",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.AssociationType)
        },
        new AnnotationPropertyInitTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute.ForeignKeyA)} uses init",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.ForeignKeyA)
        },
        new AnnotationPropertyInitTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute.ForeignKeyB)} uses init",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.ForeignKeyB)
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] RequiredAnnotationPropertyTheoryData =>
    [
        new RequiredAnnotationPropertyTest
        {
            Name = $"{nameof(ApiRelationshipAttribute)}.{nameof(ApiRelationshipAttribute.ApiName)} is required",
            AnnotationType = typeof(ApiRelationshipAttribute),
            PropertyName = nameof(ApiRelationshipAttribute.ApiName)
        },
        new RequiredAnnotationPropertyTest
        {
            Name =
                $"{nameof(ApiRelationshipAttribute)}." +
                $"{nameof(ApiRelationshipAttribute.Kind)} is required",
            AnnotationType = typeof(ApiRelationshipAttribute),
            PropertyName = nameof(ApiRelationshipAttribute.Kind)
        },
        new RequiredAnnotationPropertyTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiRelationshipDefinitionAttribute.ApiName)} is required",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiRelationshipDefinitionAttribute.ApiName)
        },
        new RequiredAnnotationPropertyTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiRelationshipDefinitionAttribute.Kind)} is required",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiRelationshipDefinitionAttribute.Kind)
        },
        new RequiredAnnotationPropertyTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiRelationshipDefinitionAttribute.PrincipalType)} is required",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiRelationshipDefinitionAttribute.PrincipalType)
        },
        new RequiredAnnotationPropertyTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiRelationshipDefinitionAttribute.DependentType)} is required",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiRelationshipDefinitionAttribute.DependentType)
        },
        new RequiredAnnotationPropertyTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)}.{nameof(ApiManyToManyRelationshipAttribute.ApiName)} is required",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipAttribute.ApiName)
        },
        new RequiredAnnotationPropertyTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)}.{nameof(ApiManyToManyRelationshipAttribute.AssociationType)} is required",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipAttribute.AssociationType)
        },
        new RequiredAnnotationPropertyTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)}.{nameof(ApiManyToManyRelationshipAttribute.OtherPrincipalType)} is required",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipAttribute.OtherPrincipalType)
        },
        new RequiredAnnotationPropertyTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute.ApiName)} is required",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.ApiName)
        },
        new RequiredAnnotationPropertyTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute.PrincipalTypeA)} " +
                    "is required",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.PrincipalTypeA)
        },
        new RequiredAnnotationPropertyTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute.PrincipalTypeB)} " +
                    "is required",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.PrincipalTypeB)
        },
        new RequiredAnnotationPropertyTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute.AssociationType)} " +
                    "is required",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.AssociationType)
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] OptionalAnnotationPropertyTheoryData =>
    [
        new OptionalAnnotationPropertyTest
        {
            Name = $"{nameof(ApiNamedElementAttribute)}.{nameof(ApiNamedElementAttribute.ApiName)} is nullable",
            AnnotationType = typeof(ApiNamedElementAttribute),
            PropertyName = nameof(ApiNamedElementAttribute.ApiName)
        },
        new OptionalAnnotationPropertyTest
        {
            Name = $"{nameof(ApiObjectAttribute)}.{nameof(ApiObjectAttribute.ApiName)} is nullable",
            AnnotationType = typeof(ApiObjectAttribute),
            PropertyName = nameof(ApiObjectAttribute.ApiName)
        },
        new OptionalAnnotationPropertyTest
        {
            Name = $"{nameof(ApiScalarAttribute)}.{nameof(ApiScalarAttribute.ApiName)} is nullable",
            AnnotationType = typeof(ApiScalarAttribute),
            PropertyName = nameof(ApiScalarAttribute.ApiName)
        },
        new OptionalAnnotationPropertyTest
        {
            Name = $"{nameof(ApiEnumAttribute)}.{nameof(ApiEnumAttribute.ApiName)} is nullable",
            AnnotationType = typeof(ApiEnumAttribute),
            PropertyName = nameof(ApiEnumAttribute.ApiName)
        },
        new OptionalAnnotationPropertyTest
        {
            Name = $"{nameof(ApiEnumValueAttribute)}.{nameof(ApiEnumValueAttribute.ApiName)} is nullable",
            AnnotationType = typeof(ApiEnumValueAttribute),
            PropertyName = nameof(ApiEnumValueAttribute.ApiName)
        },
        new OptionalAnnotationPropertyTest
        {
            Name = $"{nameof(ApiPropertyAttribute)}.{nameof(ApiPropertyAttribute.ApiName)} is nullable",
            AnnotationType = typeof(ApiPropertyAttribute),
            PropertyName = nameof(ApiPropertyAttribute.ApiName)
        },
        new OptionalAnnotationPropertyTest
        {
            Name = $"{nameof(ApiKeyAttribute)}.{nameof(ApiKeyAttribute.ClrRootType)} is nullable",
            AnnotationType = typeof(ApiKeyAttribute),
            PropertyName = nameof(ApiKeyAttribute.ClrRootType)
        },
        new OptionalAnnotationPropertyTest
        {
            Name = $"{nameof(ApiKeyAttribute)}.{nameof(ApiKeyAttribute.ClrPath)} is nullable",
            AnnotationType = typeof(ApiKeyAttribute),
            PropertyName = nameof(ApiKeyAttribute.ClrPath)
        },
        new OptionalAnnotationPropertyTest
        {
            Name = $"{nameof(ApiRelationshipAttribute)}.{nameof(ApiRelationshipAttribute.ForeignKey)} is nullable",
            AnnotationType = typeof(ApiRelationshipAttribute),
            PropertyName = nameof(ApiRelationshipAttribute.ForeignKey)
        },
        new OptionalAnnotationPropertyTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiRelationshipDefinitionAttribute.ForeignKey)} is nullable",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiRelationshipDefinitionAttribute.ForeignKey)
        },
        new OptionalAnnotationPropertyTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)}.{nameof(ApiManyToManyRelationshipAttribute.ForeignKeyA)} is nullable",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipAttribute.ForeignKeyA)
        },
        new OptionalAnnotationPropertyTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)}.{nameof(ApiManyToManyRelationshipAttribute.ForeignKeyB)} is nullable",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipAttribute.ForeignKeyB)
        },
        new OptionalAnnotationPropertyTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute.ForeignKeyA)} is nullable",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.ForeignKeyA)
        },
        new OptionalAnnotationPropertyTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)}." +
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute.ForeignKeyB)} is nullable",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.ForeignKeyB)
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] RequiredAnnotationApiNameTheoryData =>
    [
        new RequiredAnnotationApiNameTest
        {
            Name = $"{nameof(ApiKeyAttribute)} rejects a null API name",
            AnnotationType = typeof(ApiKeyAttribute),
            ApiName = null
        },
        new RequiredAnnotationApiNameTest
        {
            Name = $"{nameof(ApiKeyAttribute)} rejects an empty API name",
            AnnotationType = typeof(ApiKeyAttribute),
            ApiName = string.Empty
        },
        new RequiredAnnotationApiNameTest
        {
            Name = $"{nameof(ApiKeyAttribute)} rejects a whitespace API name",
            AnnotationType = typeof(ApiKeyAttribute),
            ApiName = " "
        },
        new RequiredAnnotationApiNameTest
        {
            Name = $"{nameof(ApiRelationshipAttribute)} rejects a null API name",
            AnnotationType = typeof(ApiRelationshipAttribute),
            ApiName = null
        },
        new RequiredAnnotationApiNameTest
        {
            Name = $"{nameof(ApiRelationshipAttribute)} rejects an empty API name",
            AnnotationType = typeof(ApiRelationshipAttribute),
            ApiName = string.Empty
        },
        new RequiredAnnotationApiNameTest
        {
            Name = $"{nameof(ApiRelationshipAttribute)} rejects a whitespace API name",
            AnnotationType = typeof(ApiRelationshipAttribute),
            ApiName = " "
        },
        new RequiredAnnotationApiNameTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)} rejects a null API name",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            ApiName = null
        },
        new RequiredAnnotationApiNameTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)} rejects an empty API name",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            ApiName = string.Empty
        },
        new RequiredAnnotationApiNameTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)} rejects a whitespace API name",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            ApiName = " "
        },
        new RequiredAnnotationApiNameTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)} rejects a null API name",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            ApiName = null
        },
        new RequiredAnnotationApiNameTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)} rejects an empty API name",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            ApiName = string.Empty
        },
        new RequiredAnnotationApiNameTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)} rejects a whitespace API name",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            ApiName = " "
        },
        new RequiredAnnotationApiNameTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)} rejects a null API name",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            ApiName = null
        },
        new RequiredAnnotationApiNameTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)} rejects an empty API name",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            ApiName = string.Empty
        },
        new RequiredAnnotationApiNameTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)} " +
                    "rejects a whitespace API name",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            ApiName = " "
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] RequiredAnnotationClrTypeTheoryData =>
    [
        new RequiredAnnotationClrTypeTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)} rejects a null principal type",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiRelationshipDefinitionAttribute.PrincipalType)
        },
        new RequiredAnnotationClrTypeTest
        {
            Name =
                $"{nameof(ApiRelationshipDefinitionAttribute)} rejects a null dependent type",
            AnnotationType = typeof(ApiRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiRelationshipDefinitionAttribute.DependentType)
        },
        new RequiredAnnotationClrTypeTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)} rejects a null association type",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipAttribute.AssociationType)
        },
        new RequiredAnnotationClrTypeTest
        {
            Name = $"{nameof(ApiManyToManyRelationshipAttribute)} rejects a null other principal type",
            AnnotationType = typeof(ApiManyToManyRelationshipAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipAttribute.OtherPrincipalType)
        },
        new RequiredAnnotationClrTypeTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)} " +
                    "rejects a null principal type A",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.PrincipalTypeA)
        },
        new RequiredAnnotationClrTypeTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)} " +
                    "rejects a null principal type B",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.PrincipalTypeB)
        },
        new RequiredAnnotationClrTypeTest
        {
            Name =
                $"{nameof(ApiManyToManyRelationshipDefinitionAttribute)} " +
                    "rejects a null association type",
            AnnotationType = typeof(ApiManyToManyRelationshipDefinitionAttribute),
            PropertyName = nameof(ApiManyToManyRelationshipDefinitionAttribute.AssociationType)
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(AnnotationConstructorTheoryData))]
    public void AnnotationConstructor(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(AnnotationPropertyInitTheoryData))]
    public void AnnotationPropertyInit(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(RequiredAnnotationPropertyTheoryData))]
    public void RequiredAnnotationProperty(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(OptionalAnnotationPropertyTheoryData))]
    public void OptionalAnnotationProperty(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(RequiredAnnotationApiNameTheoryData))]
    public void RequiredAnnotationApiName(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(RequiredAnnotationClrTypeTheoryData))]
    public void RequiredAnnotationClrType(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Private Methods
    private static Attribute CreateAnnotationWithApiName(Type annotationType, string? apiName)
    {
        Attribute attribute = annotationType switch
        {
            var type when type == typeof(ApiKeyAttribute) => new ApiKeyAttribute
            {
                ApiName = apiName!
            },
            var type when type == typeof(ApiRelationshipAttribute) => new ApiRelationshipAttribute
            {
                ApiName = apiName!,
                Kind = ApiRelationshipKind.OneToMany
            },
            var type when type == typeof(ApiRelationshipDefinitionAttribute) =>
                new ApiRelationshipDefinitionAttribute
                {
                    ApiName = apiName!,
                    PrincipalType = typeof(AnnotationPrincipal),
                    Kind = ApiRelationshipKind.OneToMany,
                    DependentType = typeof(AnnotationDependent)
                },
            var type when type == typeof(ApiManyToManyRelationshipAttribute) =>
                new ApiManyToManyRelationshipAttribute
                {
                    ApiName = apiName!,
                    AssociationType = typeof(AnnotationAssociation),
                    OtherPrincipalType = typeof(AnnotationPrincipal)
                },
            var type when type == typeof(ApiManyToManyRelationshipDefinitionAttribute) =>
                new ApiManyToManyRelationshipDefinitionAttribute
                {
                    ApiName = apiName!,
                    PrincipalTypeA = typeof(AnnotationPrincipal),
                    PrincipalTypeB = typeof(AnnotationDependent),
                    AssociationType = typeof(AnnotationAssociation)
                },
            _ => throw new ArgumentException("Unknown annotation type.", nameof(annotationType))
        };

        return attribute;
    }

    private static Attribute CreateAnnotationWithNullClrType
    (
        Type annotationType,
        string propertyName
    )
    {
        Attribute attribute = annotationType switch
        {
            var type when type == typeof(ApiRelationshipDefinitionAttribute) =>
                new ApiRelationshipDefinitionAttribute
                {
                    ApiName = "Relationship",
                    Kind = ApiRelationshipKind.OneToMany,
                    PrincipalType =
                    propertyName == nameof(ApiRelationshipDefinitionAttribute.PrincipalType)
                    ? null!
                    : typeof(AnnotationPrincipal),
                    DependentType =
                    propertyName == nameof(ApiRelationshipDefinitionAttribute.DependentType)
                    ? null!
                    : typeof(AnnotationDependent)
                },
            var type when type == typeof(ApiManyToManyRelationshipAttribute) =>
                new ApiManyToManyRelationshipAttribute
                {
                    ApiName = "Relationship",
                    AssociationType =
                    propertyName == nameof(ApiManyToManyRelationshipAttribute.AssociationType)
                        ? null!
                        : typeof(AnnotationAssociation),
                    OtherPrincipalType =
                    propertyName == nameof(ApiManyToManyRelationshipAttribute.OtherPrincipalType)
                        ? null!
                        : typeof(AnnotationPrincipal)
                },
            var type when type == typeof(ApiManyToManyRelationshipDefinitionAttribute) =>
                new ApiManyToManyRelationshipDefinitionAttribute
                {
                    ApiName = "Relationship",
                    PrincipalTypeA =
                    propertyName ==
                        nameof(ApiManyToManyRelationshipDefinitionAttribute.PrincipalTypeA)
                        ? null!
                        : typeof(AnnotationPrincipal),
                    PrincipalTypeB =
                    propertyName ==
                        nameof(ApiManyToManyRelationshipDefinitionAttribute.PrincipalTypeB)
                        ? null!
                        : typeof(AnnotationDependent),
                    AssociationType =
                    propertyName ==
                        nameof(ApiManyToManyRelationshipDefinitionAttribute.AssociationType)
                        ? null!
                        : typeof(AnnotationAssociation)
                },
            _ => throw new ArgumentException("Unknown annotation type.", nameof(annotationType))
        };

        return attribute;
    }
    #endregion

    #region Fixture Types
    private sealed class AnnotationPrincipal;

    private sealed class AnnotationDependent;

    private sealed class AnnotationAssociation;

    [ApiObject(ApiName = "AnnotatedPoco")]
    [ApiKey(ApiName = "AnnotatedPocoKey", ClrPath = nameof(Id))]
    [ApiRelationshipDefinition
    (
        ApiName = "AnnotatedPocoDependent",
        PrincipalType = typeof(AnnotatedPoco),
        DependentType = typeof(AnnotationDependent),
        Kind = ApiRelationshipKind.OneToMany,
        DeleteBehavior = ApiRelationshipDeleteBehavior.Delete
    )]
    [ApiManyToManyRelationshipDefinition
    (
        ApiName = "AnnotatedPocoAssociations",
        PrincipalTypeA = typeof(AnnotatedPoco),
        PrincipalTypeB = typeof(AnnotationDependent),
        AssociationType = typeof(AnnotationAssociation)
    )]
    private sealed class AnnotatedPoco
    {
        [ApiKey]
        [ApiProperty(ApiName = "id", IsRequired = true)]
        public int Id { get; set; }

        [ApiIgnore]
        public string Ignored { get; set; } = string.Empty;

        [ApiRelationship
        (
            ApiName = "AnnotatedPocoNavigation",
            Kind = ApiRelationshipKind.OneToOne,
            DeleteBehavior = ApiRelationshipDeleteBehavior.None
        )]
        public AnnotationDependent? Dependent { get; set; }

        [ApiManyToManyRelationship
        (
            ApiName = "AnnotatedPocoManyToMany",
            AssociationType = typeof(AnnotationAssociation),
            OtherPrincipalType = typeof(AnnotationDependent)
        )]
        public List<AnnotationDependent> Dependents { get; } = [];
    }

    [ApiScalar(ApiName = "AnnotatedScalar")]
    private readonly record struct AnnotatedScalar(int Value);

    [ApiEnum(ApiName = "AnnotatedEnum")]
    private enum AnnotatedEnum
    {
        [ApiEnumValue(ApiName = "first")]
        First
    }
    #endregion
}
