// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Types.Internal;

public class ApiTypeReferenceBindingTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private enum GuardCase
    {
        BindNullTarget,
        BindTwice,
        BindWithReference,
        ResolveWithoutReference,
        ResolveTwice,
    }

    private enum ResolutionCase
    {
        ApiNamed,
        ClrType,
        WrongType,
        Unresolved,
        InvalidForm,
    }

    private sealed class BindOwnerSuppliedTypeTest : XUnitTest
    {
        #region Calculated Properties
        private ApiTypeReferenceBinding<ApiObjectType>? Binding { get; set; }

        private ApiObjectType? ExpectedApiType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ExpectedApiType = ApiSchemaFactory.SimpleApiSchema.GetObjectTypeByApiName
                (nameof(Person));
            this.Binding = new(apiTypeReference: null);
        }

        protected override void Act()
        {
            this.Binding!.Bind(this.ExpectedApiType!);
        }

        protected override void Assert()
        {
            this.Binding.Should().NotBeNull();
            this.Binding.HasReference.Should().BeFalse();
            this.Binding.ApiTypeReference.Should().BeNull();
            this.Binding.IsResolved.Should().BeTrue();
            this.Binding.ApiResolvedType.Should().BeSameAs(this.ExpectedApiType);
            this.Binding.ApiType.Should().BeSameAs(this.ExpectedApiType);
        }
        #endregion
    }

    private sealed class GuardBindingTest : XUnitTest
    {
        #region User Supplied Properties
        public required GuardCase GuardCase { get; init; }

        public required Type ExpectedExceptionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiTypeReferenceBinding<ApiObjectType>? Binding { get; set; }

        private ApiSchemaCompilationContext? Context { get; set; }

        private Exception? Exception { get; set; }

        private ApiObjectType? ObjectType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var schema = ApiSchemaFactory.SimpleApiSchema;
            this.ObjectType = schema.GetObjectTypeByApiName(nameof(Person));
            this.Context = CreateContext(schema);

            this.Binding = this.GuardCase switch
            {
                GuardCase.BindWithReference or GuardCase.ResolveTwice => new
                (
                    new ApiTypeReference(typeof(Person))
                ),
                _ => new(apiTypeReference: null),
            };

            switch (this.GuardCase)
            {
                case GuardCase.BindTwice:
                    this.Binding.Bind(this.ObjectType);
                    break;

                case GuardCase.ResolveTwice:
                    this.Binding.Resolve
                    (
                        this.Context,
                        ApiSchemaCompilationCode.ApiKeyPathUnresolvedRootType,
                        nameof(ApiKeyPath.ApiRootTypeReference),
                        nameof(ApiKeyPath.ApiRootObjectType)
                    ).Should().BeTrue();
                    break;
            }
        }

        protected override void Act()
        {
            try
            {
                switch (this.GuardCase)
                {
                    case GuardCase.BindNullTarget:
                        this.Binding!.Bind(null!);
                        break;

                    case GuardCase.BindTwice:
                    case GuardCase.BindWithReference:
                        this.Binding!.Bind(this.ObjectType!);
                        break;

                    case GuardCase.ResolveWithoutReference:
                    case GuardCase.ResolveTwice:
                        this.Binding!.Resolve
                        (
                            this.Context!,
                            ApiSchemaCompilationCode.ApiKeyPathUnresolvedRootType,
                            nameof(ApiKeyPath.ApiRootTypeReference),
                            nameof(ApiKeyPath.ApiRootObjectType)
                        );
                        break;

                    default:
                        throw new InvalidOperationException
                        (
                            $"Unsupported {nameof(this.GuardCase)} value '{this.GuardCase}'."
                        );
                }
            }
            catch (Exception exception)
            {
                this.Exception = exception;
            }
        }

        protected override void Assert()
        {
            this.Exception.Should().BeOfType(this.ExpectedExceptionType);
        }
        #endregion
    }

    private sealed class ResolveReferenceTest : XUnitTest
    {
        #region User Supplied Properties
        public required ResolutionCase ResolutionCase { get; init; }

        public ApiSchemaCompilationCode? ExpectedIssueCode { get; init; }
        #endregion

        #region Calculated Properties
        private ApiTypeReferenceBinding<ApiObjectType>? Binding { get; set; }

        private ApiSchemaCompilationContext? Context { get; set; }

        private ApiObjectType? ExpectedApiType { get; set; }

        private bool IsResolved { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var schema = ApiSchemaFactory.SimpleApiSchema;
            this.ExpectedApiType = schema.GetObjectTypeByApiName(nameof(Person));
            this.Context = CreateContext(schema);

            var apiTypeReference = this.ResolutionCase switch
            {
                ResolutionCase.ApiNamed => new ApiTypeReference
                    (ApiTypeKind.Object, nameof(Person)),
                ResolutionCase.ClrType => new ApiTypeReference(typeof(Person)),
                ResolutionCase.WrongType => new ApiTypeReference
                    (ApiTypeKind.Scalar, nameof(String)),
                ResolutionCase.Unresolved => new ApiTypeReference
                    (ApiTypeKind.Object, "Missing"),
                ResolutionCase.InvalidForm => new ApiTypeReference
                    (ApiTypeKind.Object, apiName: null, clrType: null, hasInvalidApiKind: false),
                _ => throw new InvalidOperationException
                (
                    $"Unsupported {nameof(this.ResolutionCase)} value '{this.ResolutionCase}'."
                ),
            };
            this.Binding = new(apiTypeReference);
        }

        protected override void Act()
        {
            this.IsResolved = this.Binding!.Resolve
            (
                this.Context!,
                ApiSchemaCompilationCode.ApiKeyPathUnresolvedRootType,
                nameof(ApiKeyPath.ApiRootTypeReference),
                nameof(ApiKeyPath.ApiRootObjectType)
            );
        }

        protected override void Assert()
        {
            this.Binding.Should().NotBeNull();
            this.Context.Should().NotBeNull();
            this.Binding!.HasReference.Should().BeTrue();

            if (this.ExpectedIssueCode is null)
            {
                this.IsResolved.Should().BeTrue();
                this.Binding.IsResolved.Should().BeTrue();
                this.Binding.ApiResolvedType.Should().BeSameAs(this.ExpectedApiType);
                this.Binding.ApiType.Should().BeSameAs(this.ExpectedApiType);
                this.Context!.Issues.Should().BeEmpty();
                return;
            }

            this.IsResolved.Should().BeFalse();
            this.Binding.IsResolved.Should().BeFalse();
            this.Binding.ApiResolvedType.Should().BeNull();
            this.Context!.Issues.Should().ContainSingle().Which.Code.Should()
                .Be(this.ExpectedIssueCode);

            if (this.ResolutionCase == ResolutionCase.WrongType)
            {
                this.Context.Issues.Single().Description.Should().Be($"{nameof(ApiKeyPath.ApiRootTypeReference)} resolved to {nameof(ApiScalarType)}, not {nameof(ApiObjectType)}");
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BindOwnerSuppliedTypeTheoryData =>
    [
        new BindOwnerSuppliedTypeTest
        {
            Name = "Binds an owner-supplied API object type without a reference",
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] GuardBindingTheoryData =>
    [
        new GuardBindingTest
        {
            Name = "Rejects a null directly supplied API type",
            GuardCase = GuardCase.BindNullTarget,
            ExpectedExceptionType = typeof(ArgumentNullException),
        },
        new GuardBindingTest
        {
            Name = "Rejects rebinding a directly supplied API type",
            GuardCase = GuardCase.BindTwice,
            ExpectedExceptionType = typeof(InvalidOperationException),
        },
        new GuardBindingTest
        {
            Name = "Rejects direct binding when a reference is configured",
            GuardCase = GuardCase.BindWithReference,
            ExpectedExceptionType = typeof(InvalidOperationException),
        },
        new GuardBindingTest
        {
            Name = "Rejects resolution without a configured reference",
            GuardCase = GuardCase.ResolveWithoutReference,
            ExpectedExceptionType = typeof(InvalidOperationException),
        },
        new GuardBindingTest
        {
            Name = "Rejects resolving a reference twice",
            GuardCase = GuardCase.ResolveTwice,
            ExpectedExceptionType = typeof(InvalidOperationException),
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] ResolveReferenceTheoryData =>
    [
        new ResolveReferenceTest
        {
            Name = "Resolves an API-named object type reference",
            ResolutionCase = ResolutionCase.ApiNamed,
        },
        new ResolveReferenceTest
        {
            Name = "Resolves a CLR object type reference",
            ResolutionCase = ResolutionCase.ClrType,
        },
        new ResolveReferenceTest
        {
            Name = "Rejects a reference that resolves to the wrong API type",
            ResolutionCase = ResolutionCase.WrongType,
            ExpectedIssueCode = ApiSchemaCompilationCode.ApiKeyPathUnresolvedRootType,
        },
        new ResolveReferenceTest
        {
            Name = "Reports an unresolved API type reference",
            ResolutionCase = ResolutionCase.Unresolved,
            ExpectedIssueCode = ApiSchemaCompilationCode.ApiKeyPathUnresolvedRootType,
        },
        new ResolveReferenceTest
        {
            Name = "Reports an invalid API type reference form",
            ResolutionCase = ResolutionCase.InvalidForm,
            ExpectedIssueCode = ApiSchemaCompilationCode.ApiTypeReferenceInvalidForm,
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BindOwnerSuppliedTypeTheoryData))]
    public void BindOwnerSuppliedType(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(GuardBindingTheoryData))]
    public void GuardBinding(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ResolveReferenceTheoryData))]
    public void ResolveReference(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Implementation Methods
    private static ApiSchemaCompilationContext CreateContext(ApiSchema schema)
    {
        var session = new ApiSchemaCompilationSession(schema, new ApiSchemaContext(schema));
        return session.CreateContext(schema, location: default);
    }
    #endregion
}
