/*
 * Thanks to György Kőszeg for the implementation for this class.
 * https://stackoverflow.com/a/43339950
 * License: CC BY-SA 4.0 Deed
 *          https://creativecommons.org/licenses/by-sa/4.0/
 */

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal;

namespace NUnit.Framework;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseGenericAttribute : TestCaseAttribute, ITestBuilder
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public TestCaseGenericAttribute(params object[] arguments)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base(arguments)
    {
    }

    public Type[] TypeArguments { get; set; }

#pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
    IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test suite)
#pragma warning restore CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
    {
        if (!method.IsGenericMethodDefinition)
            return base.BuildFrom(method, suite);

        if (TypeArguments == null || TypeArguments.Length != method.GetGenericArguments().Length)
        {
            var @params = new TestCaseParameters { RunState = RunState.NotRunnable };
            @params.Properties.Set(PropertyNames.SkipReason, $"{nameof(TypeArguments)} should have {method.GetGenericArguments().Length} elements");
            return new[] { new NUnitTestCaseBuilder().BuildTestMethod(method, suite, @params) };
        }

        var genMethod = method.MakeGenericMethod(TypeArguments);
        return base.BuildFrom(genMethod, suite);
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute<T> : TestCaseGenericAttribute
{
    public TestCaseAttribute(params object[] arguments)
        : base(arguments) => TypeArguments = new[] { typeof(T) };
}

// For exactly two type arguments. See the base implementation above.
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute<T1, T2> : TestCaseGenericAttribute
{
    public TestCaseAttribute(params object[] arguments)
        : base(arguments) => TypeArguments = new[] { typeof(T1), typeof(T2) };
}