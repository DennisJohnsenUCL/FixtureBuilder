using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class OptionsTests
    {
        [Test]
        public void Set_ReplacesOptions()
        {
            var factory = new FixtureFactory();
            var options = new FixtureOptions { AllowPrivateConstructors = false };

            factory.Options = options;

            Assert.That(factory.Options, Is.SameAs(options));
        }

        [Test]
        public void Set_FixturesUseNewOptions()
        {
            var factory = new FixtureFactory { Options = new FixtureOptions { AllowPrivateConstructors = false } };
            var fixture = factory.New<PrivateConstructorClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.Build());
        }

        [Test]
        public void SetNull_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            Assert.Throws<ArgumentNullException>(() => factory.Options = null!);
        }
    }
}
