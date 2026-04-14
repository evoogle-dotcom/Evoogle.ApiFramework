// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Abstract intermediate base class for relationships that connect exactly two participating
///     <see cref="ApiObjectType"/> instances with a single principal end and a single dependent end.
/// </summary>
/// <remarks>
///     Concrete subclasses are <see cref="ApiRelationshipOneToOne"/> and <see cref="ApiRelationshipOneToMany"/>.
///     The FK always resides on the dependent side; the principal side owns the join-key identity.
///     Self-referential relationships are supported by setting both ends to the same
///     <see cref="ApiRelationshipEnd.ClrObjectType"/>.
/// </remarks>
public abstract class ApiRelationshipOneTo
(
    string apiName,
    ApiRelationshipPrincipalEnd apiPrincipalEnd,
    ApiRelationshipDependentEnd apiDependentEnd
) : ApiRelationship(apiName)
{
    #region ApiRelationshipOneTo Properties
    /// <summary>Gets the principal end of the relationship, which owns the join key identity.</summary>
    public ApiRelationshipPrincipalEnd ApiPrincipalEnd { get; } = apiPrincipalEnd;

    /// <summary>Gets the dependent end of the relationship, which holds the FK key paths.</summary>
    public ApiRelationshipDependentEnd ApiDependentEnd { get; } = apiDependentEnd;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiKind = this.ApiKind.SafeToString();
        var principalType = this.ApiPrincipalEnd?.ClrObjectType?.Name.SafeToString();
        var dependentType = this.ApiDependentEnd?.ClrObjectType?.Name.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{this.GetType().Name} {{{nameof(this.ApiKind)}={apiKind}, {nameof(this.ApiName)}={apiName}, Principal={principalType}, Dependent={dependentType}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiPrincipalEnd(context);
        this.InitializeApiDependentEnd(context);
        this.WireBackReferences();
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiPrincipalEnd(ApiInitializationContext context)
    {
        if (this.ApiPrincipalEnd is null)
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_NULL_PRINCIPAL_END,
                $"{nameof(this.ApiPrincipalEnd)} must not be null",
                $"Provide a valid {nameof(ApiRelationshipPrincipalEnd)}");
            return;
        }

        var endContext = context.WithDeclaringSchemaElement(this);
        this.ApiPrincipalEnd.Initialize(endContext);
    }

    private void InitializeApiDependentEnd(ApiInitializationContext context)
    {
        if (this.ApiDependentEnd is null)
        {
            context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
                ApiInitializationCode.API_RELATIONSHIP_NULL_DEPENDENT_END,
                $"{nameof(this.ApiDependentEnd)} must not be null",
                $"Provide a valid {nameof(ApiRelationshipDependentEnd)}");
            return;
        }

        var endContext = context.WithDeclaringSchemaElement(this);
        this.ApiDependentEnd.Initialize(endContext);
    }

    private void WireBackReferences()
    {
        if (this.ApiPrincipalEnd is null || this.ApiDependentEnd is null)
        {
            return;
        }

        this.ApiPrincipalEnd.WireBackReferences(this, this.ApiDependentEnd);
        this.ApiDependentEnd.WireBackReferences(this, this.ApiPrincipalEnd);
    }
    #endregion
}
