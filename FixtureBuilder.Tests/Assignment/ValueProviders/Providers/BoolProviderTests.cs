using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class BoolProviderTests
    {
        [Test]
        public void ResolveValue_Bool_ReturnsTrue()
        {
            var provider = new BoolProvider();
            var request = TestHelper.MakeRequest(typeof(bool));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.True);
        }

        [TestCase(typeof(bool?))]
        [TestCase(typeof(string))]
        [TestCase(typeof(int))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new BoolProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
