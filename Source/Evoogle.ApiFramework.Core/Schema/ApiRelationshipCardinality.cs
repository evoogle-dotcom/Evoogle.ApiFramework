// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the relationship cardinality from an API object type to a related API object type.
/// </summary>
public enum ApiRelationshipCardinality
{
    #region Values
    /// <summary>Represents a to-one relationship.</summary>
    ToOne,

    /// <summary>Represents a to-many relationship.</summary>
    ToMany
    #endregion
}
