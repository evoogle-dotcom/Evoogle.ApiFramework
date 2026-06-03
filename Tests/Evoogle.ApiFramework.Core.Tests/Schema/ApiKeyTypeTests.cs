// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiKeyTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Data
    private static ApiSchema KeyApiSchema { get; } = ApiSchemaFactory.KeyApiSchema;

    private static ApiKeyType GetPrimaryKeyType(string apiObjectTypeName)
    {
        var apiObjectType = KeyApiSchema.GetObjectTypeByApiName(apiObjectTypeName);
        return apiObjectType.ApiPrimaryKeyType ?? throw new InvalidOperationException($"No primary key type on '{apiObjectTypeName}'.");
    }

    private static ApiKeyType GetKeyTypeByName(string apiObjectTypeName, string apiKeyTypeName)
    {
        var apiObjectType = KeyApiSchema.GetObjectTypeByApiName(apiObjectTypeName);
        return apiObjectType.TryGetKeyTypeByApiName(apiKeyTypeName, out var keyType)
            ? keyType
            : throw new InvalidOperationException($"Key type '{apiKeyTypeName}' not found on '{apiObjectTypeName}'.");
    }

    // CLR instances with fully populated values
    private static KeyOneScalarPart KeyOneScalarPartInstance { get; } = new() { Id = 42, Name = "TestName" };
    private static KeyTwoScalarPartComposite KeyTwoScalarPartCompositeInstance { get; } = new() { Id1 = 1, Id2 = "abc" };
    private static KeyThreeScalarPartComposite KeyThreeScalarPartCompositeInstance { get; } = new() { Id1 = 10, Id2 = "xyz", Id3 = Guid.Parse("11111111-1111-1111-1111-111111111111") };
    private static KeyNested KeyNestedInstance { get; } = new() { Id = 5 };
    private static KeyNestedComposite KeyNestedCompositeInstance { get; } = new() { NestedPart = KeyNestedInstance, Name = "Nested" };
    private static KeyOwner KeyOwnerInstance { get; } = new() { Id = 99 };
    private static KeyOwnedComposite KeyOwnedCompositeInstance { get; } = new() { LineNumber = 3, Description = "Line3" };
    private static KeyOwnedDependent KeyOwnedDependentInstance { get; } = new() { Description = "Dep" };

    // CLR instances with null property values for null-handling tests
    private static KeyTwoScalarPartComposite KeyTwoScalarPartCompositeWithNullId2PropertyInstance { get; } = new() { Id1 = 1, Id2 = null };
    private static KeyNestedComposite KeyNestedCompositeWithNullNestedPartPropertyInstance { get; } = new() { NestedPart = null!, Name = "NoNested" };
    #endregion
}
