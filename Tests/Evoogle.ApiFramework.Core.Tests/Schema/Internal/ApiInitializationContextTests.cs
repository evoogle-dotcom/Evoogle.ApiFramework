// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

using FluentAssertions;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Internal;

public class ApiInitializationContextTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private enum DiagnosticPathTestCase
    {
        KeyPath,
        OneToRelationship,
        ManyToManyRelationship,
        BlankIndexedLabel
    }

    private sealed class DiagnosticPathTest : XUnitTest
    {
        #region User Supplied Properties
        public required DiagnosticPathTestCase TestCase { get; init; }
        public required string[] ExpectedPaths { get; init; }
        #endregion

        #region Calculated Properties
        private string[]? ActualPaths { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.ActualPaths = this.TestCase switch
            {
                DiagnosticPathTestCase.KeyPath => GetKeyPaths(),
                DiagnosticPathTestCase.OneToRelationship => GetOneToRelationshipPaths(),
                DiagnosticPathTestCase.ManyToManyRelationship =>
                    GetManyToManyRelationshipPaths(),
                DiagnosticPathTestCase.BlankIndexedLabel => GetBlankIndexedLabelPath(),
                _ => throw new InvalidOperationException
                (
                    $"Unsupported {nameof(DiagnosticPathTestCase)} value '{this.TestCase}'."
                ),
            };
        }

        protected override void Assert()
        {
            this.ActualPaths.Should().Equal(this.ExpectedPaths);
        }
        #endregion
    }

    private sealed class IssueLoggingTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiInitializationCode Code { get; init; }
        public required LogLevel ExpectedLogLevel { get; init; }
        public required string? Remediation { get; init; }
        public required ApiInitializationSeverity Severity { get; init; }
        #endregion

        #region Calculated Properties
        private RecordingLogger? Logger { get; set; }
        private ApiInitializationSession? Session { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Logger = new RecordingLogger();
            var schema = CreateSchema("IssueLogging");
            this.Session = new ApiInitializationSession(schema, this.Logger);
        }

        protected override void Act()
        {
            var context = this.Session!.CreateContext
            (
                this.Session.ApiSchema,
                location: default
            );

            context.AddIssue
            (
                this.Severity,
                this.Code,
                "Description",
                this.Remediation
            );
        }

        protected override void Assert()
        {
            this.Session!.Issues.Should().ContainSingle();
            this.Logger!.Entries.Should().ContainSingle();

            var entry = this.Logger.Entries.Single();
            entry.LogLevel.Should().Be(this.ExpectedLogLevel);
            entry.EventId.Id.Should().Be((int)this.Code);
            entry.EventId.Name.Should().Be(this.Code.ToString());
            entry.Properties["InitializationCode"].Should().Be(this.Code);
            entry.Properties["ApiPath"].Should().Be
            (
                $"{nameof(ApiSchema)}[\"IssueLogging\"]"
            );
            entry.Properties["Description"].Should().Be("Description");

            if (this.Remediation is null)
            {
                entry.Properties.Should().NotContainKey("Remediation");
            }
            else
            {
                entry.Properties["Remediation"].Should().Be(this.Remediation);
            }
        }
        #endregion
    }

    private class TestElement(string apiLabel) : ApiSchemaElement
    {
        #region ApiSchemaElement Properties
        public override ApiSchemaElementKind Kind => ApiSchemaElementKind.Property;

        protected override string ApiElementName => "TestElement";
        #endregion

        #region ApiSchemaElement Methods
        protected override string BuildPath(string? apiPreviousPath)
            => ApiSchemaPathFormatting.BuildPath
            (
                apiPreviousPath,
                this.ApiElementName,
                apiLabel
            );
        #endregion
    }

    private sealed class SessionIsolationTest : XUnitTest
    {
        #region Calculated Properties
        private ApiInitializationSession? FirstSession { get; set; }

        private ApiInitializationSession? SecondSession { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var schema = CreateSchema("SessionIsolation");
            this.FirstSession = new ApiInitializationSession(schema, new RecordingLogger());
            this.SecondSession = new ApiInitializationSession(schema, new RecordingLogger());
        }

        protected override void Act()
        {
            this.FirstSession!.AddIssue
            (
                this.FirstSession.ApiSchema.ApiPath,
                ApiInitializationSeverity.Warning,
                ApiInitializationCode.ApiObjectTypeNullOrEmptyProperties,
                "First-session issue",
                remediation: null
            );
        }

        protected override void Assert()
        {
            this.FirstSession!.Issues.Should().ContainSingle();
            this.SecondSession!.Issues.Should().BeEmpty();
            this.SecondSession.Should().NotBeSameAs(this.FirstSession);
        }
        #endregion
    }

    private sealed class RecordingLogger : ILogger
    {
        #region Properties
        public List<LogEntry> Entries { get; } = [];
        #endregion

        #region ILogger Methods
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>
        (
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            var properties = state is IEnumerable<KeyValuePair<string, object?>> values
                ? values.ToDictionary(static value => value.Key, static value => value.Value)
                : [];

            this.Entries.Add
            (
                new LogEntry(logLevel, eventId, properties, formatter(state, exception))
            );
        }
        #endregion
    }

    private sealed record LogEntry
    (
        LogLevel LogLevel,
        EventId EventId,
        IReadOnlyDictionary<string, object?> Properties,
        string Message
    );
    #endregion

    #region Test Data
    public static TheoryDataRow<IXUnitTest>[] SessionIsolationTheoryData =>
    [
        new SessionIsolationTest
        {
            Name = "Initialization Sessions Isolate Issues"
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] DiagnosticPathTheoryData =>
    [
        new DiagnosticPathTest
        {
            Name = "Key Paths Are Fully Qualified And Indexed",
            TestCase = DiagnosticPathTestCase.KeyPath,
            ExpectedPaths =
            [
                "ApiSchema[\"Key\"].ApiObjectType[\"KeyNestedComposite\"]." +
                    "ApiNamedKeyType[\"PK_KeyNestedComposite\"]." +
                    "ApiKeyPath[0][\"KeyNestedComposite.NestedPart.Id\"]",
                "ApiSchema[\"Key\"].ApiObjectType[\"KeyNestedComposite\"]." +
                    "ApiNamedKeyType[\"PK_KeyNestedComposite\"]." +
                    "ApiKeyPath[0][\"KeyNestedComposite.NestedPart.Id\"]." +
                    "ApiKeyPathSegment[0][\"NestedPart\"]",
                "ApiSchema[\"Key\"].ApiObjectType[\"KeyNestedComposite\"]." +
                    "ApiNamedKeyType[\"PK_KeyNestedComposite\"]." +
                    "ApiKeyPath[0][\"KeyNestedComposite.NestedPart.Id\"]." +
                    "ApiKeyPathSegment[1][\"Id\"]",
                "ApiSchema[\"Key\"].ApiObjectType[\"KeyNestedComposite\"]." +
                    "ApiNamedKeyType[\"PK_KeyNestedComposite\"]." +
                    "ApiKeyPath[1][\"KeyNestedComposite.Name\"]",
            ]
        },
        new DiagnosticPathTest
        {
            Name = "One-To Relationship Paths Use Semantic Roles",
            TestCase = DiagnosticPathTestCase.OneToRelationship,
            ExpectedPaths =
            [
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipOneToMany[\"REL_User_Post_1toN_ViaScalar\"]." +
                    "ApiPrincipalEnd",
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipOneToMany[\"REL_User_Post_1toN_ViaScalar\"]." +
                    "ApiDependentEnd",
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipOneToMany[\"REL_User_Post_1toN_ViaScalar\"]." +
                    "ApiDependentEnd.ApiForeignKeyType",
            ]
        },
        new DiagnosticPathTest
        {
            Name = "Many-To-Many Relationship Paths Use Semantic Roles",
            TestCase = DiagnosticPathTestCase.ManyToManyRelationship,
            ExpectedPaths =
            [
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipManyToMany[\"REL_Post_Tag_NtoN_ViaPostTag\"]." +
                    "ApiPrincipalEndA",
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipManyToMany[\"REL_Post_Tag_NtoN_ViaPostTag\"]." +
                    "ApiPrincipalEndB",
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipManyToMany[\"REL_Post_Tag_NtoN_ViaPostTag\"]." +
                    "ApiAssociation",
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipManyToMany[\"REL_Post_Tag_NtoN_ViaPostTag\"]." +
                    "ApiAssociation.ApiForeignKeyTypeA",
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipManyToMany[\"REL_Post_Tag_NtoN_ViaPostTag\"]." +
                    "ApiAssociation.ApiForeignKeyTypeB",
            ]
        },
        new DiagnosticPathTest
        {
            Name = "Indexed Paths Omit Blank Labels",
            TestCase = DiagnosticPathTestCase.BlankIndexedLabel,
            ExpectedPaths =
            [
                "ApiSchema[\"Location\"].TestElement[\"Root\"].TestElement[2]",
            ]
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] IssueLoggingTheoryData =>
    [
        new IssueLoggingTest
        {
            Name = "Information Issues Are Logged Without Remediation",
            Severity = ApiInitializationSeverity.Info,
            Code = ApiInitializationCode.ApiSchemaInvalidName,
            ExpectedLogLevel = LogLevel.Information,
            Remediation = null
        },
        new IssueLoggingTest
        {
            Name = "Warning Issues Are Logged With Remediation",
            Severity = ApiInitializationSeverity.Warning,
            Code = ApiInitializationCode.ApiObjectTypeNullOrEmptyProperties,
            ExpectedLogLevel = LogLevel.Warning,
            Remediation = "Remediation"
        },
        new IssueLoggingTest
        {
            Name = "Error Issues Are Logged With Remediation",
            Severity = ApiInitializationSeverity.Error,
            Code = ApiInitializationCode.ApiKeyTypeNullOrEmptyPaths,
            ExpectedLogLevel = LogLevel.Error,
            Remediation = "Remediation"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(SessionIsolationTheoryData))]
    public void SessionIsolation(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(DiagnosticPathTheoryData))]
    public void DiagnosticPath(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(IssueLoggingTheoryData))]
    public void IssueLogging(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Implementation Methods
    private static ApiSchema CreateSchema(string apiName)
    {
        var schema = new ApiSchema
        (
            apiName,
            apiVersion: null,
            apiOptions: null,
            apiNamedTypes: [],
            apiRelationships: []
        );

        ApiSchemaCompiler.Compile(schema).ThrowIfInvalid();
        return schema;
    }

    private static string[] GetBlankIndexedLabelPath()
    {
        var schema = CreateSchema("Location");
        var location = ApiInitializationLocation.ForIndexedLabel(2, "   ");
        var child = new TestElement("Child");
        var apiPath = location.BuildPath
        (
            child,
            $"{schema.ApiPath}.TestElement[\"Root\"]"
        );

        return [apiPath];
    }

    private static string[] GetKeyPaths()
    {
        var apiObjectType = ApiSchemaFactory.KeyApiSchema.GetObjectTypeByApiName
        (
            nameof(KeyNestedComposite)
        );
        apiObjectType.TryGetKeyTypeByApiName
        (
            "PK_KeyNestedComposite",
            out var apiKeyType
        ).Should().BeTrue();

        var firstPath = apiKeyType!.ApiKeyPaths[0];
        var secondPath = apiKeyType.ApiKeyPaths[1];
        return
        [
            firstPath.ApiPath,
            firstPath.ApiSegments[0].ApiPath,
            firstPath.ApiSegments[1].ApiPath,
            secondPath.ApiPath,
        ];
    }

    private static string[] GetManyToManyRelationshipPaths()
    {
        ApiSchemaFactory.RelationshipApiSchema.TryGetRelationshipByApiName
        (
            "REL_Post_Tag_NtoN_ViaPostTag",
            out var apiRelationship
        ).Should().BeTrue();

        var manyToMany = apiRelationship.Should().BeOfType<ApiRelationshipManyToMany>().Subject;
        return
        [
            manyToMany.ApiPrincipalEndA.ApiPath,
            manyToMany.ApiPrincipalEndB.ApiPath,
            manyToMany.ApiAssociation.ApiPath,
            manyToMany.ApiAssociation.ApiForeignKeyTypeA.ApiPath,
            manyToMany.ApiAssociation.ApiForeignKeyTypeB.ApiPath,
        ];
    }

    private static string[] GetOneToRelationshipPaths()
    {
        ApiSchemaFactory.RelationshipApiSchema.TryGetRelationshipByApiName
        (
            "REL_User_Post_1toN_ViaScalar",
            out var apiRelationship
        ).Should().BeTrue();

        var oneTo = apiRelationship.Should().BeOfType<ApiRelationshipOneToMany>().Subject;
        return
        [
            oneTo.ApiPrincipalEnd.ApiPath,
            oneTo.ApiDependentEnd.ApiPath,
            oneTo.ApiDependentEnd.ApiForeignKeyType.ApiPath,
        ];
    }
    #endregion
}
