// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Nodes;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Configuration;
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Types;

public class ApiPresenceTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class InvalidPresenceTest : XUnitTest
    {
        #region User Supplied Properties
        public int SuppliedNumber { get; init; }
        public string? JsonToken { get; init; }
        #endregion

        #region Calculated Properties
        private string? SchemaJson { get; set; }
        private ApiSchemaCompilationException? ActualException { get; set; }
        private InvalidPresenceTest RestoredTestCase { get; set; } = null!;
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario:      {this.Name}");
            this.WriteLine("API Parameter: Echo.text");
            this.WriteLine();
            this.WriteLine($"Supplied Number: {this.SuppliedNumber}");
            this.WriteLine($"JSON Token:      {this.JsonToken ?? "<direct construction>"}");
            this.WriteLine();
            this.WriteLine($"Expected Issue:     {ApiSchemaCompilationCode.ApiQuerySourceParameterInvalidPresence}");
            this.WriteLine($"Expected Exception: {nameof(ApiSchemaCompilationException)}");

            var serializer = new XUnitTestSerializer();
            this.RestoredTestCase = (InvalidPresenceTest)serializer.Deserialize(
                this.GetType(), serializer.Serialize(this));
            if (this.JsonToken is not null)
            {
                var schema = CreateBuilder(ApiPresence.Required).Build();
                var json = JsonNode.Parse(JsonSerializer.Serialize(schema))!.AsObject();
                var parameter = json[nameof(ApiSchema.ApiQuerySources)]![0]!
                    [nameof(ApiQuerySource.ApiInputParameters)]![0]!.AsObject();
                parameter[nameof(ApiQuerySourceParameter.ApiPresence)] = JsonNode.Parse(this.JsonToken);
                this.SchemaJson = json.ToJsonString();
            }
        }

        protected override void Act()
        {
            try
            {
                if (this.SchemaJson is null)
                {
                    CreateBuilder((ApiPresence)this.SuppliedNumber).Build();
                }
                else
                {
                    JsonSerializer.Deserialize<ApiSchema>(this.SchemaJson);
                }
            }
            catch (ApiSchemaCompilationException exception)
            {
                this.ActualException = exception;
            }

            this.WriteLine();
            this.WriteLine($"Actual Success: {this.ActualException is null}");
            this.WriteLine($"Actual Issues:  {string.Join(", ", this.ActualException?.Errors.Select(e => e.Code) ?? [])}");
            this.WriteLine();
            this.WriteLine($"Captured Exception: {this.ActualException?.ToString() ?? "None"}");
        }

        protected override void Assert()
        {
            this.RestoredTestCase.SuppliedNumber.Should().Be(this.SuppliedNumber);
            this.RestoredTestCase.JsonToken.Should().Be(this.JsonToken);
            this.ActualException.Should().NotBeNull();
            var issue = this.ActualException!.Errors.Should().ContainSingle().Subject;
            issue.Code.Should().Be(ApiSchemaCompilationCode.ApiQuerySourceParameterInvalidPresence);
            issue.ApiPath.Should().EndWith("ApiQuerySource[\"Echo\"].ApiQuerySourceParameter[\"text\"]");
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] InvalidPresenceTheoryData =>
    [
        .. from number in new[] { -1, 2, 3 }
           select (TheoryDataRow<IXUnitTest>)new InvalidPresenceTest
           {
               Name = $"Parameter rejects undefined C# presence {number}",
               SuppliedNumber = number,
           },
        .. from token in new[] { "null", "\"Unknown\"", "\"Nullable\"", "\" \"", "\"2\"", "2",
               "true", "false", "{}", "[]", "\"Optional, Required\"" }
           select (TheoryDataRow<IXUnitTest>)new InvalidPresenceTest
           {
               Name = $"Parameter rejects JSON presence {token}",
               JsonToken = token,
           },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(InvalidPresenceTheoryData))]
    public void CompileRejectsInvalidParameterPresence(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Implementation Methods
    private static ApiSchemaBuilder CreateBuilder(ApiPresence apiPresence) => new ApiSchemaBuilder()
        .WithName("PresenceTests")
        .AddScalar<string>()
        .AddQuerySource("Echo", source => source
            .AddInputParameter<string>("text", parameter => parameter.WithPresence(apiPresence))
            .WithOutput<string>());
    #endregion
}
