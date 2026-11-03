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

        [SetUp]
        public void SetUp()
        {
            _options = new FixtureOptions();
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
                () => { converterCalled = true; return Mock.Of<ICompositeConverter>(); },
                () => { typeLinkCalled = true; return Mock.Of<ICompositeTypeLink>(); },
                () => { uninitializedCalled = true; return Mock.Of<IRootUninitializedProvider>(); },
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
