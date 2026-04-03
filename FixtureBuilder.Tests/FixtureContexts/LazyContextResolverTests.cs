using FixtureBuilder.FixtureContexts;
using FixtureBuilder.FixtureProviders;
using FixtureBuilder.TypeLinks;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.ValueConverters;
using Moq;

namespace FixtureBuilder.Tests.FixtureContexts
{
    internal sealed class LazyContextResolverTests
    {
        [Test]
        public void Constructor_NullConverter_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LazyContextResolver(
                null!,
                () => Mock.Of<ITypeLink>(),
                () => Mock.Of<IFixtureUninitializedProvider>(),
                () => Mock.Of<IFixtureProvider>()));
        }

        [Test]
        public void Constructor_NullTypeLink_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LazyContextResolver(
                () => Mock.Of<IValueConverter>(),
                null!,
                () => Mock.Of<IFixtureUninitializedProvider>(),
                () => Mock.Of<IFixtureProvider>()));
        }

        [Test]
        public void Constructor_NullUninitializedProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LazyContextResolver(
                () => Mock.Of<IValueConverter>(),
                () => Mock.Of<ITypeLink>(),
                null!,
                () => Mock.Of<IFixtureProvider>()));
        }

        [Test]
        public void Constructor_NullFixtureProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LazyContextResolver(
                () => Mock.Of<IValueConverter>(),
                () => Mock.Of<ITypeLink>(),
                () => Mock.Of<IFixtureUninitializedProvider>(),
                null!));
        }

        [Test]
        public void GetConverter_ReturnsInstanceFromFactory()
        {
            var expected = Mock.Of<IValueConverter>();
            var sut = new LazyContextResolver(
                () => expected,
                () => Mock.Of<ITypeLink>(),
                () => Mock.Of<IFixtureUninitializedProvider>(),
                () => Mock.Of<IFixtureProvider>());

            var result = sut.GetConverter();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void GetConverter_CalledTwice_ReturnsSameInstance()
        {
            var callCount = 0;
            var instance = Mock.Of<IValueConverter>();
            var sut = new LazyContextResolver(
                () => { callCount++; return instance; },
                () => Mock.Of<ITypeLink>(),
                () => Mock.Of<IFixtureUninitializedProvider>(),
                () => Mock.Of<IFixtureProvider>());

            var first = sut.GetConverter();
            var second = sut.GetConverter();

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
            var sut = new LazyContextResolver(
                () => Mock.Of<IValueConverter>(),
                () => expected,
                () => Mock.Of<IFixtureUninitializedProvider>(),
                () => Mock.Of<IFixtureProvider>());

            var result = sut.GetTypeLink();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void GetTypeLink_CalledTwice_ReturnsSameInstance()
        {
            var callCount = 0;
            var instance = Mock.Of<ITypeLink>();
            var sut = new LazyContextResolver(
                () => Mock.Of<IValueConverter>(),
                () => { callCount++; return instance; },
                () => Mock.Of<IFixtureUninitializedProvider>(),
                () => Mock.Of<IFixtureProvider>());

            var first = sut.GetTypeLink();
            var second = sut.GetTypeLink();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(second, Is.SameAs(first));
                Assert.That(callCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetUninitializedProvider_ReturnsInstanceFromFactory()
        {
            var expected = Mock.Of<IFixtureUninitializedProvider>();
            var sut = new LazyContextResolver(
                () => Mock.Of<IValueConverter>(),
                () => Mock.Of<ITypeLink>(),
                () => expected,
                () => Mock.Of<IFixtureProvider>());

            var result = sut.GetUninitializedProvider();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void GetUninitializedProvider_CalledTwice_ReturnsSameInstance()
        {
            var callCount = 0;
            var instance = Mock.Of<IFixtureUninitializedProvider>();
            var sut = new LazyContextResolver(
                () => Mock.Of<IValueConverter>(),
                () => Mock.Of<ITypeLink>(),
                () => { callCount++; return instance; },
                () => Mock.Of<IFixtureProvider>());

            var first = sut.GetUninitializedProvider();
            var second = sut.GetUninitializedProvider();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(second, Is.SameAs(first));
                Assert.That(callCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetFixtureProvider_ReturnsInstanceFromFactory()
        {
            var expected = Mock.Of<IFixtureProvider>();
            var sut = new LazyContextResolver(
                () => Mock.Of<IValueConverter>(),
                () => Mock.Of<ITypeLink>(),
                () => Mock.Of<IFixtureUninitializedProvider>(),
                () => expected);

            var result = sut.GetFixtureProvider();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void GetFixtureProvider_CalledTwice_ReturnsSameInstance()
        {
            var callCount = 0;
            var instance = Mock.Of<IFixtureProvider>();
            var sut = new LazyContextResolver(
                () => Mock.Of<IValueConverter>(),
                () => Mock.Of<ITypeLink>(),
                () => Mock.Of<IFixtureUninitializedProvider>(),
                () => { callCount++; return instance; });

            var first = sut.GetFixtureProvider();
            var second = sut.GetFixtureProvider();

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
            var fixtureProviderCalled = false;

            _ = new LazyContextResolver(
                () => { converterCalled = true; return Mock.Of<IValueConverter>(); },
                () => { typeLinkCalled = true; return Mock.Of<ITypeLink>(); },
                () => { providerCalled = true; return Mock.Of<IFixtureUninitializedProvider>(); },
                () => { fixtureProviderCalled = true; return Mock.Of<IFixtureProvider>(); });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(converterCalled, Is.False);
                Assert.That(typeLinkCalled, Is.False);
                Assert.That(providerCalled, Is.False);
                Assert.That(fixtureProviderCalled, Is.False);
            }
        }
    }
}
