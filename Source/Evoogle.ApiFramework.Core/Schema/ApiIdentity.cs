// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

public sealed class ApiIdentity(string apiName, IEnumerable<ApiIdentityPart> apiIdentityParts) : ApiSchemaElement
{
    #region Properties
    public string ApiName { get; } = apiName;

    public ApiIdentityPart[] ApiIdentityParts { get; } = [.. apiIdentityParts.EmptyIfNull().Where(x => x is not null).OrderBy(x => x.ApiPropertyName, StringComparer.OrdinalIgnoreCase)];

    public bool IsComposite => this.ApiIdentityParts.Length >= 2;
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
