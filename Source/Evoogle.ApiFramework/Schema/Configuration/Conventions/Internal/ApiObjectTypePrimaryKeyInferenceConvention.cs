// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiObjectTypePrimaryKeyInferenceConvention : IApiObjectTypeConvention
{
    #region Properties
    /// <inheritdoc />
    public ApiConventionPhase Phase => ApiConventionPhase.Configuration;
    #endregion

    #region Fields
    private static readonly HashSet<Type> _keyCompatibleTypes =
    [
        typeof(Guid),
        typeof(int),
        typeof(long),
        typeof(string),
    ];
    #endregion

    #region IApiObjectTypeConvention
    /// <inheritdoc />
    public void Apply(ApiObjectTypeBuilder builder)
    {
        var clrType = builder.ClrType;
        var idName = "Id";
        var classIdName = clrType.Name + "Id";

        var clrName = FindIdMember(clrType, idName) ?? FindIdMember(clrType, classIdName);

        if (clrName == null)
        {
            return;
        }

        builder.AddKeyIfAbsent("PrimaryKey", b => b.AddPath(clrType, clrName));
    }
    #endregion

    #region Implementation Methods
    private static string? FindIdMember(Type clrType, string candidateName)
    {
        var property = clrType.GetProperty
        (
            candidateName,
            BindingFlags.Public | BindingFlags.Instance
        );
        if (property != null && IsKeyCompatible(property.PropertyType))
        {
            return candidateName;
        }

        var field = clrType.GetField(candidateName, BindingFlags.Public | BindingFlags.Instance);
        if (field != null && IsKeyCompatible(field.FieldType))
        {
            return candidateName;
        }

        return null;
    }

    private static bool IsKeyCompatible(Type type)
    {
        // Unwrap Nullable<T> to check the underlying type.
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        return _keyCompatibleTypes.Contains(underlying);
    }
    #endregion
}
