// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;

namespace Evoogle.ApiFramework.Schema.Types.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
/// <remarks>
///     Binds an optional API type reference to one resolved API type of the required kind.
/// </remarks>
/// <typeparam name="TApiType">The required resolved API type.</typeparam>
internal sealed class ApiTypeReferenceBinding<TApiType>(ApiTypeReference? apiTypeReference)
    where TApiType : ApiType
{
    #region Fields
    private bool _hasBindingAttempted;
    private TApiType? _apiResolvedType;
    #endregion

    #region Properties
    public ApiTypeReference? ApiTypeReference { get; } = apiTypeReference;

    public TApiType ApiType => this.RequireValue(_apiResolvedType);

    public TApiType? ApiResolvedType => _apiResolvedType;
    #endregion

    #region Computed Properties
    public bool HasReference => this.ApiTypeReference is not null;

    public bool IsResolved => _apiResolvedType is not null;
    #endregion

    #region Methods
    public void Bind(TApiType apiType)
    {
        ArgumentNullException.ThrowIfNull(apiType);

        if (this.HasReference)
        {
            throw new InvalidOperationException($"A directly supplied {nameof(Types.ApiType)} cannot be bound when {nameof(this.ApiTypeReference)} is configured.");
        }

        this.BeginBinding();
        _apiResolvedType = apiType;
    }

    public bool Resolve
    (
        ApiSchemaCompilationContext context,
        ApiSchemaCompilationCode unresolvedCode,
        string apiTypeReferenceName,
        string apiTypeName
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiTypeReferenceName);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiTypeName);

        if (!this.HasReference)
        {
            throw new InvalidOperationException($"An {nameof(this.ApiTypeReference)} must be configured before it can be resolved.");
        }

        this.BeginBinding();

        var apiType = this.ApiTypeReference!.Resolve(context, unresolvedCode, apiTypeName);
        if (apiType is null)
        {
            return false;
        }

        if (apiType is TApiType apiResolvedType)
        {
            _apiResolvedType = apiResolvedType;
            return true;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = unresolvedCode;
        var description = $"{apiTypeReferenceName} resolved to {apiType.GetType().Name}, not {typeof(TApiType).Name}";
        var remediation = $"Reference a declared {typeof(TApiType).Name}";

        context.AddIssue(severity, code, description, remediation);
        return false;
    }
    #endregion

    #region Implementation Methods
    private void BeginBinding()
    {
        if (_hasBindingAttempted)
        {
            throw new InvalidOperationException($"An {nameof(ApiTypeReferenceBinding<>)} can only be bound once.");
        }

        _hasBindingAttempted = true;
    }
    #endregion
}
