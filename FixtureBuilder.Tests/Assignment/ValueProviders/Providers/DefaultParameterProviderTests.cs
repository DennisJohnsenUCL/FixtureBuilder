#pragma warning disable IDE0060

using System.Reflection;
using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class DefaultParameterProviderTests
    {
        private DefaultParameterProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        private static ParameterInfo GetParameterInfo(string methodName, string paramName)
        {
            return typeof(SampleMethods)
                .GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic)!
                .GetParameters()
                .First(p => p.Name == paramName);
        }

        private static class SampleMethods
        {
            internal static void WithDefault(int value = 42) { }
            internal static void WithoutDefault(int value) { }
            internal static void WithNullDefault(string? value = null) { }
            internal static void WithStringDefault(string value = "hello") { }
        }

        [SetUp]
        public void SetUp()
        {
            _sut = new DefaultParameterProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new DefaultParameterProvider());
        }

        [Test]
        public void Resolve_ParameterWithDefaultValue_OptionEnabled_ReturnsDefaultValue()
        {
            var paramInfo = GetParameterInfo(nameof(SampleMethods.WithDefault), "value");
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo);
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void Resolve_ParameterWithDefaultValue_OptionDisabled_ReturnsNoResult()
        {
            var paramInfo = GetParameterInfo(nameof(SampleMethods.WithDefault), "value");
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo);
            var options = new FixtureOptions { PreferDefaultParameterValues = false };
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_ParameterWithoutDefaultValue_OptionEnabled_ReturnsNoResult()
        {
            var paramInfo = GetParameterInfo(nameof(SampleMethods.WithoutDefault), "value");
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo);
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_ParameterWithNullDefault_OptionEnabled_ReturnsNull()
        {
            var paramInfo = GetParameterInfo(nameof(SampleMethods.WithNullDefault), "value");
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo);
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_ParameterWithStringDefault_OptionEnabled_ReturnsStringDefault()
        {
            var paramInfo = GetParameterInfo(nameof(SampleMethods.WithStringDefault), "value");
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo);
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.EqualTo("hello"));
        }

        [Test]
        public void Resolve_NonParameterSource_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(int));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
