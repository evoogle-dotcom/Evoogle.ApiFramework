// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;

using Evoogle.ApiFramework.TestData;

namespace Evoogle.ApiFramework.Schema.Configuration;

[DynamicLinqType]
public static class ApiObjectTypeBuilderTestsGenericTestFactory
{
    #region Helper Types
    /// <summary>Local configuration class used by <see cref="IApiObjectTypeConfiguration{TObject}"/> tests.</summary>
    internal sealed class TestCustomerConfiguration : IApiObjectTypeConfiguration<Customer>
    {
        public void Configure(ApiObjectTypeBuilder<Customer> builder)
        {
            builder
                .WithName("Customer")
                .AddProperty(c => c.Id)
                .AddProperty(c => c.Name)
                .AddKey("PK_Person_Id", c => c.AddPath(p => p.Id));
        }
    }
    #endregion

    #region Generic Factory Methods
    public static ApiObjectType BuildFromSimpleApiSchemaTheEmptyType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheEmptyTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheEmptyType<TExtension>()
        where TExtension : class, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheEmptyTypeBuilder(ctx)
            .AddObjectTypeExtension(new TExtension());

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheEmptyType<TExtension1, TExtension2>()
        where TExtension1 : class, new()
        where TExtension2 : class, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheEmptyTypeBuilder(ctx)
            .AddObjectTypeExtension(new TExtension1())
            .AddObjectTypeExtension(new TExtension2());

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheScalarsOnlyType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheScalarsOnlyTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheScalarsOnlyType<TExtension>()
        where TExtension : class, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheScalarsOnlyTypeBuilder(ctx)
            .AddObjectTypeExtension(new TExtension());

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheScalarsOnlyType<TExtension1, TExtension2>()
        where TExtension1 : class, new()
        where TExtension2 : class, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheScalarsOnlyTypeBuilder(ctx)
            .AddObjectTypeExtension(new TExtension1())
            .AddObjectTypeExtension(new TExtension2());

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheScalarsOnlyTypeWithAddRequiredOrOptionalProperty()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheScalarsOnlyTypeWithAddRequiredOrOptionalPropertyBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheCompanyType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheCompanyTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheCompanyTypeWithConfigureRequiredOrOptionalProperty()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheCompanyTypeWithConfigureRequiredOrOptionalPropertyBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheCompanyType<TExtension>()
        where TExtension : class, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheCompanyTypeBuilder(ctx)
            .AddObjectTypeExtension(new TExtension());

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheCompanyType<TExtension1, TExtension2>()
        where TExtension1 : class, new()
        where TExtension2 : class, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheCompanyTypeBuilder(ctx)
            .AddObjectTypeExtension(new TExtension1())
            .AddObjectTypeExtension(new TExtension2());

        return builder.Build();
    }

    public static ApiObjectType BuildFromKeyApiSchemaTheKeyOneScalarPartType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromKeyApiSchemaTheKeyOneScalarPartTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromKeyApiSchemaTheKeyTwoScalarPartCompositeType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromKeyApiSchemaTheKeyTwoScalarPartCompositeTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromKeyApiSchemaTheKeyThreeScalarPartCompositeType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromKeyApiSchemaTheKeyThreeScalarPartCompositeTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromKeyApiSchemaTheKeyNestedCompositeType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromKeyApiSchemaTheKeyNestedCompositeTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromKeyApiSchemaTheKeyOwnedCompositeType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromKeyApiSchemaTheKeyOwnedCompositeTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromKeyApiSchemaTheKeyOwnedDependentType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromKeyApiSchemaTheKeyOwnedDependentTypeBuilder(ctx);

        return builder.Build();
    }
    #endregion

    #region Implementation Methods
    private static ApiObjectTypeBuilder<Empty> CreateFromSimpleApiSchemaTheEmptyTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<Empty>(ctx)
            .WithName(nameof(Empty))
            .WithOptions(o => o.ThrowOnNullKeyPart());

        return builder;
    }

    private static ApiObjectTypeBuilder<ScalarsOnly> CreateFromSimpleApiSchemaTheScalarsOnlyTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<ScalarsOnly>(ctx)
            .WithName(nameof(ScalarsOnly))
            .WithOptions(o => o.ThrowOnNullKeyPart())
            .AddProperty(c => c.RequiredName)
            .AddProperty(c => c.RequiredNumber)
            .AddProperty(c => c.RequiredPredicate)
            .AddProperty(c => c.OptionalName)
            .AddProperty(c => c.OptionalNumber)
            .AddProperty(c => c.OptionalPredicate);

        return builder;
    }

    private static ApiObjectTypeBuilder<ScalarsOnly> CreateFromSimpleApiSchemaTheScalarsOnlyTypeWithAddRequiredOrOptionalPropertyBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<ScalarsOnly>(ctx)
            .WithName(nameof(ScalarsOnly))
            .WithOptions(o => o.ThrowOnNullKeyPart())
            .AddRequiredProperty(c => c.RequiredName)
            .AddRequiredProperty(c => c.RequiredNumber)
            .AddRequiredProperty(c => c.RequiredPredicate)
            .AddOptionalProperty(c => c.OptionalName)
            .AddOptionalProperty(c => c.OptionalNumber)
            .AddOptionalProperty(c => c.OptionalPredicate);

        return builder;
    }

    private static ApiObjectTypeBuilder<Company> CreateFromSimpleApiSchemaTheCompanyTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<Company>(ctx)
            .WithName(nameof(Company))
            .WithOptions(o => o.ThrowOnNullKeyPart())
            .AddProperty(c => c.Id)
            .AddProperty(c => c.Name)
            .AddProperty(c => c.Owner)
            .AddProperty(c => c.Employees)
            .AddKey("PK_Company_Id", c => c.AddPath(p => p.Id))
            .AddKey("AK_Company_Name", c => c.AddPath(p => p.Name));

        return builder;
    }

    private static ApiObjectTypeBuilder<Company> CreateFromSimpleApiSchemaTheCompanyTypeWithConfigureRequiredOrOptionalPropertyBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<Company>(ctx)
            .WithName(nameof(Company))
            .WithOptions(o => o.ThrowOnNullKeyPart())
            .AddProperty(c => c.Id, p => p.AsRequired())
            .AddProperty(c => c.Name, p => p.AsRequired())
            .AddProperty(c => c.Owner, p => p.AsOptional())
            .AddProperty(c => c.Employees, p => p.AsOptional())
            .AddKey("PK_Company_Id", c => c.AddPath(p => p.Id))
            .AddKey("AK_Company_Name", c => c.AddPath(p => p.Name));

        return builder;
    }

    private static ApiObjectTypeBuilder<KeyOneScalarPart> CreateFromKeyApiSchemaTheKeyOneScalarPartTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<KeyOneScalarPart>(ctx)
            .WithName(nameof(KeyOneScalarPart))
            .AddProperty(c => c.Id)
            .AddProperty(c => c.Name)
            .AddKey("PK_KeyOneScalarPart", c => c.AddPath(p => p.Id))
            .AddKey("AK_KeyOneScalarPart", c => c.AddPath(p => p.Name));

        return builder;
    }

    private static ApiObjectTypeBuilder<KeyTwoScalarPartComposite> CreateFromKeyApiSchemaTheKeyTwoScalarPartCompositeTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<KeyTwoScalarPartComposite>(ctx)
            .WithName(nameof(KeyTwoScalarPartComposite))
            .AddProperty(c => c.Id1)
            .AddProperty(c => c.Id2)
            .AddProperty(c => c.Description)
            .AddKey("PK_KeyTwoScalarPartComposite", p => p.Id1, p => p.Id2);

        return builder;
    }

    private static ApiObjectTypeBuilder<KeyThreeScalarPartComposite> CreateFromKeyApiSchemaTheKeyThreeScalarPartCompositeTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<KeyThreeScalarPartComposite>(ctx)
            .WithName(nameof(KeyThreeScalarPartComposite))
            .AddProperty(c => c.Id1)
            .AddProperty(c => c.Id2)
            .AddProperty(c => c.Id3)
            .AddProperty(c => c.Description)
            .AddKey("PK_KeyThreeScalarPartComposite", p => p.Id1, p => p.Id2, p => p.Id3);

        return builder;
    }

    private static ApiObjectTypeBuilder<KeyNestedComposite> CreateFromKeyApiSchemaTheKeyNestedCompositeTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<KeyNestedComposite>(ctx)
            .WithName(nameof(KeyNestedComposite))
            .AddProperty(c => c.NestedPart)
            .AddProperty(c => c.Name)
            .AddKey("PK_KeyNestedComposite", p => p.NestedPart.Id, p => p.Name);

        return builder;
    }

    private static ApiObjectTypeBuilder<KeyOwnedComposite> CreateFromKeyApiSchemaTheKeyOwnedCompositeTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<KeyOwnedComposite>(ctx)
            .WithName(nameof(KeyOwnedComposite))
            .AddProperty(c => c.LineNumber)
            .AddProperty(c => c.Description)
            .AddKey("PK_KeyOwnedComposite", c => c
                .AddPathFrom<KeyOwner>(p => p.Id)
                .AddPath(p => p.LineNumber));

        return builder;
    }

    private static ApiObjectTypeBuilder<KeyOwnedDependent> CreateFromKeyApiSchemaTheKeyOwnedDependentTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<KeyOwnedDependent>(ctx)
            .WithName(nameof(KeyOwnedDependent))
            .AddProperty(c => c.Description)
            .AddKey("PK_KeyOwnedDependent", c => c
                .AddPathFrom<KeyOwner>(p => p.Id));

        return builder;
    }
    #endregion
}
