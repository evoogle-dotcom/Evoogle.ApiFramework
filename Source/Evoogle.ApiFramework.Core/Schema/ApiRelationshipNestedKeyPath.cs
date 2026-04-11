// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a key path that navigates into a nested object property on the dependent type
///     to locate FK scalar values deeper in the object graph.
/// </summary>
/// <param name="clrPropertyName">The CLR property name of the nested object on the dependent type to navigate into.</param>
/// <param name="apiKeyPaths">The child key paths that resolve FK scalar values within the nested object type.</param>
public sealed class ApiRelationshipNestedKeyPath
(
    string clrPropertyName,
    IEnumerable<ApiRelationshipKeyPath> apiKeyPaths
) : ApiRelationshipKeyPath
{
    #region ApiRelationshipNestedKeyPath Fields
    private ApiProperty? _apiResolvedProperty = null;
    private ApiObjectType? _apiResolvedObjectType = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipNestedKeyPath);
    #endregion

    #region ApiRelationshipKeyPath Properties
    /// <inheritdoc/>
    public override ApiRelationshipKeyPathKind ApiKind => ApiRelationshipKeyPathKind.Nested;
    #endregion

    #region ApiRelationshipNestedKeyPath Properties
    /// <summary>Gets the CLR property name of the nested object on the dependent type to navigate into.</summary>
    public string ClrPropertyName { get; } = clrPropertyName;

    /// <summary>Gets the child key paths that resolve FK scalar values within the nested object type.</summary>
    public ApiRelationshipKeyPath[] ApiKeyPaths { get; } = [.. apiKeyPaths.EmptyIfNull().Where(x => x is not null)];

    /// <summary>Gets the resolved navigation <see cref="ApiProperty"/> that leads into the nested object. Available after initialization.</summary>
    public ApiProperty ApiProperty => this.ThrowIfNotInitialized(_apiResolvedProperty);

    /// <summary>Gets the resolved <see cref="ApiObjectType"/> of the nested object. Available after initialization.</summary>
    public ApiObjectType ApiObjectType => this.ThrowIfNotInitialized(_apiResolvedObjectType);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrPropertyName = this.ClrPropertyName.SafeToString();
        var bindingCount = this.ApiKeyPaths.Length.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipNestedKeyPath)} {{{nameof(this.ClrPropertyName)}={clrPropertyName}, BindingCount={bindingCount}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: this.ClrPropertyName);

    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeClrPropertyName(context);
        this.InitializeApiProperty(context);
        this.InitializeApiKeyPaths(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeClrPropertyName(ApiInitializationContext context)
    {
        if (!ApiSchemaHelpers.IsNameInvalid(this.ClrPropertyName))
        {
            return;
        }

        context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
            ApiInitializationCode.API_RELATIONSHIP_KEY_PATH_INVALID_CLR_PROPERTY_NAME,
            $"{nameof(this.ClrPropertyName)} must not be null, empty, or whitespace",
            $"Specify a valid {nameof(this.ClrPropertyName)} value");
    }

    private void InitializeApiProperty(ApiInitializationContext context)
    {
        _apiResolvedProperty = null;
        _apiResolvedObjectType = null;

        if (ApiSchemaHelpers.IsNameInvalid(this.ClrPropertyName))
        {
            return;
        }

        var apiDeclaringObjectType = context.ApiDeclaringObjectType;
        if (!apiDeclaringObjectType.TryGetPropertyByClrName(this.ClrPropertyName, out var apiResolvedProperty))
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_KEY_PATH_UNRESOLVED_API_PROPERTY,
                $"Property with CLR name '{this.ClrPropertyName}' could not be found on object type '{apiDeclaringObjectType.ApiName}'",
                $"Verify the CLR property name or add a property with CLR name '{this.ClrPropertyName}' to '{apiDeclaringObjectType.ApiName}'");
            return;
        }

        if (apiResolvedProperty.ApiType is not ApiObjectType apiNestedObjectType)
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_KEY_PATH_INVALID_API_PROPERTY_TYPE,
                $"Property '{this.ClrPropertyName}' must be an object type for a nested key path",
                $"Use an object-typed property or switch to {nameof(ApiRelationshipScalarKeyPath)}");
            return;
        }

        _apiResolvedProperty = apiResolvedProperty;
        _apiResolvedObjectType = apiNestedObjectType;
    }

    private void InitializeApiKeyPaths(ApiInitializationContext context)
    {
        if (this.ApiKeyPaths.Length == 0)
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_KEY_PATH_NULL_OR_EMPTY_PATHS,
                $"{nameof(this.ApiKeyPaths)} must contain at least one key path",
                $"Add at least one {nameof(ApiRelationshipKeyPath)} inside '{this.ClrPropertyName}'");
            return;
        }

        if (_apiResolvedObjectType is null)
        {
            return;
        }

        // Child paths resolve against the nested object's type, not the declaring parent type.
        var nestedContext = context.WithDeclaringObjectType(_apiResolvedObjectType);
        foreach (var keyPath in this.ApiKeyPaths)
        {
            keyPath.Initialize(nestedContext);
        }
    }
    #endregion
}
