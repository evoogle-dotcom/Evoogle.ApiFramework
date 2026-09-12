// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Version;

namespace Evoogle.ApiFramework.Schema.Configuration.Version;

/// <summary>Builds an <see cref="ApiVersionType"/> definition.</summary>
public sealed class ApiVersionTypeBuilder : ExtensionBuilder<ApiVersionTypeBuilder>
{
    #region Fields
    private readonly string? _clrMemberName;
    private readonly Type? _clrRepositoryType;
    #endregion

    #region Constructors
    internal ApiVersionTypeBuilder(string clrMemberName)
    {
        _clrMemberName = clrMemberName;
    }

    internal ApiVersionTypeBuilder(Type clrType)
    {
        _clrRepositoryType = clrType;
    }
    #endregion

    #region AddExtension Methods
    /// <summary>Adds extension metadata to the version type.</summary>
    /// <param name="extensionType">The extension's registration type.</param>
    /// <param name="extension">The extension metadata value.</param>
    /// <returns>The current builder.</returns>
    public ApiVersionTypeBuilder AddVersionTypeExtension(Type extensionType, object extension)
        => this.AddExtension(extensionType, extension);
    #endregion

    #region Build Methods
    internal ApiVersionType Build()
    {
        var apiVersionType = _clrMemberName is not null
            ? new ApiVersionType(_clrMemberName)
            : new ApiVersionType(_clrRepositoryType!);
        var extensions = this.BuildExtensions();
        if (extensions is not null)
        {
            apiVersionType.AttachExtensions(extensions);
        }

        return apiVersionType;
    }
    #endregion
}
