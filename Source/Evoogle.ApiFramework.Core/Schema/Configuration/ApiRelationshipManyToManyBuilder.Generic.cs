// // Copyright (c) 2024-2025 Evoogle.com
// // SPDX-License-Identifier: MIT
// //
// // This file is licensed under the MIT License.
// // See the LICENSE file in the project root for more information.
// namespace Evoogle.ApiFramework.Schema.Configuration;

// public sealed class ApiRelationshipManyToManyBuilder<TAssociation>(string apiName) : ApiRelationshipManyToManyBuilder(apiName)
// {
//     #region With Methods
//     public ApiRelationshipManyToManyBuilder<TAssociation> WithAssociation(Action<ApiRelationshipAssociationBuilder<TAssociation>>? configure = null)
//     {
//         var builder = new ApiRelationshipAssociationBuilder<TAssociation>();
//         configure?.Invoke(builder);
//         _associationBuilder = builder;
//         return this;
//     }

//     /// <inheritdoc cref="ApiRelationshipManyToManyBuilder.WithPrincipalEndA{TPrincipal}"/>
//     public new ApiRelationshipManyToManyBuilder<TAssociation> WithPrincipalEndA<TPrincipal>(Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
//     {
//         base.WithPrincipalEndA<TPrincipal>(configure);
//         return this;
//     }

//     /// <inheritdoc cref="ApiRelationshipManyToManyBuilder.WithPrincipalEndB{TPrincipal}"/>
//     public new ApiRelationshipManyToManyBuilder<TAssociation> WithPrincipalEndB<TPrincipal>(Action<ApiRelationshipPrincipalEndBuilder>? configure = null)
//     {
//         base.WithPrincipalEndB<TPrincipal>(configure);
//         return this;
//     }
//     #endregion
// }
