// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Query;

/// <summary>Declares a schema-owned input to a query source, independently of operation parameters.</summary>
/// <remarks>
/// Argument presence and nullability are independent. <see cref="ApiPresence"/> determines whether an
/// argument must be supplied; <see cref="ApiNullability.NonNullable"/> prohibits null when an argument is supplied.
/// <list type="table">
///     <listheader>
///         <term>ApiPresence</term>
///         <term>ApiNullability</term>
///         <description>Meaning</description>
///     </listheader>
///     <item>
///         <term>Required</term>
///         <term>NonNullable</term>
///         <description>Must supply a non-null value.</description>
///     </item>
///     <item>
///         <term>Required</term>
///         <term>Nullable</term>
///         <description>Must supply an argument; null is valid.</description>
///     </item>
///     <item>
///         <term>Optional</term>
///         <term>NonNullable</term>
///         <description>May omit the argument; if supplied, it must be non-null.</description>
///     </item>
///     <item>
///         <term>Optional</term>
///         <term>Nullable</term>
///         <description>May omit the argument or supply null.</description>
///     </item>
/// </list>
/// </remarks>
/// <param name="apiName">The parameter's API name.</param>
/// <param name="apiTypeExpression">The declared input type.</param>
/// <param name="apiNullability">Input nullability; NonNullable prohibits null.</param>
/// <param name="apiPresence">Whether an argument must be supplied, independently of nullability.</param>
[JsonConverter(typeof(ApiQuerySourceParameterJsonConverter))]
public sealed class ApiQuerySourceParameter
(
    string apiName,
    ApiTypeExpression apiTypeExpression,
    ApiNullability apiNullability,
    ApiPresence apiPresence
) : ApiSchemaElement
{
    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override ApiSchemaElementKind Kind => ApiSchemaElementKind.QuerySourceParameter;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiQuerySourceParameter);
    #endregion

    #region ApiQuerySourceParameter Properties
    /// <summary>Gets the parameter's API name.</summary>
    public string ApiName { get; } = apiName;

    /// <summary>Gets the resolved input type after successful schema compilation.</summary>
    public ApiType ApiType => this.ApiTypeExpression.ApiType;

    /// <summary>Gets input nullability independently of argument presence.</summary>
    public ApiNullability ApiNullability { get; } = apiNullability;

    /// <summary>Gets whether the source requires an argument for this parameter.</summary>
    public ApiPresence ApiPresence { get; } = apiPresence;

    internal ApiTypeExpression ApiTypeExpression { get; } = apiTypeExpression;
    #endregion

    #region Computed Properties
    /// <summary>Gets whether an argument must be supplied.</summary>
    public bool IsRequired => this.ApiPresence == ApiPresence.Required;

    /// <summary>Gets whether an argument may be omitted.</summary>
    public bool IsOptional => this.ApiPresence == ApiPresence.Optional;
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath) => ApiSchemaPathFormatting.BuildPath
        (apiPreviousPath, this.ApiElementName, this.ApiName);

    /// <inheritdoc/>
    internal override IEnumerable<ApiSchemaElement> GetOwnedElements()
    {
        if (this.ApiTypeExpression?.ApiInlineType is { } inlineType)
        {
            yield return inlineType;
        }
    }

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        base.CompileCore(context);

        this.ValidateApiName(context);
        this.ValidateApiNullability(context);
        this.ValidateApiPresence(context);
        this.ResolveApiTypeExpression(context);
    }
    #endregion

    #region Implementation Methods
    private void ValidateApiPresence(ApiSchemaCompilationContext context)
    {
        var jsonEnumFinding = this.GetJsonEnumFinding(nameof(this.ApiPresence));
        if (jsonEnumFinding is null && Enum.IsDefined(this.ApiPresence))
        {
            return;
        }

        var apiPath = this.ApiPath;
        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiQuerySourceParameterInvalidApiPresence;
        var description = jsonEnumFinding?.DescribeProblem() ?? $"{nameof(this.ApiPresence)} is invalid";
        var remediation = $"Specify {ApiPresence.Optional} or {ApiPresence.Required} for {nameof(this.ApiPresence)}";

        context.AddIssue(apiPath, severity, code, description, remediation);
    }

    private void ResolveApiTypeExpression(ApiSchemaCompilationContext context)
    {
        if (this.ApiTypeExpression is null)
        {
            var apiPath = this.ApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiQuerySourceParameterMissingType;
            var description = $"{nameof(this.ApiType)} is required";
            var remediation = $"Specify a valid {nameof(this.ApiType)}";

            context.AddIssue(apiPath, severity, code, description, remediation);
            return;
        }

        this.ApiTypeExpression.ResolveForQuerySourceParameter(context);
    }

    private void ValidateApiName(ApiSchemaCompilationContext context)
    {
        var isApiNameInvalid = ApiSchemaNameValidation.IsNameInvalid(this.ApiName);
        if (isApiNameInvalid)
        {
            var apiPath = this.ApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiQuerySourceParameterInvalidApiName;
            var description = $"{nameof(this.ApiName)} must contain a non-whitespace value";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(apiPath, severity, code, description, remediation);
        }
    }

    private void ValidateApiNullability(ApiSchemaCompilationContext context)
    {
        var jsonEnumFinding = this.GetJsonEnumFinding(nameof(this.ApiNullability));
        if (jsonEnumFinding is null && Enum.IsDefined(this.ApiNullability))
        {
            return;
        }

        var apiPath = this.ApiPath;
        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiQuerySourceParameterInvalidApiNullability;
        var description = jsonEnumFinding?.DescribeProblem() ?? $"{nameof(this.ApiNullability)} is invalid";
        var remediation = $"Specify {ApiNullability.Nullable} or {ApiNullability.NonNullable} for {nameof(this.ApiNullability)}";

        context.AddIssue(apiPath, severity, code, description, remediation);
    }
    #endregion
}
