using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.ConverterBuilders;
using FixtureBuilder.Core.FixtureContexts.ContextResolvers;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.ContextResolvers
{
    internal sealed class LazyContextResolverTests
    {
        private Func<ConverterGraph> _converter;
        private Func<ICompositeTypeLink> _typeLink;
        private Func<IUninitializedProvider> _uninitializedProvider;
        private Func<ICompositeValueProvider> _valueProvider;
        private Func<IAutoConstructingProvider> _autoConstructingProvider;
        private Func<IConstructingProvider> _constructingProvider;

        private static ConverterGraph CreateGraph() =>
            new(new Mock<IValueConverter>().Object, new Mock<ICompositeConverter>().Object);

        [SetUp]
        public void SetUp()
        {
            _converter = () => CreateGraph();
            _typeLink = () => Mock.Of<ICompositeTypeLink>();
            _uninitializedProvider = () => Mock.Of<IUninitializedProvider>();
            _valueProvider = () => Mock.Of<ICompositeValueProvider>();
            _autoConstructingProvider = () => Mock.Of<IAutoConstructingProvider>();
            _constructingProvider = () => Mock.Of<ConstructingProvider>();
        }

        private LazyContextResolver CreateSut() => new(
            _converter, _typeLink, _uninitializedProvider,
            _valueProvider, _autoConstructingProvider, _constructingProvider);

        [Test]
        public void Constructor_NullConverter_Throws()
        {
            _converter = null!;
            Assert.Throws<ArgumentNullException>(() => CreateSut());
        }

        [Test]
        public void Constructor_NullTypeLink_Throws()
        {
            _typeLink = null!;
            Assert.Throws<ArgumentNullException>(() => CreateSut());
        }

        [Test]
        public void Constructor_NullUninitializedProvider_Throws()
        {
            _uninitializedProvider = null!;
            Assert.Throws<ArgumentNullException>(() => CreateSut());
        }

        [Test]
        public void Constructor_NullValueProvider_Throws()
        {
            _valueProvider = null!;
            Assert.Throws<ArgumentNullException>(() => CreateSut());
        }

        [Test]
        public void Constructor_NullAutoConstructingProvider_Throws()
        {
            _autoConstructingProvider = null!;
            Assert.Throws<ArgumentNullException>(() => CreateSut());
        }

        [Test]
        public void Constructor_NullConstructingProvider_Throws()
        {
            _constructingProvider = null!;
            Assert.Throws<ArgumentNullException>(() => CreateSut());
        }

        [Test]
        public void GetConverter_ReturnsInstanceFromFactory()
        {
            var expected = CreateGraph();
            _converter = () => expected;

            var result = CreateSut().Converter;

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void GetConverter_CalledTwice_ReturnsSameInstance()
        {
            var callCount = 0;
            var instance = CreateGraph();
            _converter = () => { callCount++; return instance; };
            var sut = CreateSut();

            var first = sut.Converter;
            var second = sut.Converter;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(second, Is.SameAs(first));
                Assert.That(callCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetTypeLink_ReturnsInstanceFromFactory()
        {
            var expected = Mock.Of<ICompositeTypeLink>();
            _typeLink = () => expected;

            var result = CreateSut().TypeLink;

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void GetTypeLink_CalledTwice_ReturnsSameInstance()
        {
            var callCount = 0;
            var instance = Mock.Of<ICompositeTypeLink>();
            _typeLink = () => { callCount++; return instance; };
            var sut = CreateSut();

            var first = sut.TypeLink;
            var second = sut.TypeLink;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(second, Is.SameAs(first));
                Assert.That(callCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetUninitializedProvider_ReturnsInstanceFromFactory()
        {
            var expected = Mock.Of<IUninitializedProvider>();
            _uninitializedProvider = () => expected;

            var result = CreateSut().UninitializedProvider;

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void GetUninitializedProvider_CalledTwice_ReturnsSameInstance()
        {
            var callCount = 0;
            var instance = Mock.Of<IUninitializedProvider>();
            _uninitializedProvider = () => { callCount++; return instance; };
            var sut = CreateSut();

            var first = sut.UninitializedProvider;
            var second = sut.UninitializedProvider;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(second, Is.SameAs(first));
                Assert.That(callCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetValueProvider_ReturnsInstanceFromFactory()
        {
            var expected = Mock.Of<ICompositeValueProvider>();
            _valueProvider = () => expected;

            var result = CreateSut().ValueProvider;

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void GetValueProvider_CalledTwice_ReturnsSameInstance()
        {
            var callCount = 0;
            var instance = Mock.Of<ICompositeValueProvider>();
            _valueProvider = () => { callCount++; return instance; };
            var sut = CreateSut();

            var first = sut.ValueProvider;
            var second = sut.ValueProvider;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(second, Is.SameAs(first));
                Assert.That(callCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetAutoConstructingProvider_ReturnsInstanceFromFactory()
        {
            var expected = Mock.Of<IAutoConstructingProvider>();
            _autoConstructingProvider = () => expected;

            var result = CreateSut().AutoConstructingProvider;

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void GetAutoConstructingProvider_CalledTwice_ReturnsSameInstance()
        {
            var callCount = 0;
            var instance = Mock.Of<IAutoConstructingProvider>();
            _autoConstructingProvider = () => { callCount++; return instance; };
            var sut = CreateSut();

            var first = sut.AutoConstructingProvider;
            var second = sut.AutoConstructingProvider;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(second, Is.SameAs(first));
                Assert.That(callCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetConstructingProvider_ReturnsInstanceFromFactory()
        {
            var expected = Mock.Of<IConstructingProvider>();
            _constructingProvider = () => expected;

            var result = CreateSut().ConstructingProvider;

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void GetConstructingProvider_CalledTwice_ReturnsSameInstance()
        {
            var callCount = 0;
            var instance = Mock.Of<IConstructingProvider>();
            _constructingProvider = () => { callCount++; return instance; };
            var sut = CreateSut();

            var first = sut.ConstructingProvider;
            var second = sut.ConstructingProvider;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(second, Is.SameAs(first));
                Assert.That(callCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void FactoriesAreNotCalledUntilAccessed()
        {
            var converterCalled = false;
            var typeLinkCalled = false;
            var providerCalled = false;
            var valueProviderCalled = false;
            var parameterProviderCalled = false;
            var autoConstructingProviderCalled = false;
            var constructingProviderCalled = false;

            _ = new LazyContextResolver(
                () => { converterCalled = true; return CreateGraph(); },
                () => { typeLinkCalled = true; return Mock.Of<ICompositeTypeLink>(); },
                () => { providerCalled = true; return Mock.Of<IUninitializedProvider>(); },
                () => { valueProviderCalled = true; return Mock.Of<ICompositeValueProvider>(); },
                () => { autoConstructingProviderCalled = true; return Mock.Of<IAutoConstructingProvider>(); },
                () => { constructingProviderCalled = true; return Mock.Of<IConstructingProvider>(); });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(converterCalled, Is.False);
                Assert.That(typeLinkCalled, Is.False);
                Assert.That(providerCalled, Is.False);
                Assert.That(valueProviderCalled, Is.False);
                Assert.That(parameterProviderCalled, Is.False);
                Assert.That(autoConstructingProviderCalled, Is.False);
                Assert.That(constructingProviderCalled, Is.False);
            }
        }
    }
}
