// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiInitializationContext
{
    #region Properties
    public IEnumerable<ApiSchemaElement> Ancestors
    {
        get
        {
            var ancestor = this.Parent;
            while (ancestor is not null)
            {
                yield return ancestor.CurrentElement;
                ancestor = ancestor.Parent;
            }
        }
    }

    public ApiSchema ApiSchema => this.Session.ApiSchema;

    public string ApiPath { get; }

    public ApiSchemaElement CurrentElement { get; }

    public IEnumerable<ApiInitializationIssue> Issues => this.Session.Issues;

    public ApiInitializationLocation Location { get; }

    public ApiInitializationContext? Parent { get; }

    public ApiSchemaElement? ParentElement => this.Parent?.CurrentElement;

    public ApiInitializationSession Session { get; }
    #endregion

    #region Constructors
    internal ApiInitializationContext
    (
        ApiInitializationSession session,
        ApiSchemaElement currentElement,
        ApiInitializationContext? parent,
        ApiInitializationLocation location,
        string apiPath
    )
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(currentElement);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiPath);

        this.Session = session;
        this.CurrentElement = currentElement;
        this.Parent = parent;
        this.Location = location;
        this.ApiPath = apiPath;
    }
    #endregion

    #region Methods
    public void AddIssue
    (
        ApiInitializationSeverity severity,
        ApiInitializationCode code,
        string description,
        string? remediation
    )
    => this.Session.AddIssue(this.ApiPath, severity, code, description, remediation);

    public void AddIssue
    (
        string apiPath,
        ApiInitializationSeverity severity,
        ApiInitializationCode code,
        string description,
        string? remediation
    )
    => this.Session.AddIssue(apiPath, severity, code, description, remediation);

    public TElement GetNearestAncestor<TElement>()
        where TElement : ApiSchemaElement
    {
        if (this.TryGetNearestAncestor<TElement>(out var ancestor))
        {
            return ancestor;
        }

        throw new ApiSchemaException
        (
            $"No ancestor {typeof(TElement).Name} exists in the initialization context."
        );
    }

    public bool TryGetNearestAncestor<TElement>([NotNullWhen(true)] out TElement? ancestor)
        where TElement : ApiSchemaElement
    {
        var current = this.Parent;
        while (current is not null)
        {
            if (current.CurrentElement is TElement typedAncestor)
            {
                ancestor = typedAncestor;
                return true;
            }

            current = current.Parent;
        }

        ancestor = null;
        return false;
    }
    #endregion
}
