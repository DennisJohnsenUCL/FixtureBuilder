using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class GuidProviderTests
    {
        [Test]
        public void ResolveValue_Guid_ReturnsNonEmptyGuid()
        {
            var provider = new GuidProvider();
            var request = TestHelper.MakeRequest(typeof(Guid));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<Guid>());
                Assert.That(result, Is.Not.EqualTo(Guid.Empty));
            }
        }

        [TestCase(typeof(Guid?))]
        [TestCase(typeof(string))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new GuidProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
