// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Creates canonical closed-generic builder instances for runtime CLR types.
/// </summary>
internal static class ApiBuilderFactory
{
    #region Factory Methods
    internal static TBuilder CreateClosedGeneric<TBuilder>
    (
        Type genericBuilderDefinition,
        Type clrType,
        params object?[] arguments
    )
    {
        ArgumentNullException.ThrowIfNull(genericBuilderDefinition);
        ArgumentNullException.ThrowIfNull(clrType);

        try
        {
            var builderType = genericBuilderDefinition.MakeGenericType(clrType);
            return (TBuilder)Activator.CreateInstance(builderType, arguments)!;
        }
        catch (Exception exception) when
        (
            exception is ArgumentException or
            InvalidOperationException or
            MissingMethodException or
            System.Reflection.TargetInvocationException
        )
        {
            throw new ApiSchemaConfigurationException
            (
                $"Unable to create {genericBuilderDefinition.Name} for CLR type '{clrType.Name}'.",
                exception
            );
        }
    }
    #endregion
}
