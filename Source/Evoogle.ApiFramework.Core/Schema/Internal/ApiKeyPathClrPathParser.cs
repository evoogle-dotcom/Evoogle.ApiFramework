// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiKeyPathClrPathParser
{
    #region Utility Methods
    public static ParseResult Parse(string clrPath)
    {
        ArgumentNullException.ThrowIfNull(clrPath);

        var clrPropertyNames = clrPath.Trim().Split('.').Select(static name => name.Trim()).ToArray();
        var validationMessage = clrPropertyNames.Length == 0 || clrPropertyNames.Any(string.IsNullOrWhiteSpace)
            ? "CLR paths must contain one or more non-empty dot-delimited property names."
            : null;

        return new(clrPropertyNames, validationMessage);
    }
    #endregion

    #region Nested Types
    internal sealed class ParseResult
    {
        #region Constructors
        public ParseResult(string[] clrPropertyNames, string? validationMessage)
        {
            this.ClrPropertyNames = clrPropertyNames;
            this.ValidationMessage = validationMessage;
        }
        #endregion

        #region Properties
        public IReadOnlyList<string> ClrPropertyNames { get; }

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
