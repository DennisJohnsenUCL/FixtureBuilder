using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts
{
    internal sealed class LazyContextTests
    {
        private FixtureOptions _options;
        private Func<ConverterGraph> _converterFactory;
        private Func<ICompositeTypeLink> _typeLinkFactory;
        private Func<IUninitializedProvider> _uninitializedFactory;
        private Func<ICompositeValueProvider> _valueProviderFactory;
        private Func<IAutoConstructingProvider> _autoConstructingFactory;
        private Func<IConstructingProvider> _constructingFactory;

        [SetUp]
        public void SetUp()
        {
            _options = new FixtureOptions();
            _converterFactory = () => new ConverterGraph(new Mock<IValueConverter>().Object, new Mock<ICompositeConverter>().Object);
            _typeLinkFactory = () => new Mock<ICompositeTypeLink>().Object;
            _uninitializedFactory = () => new Mock<IUninitializedProvider>().Object;
            _valueProviderFactory = () => new Mock<ICompositeValueProvider>().Object;
            _autoConstructingFactory = () => new Mock<IAutoConstructingProvider>().Object;
            _constructingFactory = () => new Mock<IConstructingProvider>().Object;
        }

        [Test]
        public void Constructor_NullOptions_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LazyContext(null!, _converterFactory, _typeLinkFactory, _uninitializedFactory, _valueProviderFactory, _autoConstructingFactory, _constructingFactory));
        }

        [Test]
        public void Constructor_NullConverter_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LazyContext(_options, null!, _typeLinkFactory, _uninitializedFactory, _valueProviderFactory, _autoConstructingFactory, _constructingFactory));
        }

        [Test]
        public void Constructor_NullTypeLink_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LazyContext(_options, _converterFactory, null!, _uninitializedFactory, _valueProviderFactory, _autoConstructingFactory, _constructingFactory));
        }

        [Test]
        public void Constructor_NullUninitializedProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LazyContext(_options, _converterFactory, _typeLinkFactory, null!, _valueProviderFactory, _autoConstructingFactory, _constructingFactory));
        }

        [Test]
        public void Constructor_NullValueProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LazyContext(_options, _converterFactory, _typeLinkFactory, _uninitializedFactory, null!, _autoConstructingFactory, _constructingFactory));
        }

        [Test]
        public void Constructor_NullAutoConstructingProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LazyContext(_options, _converterFactory, _typeLinkFactory, _uninitializedFactory, _valueProviderFactory, null!, _constructingFactory));
        }

        [Test]
        public void Constructor_NullConstructingProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LazyContext(_options, _converterFactory, _typeLinkFactory, _uninitializedFactory, _valueProviderFactory, _autoConstructingFactory, null!));
        }

        [Test]
        public void FactoriesAreNotCalledUntilAccessed()
        {
            var converterCalled = false;
            var typeLinkCalled = false;
            var uninitializedCalled = false;
            var valueProviderCalled = false;
            var autoConstructingCalled = false;
            var constructingCalled = false;

            _ = new LazyContext(
                _options,
                () => { converterCalled = true; return new ConverterGraph(new Mock<IValueConverter>().Object, new Mock<ICompositeConverter>().Object); },
                () => { typeLinkCalled = true; return Mock.Of<ICompositeTypeLink>(); },
                () => { uninitializedCalled = true; return Mock.Of<IUninitializedProvider>(); },
                () => { valueProviderCalled = true; return Mock.Of<ICompositeValueProvider>(); },
                () => { autoConstructingCalled = true; return Mock.Of<IAutoConstructingProvider>(); },
                () => { constructingCalled = true; return Mock.Of<IConstructingProvider>(); });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(converterCalled, Is.False);
                Assert.That(typeLinkCalled, Is.False);
                Assert.That(uninitializedCalled, Is.False);
                Assert.That(valueProviderCalled, Is.False);
                Assert.That(autoConstructingCalled, Is.False);
                Assert.That(constructingCalled, Is.False);
            }
        }
    }
}
