// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API object that has API named properties that can be read/written from/to an API service.
/// </summary>
public sealed class ApiObjectType : ApiNamedType
{
    #region ApiType Properties
    /// <inheritdoc/>
    public override ApiTypeKind Kind => ApiTypeKind.Object;
    #endregion

    #region ApiObject Fields
    private readonly Dictionary<string, ApiProperty> _propertyApiNameLookup;
    private readonly Dictionary<string, ApiProperty> _propertyClrNameLookup;
    private readonly Dictionary<string, ApiRelationship> _relationshipApiNameLookup;
    #endregion

    #region ApiObject Properties
    /// <summary>
    ///     Gets the collection of API properties defined on this object type.
    /// </summary>
    public IReadOnlyCollection<ApiProperty> ApiProperties { get; }

    /// <summary>
    ///     Gets the collection of API relationships defined on this object type.
    /// </summary>
    public IReadOnlyCollection<ApiRelationship> ApiRelationships { get; }
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiObjectType"/> class.
    /// </summary>
    /// <param name="apiName">The API name of the object type.</param>
    /// <param name="apiProperties">The collection of API properties defined on this object type.</param>
    /// <param name="apiRelationships">The collection of API relationships defined on this object type.</param>
    /// <param name="clrObjectType">The CLR type representing this API object.</param>
    /// <exception cref="ApiSchemaException">Thrown if duplicate API or CLR property names are detected.</exception>
    public ApiObjectType(string apiName, IEnumerable<ApiProperty> apiProperties, IEnumerable<ApiRelationship> apiRelationships, Type clrObjectType)
        : base(apiName, clrObjectType)
    {
        this.ApiProperties = apiProperties.SafeToArray();
        this.ApiRelationships = apiRelationships.SafeToArray();

        ApiSchemaHelpers.ValidateUnique<ApiObjectType, ApiProperty, string>(this.ApiProperties, x => x.ApiName, nameof(ApiProperty.ApiName));
        ApiSchemaHelpers.ValidateUnique<ApiObjectType, ApiProperty, string>(this.ApiProperties, x => x.ClrName, nameof(ApiProperty.ClrName));
        ApiSchemaHelpers.ValidateUnique<ApiObjectType, ApiRelationship, string>(this.ApiRelationships, x => x.ApiName, nameof(ApiRelationship.ApiName));

        _propertyApiNameLookup = this.ApiProperties.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        _propertyClrNameLookup = this.ApiProperties.ToDictionary(x => x.ClrName, StringComparer.OrdinalIgnoreCase);
        _relationshipApiNameLookup = this.ApiRelationships.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
    }
    #endregion

    #region ApiObjectType Methods
    /// <summary>
    ///     Attempts to retrieve an API property by its API name.
    /// </summary>
    /// <param name="apiName">The API name of the property to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiProperty"/> if found; otherwise, null.</param>
    /// <returns>True if the property was found; otherwise, false.</returns>
    public bool TryGetPropertyByApiName(string apiName, out ApiProperty? value) => _propertyApiNameLookup.TryGetValue(apiName, out value);

    /// <summary>
    ///     Attempts to retrieve an API property by its CLR name.
    /// </summary>
    /// <param name="clrName">The CLR name of the property to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiProperty"/> if found; otherwise, null.</param>
    /// <returns>True if the property was found; otherwise, false.</returns>
    public bool TryGetPropertyByClrName(string clrName, out ApiProperty? value) => _propertyClrNameLookup.TryGetValue(clrName, out value);

    /// <summary>
    ///     Attempts to retrieve an API relationship by its API name.
    /// </summary>
    /// <param name="apiName">The API name of the relationship to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiRelationship"/> if found; otherwise, null.</param>
    /// <returns>True if the relationship was found; otherwise, false.</returns>
    public bool TryGetRelationshipByApiName(string apiName, out ApiRelationship? value) => _relationshipApiNameLookup.TryGetValue(apiName, out value);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrType = this.ClrType.SafeToString();

        return $"{nameof(ApiObjectType)} {{{nameof(this.ApiName)}={apiName}}} [{clrType}]";
    }
    #endregion
}
