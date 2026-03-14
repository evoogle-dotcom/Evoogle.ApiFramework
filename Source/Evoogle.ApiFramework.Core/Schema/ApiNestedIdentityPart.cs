// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents an identity part that sources its value from the <see cref="ApiIdentity"/> of a nested object property.
/// </summary>
/// <param name="apiPropertyName">The API property name of the nested object whose identity is used.</param>
/// <param name="apiIdentityName">The optional explicit name of the identity to use on the nested object type. When <see langword="null"/>, the primary identity is used.</param>
public class ApiNestedIdentityPart(string apiPropertyName, string? apiIdentityName = null) : ApiPropertyIdentityPart(apiPropertyName)
{
    #region ApiNestedIdentityPart Fields
    private ApiIdentity? _apiResolvedIdentity = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiNestedIdentityPart);
    #endregion

    #region ApiIdentityPart Properties
    /// <inheritdoc/>
    public override ApiIdentityPartKind ApiKind => ApiIdentityPartKind.Nested;
    #endregion

    #region ApiNestedIdentityPart Properties
    /// <summary>Gets the resolved <see cref="ApiIdentity"/> from the nested object type. Available after initialization.</summary>
    public ApiIdentity ApiIdentity => this.ThrowIfNotInitialized(_apiResolvedIdentity);

    /// <summary>
    ///     Gets the optional explicit identity name used to select a specific identity on the nested object type.
    ///     When <see langword="null"/>, the primary identity of the nested type is used.
    /// </summary>
    public string? ApiIdentityName { get; } = apiIdentityName;
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        var apiPropertyName = this.ApiPropertyName.SafeToString();
        var apiIdentityName = this.ApiIdentityName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiNestedIdentityPart)} {{{nameof(this.ApiPropertyName)}={apiPropertyName}, {nameof(this.ApiIdentityName)}={apiIdentityName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiIdentity(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiIdentity(ApiInitializationContext context)
    {
        if (_apiResolvedProperty is null)
        {
            _apiResolvedIdentity = null;
            return;
        }

        var apiPropertyObjectType = (ApiObjectType)_apiResolvedProperty.ApiType;

        if (this.ApiIdentityName is not null)
        {
            // Resolve by explicit name
            if (apiPropertyObjectType.TryGetIdentityByApiName(this.ApiIdentityName, out var apiIdentity))
            {
                _apiResolvedIdentity = apiIdentity;
            }
            else
            {
                var path = this.ApiPath;
                var severity = ApiInitializationSeverity.Error;
                var code = ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_NESTED_IDENTITY;
                var description = $"Referenced identity '{this.ApiIdentityName}' could not be found on object type '{apiPropertyObjectType.ApiName}'";
                var availableIdentities = string.Join(", ", apiPropertyObjectType.ApiIdentities.Select(i => $"'{i.ApiName}'"));
                var remediation = !string.IsNullOrEmpty(availableIdentities)
                    ? $"Use one of the available identities: {availableIdentities}"
                    : $"Define an identity on '{apiPropertyObjectType.ApiName}' or remove {nameof(this.ApiIdentityName)}";

                context.AddIssue(path, severity, code, description, remediation);

                _apiResolvedIdentity = null;
            }
        }
        else
        {
            // Use primary identity (first by convention)
            _apiResolvedIdentity = apiPropertyObjectType.ApiPrimaryIdentity;

            if (_apiResolvedIdentity is null)
            {
                var path = this.ApiPath;
                var severity = ApiInitializationSeverity.Error;
                var code = ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_NESTED_IDENTITY;
                var description = $"Property '{this.ApiPropertyName}' references object type '{apiPropertyObjectType.ApiName}' which has no primary identity";
                var remediation = $"Define a primary identity on '{apiPropertyObjectType.ApiName}' or specify {nameof(this.ApiIdentityName)} explicitly";

                context.AddIssue(path, severity, code, description, remediation);
            }
        }
    }
    #endregion
}
