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
/// Argument presence and nullability are independent. <see cref="ApiRequiresArgument"/> determines whether an
/// argument must be supplied; <see cref="ApiTypeModifiers.Required"/> prohibits null when an argument is supplied.
/// <list type="table">
///     <listheader>
///         <term>ApiRequiresArgument</term>
///         <term>ApiTypeModifiers.Required</term>
///         <description>Meaning</description>
///     </listheader>
///     <item>
///         <term><see langword="true"/></term>
///         <term>Set</term>
///         <description>Must supply a non-null value.</description>
///     </item>
///     <item>
///         <term><see langword="true"/></term>
///         <term>Unset</term>
///         <description>Must supply an argument; null is valid.</description>
///     </item>
///     <item>
///         <term><see langword="false"/></term>
///         <term>Set</term>
///         <description>May omit the argument; if supplied, it must be non-null.</description>
///     </item>
///     <item>
///         <term><see langword="false"/></term>
///         <term>Unset</term>
///         <description>May omit the argument or supply null.</description>
///     </item>
/// </list>
/// </remarks>
/// <param name="apiName">The parameter's API name.</param>
/// <param name="apiTypeExpression">The declared input type.</param>
/// <param name="apiTypeModifiers">Input nullability; Required prohibits null.</param>
/// <param name="requiresArgument">Whether an argument must be supplied, independently of nullability.</param>
[JsonConverter(typeof(ApiQuerySourceParameterJsonConverter))]
public sealed class ApiQuerySourceParameter
(
    string apiName,
    ApiTypeExpression apiTypeExpression,
    ApiTypeModifiers apiTypeModifiers,
    bool requiresArgument
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
    public ApiTypeModifiers ApiTypeModifiers { get; } = apiTypeModifiers;

    /// <summary>Gets whether the source requires an argument for this parameter.</summary>
    public bool ApiRequiresArgument { get; } = requiresArgument;

    internal ApiTypeExpression ApiTypeExpression { get; } = apiTypeExpression;
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
        this.ValidateApiTypeModifiers(context);
        this.ResolveApiTypeExpression(context);
    }
    #endregion

    #region Implementation Methods
    private void ResolveApiTypeExpression(ApiSchemaCompilationContext context)
    {
        if (this.ApiTypeExpression is null)
        {
            var apiPath = this.ApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiQuerySourceParameterTypeUnspecified;
            var description = $"{nameof(this.ApiType)} was not specified";
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
            var description = $"{nameof(this.ApiName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(apiPath, severity, code, description, remediation);
        }
    }

    private void ValidateApiTypeModifiers(ApiSchemaCompilationContext context)
    {
        var hasUnsupportedApiTypeModifiers = (this.ApiTypeModifiers & ~ApiTypeModifiers.Required) != 0;
        if (hasUnsupportedApiTypeModifiers)
        {
            var apiPath = this.ApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiQuerySourceParameterInvalidTypeModifiers;
            var description = $"{nameof(this.ApiTypeModifiers)} are invalid";
            var remediation = $"Use {ApiTypeModifiers.None} for nullable input or {ApiTypeModifiers.Required} for non-null input";

            context.AddIssue(apiPath, severity, code, description, remediation);
        }
    }
    #endregion
}
