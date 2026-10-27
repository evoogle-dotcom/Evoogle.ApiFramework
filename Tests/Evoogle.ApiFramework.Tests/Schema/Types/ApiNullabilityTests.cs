// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Types;

public class ApiNullabilityTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    public enum NullabilityContext
    {
        Property,
        CollectionItem,
        InputParameter,
        Output,
    }

    public sealed class NullabilityModel
    {
        public string? Text { get; set; }
        public List<List<string?>?>? Nested { get; set; }
    }

    private sealed class InvalidNullabilityTest : XUnitTest
    {
        #region User Supplied Properties
        public required NullabilityContext Context { get; init; }
        public required ApiSchemaCompilationCode ExpectedCode { get; init; }
        public int SuppliedNumber { get; init; }
        public string? JsonToken { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema Schema { get; set; } = null!;
        private string? SchemaJson { get; set; }
        private ApiSchemaCompilationResult? ActualResult { get; set; }
        private Exception? ActualException { get; set; }
        private InvalidNullabilityTest RestoredTestCase { get; set; } = null!;
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario:    {this.Name}");
            this.WriteLine($"API Context: {this.Context}");
            this.WriteLine();
            this.WriteLine($"Supplied Number: {this.SuppliedNumber}");
            this.WriteLine($"JSON Token:      {this.JsonToken ?? "<direct construction>"}");
            this.WriteLine();
            this.WriteLine($"Expected Issue:     {this.ExpectedCode}");
            var expectedException = this.JsonToken is null ? "None" : nameof(ApiSchemaCompilationException);
            this.WriteLine($"Expected Exception: {expectedException}");

            var serializer = new XUnitTestSerializer();
            this.RestoredTestCase = (InvalidNullabilityTest)serializer.Deserialize(
                this.GetType(), serializer.Serialize(this));
            this.Schema = CreateValidationSchema(this.Context,
                this.JsonToken is null ? (ApiNullability)this.SuppliedNumber : ApiNullability.Nullable);

            if (this.JsonToken is not null)
            {
                ApiSchemaCompiler.Compile(this.Schema).ThrowIfInvalid();
                var json = JsonNode.Parse(JsonSerializer.Serialize(this.Schema))!.AsObject();
                var propertyName = this.Context switch
                {
                    NullabilityContext.CollectionItem => nameof(ApiCollectionType.ApiItemNullability),
                    NullabilityContext.Output => nameof(ApiQuerySource.ApiOutputNullability),
                    _ => nameof(ApiProperty.ApiNullability),
                };
                GetContextJson(json, this.Context)[propertyName] = JsonNode.Parse(this.JsonToken);
                this.SchemaJson = json.ToJsonString();
            }
        }

        protected override void Act()
        {
            try
            {
                if (this.SchemaJson is null)
                {
                    this.ActualResult = ApiSchemaCompiler.Compile(this.Schema);
                }
                else
                {
                    JsonSerializer.Deserialize<ApiSchema>(this.SchemaJson);
                }
            }
            catch (ApiSchemaCompilationException exception)
            {
                this.ActualException = exception;
                this.ActualResult = exception.Result;
            }
            catch (Exception exception)
            {
                this.ActualException = exception;
            }

            this.WriteLine();
            this.WriteLine($"Actual Success: {this.ActualResult?.IsValid}");
            var actualCodes = this.ActualResult?.Errors.Select(issue => issue.Code) ?? [];
            this.WriteLine($"Actual Issues:  {string.Join(", ", actualCodes)}");
            this.WriteLine();
            this.WriteLine($"Captured Exception: {this.ActualException?.ToString() ?? "None"}");
        }

        protected override void Assert()
        {
            this.RestoredTestCase.Context.Should().Be(this.Context);
            this.RestoredTestCase.ExpectedCode.Should().Be(this.ExpectedCode);
            this.RestoredTestCase.SuppliedNumber.Should().Be(this.SuppliedNumber);
            this.RestoredTestCase.JsonToken.Should().Be(this.JsonToken);
            this.ActualResult.Should().NotBeNull();
            this.ActualResult!.IsValid.Should().BeFalse();
            var issue = this.ActualResult.Errors.Should().ContainSingle().Subject;
            issue.Code.Should().Be(this.ExpectedCode);
            issue.ApiPath.Should().Contain(this.Context == NullabilityContext.Property
                ? "ApiProperty" : "ApiQuerySource");
            if (this.JsonToken is null)
            {
                this.ActualException.Should().BeNull();
            }
            else
            {
                this.ActualException.Should().BeOfType<ApiSchemaCompilationException>();
            }
        }
        #endregion
    }

    private sealed class NestedNullabilityTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiNullability ValueNullability { get; init; }
        public required ApiNullability CollectionNullability { get; init; }
        public required ApiNullability ItemNullability { get; init; }
        public required ApiPresence Presence { get; init; }
        public required bool UsesCamelCase { get; init; }
        public required JsonIgnoreCondition IgnoreCondition { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema Schema { get; set; } = null!;
        private ApiSchema ActualSchema { get; set; } = null!;
        private NestedNullabilityTest RestoredTestCase { get; set; } = null!;
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario:     {this.Name}");
            this.WriteLine("API Identity: Model.Nested; Echo.text; Echo output");
            this.WriteLine();
            this.WriteLine($"Value Nullability:      {this.ValueNullability}");
            this.WriteLine($"Collection Nullability: {this.CollectionNullability}");
            this.WriteLine($"Item Nullability:       {this.ItemNullability}");
            this.WriteLine($"Requires Argument:      {this.Presence}");
            this.WriteLine($"Camel Case:             {this.UsesCamelCase}");
            this.WriteLine($"Ignore Condition:       {this.IgnoreCondition}");
            this.WriteLine();
            this.WriteLine("Expected Result:    All contextual constraints survive schema JSON round trip");
            this.WriteLine("Expected Exception: None");

            var serializer = new XUnitTestSerializer();
            this.RestoredTestCase = (NestedNullabilityTest)serializer.Deserialize(
                this.GetType(), serializer.Serialize(this));
            var inner = new ApiCollectionType(TextExpression(), this.ItemNullability, typeof(List<string>));
            var outer = new ApiCollectionType(new ApiTypeExpression(inner),
                this.CollectionNullability, typeof(List<List<string>>));
            var property = new ApiProperty(nameof(NullabilityModel.Nested), new ApiTypeExpression(outer),
                this.ValueNullability, new ClrMemberReference(ClrMemberKind.Property, nameof(NullabilityModel.Nested)));
            var source = new ApiQuerySource("Echo",
                [new ApiQuerySourceParameter("text", TextExpression(), this.ValueNullability, this.Presence)],
                TextExpression(), this.ValueNullability, ApiQueryComposition.Projection);
            this.Schema = CreateSchema(property, source);
            ApiSchemaCompiler.Compile(this.Schema).ThrowIfInvalid();
        }

        protected override void Act()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = this.UsesCamelCase ? JsonNamingPolicy.CamelCase : null,
                DefaultIgnoreCondition = this.IgnoreCondition,
            };
            var json = JsonSerializer.Serialize(this.Schema, options);
            this.ActualSchema = JsonSerializer.Deserialize<ApiSchema>(json, options)!;
            var property = this.ActualSchema.ApiObjectTypes.Single().ApiProperties.Single();
            var outer = (ApiCollectionType)property.ApiType;
            var inner = (ApiCollectionType)outer.ApiItemType;
            this.WriteLine();
            this.WriteLine($"Actual Value Nullability:      {property.ApiNullability}");
            this.WriteLine($"Actual Collection Nullability: {outer.ApiItemNullability}");
            this.WriteLine($"Actual Item Nullability:       {inner.ApiItemNullability}");
            var parameter = this.ActualSchema.ApiQuerySources.Single().ApiInputParameters.Single();
            this.WriteLine($"Actual Requires Argument:      {parameter.ApiPresence}");
            this.WriteLine("Schema Compilation:            Succeeded");
        }

        protected override void Assert()
        {
            this.RestoredTestCase.ValueNullability.Should().Be(this.ValueNullability);
            this.RestoredTestCase.CollectionNullability.Should().Be(this.CollectionNullability);
            this.RestoredTestCase.ItemNullability.Should().Be(this.ItemNullability);
            this.RestoredTestCase.Presence.Should().Be(this.Presence);
            this.RestoredTestCase.UsesCamelCase.Should().Be(this.UsesCamelCase);
            this.RestoredTestCase.IgnoreCondition.Should().Be(this.IgnoreCondition);
            var property = this.ActualSchema.ApiObjectTypes.Single().ApiProperties.Single();
            property.ApiNullability.Should().Be(this.ValueNullability);
            property.IsNullable.Should().Be(this.ValueNullability == ApiNullability.Nullable);
            property.IsNonNullable.Should().Be(this.ValueNullability == ApiNullability.NonNullable);
            var outer = property.ApiType.Should().BeOfType<ApiCollectionType>().Subject;
            outer.ApiItemNullability.Should().Be(this.CollectionNullability);
            outer.IsItemNullable.Should().Be(this.CollectionNullability == ApiNullability.Nullable);
            outer.IsItemNonNullable.Should().Be(this.CollectionNullability == ApiNullability.NonNullable);
            var inner = outer.ApiItemType.Should().BeOfType<ApiCollectionType>().Subject;
            inner.ApiItemNullability.Should().Be(this.ItemNullability);
            inner.IsItemNullable.Should().Be(this.ItemNullability == ApiNullability.Nullable);
            inner.IsItemNonNullable.Should().Be(this.ItemNullability == ApiNullability.NonNullable);
            var source = this.ActualSchema.ApiQuerySources.Single();
            source.ApiOutputNullability.Should().Be(this.ValueNullability);
            var parameter = source.ApiInputParameters.Single();
            parameter.ApiNullability.Should().Be(this.ValueNullability);
            parameter.ApiPresence.Should().Be(this.Presence);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] InvalidNullabilityTheoryData =>
    [
        .. from context in Enum.GetValues<NullabilityContext>()
           from number in new[] { -1, 2, 3 }
           select (TheoryDataRow<IXUnitTest>)new InvalidNullabilityTest
           {
               Name = $"{context} rejects undefined C# nullability {number}",
               Context = context,
               ExpectedCode = GetInvalidCode(context),
               SuppliedNumber = number,
           },
        .. from context in Enum.GetValues<NullabilityContext>()
           from token in new[] { "null", "\"Unknown\"", "\"None\"", "\"Required\"", "2", "true", "{}", "[]",
               "\" \"", "\"2\"", "\"Nullable, NonNullable\"" }
           select (TheoryDataRow<IXUnitTest>)new InvalidNullabilityTest
           {
               Name = $"{context} rejects JSON nullability {token}",
               Context = context,
               ExpectedCode = GetInvalidCode(context),
               JsonToken = token,
           },
    ];

    public static TheoryDataRow<IXUnitTest>[] NestedNullabilityTheoryData =>
    [
        .. from valueNullability in Enum.GetValues<ApiNullability>()
           from collectionNullability in Enum.GetValues<ApiNullability>()
           from itemNullability in Enum.GetValues<ApiNullability>()
           from presence in Enum.GetValues<ApiPresence>()
           from usesCamelCase in new[] { false, true }
           from ignoreCondition in new[] { JsonIgnoreCondition.Never, JsonIgnoreCondition.WhenWritingNull,
               JsonIgnoreCondition.WhenWritingDefault }
           select (TheoryDataRow<IXUnitTest>)new NestedNullabilityTest
           {
               Name = $"Value={valueNullability}, Collection={collectionNullability}, Item={itemNullability}, " +
                   $"Presence={presence}, CamelCase={usesCamelCase}, {ignoreCondition}",
               ValueNullability = valueNullability,
               CollectionNullability = collectionNullability,
               ItemNullability = itemNullability,
               Presence = presence,
               UsesCamelCase = usesCamelCase,
               IgnoreCondition = ignoreCondition,
           },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(InvalidNullabilityTheoryData))]
    public void CompileRejectsInvalidContextualNullability(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(NestedNullabilityTheoryData))]
    public void JsonRoundtripPreservesNestedAndContextualNullability(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Implementation Methods
    private static ApiTypeExpression TextExpression() => new(new ApiTypeReference(typeof(string)));

    private static ApiSchema CreateSchema(ApiProperty property, ApiQuerySource source) => new
    (
        "NullabilityTests", null, null,
        [new ApiScalarType("String", typeof(string))], [],
        [new ApiObjectType("Model", null, [property], [], null, null, typeof(NullabilityModel))], [], [source]
    );

    private static ApiSchema CreateValidationSchema(NullabilityContext context, ApiNullability apiNullability)
    {
        var property = new ApiProperty(nameof(NullabilityModel.Text), TextExpression(),
            context == NullabilityContext.Property ? apiNullability : ApiNullability.Nullable,
            new ClrMemberReference(ClrMemberKind.Property, nameof(NullabilityModel.Text)));
        var output = context == NullabilityContext.CollectionItem
            ? new ApiTypeExpression(new ApiCollectionType(TextExpression(), apiNullability, typeof(List<string>)))
            : TextExpression();
        var source = new ApiQuerySource("Echo",
            [new ApiQuerySourceParameter("text", TextExpression(),
                context == NullabilityContext.InputParameter ? apiNullability : ApiNullability.Nullable, ApiPresence.Required)],
            output, context == NullabilityContext.Output ? apiNullability : ApiNullability.Nullable,
            ApiQueryComposition.Projection);
        return CreateSchema(property, source);
    }

    private static ApiSchemaCompilationCode GetInvalidCode(NullabilityContext context) => context switch
    {
        NullabilityContext.Property => ApiSchemaCompilationCode.ApiPropertyInvalidApiNullability,
        NullabilityContext.CollectionItem => ApiSchemaCompilationCode.ApiCollectionTypeInvalidApiItemNullability,
        NullabilityContext.InputParameter => ApiSchemaCompilationCode.ApiQuerySourceParameterInvalidApiNullability,
        NullabilityContext.Output => ApiSchemaCompilationCode.ApiQuerySourceInvalidApiOutputNullability,
        _ => throw new ArgumentOutOfRangeException(nameof(context)),
    };

    private static JsonObject GetContextJson(JsonObject schema, NullabilityContext context)
    {
        if (context == NullabilityContext.Property)
        {
            return schema[nameof(ApiSchema.ApiObjectTypes)]![0]![nameof(ApiObjectType.ApiProperties)]![0]!.AsObject();
        }

        var source = schema[nameof(ApiSchema.ApiQuerySources)]![0]!.AsObject();
        return context switch
        {
            NullabilityContext.CollectionItem => source[nameof(ApiQuerySource.ApiOutputType)]!.AsObject(),
            NullabilityContext.InputParameter => source[nameof(ApiQuerySource.ApiInputParameters)]![0]!.AsObject(),
            _ => source,
        };
    }
    #endregion
}
