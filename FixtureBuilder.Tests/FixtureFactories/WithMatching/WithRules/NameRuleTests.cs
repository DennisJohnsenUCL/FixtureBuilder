using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching.WithRules
{
    internal sealed class NameRuleTests
    {
        [Test]
        public void IsMatch_RequestSourceWithoutName_ReturnsFalse()
        {
            var parameter = TestHelper.UnnamedParameter(typeof(string));
            var request = new FixtureRequest(typeof(string), parameter, typeof(StringConstructorClass));
            var rule = new NameRule("value");

            var result = rule.IsMatch(request);

            Assert.That(result, Is.False);
        }
    }
}
