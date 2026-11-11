using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class TimeSpanProviderTests
    {
        [Test]
        public void ResolveValue_TimeSpan_ReturnsThirtyMinutes()
        {
            var provider = new TimeSpanProvider();
            var request = TestHelper.MakeRequest(typeof(TimeSpan));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(TimeSpan.FromMinutes(30)));
        }

        [TestCase(typeof(TimeSpan?))]
        [TestCase(typeof(DateTime))]
        [TestCase(typeof(int))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new TimeSpanProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
