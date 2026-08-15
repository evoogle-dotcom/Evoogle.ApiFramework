// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.Schema.Configuration.Trace;
using Evoogle.Extensions;
using Evoogle.XUnit;
using Evoogle.XUnit.Json;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

public partial class ApiConventionTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    // private class BuildTraceTest : XUnitTest
    // {
    //     #region Fields
    //     private static readonly JsonSerializerOptions _defaultToJsonOptions = new()
    //     {
    //         DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    //         WriteIndented = false,
    //     };
    //     #endregion

    //     #region User Supplied Properties
    //     public required string ApiSchemaExpectedJson { get; init; }
    //     public required IReadOnlyList<string> EventsExpected { get; init; }

    //     [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiSchemaBuilder>))]
    //     public required Expression<Func<ApiSchemaBuilder>> BuildExpression { get; init; }

    //     [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiSchemaBuildTrace, IReadOnlyList<string>>))]
    //     public required Expression<Func<ApiSchemaBuildTrace, IReadOnlyList<string>>> EventsExpression
    //     {
    //         get;
    //         init;
    //     }
    //     #endregion

    //     #region Calculated Properties
    //     private ApiSchema? ApiSchemaExpected { get; set; }
    //     private ApiSchema? ApiSchemaActual { get; set; }
    //     private ApiSchemaBuildTrace? BuildTraceActual { get; set; }
    //     #endregion

    //     #region Constructors
    //     public BuildTraceTest()
    //     {
    //         this.Name = nameof(BuildTraceTest);
    //         this.ExcludeMembers = ApiSchemaExcludeMembers.SchemaInitialized;
    //     }
    //     #endregion

    //     #region XUnitTest Methods
    //     protected override void Arrange()
    //     {
    //         this.ApiSchemaExpected = JsonSerializer.Deserialize<ApiSchema>
    //         (
    //             this.ApiSchemaExpectedJson
    //         );

    //         this.WriteLine("ApiSchemaExpected:");
    //         this.WriteLine($"{this.ApiSchemaExpected.SafeToJson(_defaultToJsonOptions)}");
    //         this.WriteLine();
    //         this.WriteLine($"EventsExpected: {this.EventsExpected.SafeToJson()}");
    //         this.WriteLine();
    //     }

    //     protected override void Act()
    //     {
    //         var schemaBuilder = this.BuildExpression.Compile()();
    //         var sink = new ApiInMemorySchemaBuildTraceSink();
    //         this.ApiSchemaActual = schemaBuilder.Build(sink);
    //         this.BuildTraceActual = sink.CreateTrace();

    //         this.WriteLine("ApiSchemaActual:");
    //         this.WriteLine
    //         (
    //             $"{this.ApiSchemaActual.SafeToJson(_defaultToJsonOptions)}"
    //         );
    //         this.WriteLine();
    //         this.WriteLine($"EventsActual: {this.BuildTraceActual.Events.SafeToJson()}");
    //     }

    //     protected override void Assert()
    //     {
    //         this.BuildTraceActual.Should().NotBeNull();
    //         this.AssertBeEquivalentTo
    //         (
    //             this.ApiSchemaActual,
    //             this.ApiSchemaExpected
    //         );
    //         var eventsActual = this.EventsExpression.Compile()(this.BuildTraceActual!);
    //         eventsActual.Should().Equal(this.EventsExpected);
    //     }
    //     #endregion
    // }

    // private class EnumValueNamingContextTest : XUnitTest
    // {
    //     #region User Supplied Properties
    //     public required EnumValueNamingContextSnapshot SnapshotExpected { get; init; }

    //     [JsonConverter(typeof(ExpressionFuncJsonConverter<EnumValueNamingContextSnapshot>))]
    //     public required Expression<Func<EnumValueNamingContextSnapshot>> SnapshotExpression
    //     {
    //         get;
    //         init;
    //     }
    //     #endregion

    //     #region Calculated Properties
    //     private EnumValueNamingContextSnapshot? SnapshotActual { get; set; }
    //     #endregion

    //     #region Constructors
    //     public EnumValueNamingContextTest()
    //     {
    //         this.Name = nameof(EnumValueNamingContextTest);
    //     }
    //     #endregion

    //     #region XUnitTest Methods
    //     protected override void Act()
    //     {
    //         var snapshotLambda = this.SnapshotExpression.Compile();
    //         this.SnapshotActual = snapshotLambda();
    //     }

    //     protected override void Assert()
    //     {
    //         this.SnapshotActual.Should().BeEquivalentTo(this.SnapshotExpected);
    //     }
    //     #endregion
    // }

    // private class EnumTargetValueTest : XUnitTest
    // {
    //     #region User Supplied Properties
    //     public required ApiNamingConventionTarget Target { get; init; }
    //     public required int ValueExpected { get; init; }
    //     #endregion

    //     #region Calculated Properties
    //     private int ValueActual { get; set; }
    //     #endregion

    //     #region Constructors
    //     public EnumTargetValueTest()
    //     {
    //         this.Name = nameof(EnumTargetValueTest);
    //     }
    //     #endregion

    //     #region XUnitTest Methods
    //     protected override void Act()
    //     {
    //         this.ValueActual = (int)this.Target;
    //     }

    //     protected override void Assert()
    //     {
    //         this.ValueActual.Should().Be(this.ValueExpected);
    //     }
    //     #endregion
    // }

    // private class ConventionSetTest : XUnitTest
    // {
    //     #region User Supplied Properties
    //     public required ApiConventionSetSnapshot SnapshotExpected { get; init; }

    //     [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiConventionSetSnapshot>))]
    //     public required Expression<Func<ApiConventionSetSnapshot>> SnapshotExpression
    //     {
    //         get;
    //         init;
    //     }
    //     #endregion

    //     #region Calculated Properties
    //     private ApiConventionSetSnapshot? SnapshotActual { get; set; }
    //     #endregion

    //     #region Constructors
    //     public ConventionSetTest()
    //     {
    //         this.Name = nameof(ConventionSetTest);
    //     }
    //     #endregion

    //     #region XUnitTest Methods
    //     protected override void Act()
    //     {
    //         var snapshotLambda = this.SnapshotExpression.Compile();
    //         this.SnapshotActual = snapshotLambda();
    //     }

    //     protected override void Assert()
    //     {
    //         this.SnapshotActual.Should().BeEquivalentTo(this.SnapshotExpected);
    //     }
    //     #endregion
    // }
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(BuildThrowsTheoryData))]
    public void BuildThrows(IXUnitTest test) => test.Execute(this);

    // [Theory]
    // [MemberData(nameof(BuildTraceTheoryData))]
    // public void BuildTrace(IXUnitTest test) => test.Execute(this);

    // [Theory]
    // [MemberData(nameof(ConventionContractTheoryData))]
    // public void ConventionContract(IXUnitTest test) => test.Execute(this);
    #endregion
}
