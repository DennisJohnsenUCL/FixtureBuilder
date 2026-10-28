using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching.WithRules
{
    internal sealed class RootTypeRuleTests
    {
        private class SampleClass
        {
            public int Value { get; set; }
        }

        private static PropertyInfo ValueProperty() =>
            typeof(SampleClass).GetProperty(nameof(SampleClass.Value))!;

        [Test]
        public void Constructor_NullType_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new RootTypeRule(null!));
        }

        [Test]
        public void IsMatch_MatchingRootType_ReturnsTrue()
        {
            var rule = new RootTypeRule(typeof(string));
            var request = new FixtureRequest(typeof(int), ValueProperty(), typeof(string));

            Assert.That(rule.IsMatch(request), Is.True);
        }

        [Test]
        public void IsMatch_NonMatchingRootType_ReturnsFalse()
        {
            var rule = new RootTypeRule(typeof(string));
            var request = new FixtureRequest(typeof(int), ValueProperty(), typeof(double));

            Assert.That(rule.IsMatch(request), Is.False);
        }
    }
}
