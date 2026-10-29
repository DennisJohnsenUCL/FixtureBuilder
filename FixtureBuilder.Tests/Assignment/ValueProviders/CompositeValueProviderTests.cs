#pragma warning disable IDE0060

using System.Reflection;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders
{
    internal class CompositeValueProviderTests
    {
        private Mock<IFixtureContext> _contextMock;
        private FixtureRequest _request;

        private Mock<IValueProvider> CreateProvider(object? returnValue)
        {
            var mock = new Mock<IValueProvider>();
            mock.Setup(p => p.ResolveValue(_request, _contextMock.Object)).Returns(returnValue);
            return mock;
        }

        private static ParameterInfo GetParameterInfo(string methodName)
        {
            return typeof(SampleMethods)
                .GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic)!
                .GetParameters()[0];
        }

        private static class SampleMethods
        {
            internal static void NullableInt(int? value) { }
            internal static void InInt(in int value) { }
        }

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IFixtureContext>();
            _request = new FixtureRequest(typeof(string));
        }

        [Test]
        public void Constructor_NullProviders_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CompositeValueProvider(null!));
        }

        [Test]
        public void Resolve_WhenFirstProviderReturnsNonNull_ReturnsItsResult()
        {
            var expected = "from-first";
            var first = CreateProvider(expected);
            var second = CreateProvider("from-second");
            var sut = new CompositeValueProvider([first.Object, second.Object]);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
            second.Verify(p => p.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void Resolve_WhenFirstReturnsNoResult_TriesSecondProvider()
        {
            var expected = "from-second";
            var first = CreateProvider(new NoResult());
            var second = CreateProvider(expected);
            var sut = new CompositeValueProvider([first.Object, second.Object]);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void Resolve_WhenAllProvidersReturnNull_ReturnsNull()
        {
            var first = CreateProvider(null);
            var second = CreateProvider(null);
            var sut = new CompositeValueProvider([first.Object, second.Object]);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_WhenNoProviders_ReturnsNoResult()
        {
            var sut = new CompositeValueProvider([]);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_PassesCorrectArgumentsToProviders()
        {
            var provider = new Mock<IValueProvider>();
            provider.Setup(p => p.ResolveValue(_request, _contextMock.Object)).Returns("result");
            var sut = new CompositeValueProvider([provider.Object]);

            sut.ResolveValue(_request, _contextMock.Object);

            provider.Verify(p => p.ResolveValue(_request, _contextMock.Object), Times.Once);
        }

        [Test]
        public void Resolve_WhenProviderReturnsNullForNonNullableValueType_ThrowsInvalidOperationException()
        {
            var request = new FixtureRequest(typeof(int));
            var provider = new Mock<IValueProvider>();
            provider.Setup(p => p.ResolveValue(request, _contextMock.Object)).Returns(null!);
            var sut = new CompositeValueProvider([provider.Object]);

            Assert.Throws<InvalidOperationException>(
                () => sut.ResolveValue(request, _contextMock.Object));
        }

        [Test]
        public void Resolve_WhenProviderReturnsNullForNullableValueType_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(int?));
            var provider = new Mock<IValueProvider>();
            provider.Setup(p => p.ResolveValue(request, _contextMock.Object)).Returns(null!);
            var sut = new CompositeValueProvider([provider.Object]);

            var result = sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_WhenProviderReturnsNullForUnwrappedNullableParameter_ReturnsNull()
        {
            var paramInfo = GetParameterInfo(nameof(SampleMethods.NullableInt));
            // UnwrapAndLink turns int? into int before the request is created.
            var request = new FixtureRequest(typeof(int), paramInfo, typeof(object));
            var provider = new Mock<IValueProvider>();
            provider.Setup(p => p.ResolveValue(request, _contextMock.Object)).Returns(null!);
            var sut = new CompositeValueProvider([provider.Object]);

            var result = sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_WhenProviderReturnsNullForByRefNonNullableParameter_ThrowsInvalidOperationException()
        {
            var paramInfo = GetParameterInfo(nameof(SampleMethods.InInt));
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo, typeof(object));
            var provider = new Mock<IValueProvider>();
            provider.Setup(p => p.ResolveValue(request, _contextMock.Object)).Returns(null!);
            var sut = new CompositeValueProvider([provider.Object]);

            Assert.Throws<InvalidOperationException>(
                () => sut.ResolveValue(request, _contextMock.Object));
        }

        [Test]
        public void AddProvider_AddedProviderIsUsedByResolveValue()
        {
            var request = new FixtureRequest(typeof(string));
            var expected = "from-added";

            var addedProvider = new Mock<IValueProvider>();
            addedProvider.Setup(p => p.ResolveValue(request, _contextMock.Object)).Returns(expected);

            var sut = new CompositeValueProvider([]);
            sut.AddProvider(addedProvider.Object);

            var result = sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void AddProvider_PrependedBeforeExistingProviders()
        {
            var request = new FixtureRequest(typeof(string));
            var addedResult = "from-added";
            var originalResult = "from-original";

            var originalProvider = new Mock<IValueProvider>();
            originalProvider.Setup(p => p.ResolveValue(request, _contextMock.Object)).Returns(originalResult);

            var addedProvider = new Mock<IValueProvider>();
            addedProvider.Setup(p => p.ResolveValue(request, _contextMock.Object)).Returns(addedResult);

            var sut = new CompositeValueProvider([originalProvider.Object]);
            sut.AddProvider(addedProvider.Object);

            var result = sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.SameAs(addedResult));
            originalProvider.Verify(p => p.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void AddRootProvider_ResolvedBeforeLaterAddedProviders()
        {
            var expected = "from-root";
            var rootProvider = CreateProvider(expected);
            var provider = CreateProvider("from-provider");
            var sut = new CompositeValueProvider([]);
            sut.AddRootProvider(rootProvider.Object);
            sut.AddProvider(provider.Object);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
            provider.Verify(p => p.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void AddRootProvider_PrependedBeforeEarlierRootProviders()
        {
            var expected = "from-later-root";
            var earlierRootProvider = CreateProvider("from-earlier-root");
            var laterRootProvider = CreateProvider(expected);
            var sut = new CompositeValueProvider([]);
            sut.AddRootProvider(earlierRootProvider.Object);
            sut.AddRootProvider(laterRootProvider.Object);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
            earlierRootProvider.Verify(p => p.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()), Times.Never);
        }
    }
}
