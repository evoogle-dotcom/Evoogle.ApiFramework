// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiIdentityTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Data
    private static ApiSchema IdentityApiSchema { get; } = ApiSchemaFactory.IdentityApiSchema;

    private static ApiIdentity GetPrimaryIdentity(string apiObjectTypeName)
    {
        var apiObjectType = IdentityApiSchema.GetObjectTypeByApiName(apiObjectTypeName);
        return apiObjectType.ApiPrimaryIdentity ?? throw new InvalidOperationException($"No primary identity on '{apiObjectTypeName}'.");
    }

    private static ApiIdentity GetIdentityByName(string apiObjectTypeName, string apiIdentityName)
    {
        var apiObjectType = IdentityApiSchema.GetObjectTypeByApiName(apiObjectTypeName);
        return apiObjectType.TryGetIdentityByApiName(apiIdentityName, out var identity)
            ? identity
            : throw new InvalidOperationException($"Identity '{apiIdentityName}' not found on '{apiObjectTypeName}'.");
    }

    // CLR instances with populated values
    private static IdentityScalar ScalarInstance { get; } = new() { Id = 42, Name = "TestName" };
    private static IdentityTwoScalarPartComposite TwoPartInstance { get; } = new() { Id1 = 1, Id2 = "abc" };
    private static IdentityThreeScalarPartComposite ThreePartInstance { get; } = new() { Id1 = 10, Id2 = "xyz", Id3 = Guid.Parse("11111111-1111-1111-1111-111111111111") };
    private static IdentityNested NestedPartInstance { get; } = new() { Id = 5 };
    private static IdentityNestedComposite NestedCompositeInstance { get; } = new() { NestedPart = NestedPartInstance, Name = "Nested" };
    private static IdentityOwner OwnerInstance { get; } = new() { Id = 99 };
    private static IdentityOwnedComposite OwnedCompositeInstance { get; } = new() { LineNumber = 3, Description = "Line3" };
    private static IdentityOwnedDependent OwnedDependentInstance { get; } = new() { Description = "Dep" };

    // CLR instances with null property values for null-handling tests
    private static IdentityTwoScalarPartComposite TwoPartNullId2Instance { get; } = new() { Id1 = 1, Id2 = null };
    private static IdentityNestedComposite NestedCompositeNullNestedPartInstance { get; } = new() { NestedPart = null!, Name = "NoNested" };
    #endregion
}
