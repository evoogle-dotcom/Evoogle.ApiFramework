// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Configuration.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration.Query;

/// <summary>Provides opaque, builder-scoped access to one query-source declaration.</summary>
[JsonConverter(typeof(ApiRegistrationJsonConverter<ApiQuerySourceRegistration>))]
public sealed class ApiQuerySourceRegistration
{
    #region Properties
    internal object Owner { get; }
    internal object Identity { get; }
    #endregion

    #region Constructors
    internal ApiQuerySourceRegistration(object owner, object identity)
    {
        this.Owner = owner;
        this.Identity = identity;
    }
    #endregion
}
