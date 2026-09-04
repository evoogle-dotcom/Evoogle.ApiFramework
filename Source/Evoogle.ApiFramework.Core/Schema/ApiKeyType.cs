// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Immutable;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Key;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Defines a key structure composed of one or more <see cref="ApiKeyPath"/> instances that
///     together navigate from CLR object properties to a scalar or composite runtime
///     <see cref="ApiKey"/>.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiKeyType"/> represents an anonymous structural key shape, such as a
///         relationship foreign key.
///         Named key definitions declared by an <see cref="ApiObjectType"/> are represented by
///         <see cref="ApiNamedKeyType"/>.
///     </para>
///     <para>
///         Use <see cref="MaterializeKey"/> to materialize an <see cref="ApiKey"/> at runtime by walking each path against
///         the corresponding CLR object instances supplied via an <see cref="ApiKeyMaterializationContext"/>.
///
///         The result is a composite <see cref="ApiKey"/> whose part names are formatted according to
///         <see cref="ApiKeyMaterializationContext.PartNameFormat"/>.
///     </para>
///     <para>
///         <see cref="ApiSchemaElement.Kind"/> is sealed for this extensible hierarchy. The built-in
///         <see cref="ApiNamedKeyType"/> reports <see cref="ApiSchemaElementKind.NamedKeyType"/>;
///         every other subclass reports <see cref="ApiSchemaElementKind.KeyType"/>.
///     </para>
/// </remarks>
[JsonConverter(typeof(ApiKeyTypeJsonConverter))]
public partial class ApiKeyType : ApiSchemaElement
{
    #region Constructors
    internal ApiKeyType(IEnumerable<ApiKeyPath> apiKeyPaths)
    {
        this.ApiKeyPaths = [.. apiKeyPaths.EmptyIfNull().Where(x => x is not null)];
    }
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override sealed ApiSchemaElementKind Kind => this is ApiNamedKeyType
        ? ApiSchemaElementKind.NamedKeyType
        : ApiSchemaElementKind.KeyType;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiKeyType);
    #endregion

    #region ApiKeyType Properties
    /// <summary>Gets the immutable ordered paths that compose this key type.</summary>
    public ImmutableArray<ApiKeyPath> ApiKeyPaths { get; }
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
        var apiKeyPathsCount = this.ApiKeyPaths.Length.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiKeyType)} {{{nameof(this.ApiKeyPaths)}Count={apiKeyPathsCount}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override IEnumerable<ApiSchemaElement> GetOwnedElements()
    {
        foreach (var apiKeyPath in this.ApiKeyPaths)
        {
            yield return apiKeyPath;
        }
    }

    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaPathFormatting.BuildPath
        (
            apiBasePath: apiPreviousPath,
            apiPathSegment: this.ApiElementName,
            apiPathSegmentName: null
        );

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        this.CompileApiKeyPaths(context);
    }
    #endregion

    #region Implementation Methods
    private void CompileApiKeyPaths(ApiSchemaCompilationContext context)
    {
        if (this.ApiKeyPaths.Length == 0)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiKeyTypeNullOrEmptyPaths;
            var description = $"{nameof(this.ApiKeyPaths)} must not be null or empty";
            var remediation = $"Specify at least one {nameof(ApiKeyPath)}";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        var apiKeyPathsCount = this.ApiKeyPaths.Length;
        for (var i = 0; i < apiKeyPathsCount; ++i)
        {
            var apiKeyPath = this.ApiKeyPaths[i];

            var location = ApiSchemaCompilationLocation.ForIndexedLabel
            (
                i,
                apiKeyPath.ApiPathLabel
            );
            apiKeyPath.Compile(context, location);
        }
    }
    #endregion
}
