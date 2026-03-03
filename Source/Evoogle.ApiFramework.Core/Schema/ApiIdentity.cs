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
///     Represents a unique identity for an <see cref="ApiObjectType"/> consisting of one or more <see cref="ApiIdentitySource"/> entries.
/// </summary>
/// <remarks>
///     <para>
///         An identity can be simple (single source) or composite (multiple sources).
///         Each source may be scalar (resolving directly to a value) or nested (delegating to a referenced object type's identity).
///     </para>
/// </remarks>
/// <param name="apiName">The name of the identity.</param>
/// <param name="apiIdentitySources">The collection of sources that make up this identity.</param>
[JsonConverter(typeof(ApiIdentityJsonConverter))]
public sealed class ApiIdentity
(
    string apiName,
    IEnumerable<ApiIdentitySource> apiIdentitySources
) : ApiSchemaElement
{
    #region Properties
    /// <summary>
    ///     Gets the API name of the identity.
    /// </summary>
    public string ApiName { get; } = apiName;

    /// <summary>
    ///     Gets the collection of sources that constitute this identity.
    /// </summary>
    public ApiIdentitySource[] ApiIdentitySources { get; } = [.. apiIdentitySources.EmptyIfNull().Where(x => x is not null)];

    /// <summary>
    ///     Gets a value indicating whether this identity is composite (has two or more sources).
    /// </summary>
    public bool IsComposite => this.ApiIdentitySources.Length >= 2;
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiParentPath)
        => ApiSchemaHelpers.BuildPath(apiParentPath, apiChildPath: nameof(ApiIdentity), apiApiName: this.ApiName);

    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiName(context);
        this.InitializeApiIdentitySources(context);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var sourceCount = this.ApiIdentitySources.Length.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiIdentity)} {{{nameof(this.ApiName)}={apiName}, SourceCount={sourceCount}, {nameof(this.ExtensionCount)}={extensionCount}}}";
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
            var code = ApiInitializationCode.API_IDENTITY_INVALID_API_NAME;
            var description = $"{nameof(this.ApiName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeApiIdentitySources(ApiInitializationContext context)
    {
        if (this.ApiIdentitySources is null || this.ApiIdentitySources.Length == 0)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_NULL_OR_EMPTY_SOURCES;
            var description = $"{nameof(this.ApiIdentitySources)} must not be null or empty";
            var remediation = $"Specify at least one {nameof(ApiIdentitySource)}";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        // No duplicate source property names
        ApiSchemaHelpers.ValidateUnique
        (
            parts: this.ApiIdentitySources,
            partKeySelector: x => x.ApiPropertyName,
            partKeyFilter: x => !string.IsNullOrWhiteSpace(x),
            partKeyPropertyName: nameof(ApiIdentitySource.ApiPropertyName),
            path: this.ApiPath,
            code: ApiInitializationCode.API_IDENTITY_DUPLICATE_SOURCE_API_PROPERTY_NAME,
            context: context
        );
    }
    #endregion
}
