// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.ApiFramework.Schema.Json.Internal;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Holds schema-wide configuration options for all types within an <see cref="ApiSchema"/>.
/// </summary>
[JsonConverter(typeof(ApiSchemaOptionsJsonConverter))]
public sealed record class ApiSchemaOptions
{
    #region Fields
    private static readonly ApiSchemaOptions _default = new();

    private readonly JsonEnumFinding? _apiKeyNullHandlingFinding;
    #endregion

    #region Constructors
    /// <summary>Creates schema options with all settings at their defaults.</summary>
    public ApiSchemaOptions()
    {
    }

    internal ApiSchemaOptions(JsonEnumFinding? apiKeyNullHandlingFinding)
    {
        _apiKeyNullHandlingFinding = apiKeyNullHandlingFinding;
    }
    #endregion

    #region Properties
    /// <summary>Gets the default <see cref="ApiSchemaOptions"/> instance with all settings at their out-of-the-box defaults.</summary>
    public static ApiSchemaOptions Default => _default;

    /// <summary>
    ///     Gets the null-handling strategy applied when resolving keys across the entire schema.
    ///     Defaults to <see cref="ApiKeyNullHandling.UseEmptyOnNull"/>.
    /// </summary>
    public ApiKeyNullHandling ApiKeyNullHandling { get; init; } = ApiSchemaDefaults.KeyNullHandling;
    #endregion

    #region Methods
    internal void Validate(ApiSchemaCompilationContext context)
    {
        if (_apiKeyNullHandlingFinding is null)
        {
            return;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiSchemaInvalidApiKeyNullHandling;
        var description = $"{nameof(ApiSchema.ApiOptions)}.{_apiKeyNullHandlingFinding.DescribeProblem()}";
        var remediation = $"Specify a valid {nameof(this.ApiKeyNullHandling)} value";

        context.AddIssue(severity, code, description, remediation);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public bool Equals(ApiSchemaOptions? other)
        => other is not null && this.ApiKeyNullHandling == other.ApiKeyNullHandling;

    /// <inheritdoc/>
    public override int GetHashCode() => this.ApiKeyNullHandling.GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
    {
        var apiKeyNullHandling = this.ApiKeyNullHandling.SafeToString();

        return $"{nameof(ApiSchemaOptions)} {{{nameof(this.ApiKeyNullHandling)}={apiKeyNullHandling}}}";
    }
    #endregion
}
