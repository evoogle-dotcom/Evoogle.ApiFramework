// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Abstract intermediate base class for relationships that connect exactly two participating
///     <see cref="ApiObjectType"/> instances with a single principal end and a single dependent end.
/// </summary>
/// <remarks>
///     Concrete subclasses are <see cref="ApiRelationshipOneToOne"/> and <see cref="ApiRelationshipOneToMany"/>.
///     The foreign key role always resides on the dependent side; the principal side provides the referenced principal key type.
///     Self-referential relationships are supported by setting both ends to the same <see cref="ApiRelationshipElement.ClrObjectType"/>.
/// </remarks>
/// <param name="apiName">The API name that uniquely identifies this relationship within the schema.</param>
/// <param name="apiPrincipalEnd">The principal end of the relationship, which provides the referenced principal key type.</param>
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
    #region ApiRelationshipOneTo Fields
    private ApiRelationshipKeyBinding? _apiResolvedKeyBinding = null;
    #endregion

    #region ApiRelationshipOneTo Properties
    /// <summary>Gets the principal end of the relationship, which provides the referenced principal key type.</summary>
    public ApiRelationshipPrincipalEnd ApiPrincipalEnd { get; } = apiPrincipalEnd;

    /// <summary>Gets the dependent end of the relationship, which may provide the foreign key role's key paths.</summary>
    public ApiRelationshipDependentEnd ApiDependentEnd { get; } = apiDependentEnd;

    /// <summary>Gets the resolved key binding between the principal key and dependent foreign key.</summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="IsNavigational"/> is <see langword="true"/> or initialization failed.
    ///     Check <see cref="HasKeyBinding"/> before accessing this property.
    /// </exception>
    public ApiRelationshipKeyBinding ApiKeyBinding => this.HasKeyBinding
        ? _apiResolvedKeyBinding!
        : throw new ApiSchemaException("No key binding declared or resolved for this relationship.");

    /// <summary>Gets a value indicating whether this relationship has a resolved key binding.</summary>
    public bool HasKeyBinding => _apiResolvedKeyBinding is not null;

    /// <summary>Gets a value indicating whether this relationship has no dependent foreign key binding declared at the schema level.</summary>
    public bool IsNavigational => this.ApiDependentEnd is null || !this.ApiDependentEnd.HasForeignKey;
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        _apiResolvedKeyBinding = null;

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

        if (!dependent.HasForeignKey)
        {
            this.ValidateNavigationalPrincipalKey(context, principal, nameof(ApiRelationshipPrincipalEnd.ApiPrincipalKeyTypeName));

            // Purely navigational; no key path alignment to validate.
            return;
        }

        var principalKeyDesc = principal.ApiPrincipalKeyTypeName is not null ? $"principal key type '{principal.ApiPrincipalKeyTypeName}'" : "principal key type";
        var foreignKeyPath = $"{nameof(this.ApiDependentEnd)}.{nameof(this.ApiDependentEnd.ApiForeignKeyType)}";
        var compatibilityRemediation = $"Ensure {foreignKeyPath} paths are ordered to match the principal end's principal key type and use compatible scalar types";

        _apiResolvedKeyBinding = ApiRelationshipKeyAlignment.ResolvePrincipalForeignKeyBinding
        (
            context: context,
            relationshipPath: this.ApiPath,
            principalEnd: principal,
            foreignKeyType: dependent.ApiForeignKeyType,
            countMismatchCode: ApiInitializationCode.API_RELATIONSHIP_ONE_TO_INVALID_DEPENDENT_KEY_PATHS_COUNT,
            foreignKeyPath: foreignKeyPath,
            principalCountLabel: principalKeyDesc,
            principalCompatibilityLabel: $"principal end {principalKeyDesc}",
            principalEndQualifier: null,
            explicitKeyTarget: nameof(ApiRelationshipPrincipalEnd.ApiPrincipalKeyTypeName),
            inferredForeignKeyLabel: "foreign key",
            countMismatchRemediationTarget: "the principal end's principal key type",
            compatibilityRemediation: compatibilityRemediation
        );
    }

    private void ValidateNavigationalPrincipalKey(ApiInitializationContext context, ApiRelationshipPrincipalEnd principal, string explicitKeyTarget)
    {
        if (principal.ApiPrincipalKeyTypeName is null)
        {
            return;
        }

        var path = this.ApiPath;
        var severity = ApiInitializationSeverity.Error;
        var code = ApiInitializationCode.API_RELATIONSHIP_END_PRINCIPAL_KEY_WITHOUT_FOREIGN_KEY;
        var description = $"Cannot resolve {explicitKeyTarget} '{principal.ApiPrincipalKeyTypeName}' because this relationship has no foreign key binding";
        var remediation = $"Declare {nameof(this.ApiDependentEnd)}.{nameof(ApiRelationshipDependentEnd.ApiForeignKeyType)} or remove {explicitKeyTarget}";

        context.AddIssue(path, severity, code, description, remediation);
    }
    #endregion
}
