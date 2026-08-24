// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiSchemaBuilderContextTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private class GetOrAddTest : XUnitTest
    {
        #region User Supplied Properties
        public string MethodName { get; init; } = null!;
        public string? GenericMethodName { get; init; }
        public string? SecondMethodName { get; init; }
        public Type? ClrType { get; init; }
        public string? ApiName { get; init; }
        public bool IsGenericFirst { get; init; }
        public Type? ExceptionTypeExpected { get; init; }
        public string? ExceptionMessageExpected { get; init; }
        #endregion

        #region Calculated Properties
        private object? Builder1 { get; set; }
        private object? Builder2 { get; set; }
        private Exception? ExceptionActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"MethodName: {this.MethodName.SafeToString()}");
            this.WriteLine($"GenericMethodName: {this.GenericMethodName.SafeToString()}");
            this.WriteLine($"SecondMethodName: {this.SecondMethodName.SafeToString()}");
            this.WriteLine($"ClrType: {this.ClrType.SafeToName()}");
            this.WriteLine($"ApiName: {this.ApiName.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext();
            try
            {
                if (this.SecondMethodName is not null)
                {
                    this.InvokeRelationshipMethod(context, this.MethodName);
                    this.InvokeRelationshipMethod(context, this.SecondMethodName);
                }
                else if (this.GenericMethodName is null)
                {
                    this.Builder1 = this.InvokeBuilderMethod(context, this.MethodName, isGeneric: false);
                    this.Builder2 = this.InvokeBuilderMethod(context, this.MethodName, isGeneric: false);
                }
                else if (this.IsGenericFirst)
                {
                    this.Builder1 = this.InvokeBuilderMethod(context, this.GenericMethodName, isGeneric: true);
                    this.Builder2 = this.InvokeBuilderMethod(context, this.MethodName, isGeneric: false);
                }
                else
                {
                    this.Builder1 = this.InvokeBuilderMethod(context, this.MethodName, isGeneric: false);
                    this.Builder2 = this.InvokeBuilderMethod(context, this.GenericMethodName, isGeneric: true);
                }
            }
            catch (System.Reflection.TargetInvocationException exception) when (exception.InnerException is not null)
            {
                this.ExceptionActual = exception.InnerException;
            }
            catch (Exception exception)
            {
                this.ExceptionActual = exception;
            }
        }

        protected override void Assert()
        {
            if (this.ExceptionTypeExpected is not null)
            {
                this.ExceptionActual.Should().NotBeNull();
                this.ExceptionActual.Should().BeOfType(this.ExceptionTypeExpected);

                if (this.ExceptionMessageExpected is not null)
                {
                    this.ExceptionActual!.Message.Should().Be(this.ExceptionMessageExpected);
                }

                return;
            }

            this.Builder1.Should().NotBeNull();
            this.Builder2.Should().NotBeNull();

            ReferenceEquals(this.Builder1, this.Builder2).Should().BeTrue();
        }

        private object? InvokeBuilderMethod(ApiSchemaBuilderContext context, string methodName, bool isGeneric)
        {
            var method = typeof(ApiSchemaBuilderContext).GetMethod
            (
                methodName,
                System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic,
                isGeneric ? Type.EmptyTypes : [typeof(Type)]
            );

            if (isGeneric)
            {
                return method!.MakeGenericMethod(this.ClrType!).Invoke(context, []);
            }

            return method!.Invoke(context, [this.ClrType]);
        }

        private object? InvokeRelationshipMethod(ApiSchemaBuilderContext context, string methodName)
        {
            var method = typeof(ApiSchemaBuilderContext).GetMethod
            (
                methodName,
                System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic,
                [typeof(string)]
            );

            return method!.Invoke(context, [this.ApiName]);
        }
        #endregion
    }
    #endregion

    public static TheoryDataRow<IXUnitTest>[] GetOrAddTheoryData =>
    [
        new GetOrAddTest
        {
            Name = "GetOrAddScalarTypeBuilder returns same instance for same CLR type",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddScalarTypeBuilder),
            ClrType = typeof(int)
        },
        new GetOrAddTest
        {
            Name = "GetOrAddEnumTypeBuilder returns same instance for same CLR type",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddEnumTypeBuilder),
            ClrType = typeof(OrderStatus)
        },
        new GetOrAddTest
        {
            Name = "GetOrAddObjectTypeBuilder returns same instance for same CLR type",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddObjectTypeBuilder),
            ClrType = typeof(Order)
        },
        new GetOrAddTest
        {
            Name = "GetOrAddObjectTypeBuilder generic and non-generic return same builder",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddObjectTypeBuilder),
            GenericMethodName = nameof(ApiSchemaBuilderContext.GetOrAddObjectTypeBuilder),
            ClrType = typeof(Order)
        },
        new GetOrAddTest
        {
            Name = "GetOrAddScalarTypeBuilder generic and non-generic return same builder",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddScalarTypeBuilder),
            GenericMethodName = nameof(ApiSchemaBuilderContext.GetOrAddScalarTypeBuilder),
            ClrType = typeof(int)
        },
        new GetOrAddTest
        {
            Name = "GetOrAddEnumTypeBuilder generic and non-generic return same builder",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddEnumTypeBuilder),
            GenericMethodName = nameof(ApiSchemaBuilderContext.GetOrAddEnumTypeBuilder),
            ClrType = typeof(OrderStatus)
        },
        new GetOrAddTest
        {
            Name = "GetOrAddObjectTypeBuilder non-generic returns existing generic builder",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddObjectTypeBuilder),
            GenericMethodName = nameof(ApiSchemaBuilderContext.GetOrAddObjectTypeBuilder),
            ClrType = typeof(Order),
            IsGenericFirst = true
        },
        new GetOrAddTest
        {
            Name = "GetOrAddScalarTypeBuilder non-generic returns existing generic builder",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddScalarTypeBuilder),
            GenericMethodName = nameof(ApiSchemaBuilderContext.GetOrAddScalarTypeBuilder),
            ClrType = typeof(int),
            IsGenericFirst = true
        },
        new GetOrAddTest
        {
            Name = "GetOrAddEnumTypeBuilder non-generic returns existing generic builder",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddEnumTypeBuilder),
            GenericMethodName = nameof(ApiSchemaBuilderContext.GetOrAddEnumTypeBuilder),
            ClrType = typeof(OrderStatus),
            IsGenericFirst = true
        },
        new GetOrAddTest
        {
            Name = "GetOrAddEnumTypeBuilder throws configuration exception for invalid CLR type",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddEnumTypeBuilder),
            ClrType = typeof(int),
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException),
            ExceptionMessageExpected = "Unable to create ApiEnumTypeBuilder`1 for CLR type 'Int32'."
        },
        new GetOrAddTest
        {
            Name = "GetOrAddTypedRelationshipBuilder throws configuration exception when different kind exists",
            MethodName = nameof(ApiSchemaBuilderContext.GetOrAddOneToOneRelationshipBuilder),
            SecondMethodName = nameof(ApiSchemaBuilderContext.GetOrAddOneToManyRelationshipBuilder),
            ApiName = "REL_Test",
            ExceptionTypeExpected = typeof(ApiSchemaConfigurationException)
        }
    ];

    [Theory]
    [MemberData(nameof(GetOrAddTheoryData))]
    public void GetOrAdd(IXUnitTest test) => test.Execute(this);
}
