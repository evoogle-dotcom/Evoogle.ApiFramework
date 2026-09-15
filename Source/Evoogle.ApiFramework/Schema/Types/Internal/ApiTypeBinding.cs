// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;

namespace Evoogle.ApiFramework.Schema.Types.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
/// <remarks>
///     Establishes a one-shot binding to an API type of the required kind. A binding can either
///     resolve a configured <see cref="ApiTypeReference"/> or directly bind an API type when no
///     reference is configured. A failed reference resolution still consumes the binding attempt.
/// </remarks>
/// <typeparam name="TApiType">The required bound API type.</typeparam>
internal sealed class ApiTypeBinding<TApiType>(ApiTypeReference? apiTypeReference)
    where TApiType : ApiType
{
    #region Fields
    private bool _hasBindingAttempted;
    private TApiType? _boundApiType;
    #endregion

    #region Properties
    /// <summary>Gets the configured API type reference, or null for direct binding.</summary>
    public ApiTypeReference? ApiTypeReference { get; } = apiTypeReference;

    /// <summary>Gets the bound API type.</summary>
    public TApiType ApiType => this.RequireValue(_boundApiType);

    /// <summary>Gets the bound API type, or null when binding did not succeed.</summary>
    public TApiType? BoundApiType => _boundApiType;
    #endregion

    #region Computed Properties
    /// <summary>Gets a value indicating whether an API type reference is configured.</summary>
    public bool HasReference => this.ApiTypeReference is not null;

    /// <summary>Gets a value indicating whether an API type has been bound successfully.</summary>
    public bool IsBound => _boundApiType is not null;
    #endregion

    #region Methods
    /// <summary>Directly binds the specified API type.</summary>
    public void Bind(TApiType apiType)
    {
        ArgumentNullException.ThrowIfNull(apiType);

        if (this.HasReference)
        {
            throw new ApiSchemaConfigurationException($"A directly supplied {nameof(Types.ApiType)} cannot be bound when {nameof(this.ApiTypeReference)} is configured.");
        }

        this.BeginBinding();
        _boundApiType = apiType;
    }

    /// <summary>Attempts to resolve and bind the configured API type reference.</summary>
    public bool TryResolveReference
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
            throw new ApiSchemaConfigurationException($"An {nameof(this.ApiTypeReference)} must be configured before it can be resolved.");
        }

        this.BeginBinding();

        var apiType = this.ApiTypeReference!.Resolve(context, unresolvedCode, apiTypeName);
        if (apiType is null)
        {
            return false;
        }

        if (apiType is TApiType apiResolvedType)
        {
            _boundApiType = apiResolvedType;
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
            throw new ApiSchemaConfigurationException($"An {nameof(ApiTypeBinding<>)} can only be bound once.");
        }

        _hasBindingAttempted = true;
    }
    #endregion
}
