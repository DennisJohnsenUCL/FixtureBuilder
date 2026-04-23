using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class ConstructorTests
    {
        [Test]
        public void DefaultConstructor_HasDefaultOptions()
        {
            var factory = new FixtureFactory();

            Assert.That(factory.Options.AllowPrivateConstructors, Is.EqualTo(FixtureOptions.Default.AllowPrivateConstructors));
        }

        [Test]
        public void Constructor_WithOptions_AppliesOptions()
        {
            var options = new FixtureOptions { AllowPrivateConstructors = false };

            var factory = new FixtureFactory(options);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(factory.Options.AllowPrivateConstructors, Is.False);
                Assert.That(factory.Options, Is.SameAs(options));
            }
        }

        [Test]
        public void Constructor_WithOptions_FixtureReceivesProvidedOptions()
        {
            var options = new FixtureOptions { AllowPrivateConstructors = false };
            var factory = new FixtureFactory(options);

            var fixture = factory.New<TestClass>();
            var context = TestHelper.GetContext(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(context.GetBaseOptions().AllowPrivateConstructors, Is.False);
                Assert.That(context.GetBaseOptions(), Is.SameAs(factory.Options));
            }
        }
    }
}
