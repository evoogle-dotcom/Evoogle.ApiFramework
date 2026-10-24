// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.ApiFramework.Schema.Json.Internal;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>
///     Holds optional per-type configuration for an <see cref="ApiObjectType"/>, allowing options to be overridden on a type-by-type basis.
/// </summary>
[JsonConverter(typeof(ApiObjectTypeOptionsJsonConverter))]
public sealed record class ApiObjectTypeOptions
{
    #region Fields
    private readonly JsonEnumFinding? _apiKeyNullHandlingFinding;
    #endregion

    #region Constructors
    /// <summary>Creates object type options with no overrides.</summary>
    public ApiObjectTypeOptions()
    {
    }

    internal ApiObjectTypeOptions(JsonEnumFinding? apiKeyNullHandlingFinding)
    {
        _apiKeyNullHandlingFinding = apiKeyNullHandlingFinding;
    }
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the null-handling strategy applied when resolving keys on this object type.
    ///     When <see langword="null"/>, the strategy is inherited from the containing <see cref="ApiSchema"/>'s options.
    /// </summary>
    public ApiKeyNullHandling? ApiKeyNullHandling { get; init; }
    #endregion

    #region Methods
    internal void Validate(ApiSchemaCompilationContext context)
    {
        if (_apiKeyNullHandlingFinding is null)
        {
            return;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiObjectTypeInvalidApiKeyNullHandling;
        var description = $"{nameof(ApiObjectType.ApiOptions)}.{_apiKeyNullHandlingFinding.DescribeProblem()}";
        var remediation = $"Specify a valid {nameof(this.ApiKeyNullHandling)} value";

        context.AddIssue(severity, code, description, remediation);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public bool Equals(ApiObjectTypeOptions? other)
        => other is not null && this.ApiKeyNullHandling == other.ApiKeyNullHandling;

    /// <inheritdoc/>
    public override int GetHashCode() => this.ApiKeyNullHandling.GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
    {
        var apiKeyNullHandling = this.ApiKeyNullHandling.SafeToString();

        return $"{nameof(ApiObjectTypeOptions)} {{{nameof(this.ApiKeyNullHandling)}={apiKeyNullHandling}}}";
    }
    #endregion
}
