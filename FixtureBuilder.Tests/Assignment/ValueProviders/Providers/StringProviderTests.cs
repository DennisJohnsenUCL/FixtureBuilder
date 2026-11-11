using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class StringProviderTests
    {
        [Test]
        public void ResolveValue_NamedSource_ReturnsSourceName()
        {
            var provider = new StringProvider();
            var request = TestHelper.MakeRequest(typeof(string));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo("Value"));
        }

        [Test]
        public void ResolveValue_UnnamedSource_ReturnsEmptyString()
        {
            var provider = new StringProvider();
            var request = new FixtureRequest(typeof(string), TestHelper.UnnamedParameter(typeof(string)), typeof(object));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(""));
        }

        [TestCase(typeof(int))]
        [TestCase(typeof(char))]
        [TestCase(typeof(object))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new StringProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
