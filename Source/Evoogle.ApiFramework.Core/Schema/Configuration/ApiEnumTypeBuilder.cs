// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed class ApiEnumTypeBuilder(Type clrType, ApiSchemaBuilderContext context)
    : ApiNamedTypeBuilder<ApiEnumTypeBuilder>(clrType, context)
{
    #region Fields
    private readonly List<ApiEnumValue> _values = [];
    #endregion

    #region Builder Methods
    public ApiEnumTypeBuilder AddValue(string apiName, string clrName, int clrOrdinal)
    {
        _values.Add(new ApiEnumValue(apiName, clrName, clrOrdinal));
        return this;
    }

    public ApiEnumType Build()
    {
        var finalValues = _values.Count != 0
            ? _values
            : Enum.GetValues(this.ClrType)
                  .Cast<object>()
                  .Select(value =>
                  {
                      var name = Enum.GetName(this.ClrType, value)!;
                      var ordinal = Convert.ToInt32(value);
                      return new ApiEnumValue(name, name, ordinal);
                  })
                  .ToList();

        return new ApiEnumType(this.ApiName, finalValues, this.ClrType);
    }
    #endregion
}
