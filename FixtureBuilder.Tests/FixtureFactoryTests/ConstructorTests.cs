using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Tests.FixtureFactoryTests
{
    internal sealed class ConstructorTests
    {
        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new FixtureFactory());
        }

        [Test]
        public void Constructor_WithOptions_DoesNotThrow()
        {
            var options = FixtureOptions.Default;

            Assert.DoesNotThrow(() => new FixtureFactory(options));
        }
    }
}
