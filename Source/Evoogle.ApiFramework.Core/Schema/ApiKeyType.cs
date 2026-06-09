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
/// <param name="apiKeyPaths">The ordered collection of key paths that compose this key type.</param>
[JsonConverter(typeof(ApiKeyTypeJsonConverter))]
public sealed partial class ApiKeyType(IEnumerable<ApiKeyPath> apiKeyPaths) : ApiSchemaElement
{
    #region Fields
    private string? _contextualName = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiKeyType);
    #endregion

    #region ApiKeyType Properties
    /// <summary>Gets the ordered array of <see cref="ApiKeyPath"/> instances that compose this key type.</summary>
    public ApiKeyPath[] ApiKeyPaths { get; } = [.. apiKeyPaths.EmptyIfNull().Where(x => x is not null)];

    /// <summary>Gets the contextual name assigned by the containing <see cref="ApiObjectType"/>, or null for anonymous (FK) key types.</summary>
    internal string? ContextualName => _contextualName;
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
        var name = (_contextualName ?? "(anonymous)").SafeToString();
        var pathCount = this.ApiKeyPaths.Length.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiKeyType)} {{Name={name}, PathCount={pathCount}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: _contextualName ?? "(anonymous)");

    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiKeyPaths(context);
    }
    #endregion

    #region Internal Methods
    internal void SetContextualName(string name)
    {
        _contextualName = name;
    }
    #endregion

    #region Implementation Methods
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
