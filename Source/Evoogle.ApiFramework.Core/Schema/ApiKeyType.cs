// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Key;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Defines the key structure for an <see cref="ApiObjectType"/>, composed of one or more <see cref="ApiKeyPath"/>
///     instances that together navigate from CLR object properties to a scalar or composite runtime <see cref="ApiKey"/>.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiKeyType"/> unifies the concepts of primary-key and foreign-key definitions. Both are expressed
///         as an ordered collection of <see cref="ApiKeyPath"/> instances, each of which navigates from a root object
///         type to a single terminal <see cref="ApiScalarType"/> property.
///     </para>
///     <para>
///         Use <see cref="MaterializeKey"/> to materialize an <see cref="ApiKey"/> at runtime by walking each path against
///         the corresponding CLR object instances supplied via an <see cref="ApiKeyMaterializationContext"/>.
///
///         The result is a composite <see cref="ApiKey"/> whose part names are built by
///         <see cref="ApiKeyMaterializationContext.PartNameBuilder"/>.
///     </para>
/// </remarks>
/// <param name="apiName">The API name that identifies this key type within its containing <see cref="ApiObjectType"/>.</param>
/// <param name="apiKeyPaths">The ordered collection of key paths that compose this key type.</param>
[JsonConverter(typeof(ApiKeyTypeJsonConverter))]
public sealed partial class ApiKeyType(string apiName, IEnumerable<ApiKeyPath> apiKeyPaths) : ApiSchemaElement
{
    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiKeyType);
    #endregion

    #region ApiKeyType Properties
    /// <summary>Gets the API name that uniquely identifies this key type within its containing <see cref="ApiObjectType"/>.</summary>
    public string ApiName { get; } = apiName;

    /// <summary>Gets the ordered array of <see cref="ApiKeyPath"/> instances that compose this key type.</summary>
    public ApiKeyPath[] ApiKeyPaths { get; } = [.. apiKeyPaths.EmptyIfNull().Where(x => x is not null)];
    #endregion

    #region ApiKeyType Computed Properties
    /// <summary>Gets a value indicating whether this key type is defined by a single path (produces a scalar <see cref="ApiKey"/>).</summary>
    public bool IsScalar => this.ApiKeyPaths.Length == 1;

    /// <summary>Gets a value indicating whether this key type is defined by two or more paths (produces a named-composite <see cref="ApiKey"/>).</summary>
    public bool IsComposite => this.ApiKeyPaths.Length >= 2;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var pathCount = this.ApiKeyPaths.Length.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiKeyType)} {{{nameof(this.ApiName)}={apiName}, PathCount={pathCount}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: this.ApiName);

    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiName(context);
        this.InitializeApiKeyPaths(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiName(ApiInitializationContext context)
    {
        var isApiNameInvalid = ApiSchemaHelpers.IsNameInvalid(this.ApiName);
        if (isApiNameInvalid)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_KEY_TYPE_INVALID_API_NAME;
            var description = $"{nameof(this.ApiName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeApiKeyPaths(ApiInitializationContext context)
    {
        if (this.ApiKeyPaths.Length == 0)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_KEY_TYPE_NULL_OR_EMPTY_PATHS;
            var description = $"{nameof(this.ApiKeyPaths)} must not be null or empty";
            var remediation = $"Specify at least one {nameof(ApiKeyPath)}";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        var apiKeyPathsCount = this.ApiKeyPaths.Length;
        for (var i = 0; i < apiKeyPathsCount; ++i)
        {
            var apiKeyPath = this.ApiKeyPaths[i];

            var childContext = context.WithDeclaringSchemaElement(this);
            apiKeyPath.Initialize(childContext);
        }
    }
    #endregion
}
