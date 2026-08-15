// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Schema.Annotations;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiSchemaAssemblyAnnotationScanConvention : IApiSchemaConvention
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
    ///     Initializes a new <see cref="ApiSchemaAssemblyAnnotationScanConvention"/> that scans the specified
    ///     assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for annotated types.</param>
    /// <param name="filter">
    ///     Optional predicate to limit which types are considered.
    ///     When <c>null</c> all annotated public non-abstract types are included.
    /// </param>
    internal ApiSchemaAssemblyAnnotationScanConvention(Assembly assembly, Func<Type, bool>? filter = null)
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
        var clrTypes = _assembly.GetExportedTypes();
        foreach (var clrType in clrTypes)
        {
            if (clrType == null)
            {
                continue;
            }

            if (clrType.IsAbstract || !clrType.IsClass && !clrType.IsValueType)
            {
                continue;
            }

            if (_filter != null && !_filter(clrType))
            {
                continue;
            }

            if (clrType.IsDefined(typeof(ApiObjectTypeAttribute), inherit: false))
            {
                builder.AddObject(clrType);
            }
            else if (clrType.IsDefined(typeof(ApiScalarTypeAttribute), inherit: false))
            {
                builder.AddScalar(clrType);
            }
            else if (clrType.IsEnum && clrType.IsDefined(typeof(ApiEnumTypeAttribute), inherit: false))
            {
                builder.AddEnum(clrType);
            }
        }
    }
    #endregion
}
