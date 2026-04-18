using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories.WithMatching;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching
{
    internal sealed class RootProviderBuilderTests
    {
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void With_ReturnsBuilderForChaining()
        {
            var builder = new RootProviderBuilder<string>();

            var result = builder.With(42);

            Assert.That(result, Is.SameAs(builder));
        }

        [Test]
        public void With_AddsProviderToList()
        {
            var builder = new RootProviderBuilder<string>();

            builder.With(42);

            Assert.That(builder.Providers, Has.Count.EqualTo(1));
        }

        [Test]
        public void With_MultipleCallsAddsMultipleProviders()
        {
            var builder = new RootProviderBuilder<string>();

            builder.With(42).With("hello");

            Assert.That(builder.Providers, Has.Count.EqualTo(2));
        }

        [Test]
        public void With_ProviderIncludesRootTypeRule_MatchingRootType_ReturnsValue()
        {
            var builder = new RootProviderBuilder<string>();
            builder.With(42);

            var request = new FixtureRequest(typeof(int), "source", typeof(string), null);
            var result = builder.Providers[0].ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void With_ProviderIncludesRootTypeRule_NonMatchingRootType_ReturnsNoResult()
        {
            var builder = new RootProviderBuilder<string>();
            builder.With(42);

            var request = new FixtureRequest(typeof(int), "source", typeof(double), null);
            var result = builder.Providers[0].ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
