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
    /// <summary>Local configuration class used by <see cref="IApiObjectTypeConfiguration{T}"/> tests.</summary>
    internal sealed class TestCustomerConfiguration : IApiObjectTypeConfiguration<Customer>
    {
        public void Configure(ApiObjectTypeBuilder<Customer> builder)
        {
            builder
                .WithName("Customer")
                .AddProperty(c => c.Id)
                .AddProperty(c => c.Name)
                .AddIdentity("PK_Customer", b => b.AddScalarPart(c => c.Id, typeof(string)));
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
        where TExtension : notnull, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheEmptyTypeBuilder(ctx)
            .AddObjectExtension(new TExtension());

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheEmptyType<TExtension1, TExtension2>()
        where TExtension1 : notnull, new()
        where TExtension2 : notnull, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheEmptyTypeBuilder(ctx)
            .AddObjectExtension(new TExtension1())
            .AddObjectExtension(new TExtension2());

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheScalarsOnlyType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheScalarsOnlyTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheScalarsOnlyType<TExtension>()
        where TExtension : notnull, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheScalarsOnlyTypeBuilder(ctx)
            .AddObjectExtension(new TExtension());

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheScalarsOnlyType<TExtension1, TExtension2>()
        where TExtension1 : notnull, new()
        where TExtension2 : notnull, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheScalarsOnlyTypeBuilder(ctx)
            .AddObjectExtension(new TExtension1())
            .AddObjectExtension(new TExtension2());

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
        where TExtension : notnull, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheCompanyTypeBuilder(ctx)
            .AddObjectExtension(new TExtension());

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheCompanyType<TExtension1, TExtension2>()
        where TExtension1 : notnull, new()
        where TExtension2 : notnull, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheCompanyTypeBuilder(ctx)
            .AddObjectExtension(new TExtension1())
            .AddObjectExtension(new TExtension2());

        return builder.Build();
    }

    public static ApiObjectType BuildFromIdentityApiSchemaTheIdentityScalarType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromIdentityApiSchemaTheIdentityScalarTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromIdentityApiSchemaTheIdentityTwoScalarPartCompositeType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromIdentityApiSchemaTheIdentityTwoScalarPartCompositeTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromIdentityApiSchemaTheIdentityThreeScalarPartCompositeType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromIdentityApiSchemaTheIdentityThreeScalarPartCompositeTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromIdentityApiSchemaTheIdentityNestedCompositeType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromIdentityApiSchemaTheIdentityNestedCompositeTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromIdentityApiSchemaTheIdentityOwnedCompositeType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromIdentityApiSchemaTheIdentityOwnedCompositeTypeBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromIdentityApiSchemaTheIdentityOwnedDependentType()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromIdentityApiSchemaTheIdentityOwnedDependentTypeBuilder(ctx);

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
            .AddIdentity("PK_Company_Id", b => b.AddScalarPart(c => c.Id))
            .AddIdentity("AK_Company_Name", b => b.AddScalarPart(c => c.Name));

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
            .AddIdentity("PK_Company_Id", b => b.AddScalarPart(c => c.Id))
            .AddIdentity("AK_Company_Name", b => b.AddScalarPart(c => c.Name));

        return builder;
    }

    private static ApiObjectTypeBuilder<IdentityScalar> CreateFromIdentityApiSchemaTheIdentityScalarTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<IdentityScalar>(ctx)
            .WithName(nameof(IdentityScalar))
            .AddProperty(c => c.Id)
            .AddProperty(c => c.Name)
            .AddIdentity("PK_IdentityScalar", b => b.AddScalarPart(c => c.Id))
            .AddIdentity("AK_IdentityScalar", b => b.AddScalarPart(c => c.Name));

        return builder;
    }

    private static ApiObjectTypeBuilder<IdentityTwoScalarPartComposite> CreateFromIdentityApiSchemaTheIdentityTwoScalarPartCompositeTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<IdentityTwoScalarPartComposite>(ctx)
            .WithName(nameof(IdentityTwoScalarPartComposite))
            .AddProperty(c => c.Id1)
            .AddProperty(c => c.Id2)
            .AddProperty(c => c.Description)
            .AddIdentity("PK_IdentityTwoScalarPartComposite",
                b => b.AddScalarPart(c => c.Id1)
                      .AddScalarPart(c => c.Id2));

        return builder;
    }

    private static ApiObjectTypeBuilder<IdentityThreeScalarPartComposite> CreateFromIdentityApiSchemaTheIdentityThreeScalarPartCompositeTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<IdentityThreeScalarPartComposite>(ctx)
            .WithName(nameof(IdentityThreeScalarPartComposite))
            .AddProperty(c => c.Id1)
            .AddProperty(c => c.Id2)
            .AddProperty(c => c.Id3)
            .AddProperty(c => c.Description)
            .AddIdentity("PK_IdentityThreeScalarPartComposite",
                b => b.AddScalarPart(c => c.Id1)
                      .AddScalarPart(c => c.Id2)
                      .AddScalarPart(c => c.Id3));

        return builder;
    }

    private static ApiObjectTypeBuilder<IdentityNestedComposite> CreateFromIdentityApiSchemaTheIdentityNestedCompositeTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<IdentityNestedComposite>(ctx)
            .WithName(nameof(IdentityNestedComposite))
            .AddProperty(c => c.NestedPart)
            .AddProperty(c => c.Name)
            .AddIdentity("PK_IdentityNestedComposite",
                b => b.AddNestedPart(c => c.NestedPart)
                      .AddScalarPart(c => c.Name));

        return builder;
    }

    private static ApiObjectTypeBuilder<IdentityOwnedComposite> CreateFromIdentityApiSchemaTheIdentityOwnedCompositeTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<IdentityOwnedComposite>(ctx)
            .WithName(nameof(IdentityOwnedComposite))
            .AddProperty(c => c.LineNumber)
            .AddProperty(c => c.Description)
            .AddIdentity("PK_IdentityOwnedComposite",
                b => b.AddOwnerPart()
                      .AddScalarPart(c => c.LineNumber));

        return builder;
    }

    private static ApiObjectTypeBuilder<IdentityOwnedDependent> CreateFromIdentityApiSchemaTheIdentityOwnedDependentTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<IdentityOwnedDependent>(ctx)
            .WithName(nameof(IdentityOwnedDependent))
            .AddProperty(c => c.Description)
            .AddIdentity("PK_IdentityOwnedDependent",
                b => b.AddOwnerPart());

        return builder;
    }
    #endregion
}
