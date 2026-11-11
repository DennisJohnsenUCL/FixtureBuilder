using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class CharProviderTests
    {
        [Test]
        public void ResolveValue_Char_ReturnsAsciiLetter()
        {
            var provider = new CharProvider();
            var request = TestHelper.MakeRequest(typeof(char));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<char>());
                Assert.That(result, Is.InRange('A', 'Z').Or.InRange('a', 'z'));
            }
        }

        [TestCase(typeof(char?))]
        [TestCase(typeof(string))]
        [TestCase(typeof(int))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new CharProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
