using NUnit.Framework;

namespace LimestoneDigital.QA.Core.Tas.Matchers;

/// <summary>
/// Reusable assertion primitives. Business-layer AssertionSteps compose these instead of
/// each test writing its own Assert.That block, so the failure message wording and the
/// assertion logic stay in one place.
/// </summary>
public static class Matchers
{
    public static void ShouldNotBeNullOrEmpty(this string? value, string fieldName)
    {
        Assert.That(value, Is.Not.Null.And.Not.Empty, $"'{fieldName}' should not be null or empty");
    }

    public static void ShouldBeGreaterThan(this int value, int threshold, string fieldName)
    {
        Assert.That(value, Is.GreaterThan(threshold), $"'{fieldName}' should be greater than {threshold}");
    }

    public static void ShouldBe<T>(this T actual, T expected, string fieldName)
    {
        Assert.That(actual, Is.EqualTo(expected), $"'{fieldName}' should be '{expected}' but was '{actual}'");
    }

    public static void ShouldContain(this IReadOnlyCollection<string> collection, string expected, string collectionName)
    {
        Assert.That(collection, Does.Contain(expected), $"'{collectionName}' should contain '{expected}'");
    }
}
