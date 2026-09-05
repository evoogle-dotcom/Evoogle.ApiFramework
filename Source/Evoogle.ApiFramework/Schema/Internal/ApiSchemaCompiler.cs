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
    public static ApiSchemaCompilationResult Compile
    (
        ApiSchema apiSchema,
        IEnumerable<ApiSchemaCompilationIssue>? preliminaryIssues = null,
        Action? onFreezingStarted = null,
        Action? onFreezingCompleted = null
    )
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        apiSchema.BeginCompilation();

        var isSuccessful = false;
        try
        {
            var apiSchemaContext = new ApiSchemaContext(apiSchema);
            var session = new ApiSchemaCompilationSession(apiSchema, apiSchemaContext);

            var isTopologyValid = ApiSchemaTreeBuilder.TryBuild(apiSchema, session);
            if (isTopologyValid)
            {
                apiSchema.Compile(session);
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
                return new ApiSchemaCompilationResult(null, session.Issues);
            }

            var elements = apiSchema
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

            if (session.Issues.Any(issue => issue.Severity == ApiSchemaCompilationSeverity.Error))
            {
                return new ApiSchemaCompilationResult(null, session.Issues);
            }

            onFreezingStarted?.Invoke();

            foreach (var element in elements.Where(element => !ReferenceEquals(element, apiSchema)))
            {
                element.Freeze(frozenExtensions[element]);
            }

            apiSchema.Freeze(frozenExtensions[apiSchema]);
            onFreezingCompleted?.Invoke();
            isSuccessful = true;
            return new ApiSchemaCompilationResult(apiSchema, session.Issues);
        }
        finally
        {
            apiSchema.CompleteCompilation(isSuccessful);
        }
    }
    #endregion
}
