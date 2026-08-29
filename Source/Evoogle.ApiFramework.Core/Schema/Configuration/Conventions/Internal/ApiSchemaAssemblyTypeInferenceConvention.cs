// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Schema.Configuration.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiSchemaAssemblyTypeInferenceConvention : IApiSchemaConvention
{
    #region Properties
    /// <inheritdoc />
    public ApiConventionPhase Phase => ApiConventionPhase.Discovery;
    #endregion

    #region Fields
    private readonly Assembly _assembly;
    private readonly Func<Type, bool>? _filter;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new <see cref="ApiSchemaAssemblyTypeInferenceConvention"/> that scans the specified
    ///     assembly and infers API type kinds from CLR type reflection.
    /// </summary>
    /// <param name="assembly">The assembly to scan for public types.</param>
    /// <param name="filter">
    ///     Optional predicate to limit which types are considered.
    ///     When <c>null</c> all eligible public non-abstract types are included.
    /// </param>
    internal ApiSchemaAssemblyTypeInferenceConvention(Assembly assembly, Func<Type, bool>? filter = null)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        _assembly = assembly;
        _filter = filter;
    }
    #endregion

    #region IApiSchemaConvention
    /// <inheritdoc />
    public void Apply(ApiSchemaBuilder builder)
    {
        var scan = ApiAssemblyTypeScanner.Scan(_assembly, _filter);
        foreach (var issue in scan.Issues)
        {
            builder.Context.AddConfigurationIssue(issue);
        }

        builder.AddTypes(scan.Types);
    }
    #endregion
}
