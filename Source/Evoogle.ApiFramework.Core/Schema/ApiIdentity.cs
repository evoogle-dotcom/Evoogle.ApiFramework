// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extension;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

public sealed class ApiIdentity(string apiName, IEnumerable<ApiIdentityPart> apiIdentityParts) : ExtensibleBase
{
    #region Properties
    public string ApiName { get; } = apiName;

    public ApiIdentityPart[] ApiIdentityParts { get; } = apiIdentityParts.SafeToArray();

    public bool IsComposite => this.ApiIdentityParts.Length > 1;
    #endregion

    #region ApiIdentity Methods
    internal void Initialize(ApiSchema apiSchema, ApiObjectType apiObjectType, string apiValidationPath, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(apiObjectType);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        this.InitializeApiName(apiValidationPath, ref results);
        this.InitializeApiIdentityParts(apiSchema, apiObjectType, apiValidationPath, ref results);
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
    private void InitializeApiName
    (
        string apiValidationPath,
        ref List<ValidationResult>? results
    )
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiName)} cannot be null or whitespace.", [nameof(this.ApiName)]));
            return;
        }
    }

    private void InitializeApiIdentityParts
    (
        ApiSchema apiSchema,
        ApiObjectType apiObjectType,
        string apiValidationPath,
        ref List<ValidationResult>? results
    )
    {
        // Parts collection cannot be null or empty
        if (this.ApiIdentityParts is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiIdentityParts)} cannot be null.", [nameof(this.ApiIdentityParts)]));
            return;
        }

        // Must have at least one part
        var apiIdentityPartsCount = this.ApiIdentityParts.Length;
        if (apiIdentityPartsCount == 0)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiIdentityParts)} must contain at least one part.", [nameof(this.ApiIdentityParts)]));
            return;
        }

        // No duplicate part property names
        ApiSchemaHelpers.ValidateUnique(this.ApiIdentityParts, x => x.ApiPropertyName, apiValidationPath, nameof(ApiIdentityPart.ApiPropertyName), ref results);

        // Do not mix ordered and named parts
        var anyOrdered = this.ApiIdentityParts.Any(p => p.EmitAsOrdered);
        var anyNamed = this.ApiIdentityParts.Any(p => !p.EmitAsOrdered);
        if (anyOrdered && anyNamed)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath} cannot mix ordered and named parts.", [nameof(this.ApiIdentityParts)]));
        }

        // Initialize each part
        for (var i = 0; i < apiIdentityPartsCount; ++i)
        {
            apiValidationPath = $"{apiValidationPath}.{nameof(this.ApiIdentityParts)}[{i}]";

            var apiIdentityPart = this.ApiIdentityParts[i];
            if (apiIdentityPart is null)
            {
                results ??= [];
                results.Add(new ValidationResult($"{apiValidationPath} cannot be null.", [nameof(this.ApiIdentityParts)]));
                continue;
            }

            apiIdentityPart.Initialize(apiSchema, apiObjectType, apiValidationPath, ref results);
        }
    }
    #endregion
}
