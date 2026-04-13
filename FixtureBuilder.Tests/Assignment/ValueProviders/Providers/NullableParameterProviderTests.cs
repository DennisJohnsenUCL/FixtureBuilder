#pragma warning disable IDE0060

using System.Reflection;
using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class NullableParameterProviderTests
    {
        private NullableParameterProvider _sut;
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
            internal static void NullableString(string? value) { }
            internal static void NonNullableString(string value) { }
            internal static void NullableInt(int? value) { }
            internal static void NonNullableInt(int value) { }
        }

        [SetUp]
        public void SetUp()
        {
            _sut = new NullableParameterProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new NullableParameterProvider());
        }

        [Test]
        public void Resolve_NullableReferenceParameter_OptionEnabled_ReturnsNull()
        {
            var paramInfo = GetParameterInfo(nameof(SampleMethods.NullableString), "value");
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo);
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_NullableValueTypeParameter_OptionEnabled_ReturnsNull()
        {
            var paramInfo = GetParameterInfo(nameof(SampleMethods.NullableInt), "value");
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo);
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_NullableParameter_OptionDisabled_ReturnsNoResult()
        {
            var paramInfo = GetParameterInfo(nameof(SampleMethods.NullableString), "value");
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo);
            var options = new FixtureOptions { PreferNullParameterValues = false };
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_NonNullableParameter_OptionEnabled_ReturnsNoResult()
        {
            var paramInfo = GetParameterInfo(nameof(SampleMethods.NonNullableString), "value");
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo);
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_NonParameterSource_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(string));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_NonNullableValueType_OptionEnabled_ReturnsNoResult()
        {
            var paramInfo = GetParameterInfo(nameof(SampleMethods.NonNullableInt), "value");
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo);
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
