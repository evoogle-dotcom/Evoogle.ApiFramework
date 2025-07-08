// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed class ApiScalarTypeBuilder : IApiNamedTypeBuilder
{
    public string ApiName { get; }
    public Type ClrType { get; }

    public ApiScalarTypeBuilder(string apiName, Type clrType)
    {
        ApiName = apiName ?? throw new ArgumentNullException(nameof(apiName));
        ClrType = clrType ?? throw new ArgumentNullException(nameof(clrType));
    }

    public ApiScalarType Build() => new ApiScalarType(ApiName, ClrType);
}

