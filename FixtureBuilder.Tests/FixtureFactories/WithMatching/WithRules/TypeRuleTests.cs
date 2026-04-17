using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching.WithRules
{
    internal sealed class TypeRuleTests
    {
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
        public void IsMatch_DerivedType_ReturnsFalse()
        {
            var sut = new TypeRule(typeof(object));
            var request = new FixtureRequest(typeof(string));

            Assert.That(sut.IsMatch(request), Is.False);
        }
    }
}
