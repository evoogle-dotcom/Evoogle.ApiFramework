// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Keys.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal static class ApiKeyPathClrPathParser
{
    #region Utility Methods
    public static ParseResult Parse(string clrPath)
    {
        ArgumentNullException.ThrowIfNull(clrPath);

        var clrMemberNames = clrPath.Trim().Split('.').Select(static name => name.Trim()).ToArray();
        var validationMessage = clrMemberNames.Length == 0 || clrMemberNames.Any(string.IsNullOrWhiteSpace)
            ? "CLR paths must contain one or more non-empty dot-delimited member names."
            : null;

        return new(clrMemberNames, validationMessage);
    }
    #endregion

    #region Nested Types
    /// <summary>
    ///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    internal sealed class ParseResult
    {
        #region Constructors
        public ParseResult(string[] clrMemberNames, string? validationMessage)
        {
            this.ClrMemberNames = clrMemberNames;
            this.ValidationMessage = validationMessage;
        }
        #endregion

        #region Properties
        public IReadOnlyList<string> ClrMemberNames { get; }

        public bool IsValid => this.ValidationMessage is null;

        public string? ValidationMessage { get; }
        #endregion

        #region Validation Methods
        public void ThrowIfInvalid(string parameterName)
        {
            if (!this.IsValid)
            {
                throw new ArgumentException(this.ValidationMessage, parameterName);
            }
        }
        #endregion
    }
    #endregion
}
