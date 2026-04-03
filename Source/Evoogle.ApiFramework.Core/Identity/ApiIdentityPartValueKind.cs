// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Specifies the structural shape of an <see cref="ApiIdentityPartValue"/>: whether it holds a scalar
///     primitive or a nested/owned object identity.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiIdentityPartValueKind"/> is exposed via <see cref="ApiIdentityPartValue.ApiKind"/>
///         and allows callers to distinguish between the two concrete subtypes without performing a type cast.
///     </para>
///     <para>
///         The two kinds map directly to their concrete types:
///         <list type="table">
///             <listheader>
///                 <term>Kind</term>
///                 <description>Concrete type</description>
///             </listheader>
///             <item>
///                 <term><see cref="Scalar"/></term>
///                 <description><see cref="ApiScalarIdentityPartValue"/></description>
///             </item>
///             <item>
///                 <term><see cref="Object"/></term>
///                 <description><see cref="ApiObjectIdentityPartValue"/></description>
///             </item>
///         </list>
///     </para>
/// </remarks>
public enum ApiIdentityPartValueKind
{
    /// <summary>
    ///     The part holds a flat scalar <see cref="ApiId"/> primitive (e.g., <see cref="int"/>,
    ///     <see cref="string"/>, <see cref="Guid"/>).
    /// </summary>
    /// <remarks>
    ///     The concrete type is <see cref="ApiScalarIdentityPartValue"/>. Access the underlying value via
    ///     <see cref="ApiScalarIdentityPartValue.ApiScalarValue"/>.
    /// </remarks>
    Scalar,

    /// <summary>
    ///     The part holds a nested or owner-derived <see cref="ApiIdentityValue"/>, representing either
    ///     a composite child identity or a 1-to-1 ownership relationship (e.g., a <c>UserProfile</c>
    ///     identity derived from its owning <c>User</c>).
    /// </summary>
    /// <remarks>
    ///     The concrete type is <see cref="ApiObjectIdentityPartValue"/>. Access the nested identity via
    ///     <see cref="ApiObjectIdentityPartValue.ApiObjectValue"/>, which may be <see langword="null"/>
    ///     when the object part is unresolved. Use <see cref="ApiObjectIdentityPartValue.IsResolved"/>
    ///     to check before accessing.
    /// </remarks>
    Object
}
