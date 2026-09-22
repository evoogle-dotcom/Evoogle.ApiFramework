// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Version;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Version;

/// <summary>Builds an <see cref="ApiVersionDefinition"/>.</summary>
public sealed class ApiVersionDefinitionBuilder : ExtensionBuilder<ApiVersionDefinitionBuilder>
{
    #region Fields
    private readonly ApiPropertyReference? _apiPropertyReference;
    private readonly Type? _clrRepositoryType;
    #endregion

    #region Constructors
    internal ApiVersionDefinitionBuilder(string clrMemberName)
        : this(ApiPropertyReference.ClrRef(clrMemberName))
    { }

    internal ApiVersionDefinitionBuilder(ApiPropertyReference apiPropertyReference)
    {
        _apiPropertyReference = apiPropertyReference;
    }

    internal ApiVersionDefinitionBuilder(Type clrType)
    {
        _clrRepositoryType = clrType;
    }
    #endregion

    #region AddExtension Methods
    /// <summary>Adds extension metadata to the version definition.</summary>
    /// <param name="extensionType">The extension's registration type.</param>
    /// <param name="extension">The extension metadata value.</param>
    /// <returns>The current builder.</returns>
    public ApiVersionDefinitionBuilder AddVersionExtension(Type extensionType, object extension)
        => this.AddExtension(extensionType, extension);
    #endregion

    #region Build Methods
    internal ApiVersionDefinition Build()
    {
        var apiVersionDefinition = _apiPropertyReference is not null
            ? new ApiVersionDefinition(_apiPropertyReference)
            : new ApiVersionDefinition(_clrRepositoryType!);
        var extensions = this.BuildExtensions();
        if (extensions is not null)
        {
            apiVersionDefinition.AttachExtensions(extensions);
        }

        return apiVersionDefinition;
    }
    #endregion
}
