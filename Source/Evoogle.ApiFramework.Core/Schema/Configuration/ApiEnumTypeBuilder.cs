// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed class ApiEnumTypeBuilder : IApiNamedTypeBuilder
{
    public string ApiName { get; }
    public Type ClrType { get; }
    private readonly List<ApiEnumValue> _values = new();

    public ApiEnumTypeBuilder(string apiName, Type clrEnumType)
    {
        ApiName = apiName ?? throw new ArgumentNullException(nameof(apiName));
        ClrType = clrEnumType ?? throw new ArgumentNullException(nameof(clrEnumType));

        if (!clrEnumType.IsEnum)
            throw new ArgumentException("Provided CLR type must be an enum.", nameof(clrEnumType));
    }

    public ApiEnumTypeBuilder AddValue(string apiName, string clrName, int clrOrdinal)
    {
        _values.Add(new ApiEnumValue(apiName, clrName, clrOrdinal));
        return this;
    }

    public ApiEnumType Build()
    {
        var finalValues = _values.Any()
            ? _values
            : Enum.GetValues(ClrType)
                  .Cast<object>()
                  .Select(value =>
                  {
                      var name = Enum.GetName(ClrType, value)!;
                      var ordinal = Convert.ToInt32(value);
                      return new ApiEnumValue(name, name, ordinal);
                  })
                  .ToList();

        return new ApiEnumType(ApiName, finalValues, ClrType);
    }
}
