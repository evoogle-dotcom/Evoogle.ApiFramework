// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;

using Evoogle.ApiFramework.TestData;

namespace Evoogle.ApiFramework.Schema.Configuration.Types;

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
                .AddProperty(static c => c.Id)
                .AddProperty(static c => c.Name)
                .AddKey("PK_Person_Id", static c => c.AddPath(static p => p.Id));
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
        where TExtension : class, IApiSchemaExtension, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheEmptyTypeBuilder(ctx)
            .AddObjectTypeExtension(new TExtension());

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheEmptyType<TExtension1, TExtension2>()
        where TExtension1 : class, IApiSchemaExtension, new()
        where TExtension2 : class, IApiSchemaExtension, new()
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
        where TExtension : class, IApiSchemaExtension, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheScalarsOnlyTypeBuilder(ctx)
            .AddObjectTypeExtension(new TExtension());

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheScalarsOnlyType<TExtension1, TExtension2>()
        where TExtension1 : class, IApiSchemaExtension, new()
        where TExtension2 : class, IApiSchemaExtension, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheScalarsOnlyTypeBuilder(ctx)
            .AddObjectTypeExtension(new TExtension1())
            .AddObjectTypeExtension(new TExtension2());

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheScalarsOnlyTypeWithExplicitNullability()
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

    public static ApiObjectType BuildFromSimpleApiSchemaTheCompanyTypeWithConfiguredNullability()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheCompanyTypeWithConfigureRequiredOrOptionalPropertyBuilder(ctx);

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheCompanyType<TExtension>()
        where TExtension : class, IApiSchemaExtension, new()
    {
        var ctx = new ApiSchemaBuilderContext();
        var builder = CreateFromSimpleApiSchemaTheCompanyTypeBuilder(ctx)
            .AddObjectTypeExtension(new TExtension());

        return builder.Build();
    }

    public static ApiObjectType BuildFromSimpleApiSchemaTheCompanyType<TExtension1, TExtension2>()
        where TExtension1 : class, IApiSchemaExtension, new()
        where TExtension2 : class, IApiSchemaExtension, new()
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
            .WithOptions(static o => o.ThrowOnNullKeyPart());

        return builder;
    }

    private static ApiObjectTypeBuilder<ScalarsOnly> CreateFromSimpleApiSchemaTheScalarsOnlyTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<ScalarsOnly>(ctx)
            .WithName(nameof(ScalarsOnly))
            .WithOptions(static o => o.ThrowOnNullKeyPart())
            .AddProperty(static c => c.RequiredName)
            .AddProperty(static c => c.RequiredNumber)
            .AddProperty(static c => c.RequiredPredicate)
            .AddProperty(static c => c.OptionalName)
            .AddProperty(static c => c.OptionalNumber)
            .AddProperty(static c => c.OptionalPredicate);

        return builder;
    }

    private static ApiObjectTypeBuilder<ScalarsOnly> CreateFromSimpleApiSchemaTheScalarsOnlyTypeWithAddRequiredOrOptionalPropertyBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<ScalarsOnly>(ctx)
            .WithName(nameof(ScalarsOnly))
            .WithOptions(static o => o.ThrowOnNullKeyPart())
            .AddProperty(static c => c.RequiredName, static property => property.AsNonNullable())
            .AddProperty(static c => c.RequiredNumber, static property => property.AsNonNullable())
            .AddProperty(static c => c.RequiredPredicate, static property => property.AsNonNullable())
            .AddProperty(static c => c.OptionalName, static property => property.AsNullable())
            .AddProperty(static c => c.OptionalNumber, static property => property.AsNullable())
            .AddProperty(static c => c.OptionalPredicate, static property => property.AsNullable());

        return builder;
    }

    private static ApiObjectTypeBuilder<Company> CreateFromSimpleApiSchemaTheCompanyTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<Company>(ctx)
            .WithName(nameof(Company))
            .WithOptions(static o => o.ThrowOnNullKeyPart())
            .AddProperty(static c => c.Id)
            .AddProperty(static c => c.Name)
            .AddProperty(static c => c.Owner)
            .AddProperty(static c => c.Employees)
            .AddKey("PK_Company_Id", static c => c.AddPath(static p => p.Id))
            .AddKey("AK_Company_Name", static c => c.AddPath(static p => p.Name))
            .WithCanonicalKey("AK_Company_Name");

        return builder;
    }

    private static ApiObjectTypeBuilder<Company> CreateFromSimpleApiSchemaTheCompanyTypeWithConfigureRequiredOrOptionalPropertyBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<Company>(ctx)
            .WithName(nameof(Company))
            .WithOptions(static o => o.ThrowOnNullKeyPart())
            .AddProperty(static c => c.Id, static p => p.AsNonNullable())
            .AddProperty(static c => c.Name, static p => p.AsNonNullable())
            .AddProperty(static c => c.Owner, static p => p.AsNullable())
            .AddProperty(static c => c.Employees, static p => p.AsNullable())
            .AddKey("PK_Company_Id", static c => c.AddPath(static p => p.Id))
            .AddKey("AK_Company_Name", static c => c.AddPath(static p => p.Name))
            .WithCanonicalKey("AK_Company_Name");

        return builder;
    }

    private static ApiObjectTypeBuilder<KeyOneScalarPart> CreateFromKeyApiSchemaTheKeyOneScalarPartTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<KeyOneScalarPart>(ctx)
            .WithName(nameof(KeyOneScalarPart))
            .AddProperty(static c => c.Id)
            .AddProperty(static c => c.Name)
            .AddKey("PK_KeyOneScalarPart", static c => c.AddPath(static p => p.Id))
            .AddKey("AK_KeyOneScalarPart", static c => c.AddPath(static p => p.Name))
            .WithCanonicalKey("AK_KeyOneScalarPart");

        return builder;
    }

    private static ApiObjectTypeBuilder<KeyTwoScalarPartComposite> CreateFromKeyApiSchemaTheKeyTwoScalarPartCompositeTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<KeyTwoScalarPartComposite>(ctx)
            .WithName(nameof(KeyTwoScalarPartComposite))
            .AddProperty(static c => c.Id1)
            .AddProperty(static c => c.Id2)
            .AddProperty(static c => c.Description)
            .AddKey("PK_KeyTwoScalarPartComposite", static p => p.Id1, static p => p.Id2);

        return builder;
    }

    private static ApiObjectTypeBuilder<KeyThreeScalarPartComposite> CreateFromKeyApiSchemaTheKeyThreeScalarPartCompositeTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<KeyThreeScalarPartComposite>(ctx)
            .WithName(nameof(KeyThreeScalarPartComposite))
            .AddProperty(static c => c.Id1)
            .AddProperty(static c => c.Id2)
            .AddProperty(static c => c.Id3)
            .AddProperty(static c => c.Description)
            .AddKey("PK_KeyThreeScalarPartComposite", static p => p.Id1, static p => p.Id2, static p => p.Id3);

        return builder;
    }

    private static ApiObjectTypeBuilder<KeyNestedComposite> CreateFromKeyApiSchemaTheKeyNestedCompositeTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<KeyNestedComposite>(ctx)
            .WithName(nameof(KeyNestedComposite))
            .AddProperty(static c => c.NestedPart)
            .AddProperty(static c => c.Name)
            .AddKey("PK_KeyNestedComposite", static p => p.NestedPart.Id, static p => p.Name);

        return builder;
    }

    private static ApiObjectTypeBuilder<KeyOwnedComposite> CreateFromKeyApiSchemaTheKeyOwnedCompositeTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<KeyOwnedComposite>(ctx)
            .WithName(nameof(KeyOwnedComposite))
            .AddProperty(static c => c.LineNumber)
            .AddProperty(static c => c.Description)
            .AddKey("PK_KeyOwnedComposite", static c => c
                .AddPathFrom<KeyOwner>(static p => p.Id)
                .AddPath(static p => p.LineNumber));

        return builder;
    }

    private static ApiObjectTypeBuilder<KeyOwnedDependent> CreateFromKeyApiSchemaTheKeyOwnedDependentTypeBuilder(ApiSchemaBuilderContext ctx)
    {
        var builder = new ApiObjectTypeBuilder<KeyOwnedDependent>(ctx)
            .WithName(nameof(KeyOwnedDependent))
            .AddProperty(static c => c.Description)
            .AddKey("PK_KeyOwnedDependent", static c => c
                .AddPathFrom<KeyOwner>(static p => p.Id));

        return builder;
    }
    #endregion
}
