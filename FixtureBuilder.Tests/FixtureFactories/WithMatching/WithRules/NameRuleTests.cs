using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching.WithRules
{
    internal sealed class NameRuleTests
    {
        [Test]
        public void Constructor_NullName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new NameRule(null!));
        }

        [Test]
        public void IsMatch_SameName_ReturnsTrue()
        {
            var sut = new NameRule("FirstName");
            var request = new FixtureRequest(typeof(string), this, "FirstName");

            Assert.That(sut.IsMatch(request), Is.True);
        }

        [Test]
        public void IsMatch_DifferentName_ReturnsFalse()
        {
            var sut = new NameRule("FirstName");
            var request = new FixtureRequest(typeof(string), this, "LastName");

            Assert.That(sut.IsMatch(request), Is.False);
        }

        [Test]
        public void IsMatch_NoName_ReturnsFalse()
        {
            var sut = new NameRule("FirstName");
            var request = new FixtureRequest(typeof(string));

            Assert.That(sut.IsMatch(request), Is.False);
        }

        [Test]
        public void IsMatch_CaseSensitive()
        {
            var sut = new NameRule("FirstName");
            var request = new FixtureRequest(typeof(string), this, "firstname");

            Assert.That(sut.IsMatch(request), Is.False);
        }
    }
}
