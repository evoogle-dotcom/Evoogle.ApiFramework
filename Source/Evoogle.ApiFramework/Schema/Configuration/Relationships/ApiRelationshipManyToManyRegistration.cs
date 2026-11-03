// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Configuration.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration.Relationships;

/// <summary>Provides opaque, builder-scoped access to one many-to-many relationship declaration.</summary>
[JsonConverter(typeof(ApiRegistrationJsonConverter<ApiRelationshipManyToManyRegistration>))]
public sealed class ApiRelationshipManyToManyRegistration
{
    #region Properties
    internal object Owner { get; }
    internal object Identity { get; }
    #endregion

    #region Constructors
    internal ApiRelationshipManyToManyRegistration(object owner, object identity)
    {
        this.Owner = owner;
        this.Identity = identity;
    }
    #endregion
}
