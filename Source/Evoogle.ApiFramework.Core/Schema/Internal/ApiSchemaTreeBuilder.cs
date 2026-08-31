// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be
///     used directly from your code. This API may change or be removed in future releases.
/// </summary>
internal static class ApiSchemaTreeBuilder
{
    #region Methods
    public static bool TryBuild(ApiSchema apiSchema, ApiInitializationSession session)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(session);

        if (!ReferenceEquals(apiSchema, session.ApiSchema))
        {
            throw new InvalidOperationException("A schema topology must be built by that schema's initialization session.");
        }

        return TryBuild((ApiSchemaElement)apiSchema, session);
    }

    internal static bool TryBuild
    (
        ApiSchemaElement rootElement,
        ApiInitializationSession session
    )
    {
        ArgumentNullException.ThrowIfNull(rootElement);
        ArgumentNullException.ThrowIfNull(session);

        ClearExistingLinks(rootElement);

        var apiChildrenByElement = new Dictionary<ApiSchemaElement, ApiSchemaElement[]>
        (
            ReferenceEqualityComparer.Instance
        );
        var apiParentByElement = new Dictionary<ApiSchemaElement, ApiSchemaElement?>
        (
            ReferenceEqualityComparer.Instance
        );
        var activeElements = new HashSet<ApiSchemaElement>(ReferenceEqualityComparer.Instance);
        var isValid = true;
        var apiSchemaPath = session.ApiSchema.BuildDefaultPath(apiPreviousPath: null);

        Visit(rootElement, parent: null);

        if (!isValid)
        {
            return false;
        }

        foreach (var (apiSchemaElement, apiChildren) in apiChildrenByElement)
        {
            var apiParent = apiParentByElement[apiSchemaElement];
            ApiSchemaElement? apiPreviousSibling = null;
            ApiSchemaElement? apiNextSibling = null;

            if (apiParent is not null)
            {
                var apiSiblings = apiChildrenByElement[apiParent];
                var index = Array.FindIndex
                (
                    apiSiblings,
                    sibling => ReferenceEquals(sibling, apiSchemaElement)
                );

                apiPreviousSibling = index > 0 ? apiSiblings[index - 1] : null;
                apiNextSibling = index < apiSiblings.Length - 1
                    ? apiSiblings[index + 1]
                    : null;
            }

            apiSchemaElement.SetTopology
            (
                rootElement,
                apiParent,
                apiChildren.FirstOrDefault(),
                apiChildren.LastOrDefault(),
                apiPreviousSibling,
                apiNextSibling
            );
        }

        return true;

        void Visit(ApiSchemaElement apiSchemaElement, ApiSchemaElement? parent)
        {
            if (activeElements.Contains(apiSchemaElement))
            {
                isValid = false;
                var apiPath = apiSchemaPath;
                var severity = ApiInitializationSeverity.Error;
                var code = ApiInitializationCode.ApiSchemaElementOwnershipCycle;
                var description = $"Schema element ownership contains a cycle involving '{apiSchemaElement.GetType().Name}'.";
                var remediation = "Remove the ownership cycle so every schema element belongs to an acyclic tree.";

                session.AddIssue(apiPath, severity, code, description, remediation);
                return;
            }

            if (apiParentByElement.TryGetValue(apiSchemaElement, out var existingParent))
            {
                isValid = false;
                var existingOwner = existingParent?.GetType().Name ?? nameof(ApiSchema);
                var duplicateOwner = parent?.GetType().Name ?? nameof(ApiSchema);
                var apiPath = apiSchemaPath;
                var severity = ApiInitializationSeverity.Error;
                var code = ApiInitializationCode.ApiSchemaElementDuplicateOwnership;
                var description = $"Schema element instance '{apiSchemaElement.GetType().Name}' is owned more than once by '{existingOwner}' and '{duplicateOwner}'.";
                var remediation = "Create a distinct schema element instance for each structural ownership position.";

                session.AddIssue(apiPath, severity, code, description, remediation);
                return;
            }

            apiParentByElement.Add(apiSchemaElement, parent);
            activeElements.Add(apiSchemaElement);

            var apiChildren = apiSchemaElement.GetOwnedElements().ToArray();
            apiChildrenByElement.Add(apiSchemaElement, apiChildren);

            foreach (var apiChild in apiChildren)
            {
                Visit(apiChild, apiSchemaElement);
            }

            activeElements.Remove(apiSchemaElement);
        }
    }
    #endregion

    #region Implementation Methods
    private static void ClearExistingLinks(ApiSchemaElement rootElement)
    {
        if (!rootElement.TryGetTopology(out _))
        {
            return;
        }

        var visited = new HashSet<ApiSchemaElement>(ReferenceEqualityComparer.Instance);
        var pending = new Stack<ApiSchemaElement>();
        pending.Push(rootElement);

        while (pending.TryPop(out var apiSchemaElement))
        {
            if (!visited.Add(apiSchemaElement) ||
                !apiSchemaElement.TryGetTopology(out var firstChild))
            {
                continue;
            }

            var apiChild = firstChild;
            while (apiChild is not null)
            {
                var apiNextSibling = apiChild.NextSibling;
                pending.Push(apiChild);
                apiChild = apiNextSibling;
            }

            apiSchemaElement.ClearTopology();
        }
    }
    #endregion
}
