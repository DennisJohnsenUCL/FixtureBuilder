#pragma warning disable IDE0060

using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories.WithMatching;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching
{
    internal sealed class MatchingProviderBuilderTests
    {
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IFixtureContext>();
        }

        private static class SampleMethods
        {
            internal static void Method(string value) { }
        }

        [Test]
        public void With_MatchingType_ReturnsValue()
        {
            var builder = new MatchingProviderBuilder();
            var provider = builder.With("hello");
            var request = new FixtureRequest(typeof(string));

            var result = provider.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.EqualTo("hello"));
        }

        [Test]
        public void With_NonMatchingType_ReturnsNoResult()
        {
            var builder = new MatchingProviderBuilder();
            var provider = builder.With("hello");
            var request = new FixtureRequest(typeof(int));

            var result = provider.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void WithParameter_MatchingTypeAndName_ReturnsValue()
        {
            var paramInfo = typeof(SampleMethods)
                .GetMethod(nameof(SampleMethods.Method), BindingFlags.Static | BindingFlags.NonPublic)!
                .GetParameters().First();

            var builder = new MatchingProviderBuilder();
            var provider = builder.WithParameter("world", "value");
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo, "value");

            var result = provider.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.EqualTo("world"));
        }

        [Test]
        public void WithParameter_MatchingTypeWrongName_ReturnsNoResult()
        {
            var paramInfo = typeof(SampleMethods)
                .GetMethod(nameof(SampleMethods.Method), BindingFlags.Static | BindingFlags.NonPublic)!
                .GetParameters().First();

            var builder = new MatchingProviderBuilder();
            var provider = builder.WithParameter("world", "other");
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo, "value");

            var result = provider.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void WithNamed_MatchingName_ReturnsValue()
        {
            var builder = new MatchingProviderBuilder();
            var provider = builder.WithNamed("Age", 42);
            var request = new FixtureRequest(typeof(int), this, "Age");

            var result = provider.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.EqualTo(42));
        }
    }
}
