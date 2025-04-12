// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents optional modifiers about an API type.
/// </summary>
[Flags]
public enum ApiTypeModifiers
{
    #region Values    
    /// <summary>Represents the API type has no modifiers.</summary>
    None = 0,

    /// <summary>Represents the API instance (of API type) is required and will always be non-null.</summary>
    Required = 1,
    #endregion
}
