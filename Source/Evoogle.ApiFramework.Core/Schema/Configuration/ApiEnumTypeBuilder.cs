// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to define an <see cref="ApiEnumType"/> within an <see cref="ApiSchemaBuilder"/>.
/// </summary>
/// <param name="clrType">The CLR enum type being described.</param>
/// <param name="context">The shared builder context.</param>
public sealed class ApiEnumTypeBuilder(Type clrType, ApiSchemaBuilderContext context)
    : ApiNamedTypeBuilder<ApiEnumTypeBuilder>(clrType, context)
{
    #region Fields
    private readonly List<ApiEnumValue> _values = [];
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Adds an <see cref="ApiEnumValue"/> definition to the enumeration.
    /// </summary>
    /// <param name="apiName">The API name of the enumeration value.</param>
    /// <param name="clrName">The CLR name of the enumeration value.</param>
    /// <param name="clrOrdinal">The CLR ordinal of the enumeration value.</param>
    /// <returns>The current builder instance.</returns>
    public ApiEnumTypeBuilder AddValue(string apiName, string clrName, int clrOrdinal)
    {
        _values.Add(new ApiEnumValue(apiName, clrName, clrOrdinal));
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="ApiEnumType"/> using the configured values.
    /// </summary>
    /// <returns>The constructed <see cref="ApiEnumType"/>.</returns>
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
