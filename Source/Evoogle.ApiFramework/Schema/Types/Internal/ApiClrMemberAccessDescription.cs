// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Types.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal readonly record struct ApiClrMemberAccessDescription
{
    #region Constructors
    private ApiClrMemberAccessDescription(string subject, string unavailableReason, bool wrapUnavailableException)
    {
        this.Subject = subject;
        this.UnavailableReason = unavailableReason;
        this.WrapUnavailableException = wrapUnavailableException;
    }
    #endregion

    #region Properties
    public string Subject { get; }

    private string UnavailableReason { get; }

    public bool WrapUnavailableException { get; }
    #endregion

    #region Factory Methods
    public static ApiClrMemberAccessDescription ForProperty(string clrMemberName) => new
    (
        $"property '{clrMemberName}'",
        "The CLR value member may not support the operation, or the schema may not have compiled successfully.",
        wrapUnavailableException: false
    );

    public static ApiClrMemberAccessDescription ForTraversal(string apiName, string clrMemberName) => new
    (
        $"traversal '{apiName}' CLR navigation member '{clrMemberName}'",
        "The traversal may not have a CLR navigation member reference, the CLR navigation member may not support " +
            "the operation, or the schema may not have compiled successfully.",
        wrapUnavailableException: true
    );
    #endregion

    #region Methods
    public string CreateUnavailableMessage(string operation, string accessorKind) =>
        $"Cannot {operation} value for {this.Subject}: no compiled {accessorKind} available. {this.UnavailableReason}";
    #endregion
}
