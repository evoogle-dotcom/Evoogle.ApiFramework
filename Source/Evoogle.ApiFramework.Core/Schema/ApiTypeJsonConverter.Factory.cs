// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.Extension;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiTypeJsonConverter : JsonConverter<ApiType>
{
    #region Factory Implementation Methods
    private static ApiType CreateApiType(in ReadContext context)
    {
        // Validate all required properties are non-null.
        var validationResults = default(List<ValidationResult>);
        ValidateApiTypeProperties(context, ref validationResults);

        var kindAsString = context.ReadData.ApiType!.Kind;
        if (Enum.TryParse<ApiTypeKind>(kindAsString, out var kind) == false)
        {
            context.Logger.LogError("Invalid Kind: '{Kind}'", kindAsString);

            throw new JsonException($"Unable to parse '{kindAsString}' into an {nameof(ApiTypeKind)} enumeration.");
        }

        // Construct the appropriate ApiType based on its declared Kind, after applying kind-specific validation logic.
        var apiType = default(ApiType);
        switch (kind)
        {
            case ApiTypeKind.Collection:
                {
                    ValidateApiCollectionTypeProperties(context, ref validationResults);

                    apiType = CreateApiCollectionType(context, kind, validationResults);
                }
                break;

            case ApiTypeKind.Enum:
                {
                    ValidateApiNamedTypeProperties(context, ref validationResults);
                    ValidateApiEnumTypeProperties(context, ref validationResults);

                    apiType = CreateApiEnumType(context, kind, validationResults);
                }
                break;

            case ApiTypeKind.Object:
                {
                    ValidateApiNamedTypeProperties(context, ref validationResults);
                    ValidateApiObjectTypeProperties(context, ref validationResults);

                    apiType = CreateApiObjectType(context, kind, validationResults);
                }
                break;

            case ApiTypeKind.Scalar:
                {
                    ValidateApiNamedTypeProperties(context, ref validationResults);

                    apiType = CreateApiScalarType(context, kind, validationResults);
                }
                break;

            default:
                {
                    context.Logger.LogError("Unsupported Kind: '{Kind}'", kindAsString);

                    throw new JsonException($"Unable to create a derived {nameof(ApiType)} because no factory method exists for {kind} enumeration.");
                }
        }

        AttachExtensions(context, apiType);

        return apiType;
    }

    private static void AttachExtensions(in ReadContext context, ExtensibleBase extensibleBase)
    {
        ApiJsonConverterHelpers.AttachExtensions(extensibleBase, context.ReadData.ExtensibleBase?.Extensions);
    }

    private static ApiCollectionType CreateApiCollectionType(in ReadContext context, ApiTypeKind kind, List<ValidationResult>? validationResults)
    {
        ThrowIfInvalid(context, kind, validationResults);

        var apiItemType = context.ReadData.ApiCollectionType!.ApiItemType!;
        var apiItemTypeModifiers = context.ReadData.ApiCollectionType!.ApiItemTypeModifiers!.Value;
        var clrCollectionType = context.ReadData.ApiType!.ClrType!;

        return new ApiCollectionType(apiItemType, apiItemTypeModifiers, clrCollectionType);
    }

    private static ApiEnumType CreateApiEnumType(in ReadContext context, ApiTypeKind kind, List<ValidationResult>? validationResults)
    {
        ThrowIfInvalid(context, kind, validationResults);

        var apiName = context.ReadData.ApiNamedType!.ApiName!;
        var apiEnumValues = CreateApiEnumValues(context);
        var clrType = context.ReadData.ApiType!.ClrType!;

        return new ApiEnumType(apiName, apiEnumValues, clrType);
    }

    private static List<ApiEnumValue> CreateApiEnumValues(in ReadContext context)
    {
        var apiEnumValues = context.ReadData.ApiEnumType!.ApiEnumValues!.Select(x =>
        {
            var apiName = x.ApiName!;
            var clrName = x.ClrName!;
            var clrOrdinal = x.ClrOrdinal!.Value;

            var apiEnumValue = new ApiEnumValue(apiName, clrName, clrOrdinal);
            return apiEnumValue;
        })
        .ToList();

        return apiEnumValues;
    }

    private static ApiObjectType CreateApiObjectType(in ReadContext context, ApiTypeKind kind, List<ValidationResult>? validationResults)
    {
        ThrowIfInvalid(context, kind, validationResults);

        var apiName = context.ReadData.ApiNamedType!.ApiName!;
        var apiProperties = context.ReadData.ApiObjectType!.ApiProperties!.Select(x =>
        {
            var apiName = x.ApiName!;
            var apiType = x.ApiType!;
            var apiTypeModifiers = x.ApiTypeModifiers!.Value;
            var clrName = x.ClrName!;

            var apiProperty = new ApiProperty(apiName, apiType, apiTypeModifiers, clrName);
            return apiProperty;
        })
        .ToList();
        var clrType = context.ReadData.ApiType!.ClrType!;

        return new ApiObjectType(apiName, apiProperties, clrType);
    }

    private static ApiScalarType CreateApiScalarType(in ReadContext context, ApiTypeKind kind, List<ValidationResult>? validationResults)
    {
        ThrowIfInvalid(context, kind, validationResults);

        var apiName = context.ReadData.ApiNamedType!.ApiName!;
        var clrType = context.ReadData.ApiType!.ClrType!;

        return new ApiScalarType(apiName, clrType);
    }

    private static void ThrowIfInvalid(in ReadContext context, ApiTypeKind kind, IEnumerable<ValidationResult>? validationResults)
    {
        if (validationResults == null)
            return;

        if (validationResults.Any() == false)
            return;

        // Create a delimited string of all the failed validation result error messages.
        var validationErrorMessage = validationResults.Where(x => x != ValidationResult.Success && !string.IsNullOrWhiteSpace(x.ErrorMessage)).SafeToDelimitedString('\n');
        if (string.IsNullOrWhiteSpace(validationErrorMessage))
            return;

        context.Logger.LogError("Validation failed for ApiTypeKind '{Kind}': {Message}", kind, validationErrorMessage);

        throw new JsonException(validationErrorMessage);
    }
    #endregion
}
