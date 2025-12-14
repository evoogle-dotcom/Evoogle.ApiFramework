// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiTypeExpressionBuilder
{
    #region Factory Methods
    public static ApiTypeExpression Build(MemberNullableInfo clrPropertyNullabilityInfo)
    {
        if (clrPropertyNullabilityInfo.IsCollection)
        {
            return BuildInlineCollection(clrPropertyNullabilityInfo.CollectionChain);
        }

        return BuildClrTypeReference(clrPropertyNullabilityInfo.MemberType);
    }
    #endregion

    #region Implementation Methods
    private static ApiTypeExpression BuildClrTypeReference(Type clrType) => new(clrType);

    private static ApiTypeExpression BuildInlineCollection(IReadOnlyList<MemberNullableInfo.CollectionInfo> clrCollectionChain)
    {
        // Build the API type expression for the innermost (last) layer
        var clrCollectionChainCount = clrCollectionChain.Count;
        var clrCollectionChainLast = clrCollectionChain[clrCollectionChainCount - 1];
        var clrCollectionChainLastElementType = clrCollectionChainLast.ElementType;

        var apiTypeExpression = BuildClrTypeReference(clrCollectionChainLastElementType);

        // Wrap backward through the collection chain from innermost to outermost
        for (var i = clrCollectionChainCount - 1; i >= 0; --i)
        {
            var clrCollectionInfo = clrCollectionChain[i];
            var apiItemTypeModifiers = clrCollectionInfo.IsElementNullable ? ApiTypeModifiers.None : ApiTypeModifiers.Required;
            var clrCollectionType = clrCollectionInfo.CollectionType;

            var apiCollectionType = new ApiCollectionType(apiTypeExpression, apiItemTypeModifiers, clrCollectionType);

            apiTypeExpression = new ApiTypeExpression(apiCollectionType);
        }

        return apiTypeExpression;
    }
    #endregion
}
