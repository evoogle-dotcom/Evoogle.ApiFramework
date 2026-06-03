// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Abstract intermediate base class for relationships that connect exactly two participating
///     <see cref="ApiObjectType"/> instances with a single principal end and a single dependent end.
/// </summary>
/// <remarks>
///     Concrete subclasses are <see cref="ApiRelationshipOneToOne"/> and <see cref="ApiRelationshipOneToMany"/>.
///     The foreign key role always resides on the dependent side; the principal side provides the referenced key type.
///     Self-referential relationships are supported by setting both ends to the same <see cref="ApiRelationshipElement.ClrObjectType"/>.
/// </remarks>
/// <param name="apiName">The API name that uniquely identifies this relationship within the schema.</param>
/// <param name="apiPrincipalEnd">The principal end of the relationship, which provides the referenced key type.</param>
/// <param name="apiDependentEnd">The dependent end of the relationship, which may provide the foreign key role's key paths.</param>
/// <param name="apiDeleteBehavior">The delete behavior that governs what happens to related objects when either end is affected.</param>
public abstract class ApiRelationshipOneTo
(
    string apiName,
    ApiRelationshipPrincipalEnd apiPrincipalEnd,
    ApiRelationshipDependentEnd apiDependentEnd,
    ApiRelationshipDeleteBehavior apiDeleteBehavior
) : ApiRelationship(apiName, apiDeleteBehavior)
{
    #region ApiRelationshipOneTo Properties
    /// <summary>Gets the principal end of the relationship, which provides the referenced key type.</summary>
    public ApiRelationshipPrincipalEnd ApiPrincipalEnd { get; } = apiPrincipalEnd;

    /// <summary>Gets the dependent end of the relationship, which may provide the foreign key role's key paths.</summary>
    public ApiRelationshipDependentEnd ApiDependentEnd { get; } = apiDependentEnd;
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiPrincipalEnd(context);
        this.InitializeApiDependentEnd(context);
        this.InitializeDependentKeyPathAlignment(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiPrincipalEnd(ApiInitializationContext context)
    {
        if (this.ApiPrincipalEnd is null)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_NULL_PRINCIPAL_END;
            var description = $"{nameof(this.ApiPrincipalEnd)} must not be null";
            var remediation = $"Provide a valid {nameof(ApiRelationshipPrincipalEnd)}";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        var endContext = context.WithDeclaringSchemaElement(this);
        this.ApiPrincipalEnd.Initialize(endContext);
    }

    private void InitializeApiDependentEnd(ApiInitializationContext context)
    {
        if (this.ApiDependentEnd is null)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_NULL_DEPENDENT_END;
            var description = $"{nameof(this.ApiDependentEnd)} must not be null";
            var remediation = $"Provide a valid {nameof(ApiRelationshipDependentEnd)}";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        var endContext = context.WithDeclaringSchemaElement(this);
        this.ApiDependentEnd.Initialize(endContext);
    }

    private void InitializeDependentKeyPathAlignment(ApiInitializationContext context)
    {
        var principal = this.ApiPrincipalEnd;
        var dependent = this.ApiDependentEnd;

        if (principal is null || dependent is null)
        {
            return;
        }

        var principalKeyType = principal.ResolvedKeyType;
        if (principalKeyType is null)
        {
            // Principal key type failed to resolve; the principal end already recorded the error.
            return;
        }

        if (dependent.IsNavigational)
        {
            // Purely navigational; no key path alignment to validate.
            return;
        }
        var foreignKeyType = dependent.ApiForeignKeyType;

        var keyPathCount = foreignKeyType.ApiKeyPaths.Length;
        var keyTypePathCount = ApiSchemaHelpers.CountKeyLeaves(principalKeyType);

        if (keyTypePathCount is not null && keyPathCount != keyTypePathCount)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_ONE_TO_INVALID_DEPENDENT_KEY_PATHS_COUNT;
            var description = $"{nameof(this.ApiDependentEnd)}.{nameof(this.ApiDependentEnd.ApiForeignKeyType)}.{nameof(ApiKeyType.ApiKeyPaths)} has {keyPathCount} key path(s) but principal key type '{principalKeyType.ApiName}' has {keyTypePathCount} key path(s)";
            var remediation = $"Ensure {nameof(this.ApiDependentEnd)}.{nameof(this.ApiDependentEnd.ApiForeignKeyType)}.{nameof(ApiKeyType.ApiKeyPaths)} contains exactly {keyTypePathCount} key path(s) to match the principal end's key type";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion
}
