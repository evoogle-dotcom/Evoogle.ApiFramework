// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.TestData;

/// <summary>
///     Produces a compact but expressive ApiSchema suitable for most unit tests.
/// </summary>
public static class ApiTestSchemaUtils
{
    #region Type Helpers
    public static ApiScalarType S(string name, Type clr, OrderedDictionary<Type, object>? extensions = null)
        => new(name, clr)
        {
            Extensions = extensions
        };

    public static ApiEnumType E(string name, Type clr, IEnumerable<ApiEnumValue> values, OrderedDictionary<Type, object>? extensions = null)
        => new(name, values, clr)
        {
            Extensions = extensions
        };

    public static ApiEnumValue EV(string name, int ordinal, OrderedDictionary<Type, object>? extensions = null)
        => new(name, name, ordinal)
        {
            Extensions = extensions
        };

    public static ApiObjectType O(string name, Type clr, IEnumerable<ApiProperty> properties, IEnumerable<ApiRelationship>? relationships = null, ApiObjectTypeOptions? options = null, OrderedDictionary<Type, object>? extensions = null)
        => new(name, properties, relationships ?? [], apiIdentitySet: null, options, clr)
        {
            Extensions = extensions
        };

    public static ApiObjectTypeOptions OO(ApiIdentityNullHandling identityNullHandling)
        => new()
        {
            ApiIdentityNullHandling = identityNullHandling
        };

    public static ApiProperty P(string name, ApiTypeExpression expression, bool required, OrderedDictionary<Type, object>? extensions = null)
        => new(name, expression, required ? ApiTypeModifiers.Required : ApiTypeModifiers.None, name)
        {
            Extensions = extensions
        };

    public static ApiRelationship R(string name, string propertyName, OrderedDictionary<Type, object>? extensions = null)
        => new(name, propertyName)
        {
            Extensions = extensions
        };

    // private static IDictionary<string, object?> Ext(IDictionary<string, object?> dict) => dict;
    #endregion

    #region Type Expression Helpers
    public static class TE
    {
        public static ApiTypeExpression ClrRef<T>() => ApiTypeExpression.ClrRef<T>();

        public static ApiTypeExpression HashSetOf<T>(bool required) => ApiTypeExpression.HashSetOf<T>(required ? ApiTypeModifiers.Required : ApiTypeModifiers.None);

        public static ApiTypeExpression ListOf<T>(bool required) => ApiTypeExpression.ListOf<T>(required ? ApiTypeModifiers.Required : ApiTypeModifiers.None);
    }
    #endregion
}
