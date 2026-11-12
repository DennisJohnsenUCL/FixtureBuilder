using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class ConstructorTests
    {
        [Test]
        public void Parameterless_UsesDefaultOptions()
        {
            var factory = new FixtureFactory();

            Assert.That(factory.Options, Is.EqualTo(FixtureOptions.Default).UsingPropertiesComparer());
        }

        [Test]
        public void WithOptions_UsesThoseOptions()
        {
            var options = new FixtureOptions { AllowPrivateConstructors = false };

            var factory = new FixtureFactory(options);

            Assert.That(factory.Options, Is.SameAs(options));
        }

        [Test]
        public void WithOptions_FixturesUseThoseOptions()
        {
            var options = new FixtureOptions { AllowPrivateConstructors = false };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<PrivateConstructorClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.Build());
        }

        [Test]
        public void NullOptions_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new FixtureFactory(null!));

            Assert.That(ex.ParamName, Is.EqualTo("options"));
        }
    }
}
