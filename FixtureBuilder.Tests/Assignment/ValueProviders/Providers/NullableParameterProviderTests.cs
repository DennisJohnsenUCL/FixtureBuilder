using System.Diagnostics.CodeAnalysis;
using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class NullableParameterProviderTests
    {
        [Test]
        public void ResolveValue_NullableReferenceTypeParameter_ReturnsNull()
        {
            static void Method(string? value) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new NullableParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveValue_NullableValueTypeParameter_ReturnsNull()
        {
            static void Method(int? value) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new NullableParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveValue_AllowNullParameter_ReturnsNull()
        {
            static void Method([AllowNull] string value) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new NullableParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveValue_NullsNotPreferred_ReturnsNoResult()
        {
            static void Method(string? value) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var options = new FixtureOptions { PreferNullParameterValues = false };
            var provider = new NullableParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void ResolveValue_NonNullableReferenceTypeParameter_ReturnsNoResult()
        {
            static void Method(string value) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new NullableParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void ResolveValue_NonNullableValueTypeParameter_ReturnsNoResult()
        {
            static void Method(int value) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new NullableParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void ResolveValue_SourceIsNotParameter_ReturnsNoResult()
        {
            var request = TestHelper.MakeRequest(typeof(string));
            var provider = new NullableParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
