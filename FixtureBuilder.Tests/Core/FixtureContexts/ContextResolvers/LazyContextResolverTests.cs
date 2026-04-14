using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core.FixtureContexts.ContextResolvers;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.ContextResolvers
{
    internal sealed class LazyContextResolverTests
    {
        private Func<IValueConverter> _converter;
        private Func<ITypeLink> _typeLink;
        private Func<IUninitializedProvider> _uninitializedProvider;
        private Func<IValueProvider> _valueProvider;
        private Func<IAutoConstructingProvider> _autoConstructingProvider;

        [SetUp]
        public void SetUp()
        {
            _converter = () => Mock.Of<IValueConverter>();
            _typeLink = () => Mock.Of<ITypeLink>();
            _uninitializedProvider = () => Mock.Of<IUninitializedProvider>();
            _valueProvider = () => Mock.Of<IValueProvider>();
            _autoConstructingProvider = () => Mock.Of<IAutoConstructingProvider>();
        }

        private LazyContextResolver CreateSut() => new(
            _converter, _typeLink, _uninitializedProvider,
            _valueProvider, _autoConstructingProvider);

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
        public void GetConverter_ReturnsInstanceFromFactory()
        {
            var expected = Mock.Of<IValueConverter>();
            _converter = () => expected;

            var result = CreateSut().Converter;

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void GetConverter_CalledTwice_ReturnsSameInstance()
        {
            var callCount = 0;
            var instance = Mock.Of<IValueConverter>();
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
            var expected = Mock.Of<ITypeLink>();
            _typeLink = () => expected;

            var result = CreateSut().TypeLink;

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void GetTypeLink_CalledTwice_ReturnsSameInstance()
        {
            var callCount = 0;
            var instance = Mock.Of<ITypeLink>();
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
            var expected = Mock.Of<IValueProvider>();
            _valueProvider = () => expected;

            var result = CreateSut().ValueProvider;

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void GetValueProvider_CalledTwice_ReturnsSameInstance()
        {
            var callCount = 0;
            var instance = Mock.Of<IValueProvider>();
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
        public void FactoriesAreNotCalledUntilAccessed()
        {
            var converterCalled = false;
            var typeLinkCalled = false;
            var providerCalled = false;
            var valueProviderCalled = false;
            var parameterProviderCalled = false;
            var autoConstructingProviderCalled = false;

            _ = new LazyContextResolver(
                () => { converterCalled = true; return Mock.Of<IValueConverter>(); },
                () => { typeLinkCalled = true; return Mock.Of<ITypeLink>(); },
                () => { providerCalled = true; return Mock.Of<IUninitializedProvider>(); },
                () => { valueProviderCalled = true; return Mock.Of<IValueProvider>(); },
                () => { autoConstructingProviderCalled = true; return Mock.Of<IAutoConstructingProvider>(); });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(converterCalled, Is.False);
                Assert.That(typeLinkCalled, Is.False);
                Assert.That(providerCalled, Is.False);
                Assert.That(valueProviderCalled, Is.False);
                Assert.That(parameterProviderCalled, Is.False);
                Assert.That(autoConstructingProviderCalled, Is.False);
            }
        }
    }
}
