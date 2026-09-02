// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.NTree;

namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     Compiles an unpublished schema graph once and publishes it only after successful validation and freezing.
/// </summary>
internal static class ApiSchemaCompiler
{
    #region Methods
    public static ApiSchemaBuildResult Compile
    (
        ApiSchema apiSchema,
        IEnumerable<ApiInitializationIssue>? preliminaryIssues = null
    )
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        apiSchema.BeginCompilation();

        var isSuccessful = false;
        try
        {
            var apiSchemaContext = new ApiSchemaContext(apiSchema);
            var session = new ApiInitializationSession(apiSchema, apiSchemaContext);

            var isTopologyValid = ApiSchemaTreeBuilder.TryBuild(apiSchema, session);
            if (isTopologyValid)
            {
                apiSchema.Initialize(session);
            }

            if (preliminaryIssues is not null)
            {
                foreach (var issue in preliminaryIssues)
                {
                    session.AddIssue(issue);
                }
            }

            if (!isTopologyValid)
            {
                return new ApiSchemaBuildResult(null, session.Issues);
            }

            var elements = ((ApiSchemaElement)apiSchema)
                .SelfAndDescendants(TraversalStrategy.DepthFirst)
                .ToArray();

            var frozenExtensions = new Dictionary<ApiSchemaElement, IReadOnlyList<KeyValuePair<Type, object>>>
            (
                ReferenceEqualityComparer.Instance
            );

            foreach (var element in elements)
            {
                if (element.TryCreateFrozenExtensionSnapshot(session, out var snapshots))
                {
                    frozenExtensions.Add(element, snapshots);
                }
            }

            if (session.Issues.Any(issue => issue.Severity == ApiInitializationSeverity.Error))
            {
                return new ApiSchemaBuildResult(null, session.Issues);
            }

            foreach (var element in elements.Where(element => !ReferenceEquals(element, apiSchema)))
            {
                element.Freeze(frozenExtensions[element]);
            }

            apiSchema.Freeze(frozenExtensions[apiSchema]);
            isSuccessful = true;
            return new ApiSchemaBuildResult(apiSchema, session.Issues);
        }
        finally
        {
            apiSchema.CompleteCompilation(isSuccessful);
        }
    }
    #endregion
}
