// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.ApiFramework.Schema.Configuration.Types;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Version;

/// <summary>Version configuration extensions for strongly typed object builders.</summary>
public static class ApiObjectTypeBuilderVersionExtensions
{
    #region With Methods
    /// <summary>
    ///     Configures a property-backed version using a type-safe member expression.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type.</typeparam>
    /// <typeparam name="TVersion">The CLR member type.</typeparam>
    /// <param name="builder">The object type builder.</param>
    /// <param name="expression">The member that supplies the version.</param>
    /// <param name="configure">Optional version metadata configuration.</param>
    /// <returns>The current object type builder.</returns>
    public static ApiObjectTypeBuilder<TObject> WithVersion<TObject, TVersion>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        Expression<Func<TObject, TVersion>> expression,
        Action<ApiVersionDefinitionBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(expression);

        var clrMemberName = StaticReflection.GetMemberName(expression);
        builder.WithVersion(clrMemberName, configure);
        return builder;
    }

    /// <summary>Configures a repository-backed version using an exact CLR version type.</summary>
    /// <typeparam name="TObject">The CLR object type.</typeparam>
    /// <typeparam name="TVersion">The exact CLR version type.</typeparam>
    /// <param name="builder">The object type builder.</param>
    /// <param name="configure">Optional version metadata configuration.</param>
    /// <returns>The current object type builder.</returns>
    public static ApiObjectTypeBuilder<TObject> WithRepositoryVersion<TObject, TVersion>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        Action<ApiVersionDefinitionBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.WithRepositoryVersion(typeof(TVersion), configure);
        return builder;
    }
    #endregion
}
