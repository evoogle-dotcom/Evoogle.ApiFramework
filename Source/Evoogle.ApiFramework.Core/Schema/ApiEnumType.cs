// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API enumeration.
/// </summary>
public class ApiEnumType : ApiNamedType
{
    #region ApiType Properties
    public override ApiTypeKind Kind => ApiTypeKind.Enum;
    #endregion

    #region ApiEnumType Properties
    public IEnumerable<ApiEnumValue> ApiEnumValues { get; }

    private IReadOnlyDictionary<string, ApiEnumValue> ApiEnumValueByApiNameDictionary { get; }
    private IReadOnlyDictionary<string, ApiEnumValue> ApiEnumValueByClrNameDictionary { get; }
    private IReadOnlyDictionary<int, ApiEnumValue> ApiEnumValueByClrOrdinalDictionary { get; }
    #endregion

    #region Constructors
    public ApiEnumType(string apiName, IEnumerable<ApiEnumValue> apiEnumValues, Type clrEnumType)
        : base(apiName, clrEnumType)
    {
        if (!TypeReflection.IsEnum(clrEnumType))
        {
            var message = $"Unable to create an API enum type, the CLR type [name={clrEnumType.Name}] is not a CLR enum type.";
            throw new ApiSchemaException(message);
        }

        this.ApiEnumValues = apiEnumValues.EmptyIfNull();

        this.ApiEnumValueByApiNameDictionary = this.ApiEnumValues.ToDictionary(x => x.ApiName);
        this.ApiEnumValueByClrNameDictionary = this.ApiEnumValues.ToDictionary(x => x.ClrName);
        this.ApiEnumValueByClrOrdinalDictionary = this.ApiEnumValues.ToDictionary(x => x.ClrOrdinal);
    }
    #endregion

    #region ApiEnumType Methods
    public bool TryGetApiEnumValueByApiName(string apiName, out ApiEnumValue? apiEnumValue)
    {
        return this.ApiEnumValueByApiNameDictionary.TryGetValue(apiName, out apiEnumValue);
    }

    public bool TryGetApiEnumValueByClrName(string clrName, out ApiEnumValue? apiEnumValue)
    {
        return this.ApiEnumValueByClrNameDictionary.TryGetValue(clrName, out apiEnumValue);
    }

    public bool TryGetApiEnumValueByClrOrdinal(int clrOrdinal, out ApiEnumValue? apiEnumValue)
    {
        return this.ApiEnumValueByClrOrdinalDictionary.TryGetValue(clrOrdinal, out apiEnumValue);
    }
    #endregion

    #region Object Methods
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrType = this.ClrType.SafeToString();

        return $"{nameof(ApiEnumType)} {{{nameof(ApiName)}={apiName}}} [{clrType}]";
    }
    #endregion
}
