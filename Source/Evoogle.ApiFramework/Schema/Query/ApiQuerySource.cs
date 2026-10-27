// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Internal;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Query;

/// <summary>Declares a named, typed producer to which an operation binds.</summary>
/// <param name="apiName">The source API name.</param>
/// <param name="apiInputParameters">The input declarations owned by the source.</param>
/// <param name="apiOutputTypeExpression">The declared output type.</param>
/// <param name="apiOutputNullability">The output nullability.</param>
/// <param name="apiOutputComposition">The permitted composition over the output.</param>
[JsonConverter(typeof(ApiQuerySourceJsonConverter))]
public sealed class ApiQuerySource
(
    string apiName,
    IEnumerable<ApiQuerySourceParameter>? apiInputParameters,
    ApiTypeExpression apiOutputTypeExpression,
    ApiNullability apiOutputNullability,
    ApiQueryComposition apiOutputComposition
) : ApiSchemaElement
{
    #region ApiQuerySource Fields
    private FrozenDictionary<string, ApiQuerySourceParameter>? _apiParameterApiNameLookup;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override ApiSchemaElementKind Kind => ApiSchemaElementKind.QuerySource;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiQuerySource);
    #endregion

    #region ApiQuerySource Properties
    /// <summary>Gets the source API name.</summary>
    public string ApiName { get; } = apiName;

    /// <summary>Gets the immutable input declarations owned by this source.</summary>
    public ImmutableArray<ApiQuerySourceParameter> ApiInputParameters { get; } =
        [.. apiInputParameters.EmptyIfNull().Where(x => x is not null)];

    /// <summary>Gets the resolved output type after successful schema compilation.</summary>
    public ApiType ApiOutputType => this.ApiOutputTypeExpression.ApiType;

    /// <summary>Gets the output nullability.</summary>
    public ApiNullability ApiOutputNullability { get; } = apiOutputNullability;

    /// <summary>Gets the composition permitted over the source output.</summary>
    public ApiQueryComposition ApiOutputComposition { get; } = apiOutputComposition;

    internal ApiTypeExpression ApiOutputTypeExpression { get; } = apiOutputTypeExpression;
    #endregion

    #region ApiQuerySource Methods
    /// <summary>Attempts to find a parameter by its ordinal, case-sensitive API name.</summary>
    /// <param name="apiName">The parameter API name.</param>
    /// <param name="parameter">The matching declaration, or null.</param>
    /// <returns>Whether a parameter was found.</returns>
    public bool TryGetParameterByApiName(string apiName, [NotNullWhen(true)] out ApiQuerySourceParameter? parameter)
        => this.RequireValue(_apiParameterApiNameLookup).TryGetValue(apiName, out parameter);
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath) => ApiSchemaPathFormatting.BuildPath
        (apiPreviousPath, this.ApiElementName, this.ApiName);

    /// <inheritdoc/>
    internal override IEnumerable<ApiSchemaElement> GetOwnedElements()
    {
        if (this.ApiOutputTypeExpression?.ApiInlineType is { } inlineType)
        {
            yield return inlineType;
        }

        foreach (var parameter in this.ApiInputParameters)
        {
            yield return parameter;
        }
    }

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        this.ValidateApiName(context);
        this.BuildLookupDictionaries(context);
        this.CompileApiInputParameters(context);
        this.ValidateApiOutputNullability(context);
        this.ResolveApiOutputTypeExpression(context);
        this.ValidateApiOutputComposition(context);
    }
    #endregion

    #region Implementation Methods
    private void BuildLookupDictionaries(ApiSchemaCompilationContext context)
    {
        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: this.ApiInputParameters,
            partKeySelector: parameter => parameter.ApiName,
            partKeyFilter: ApiSchemaNameValidation.IsNameValid,
            partKeyPropertyName: nameof(ApiQuerySourceParameter.ApiName),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiQuerySourceDuplicateParameterApiName,
            session: context.Session,
            keyComparer: ApiNameComparer.Instance,
            lookupDictionary: out _apiParameterApiNameLookup
        );
    }

    private void CompileApiInputParameters(ApiSchemaCompilationContext context)
    {
        var apiInputParametersCount = this.ApiInputParameters.Length;
        for (var i = 0; i < apiInputParametersCount; ++i)
        {
            var apiParameter = this.ApiInputParameters[i];

            apiParameter.Compile(context);
        }
    }

    private void ResolveApiOutputTypeExpression(ApiSchemaCompilationContext context)
    {
        if (this.ApiOutputTypeExpression is null)
        {
            var apiPath = this.ApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiQuerySourceMissingOutputType;
            var description = $"{nameof(this.ApiOutputType)} is required";
            var remediation = $"Specify a valid {nameof(this.ApiOutputType)}";

            context.AddIssue(apiPath, severity, code, description, remediation);
            return;
        }

        this.ApiOutputTypeExpression.ResolveForQuerySource(context);
    }

    private void ValidateApiOutputComposition(ApiSchemaCompilationContext context)
    {
        var jsonEnumFinding = this.GetJsonEnumFinding(nameof(this.ApiOutputComposition));
        if (jsonEnumFinding is not null)
        {
            var findingApiPath = this.ApiPath;
            var findingSeverity = ApiSchemaCompilationSeverity.Error;
            var findingCode = ApiSchemaCompilationCode.ApiQuerySourceInvalidApiOutputComposition;
            var findingDescription = jsonEnumFinding.DescribeProblem();
            var findingRemediation = $"Specify a valid {nameof(this.ApiOutputComposition)} value";

            context.AddIssue(findingApiPath, findingSeverity, findingCode, findingDescription, findingRemediation);
            return;
        }

        const ApiQueryComposition collectionComposition = ApiQueryComposition.Filtering | ApiQueryComposition.Ordering | ApiQueryComposition.OffsetPagination;
        const ApiQueryComposition knownComposition = collectionComposition | ApiQueryComposition.Projection;

        var hasUnknownComposition = (this.ApiOutputComposition & ~knownComposition) != 0;
        var hasIncompatibleCollectionComposition = this.ApiOutputTypeExpression?.IsResolved == true &&
            this.ApiOutputType is not ApiCollectionType && (this.ApiOutputComposition & collectionComposition) != 0;
        if (hasUnknownComposition || hasIncompatibleCollectionComposition)
        {
            var apiPath = this.ApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiQuerySourceInvalidApiOutputComposition;
            var description = $"{nameof(this.ApiOutputComposition)} is invalid for the configured output type";
            var remediation = $"Use supported {nameof(this.ApiOutputComposition)} flags; filtering, ordering, and pagination require a collection";

            context.AddIssue(apiPath, severity, code, description, remediation);
        }
    }

    private void ValidateApiName(ApiSchemaCompilationContext context)
    {
        var isApiNameInvalid = ApiSchemaNameValidation.IsNameInvalid(this.ApiName);
        if (isApiNameInvalid)
        {
            var apiPath = this.ApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiQuerySourceInvalidApiName;
            var description = $"{nameof(this.ApiName)} must contain a non-whitespace value";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(apiPath, severity, code, description, remediation);
        }
    }

    private void ValidateApiOutputNullability(ApiSchemaCompilationContext context)
    {
        var jsonEnumFinding = this.GetJsonEnumFinding(nameof(this.ApiOutputNullability));
        if (jsonEnumFinding is null && Enum.IsDefined(this.ApiOutputNullability))
        {
            return;
        }

        var apiPath = this.ApiPath;
        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiQuerySourceInvalidApiOutputNullability;
        var description = jsonEnumFinding?.DescribeProblem() ?? $"{nameof(this.ApiOutputNullability)} is invalid";
        var remediation = $"Specify {ApiNullability.Nullable} or {ApiNullability.NonNullable} for {nameof(this.ApiOutputNullability)}";

        context.AddIssue(apiPath, severity, code, description, remediation);
    }
    #endregion
}
