// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Configuration.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration.Relationships;

/// <summary>Provides opaque, builder-scoped access to one one-to-many relationship declaration.</summary>
[JsonConverter(typeof(ApiRegistrationJsonConverter<ApiRelationshipOneToManyRegistration>))]
public sealed class ApiRelationshipOneToManyRegistration
{
    #region Properties
    internal object Owner { get; }
    internal object Identity { get; }
    #endregion

    #region Constructors
    internal ApiRelationshipOneToManyRegistration(object owner, object identity)
    {
        this.Owner = owner;
        this.Identity = identity;
    }
    #endregion
}
