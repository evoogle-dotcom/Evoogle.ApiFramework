// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public abstract class ExtensionBuilder<TBuilder>
    where TBuilder : ExtensionBuilder<TBuilder>
{
    #region Fields
    private readonly OrderedDictionary<Type, object> _extensions = [];
    #endregion

    #region Methods
    public TBuilder AddExtension(Type type, object value)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(value);

        _extensions[type] = value;
        return (TBuilder)this;
    }

    internal OrderedDictionary<Type, object>? BuildExtensions() =>
        _extensions.Count > 0 ? new OrderedDictionary<Type, object>(_extensions) : null;
    #endregion
}

