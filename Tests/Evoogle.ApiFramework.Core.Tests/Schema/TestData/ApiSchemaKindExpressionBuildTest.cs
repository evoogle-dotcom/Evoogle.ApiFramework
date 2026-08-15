// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema.TestData;

/// <summary>Builds the expected <see cref="ApiSchema"/> from a built-in <see cref="TestData.ApiSchemaKind"/> fixture.</summary>
public class ApiSchemaKindExpressionBuildTest : ApiSchemaExpressionBuildTest
{
    #region User Supplied Properties
    public required ApiSchemaKind ApiSchemaKind { get; init; }
    #endregion

    #region Constructors
    public ApiSchemaKindExpressionBuildTest() => this.Name = nameof(ApiSchemaKindExpressionBuildTest);
    #endregion

    #region XUnitTest Methods
    protected override void Arrange()
    {
        this.ApiSchemaExpected = BuildApiSchemaExpected(this.ApiSchemaKind, this.GetExtensionTypes());

        this.WriteLine($"ApiSchemaKind: {this.ApiSchemaKind.SafeToString()}");
        this.WriteLine();
        this.WriteLine("ApiSchemaExpected:");
        this.WriteLine($"{this.ApiSchemaExpected.SafeToJson(_defaultToJsonOptions)}");
        this.WriteLine();
    }
    #endregion

    #region ApiSchemaKindExpressionBuildTest Methods
    /// <summary>Builds an expected <see cref="ApiSchema"/> for <paramref name="apiSchemaKind"/>, merging in default instances of <paramref name="apiExtensionTypes"/>.</summary>
    public static ApiSchema BuildApiSchemaExpected(ApiSchemaKind apiSchemaKind, IEnumerable<Type> apiExtensionTypes)
    {
        var apiSchema = BuildTestApiSchema(apiSchemaKind) ?? throw new InvalidOperationException($"{nameof(ApiSchema)} creation failed.");
        var apiSchemaExpected = apiSchema.DeepCopy()!;

        var apiExtensionTypesList = apiExtensionTypes.SafeToList();
        if (apiExtensionTypesList.Count > 0)
        {
            apiSchemaExpected.Extensions ??= [];

            foreach (var apiExtensionType in apiExtensionTypesList)
            {
                var extensionInstance = Activator.CreateInstance(apiExtensionType);
                apiSchemaExpected.Extensions[apiExtensionType] = extensionInstance!;
            }
        }

        return apiSchemaExpected;
    }

    /// <summary>Gets the extension types merged into the expected schema. Defaults to none.</summary>
    protected virtual IEnumerable<Type> GetExtensionTypes() => [];
    #endregion
}

public class ApiSchemaKindExpressionBuildTest<TExtension>() : ApiSchemaKindExpressionBuildTest
{
    #region ApiSchemaKindExpressionBuildTest Methods
    protected override IEnumerable<Type> GetExtensionTypes()
    {
        yield return typeof(TExtension);
    }
    #endregion
}

public class ApiSchemaKindExpressionBuildTest<TExtension1, TExtension2>() : ApiSchemaKindExpressionBuildTest
{
    #region ApiSchemaKindExpressionBuildTest Methods
    protected override IEnumerable<Type> GetExtensionTypes()
    {
        yield return typeof(TExtension1);
        yield return typeof(TExtension2);
    }
    #endregion
}
