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
internal sealed class ApiPropertyBinding(ApiPropertyReference? apiPropertyReference)
{
    #region Fields
    private bool _hasBindingAttempted;
    private ApiProperty? _boundApiProperty;
    #endregion

    #region Properties
    public ApiPropertyReference? ApiPropertyReference { get; } = apiPropertyReference;

    public ApiProperty ApiProperty => this.RequireValue(_boundApiProperty);

    public ApiProperty? BoundApiProperty => _boundApiProperty;
    #endregion

    #region Computed Properties
    public bool HasReference => this.ApiPropertyReference is not null;

    public bool IsBound => _boundApiProperty is not null;
    #endregion

    #region Methods
    /// <summary>Directly binds the specified API property.</summary>
    public void Bind(ApiProperty apiProperty)
    {
        ArgumentNullException.ThrowIfNull(apiProperty);

        if (this.HasReference)
        {
            throw new ApiSchemaConfigurationException($"A directly supplied {nameof(Types.ApiProperty)} cannot be bound when {nameof(this.ApiPropertyReference)} is configured.");
        }

        this.BeginBinding();
        _boundApiProperty = apiProperty;
    }

    /// <summary>Attempts to resolve and bind the configured API property reference.</summary>
    public bool TryResolveReference
    (
        ApiObjectType apiObjectType,
        ApiSchemaCompilationContext context,
        ApiSchemaCompilationCode unresolvedCode,
        string referenceName
    )
    {
        ArgumentNullException.ThrowIfNull(apiObjectType);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceName);

        if (!this.HasReference)
        {
            throw new ApiSchemaConfigurationException($"An {nameof(this.ApiPropertyReference)} must be configured before it can be resolved.");
        }

        this.BeginBinding();

        _boundApiProperty = this.ApiPropertyReference!.Resolve
        (
            apiObjectType,
            context,
            unresolvedCode,
            referenceName
        );
        return this.IsBound;
    }
    #endregion

    #region Implementation Methods
    private void BeginBinding()
    {
        if (_hasBindingAttempted)
        {
            throw new ApiSchemaConfigurationException($"An {nameof(ApiPropertyBinding)} can only be bound once.");
        }

        _hasBindingAttempted = true;
    }
    #endregion
}
