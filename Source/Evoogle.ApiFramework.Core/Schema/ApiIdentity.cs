// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a unique identity for an <see cref="ApiObjectType"/> consisting of one or more property parts.
/// </summary>
/// <remarks>
///     An identity can be simple (single property) or composite (multiple properties).
///     All parts must be either ordered (positional) or named, but not mixed.
/// </remarks>
/// <param name="apiName">The name of the identity.</param>
/// <param name="apiIdentityParts">The collection of property parts that make up this identity.</param>
public sealed class ApiIdentity(string apiName, IEnumerable<ApiIdentityPart> apiIdentityParts) : ApiSchemaElement
{
    #region Properties
    /// <summary>
    ///     Gets the API name of the identity.
    /// </summary>
    public string ApiName { get; } = apiName;

    /// <summary>
    ///     Gets the collection of property parts that constitute this identity.
    /// </summary>
    public ApiIdentityPart[] ApiIdentityParts { get; } = [.. apiIdentityParts.EmptyIfNull().Where(x => x is not null).OrderBy(x => x.ApiPropertyName, StringComparer.OrdinalIgnoreCase)];

    /// <summary>
    ///     Gets a value indicating whether this identity is composite (has two or more parts).
    /// </summary>
    public bool IsComposite => this.ApiIdentityParts.Length >= 2;

    /// <summary>
    ///     Gets the type detection strategy for converting property values to <see cref="Identity.ApiId"/> scalars.
    ///     Initialized from the parent <see cref="ApiSchema.DefaultApiIdTypeDetectionStrategy"/> during initialization.
    /// </summary>
    public IApiIdTypeDetectionStrategy TypeDetectionStrategy { get; private set; } = null!;

    /// <summary>
    ///     Gets the null handling behavior for this identity.
    ///     Initialized from the parent <see cref="ApiSchema.DefaultIdentityNullHandling"/> during initialization.
    /// </summary>
    public ApiIdentityNullHandling NullHandling { get; private set; }

    /// <summary>
    ///     Gets the runtime context for the API schema containing this identity.
    ///     Available after initialization.
    /// </summary>
    public new ApiSchemaContext ApiSchemaContext => base.ApiSchemaContext;
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

        // Initialize strategy and null handling from schema defaults
        this.TypeDetectionStrategy = context.ApiSchema.DefaultApiIdTypeDetectionStrategy;
        this.NullHandling = context.ApiSchema.DefaultIdentityNullHandling;

        this.InitializeApiName(context);
        this.InitializeApiIdentityParts(context);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiIdentity)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiName(ApiInitializationContext context)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiName)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_INVALID_API_NAME;
            var description = $"{nameof(this.ApiName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeApiIdentityParts(ApiInitializationContext context)
    {
        if (this.ApiIdentityParts is null || this.ApiIdentityParts.Length == 0)
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiIdentityParts)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_NULL_OR_EMPTY_PARTS;
            var description = $"{nameof(this.ApiIdentityParts)} must not be null or empty";
            var remediation = $"Specify at least one {nameof(ApiIdentityPart)}";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        // No duplicate part property names
        ApiSchemaHelpers.ValidateUnique
        (
            parts: this.ApiIdentityParts,
            partKeySelector: x => x.ApiPropertyName,
            partKeyFilter: x => !string.IsNullOrWhiteSpace(x),
            partKeyPropertyName: nameof(ApiIdentityPart.ApiPropertyName),
            path: $"{this.ApiPath}.{nameof(this.ApiIdentityParts)}",
            code: ApiInitializationCode.API_IDENTITY_DUPLICATE_PART_API_PROPERTY_NAME,
            context: context
        );

        // Do not mix ordered and named parts
        var anyOrdered = this.ApiIdentityParts.Any(p => p.EmitAsOrdered);
        var anyNamed = this.ApiIdentityParts.Any(p => !p.EmitAsOrdered);
        if (anyOrdered && anyNamed)
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiIdentityParts)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_MIXED_ORDERED_AND_NAMED_PARTS;
            var description = $"Cannot mix ordered parts with named parts in the same {nameof(ApiIdentity)}";
            var remediation = $"Use either all ordered parts or all named parts in the {nameof(ApiIdentity)}";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        var apiIdentityPartsCount = this.ApiIdentityParts.Length;
        for (var i = 0; i < apiIdentityPartsCount; ++i)
        {
            var apiIdentityPart = this.ApiIdentityParts[i];

            var childContext = context.WithParentSchemaElement(this);
            apiIdentityPart.Initialize(childContext);
        }
    }
    #endregion
}
