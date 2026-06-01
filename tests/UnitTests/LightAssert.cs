#nullable disable
namespace UnitTests
{
    internal static class LightAssert
    {
        internal static void ShouldBe<T>(this T actual, T expected) =>
            Assert.That(actual, Is.EqualTo(expected));

        internal static void ShouldBeTrue(this bool actual) =>
            Assert.That(actual, Is.True);

        internal static void ShouldBeFalse(this bool actual) =>
            Assert.That(actual, Is.False);

        internal static void ShouldBeNull<T>(this T actual) where T : class =>
            Assert.That(actual, Is.Null);

        internal static void ShouldNotBeNull<T>(this T actual) where T : class =>
            Assert.That(actual, Is.Not.Null);

        internal static void ShouldNotBeNullOrEmpty(this string actual) =>
            Assert.That(actual, Is.Not.Null.And.Not.Empty);

        internal static TException ShouldThrow<TException>(Action action) where TException : Exception
        {
            var ex = Assert.Throws<TException>(() => action());
            Assert.That(ex, Is.Not.Null);
            return ex;
        }
    }
}
