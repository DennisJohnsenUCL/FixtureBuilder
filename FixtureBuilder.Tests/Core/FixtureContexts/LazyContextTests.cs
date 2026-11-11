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
        // Fixture.New uses a lazy context so that providers, converters and type links a fixture never uses are never built.
        [Test]
        public void Constructor_DoesNotCreateAnyComponent()
        {
            var created = 0;

            _ = new LazyContext(
                new FixtureOptions(),
                () => { created++; return Mock.Of<ICompositeConverter>(); },
                () => { created++; return Mock.Of<ICompositeTypeLink>(); },
                () => { created++; return Mock.Of<IRootUninitializedProvider>(); },
                () => { created++; return Mock.Of<ICompositeValueProvider>(); },
                () => { created++; return Mock.Of<IAutoConstructingProvider>(); },
                () => { created++; return Mock.Of<IConstructingProvider>(); });

            Assert.That(created, Is.Zero);
        }
    }
}
