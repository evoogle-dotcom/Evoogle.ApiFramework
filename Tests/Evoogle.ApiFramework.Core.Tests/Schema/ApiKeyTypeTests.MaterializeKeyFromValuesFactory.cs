// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;

using Evoogle.ApiFramework.Key;
using Evoogle.ApiFramework.TestData;

namespace Evoogle.ApiFramework.Schema;

[DynamicLinqType]
public static class ApiKeyTypeMaterializeKeyFromValuesTestFactory
{
    #region Configure Values Methods
    public static void ConfigureTextIntTerminalScalar(ApiKeyMaterializationContext context)
    {
        context.WithText<KeyOneScalarPart>(nameof(KeyOneScalarPart.Id), "1234");
    }

    public static void ConfigureTextStringTerminalScalar(ApiKeyMaterializationContext context)
    {
        context.WithText<KeyOneScalarPart>(nameof(KeyOneScalarPart.Name), "1234");
    }

    public static void ConfigureCompositeApiKeyValues(ApiKeyMaterializationContext context)
    {
        context
            .WithKey<KeyTwoScalarPartComposite>(nameof(KeyTwoScalarPartComposite.Id1), ApiKey.FromInt32(1))
            .WithKey<KeyTwoScalarPartComposite>(nameof(KeyTwoScalarPartComposite.Id2), ApiKey.FromString("abc"));
    }

    public static void ConfigureTypedConvenienceValues(ApiKeyMaterializationContext context)
    {
        context
            .WithKey<KeyThreeScalarPartComposite>(nameof(KeyThreeScalarPartComposite.Id1), 10)
            .WithText<KeyThreeScalarPartComposite>(nameof(KeyThreeScalarPartComposite.Id2), "xyz")
            .WithKey<KeyThreeScalarPartComposite>(nameof(KeyThreeScalarPartComposite.Id3), Guid.Parse("11111111-1111-1111-1111-111111111111"));
    }

    public static void ConfigureNestedClrPathValues(ApiKeyMaterializationContext context)
    {
        context
            .WithText<KeyNestedComposite>(nameof(KeyNestedComposite.NestedPart) + "." + nameof(KeyNested.Id), "5")
            .WithText<KeyNestedComposite>(nameof(KeyNestedComposite.Name), "Nested");
    }

    public static void ConfigureOwnerAndDependentRootValues(ApiKeyMaterializationContext context)
    {
        context
            .WithKey<KeyOwner>(nameof(KeyOwner.Id), 99)
            .WithKey<KeyOwnedComposite>(nameof(KeyOwnedComposite.LineNumber), 3);
    }

    public static void ConfigureOwnerOnlyDependentKey(ApiKeyMaterializationContext context)
    {
        context.WithText<KeyOwner>(nameof(KeyOwner.Id), "99");
    }

    public static void ConfigureCustomPartNameBuilderValues(ApiKeyMaterializationContext context)
    {
        context
            .WithKey<KeyTwoScalarPartComposite>(nameof(KeyTwoScalarPartComposite.Id1), 1)
            .WithText<KeyTwoScalarPartComposite>(nameof(KeyTwoScalarPartComposite.Id2), "abc");
    }

    public static void ConfigureMissingCompositeValue(ApiKeyMaterializationContext context)
    {
        context.WithKey<KeyTwoScalarPartComposite>(nameof(KeyTwoScalarPartComposite.Id1), 1);
    }

    public static void ConfigureNullText(ApiKeyMaterializationContext context)
    {
        context.WithText<KeyOneScalarPart>(nameof(KeyOneScalarPart.Id), null);
    }

    public static void ConfigureWhitespaceText(ApiKeyMaterializationContext context)
    {
        context.WithText<KeyOneScalarPart>(nameof(KeyOneScalarPart.Id), "   ");
    }

    public static void ConfigureEmptyApiKey(ApiKeyMaterializationContext context)
    {
        context.WithKey<KeyOneScalarPart>(nameof(KeyOneScalarPart.Id), ApiKey.Empty);
    }

    public static void ConfigureInvalidTextParse(ApiKeyMaterializationContext context)
    {
        context.WithText<KeyOneScalarPart>(nameof(KeyOneScalarPart.Id), "abc");
    }

    public static void ConfigureMismatchedApiKeyKind(ApiKeyMaterializationContext context)
    {
        context.WithKey<KeyOneScalarPart>(nameof(KeyOneScalarPart.Id), ApiKey.FromString("1234"));
    }
    #endregion
}
