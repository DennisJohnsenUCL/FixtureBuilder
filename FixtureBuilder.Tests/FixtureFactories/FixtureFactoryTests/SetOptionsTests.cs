using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories;

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class SetOptionsTests
    {
        private FixtureFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new FixtureFactory();
        }

        [Test]
        public void Options_DefaultConstructor_ReturnsDefaultOptions()
        {
            Assert.That(_factory.Options.AllowPrivateConstructors, Is.EqualTo(FixtureOptions.Default.AllowPrivateConstructors));
        }

        [Test]
        public void Options_ConstructorWithOptions_ReturnsProvidedOptions()
        {
            var options = new FixtureOptions { AllowPrivateConstructors = false };

            var factory = new FixtureFactory(options);

            Assert.That(factory.Options.AllowPrivateConstructors, Is.False);
        }

        [Test]
        public void Options_Set_UpdatesOptions()
        {
            var options = new FixtureOptions { AllowPrivateConstructors = false };

            _factory.Options = options;

            Assert.That(_factory.Options, Is.SameAs(options));
        }

        [Test]
        public void SetOptions_Action_UpdatesOptions()
        {
            _factory.SetOptions(o => o.AllowPrivateConstructors = false);

            Assert.That(_factory.Options.AllowPrivateConstructors, Is.False);
        }

        [Test]
        public void SetOptions_NullAction_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _factory.SetOptions(null!));
        }
    }
}
