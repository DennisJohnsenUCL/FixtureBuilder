using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching.WithRules
{
    internal sealed class TypeRuleTests
    {
        private class SampleClass
        {
            public int? NullableInt { get; set; }
        }

        [Test]
        public void Constructor_NullType_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TypeRule(null!));
        }

        [Test]
        public void IsMatch_SameType_ReturnsTrue()
        {
            var sut = new TypeRule(typeof(string));
            var request = new FixtureRequest(typeof(string));

            Assert.That(sut.IsMatch(request), Is.True);
        }

        [Test]
        public void IsMatch_DifferentType_ReturnsFalse()
        {
            var sut = new TypeRule(typeof(string));
            var request = new FixtureRequest(typeof(int));

            Assert.That(sut.IsMatch(request), Is.False);
        }

        [Test]
        public void IsMatch_OnlyOriginalTypeMatches_ReturnsTrue()
        {
            var sut = new TypeRule(typeof(int?));
            var property = typeof(SampleClass).GetProperty(nameof(SampleClass.NullableInt))!;
            // UnwrapAndLink turns int? into int before the request is created.
            var request = new FixtureRequest(typeof(int), property, typeof(SampleClass));

            Assert.That(sut.IsMatch(request), Is.True);
        }

        [Test]
        public void IsMatch_DerivedType_ReturnsFalse()
        {
            var sut = new TypeRule(typeof(object));
            var request = new FixtureRequest(typeof(string));

            Assert.That(sut.IsMatch(request), Is.False);
        }
    }
}
