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
///     Defines a named identity for an <see cref="ApiObjectType"/>, composed of one or more <see cref="ApiIdentityPart"/> instances
///     that together uniquely identify an object at runtime.
/// </summary>
/// <param name="apiName">The API name used to reference this identity within the schema.</param>
/// <param name="apiIdentityParts">The ordered collection of parts that compose this identity.</param>
[JsonConverter(typeof(ApiIdentityJsonConverter))]
public sealed partial class ApiIdentity(string apiName, IEnumerable<ApiIdentityPart> apiIdentityParts) : ApiSchemaElement
{
    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiIdentity);
    #endregion

    #region ApiIdentity Properties
    /// <summary>Gets the API name that uniquely identifies this identity within its containing <see cref="ApiObjectType"/>.</summary>
    public string ApiName { get; } = apiName;

    /// <summary>Gets the ordered array of <see cref="ApiIdentityPart"/> instances that compose this identity.</summary>
    public ApiIdentityPart[] ApiIdentityParts { get; } = [.. apiIdentityParts.EmptyIfNull().Where(x => x is not null)];
    #endregion

    #region ApiIdentity Computed Properties
    /// <summary>Gets a value indicating whether this identity is composite (has two or more parts).</summary>
    public bool IsComposite => this.ApiIdentityParts.Length >= 2;

    /// <summary>Gets a value indicating whether this identity is defined by a single scalar part.</summary>
    /// <remarks>Mirrors <see cref="Identity.ApiIdentityValue.IsScalarValue"/> at the schema definition level.</remarks>
    public bool IsScalarDefinition => this.ApiIdentityParts.Length == 1 && this.ApiIdentityParts[0] is ApiIdentityScalarPart;

    /// <summary>Gets a value indicating whether this identity is defined by a single object part (nested or owner).</summary>
    /// <remarks>Mirrors <see cref="Identity.ApiIdentityValue.IsObjectValue"/> at the schema definition level.</remarks>
    public bool IsObjectDefinition => this.ApiIdentityParts.Length == 1 && this.ApiIdentityParts[0] is ApiIdentityNestedPart or ApiIdentityOwnerPart;
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var partCount = this.ApiIdentityParts.Length.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiIdentity)} {{{nameof(this.ApiName)}={apiName}, PartCount={partCount}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: this.ApiName);

    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiName(context);
        this.InitializeApiIdentityParts(context);
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

    private void InitializeApiIdentityParts(ApiInitializationContext context)
    {
        if (this.ApiIdentityParts.Length == 0)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_NULL_OR_EMPTY_PARTS;
            var description = $"{nameof(this.ApiIdentityParts)} must not be null or empty";
            var remediation = $"Specify at least one {nameof(ApiIdentityPart)}";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        // No duplicate part property names
        var apiPropertyIdentityParts = this.ApiIdentityParts.Where(x => x is ApiIdentityPropertyPart).Cast<ApiIdentityPropertyPart>();
        ApiSchemaHelpers.ValidateUnique
        (
            parts: apiPropertyIdentityParts,
            partKeySelector: x => x.ClrPropertyName,
            partKeyFilter: x => !string.IsNullOrWhiteSpace(x),
            partKeyPropertyName: nameof(ApiIdentityPropertyPart.ClrPropertyName),
            path: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.API_IDENTITY_DUPLICATE_PART_API_PROPERTY_NAME,
            context: context
        );

        // At most one parent part allowed
        var ownerPartCount = this.ApiIdentityParts.Count(x => x is ApiIdentityOwnerPart);
        if (ownerPartCount > 1)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_MULTIPLE_OWNER_PARTS;
            var description = $"An {nameof(ApiIdentity)} may contain at most one {nameof(ApiIdentityOwnerPart)}, but {ownerPartCount} were found";
            var remediation = $"Remove the extra {nameof(ApiIdentityOwnerPart)} instances, leaving exactly one";

            context.AddIssue(path, severity, code, description, remediation);
        }

        var apiIdentityPartsCount = this.ApiIdentityParts.Length;
        for (var i = 0; i < apiIdentityPartsCount; ++i)
        {
            var apiIdentityPart = this.ApiIdentityParts[i];

            var childContext = context.WithDeclaringSchemaElement(this);
            apiIdentityPart.Initialize(childContext);
        }
    }
    #endregion
}
