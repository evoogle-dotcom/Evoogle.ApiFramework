// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Defines a named key type declared by an <see cref="ApiObjectType"/>.
/// </summary>
/// <param name="apiName">The required API name of this key type.</param>
/// <param name="apiKeyPaths">
///     The ordered collection of key paths that compose this key type.
/// </param>
[JsonConverter(typeof(ApiNamedKeyTypeJsonConverter))]
public sealed class ApiNamedKeyType(string apiName, IEnumerable<ApiKeyPath> apiKeyPaths)
    : ApiKeyType(apiKeyPaths)
{
    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiNamedKeyType);
    #endregion

    #region ApiNamedKeyType Properties
    /// <summary>Gets the required API name for this key type.</summary>
    public string ApiName { get; } = apiName;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiKeyPathsCount = this.ApiKeyPaths.Length.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiNamedKeyType)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiKeyPaths)}Count={apiKeyPathsCount}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaPathFormatting.BuildPath
        (
            apiBasePath: apiPreviousPath,
            apiPathSegment: this.ApiElementName,
            apiPathSegmentName: this.ApiName
        );

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        this.ValidateApiName(context);
    }
    #endregion

    #region Implementation Methods
    private void ValidateApiName(ApiSchemaCompilationContext context)
    {
        var isApiNameInvalid = ApiSchemaNameValidation.IsNameInvalid(this.ApiName);
        if (isApiNameInvalid)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiNamedKeyTypeInvalidApiName;
            var description = $"{nameof(this.ApiName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(severity, code, description, remediation);
        }
    }
    #endregion
}
