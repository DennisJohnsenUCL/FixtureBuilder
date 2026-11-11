using Bogus;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusFixtureFactoryTests
{
    internal sealed class NewTests
    {
        [Test]
        public void CalledTwice_ReturnsIndependentFixtures()
        {
            var factory = FixtureFactory.WithBogus();

            var first = factory.New<PropertyHolder<string>>().With(x => x.Value, "first").Build();
            var second = factory.New<PropertyHolder<string>>().With(x => x.Value, "second").Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(first.Value, Is.EqualTo("first"));
                Assert.That(second.Value, Is.EqualTo("second"));
            }
        }

        [Test]
        public void UsesFactoryLocale()
        {
            var factory = FixtureFactory.WithBogus("de");

            var fixture = factory.New<EmptyClass>();

            Assert.That(fixture.Locale, Is.EqualTo("de"));
        }

        [Test]
        public void UsesFactoryRandom()
        {
            var factory = FixtureFactory.WithBogus();
            factory.Random = new Randomizer(42);

            var fixture = factory.New<EmptyClass>();

            Assert.That(fixture.Random, Is.SameAs(factory.Random));
        }

        [Test]
        public void Instance_BuildReturnsInstance()
        {
            var factory = FixtureFactory.WithBogus();
            var instance = new EmptyClass();

            var result = factory.New(instance).Build();

            Assert.That(result, Is.SameAs(instance));
        }

        [Test]
        public void Instance_UsesFactoryLocale()
        {
            var factory = FixtureFactory.WithBogus("de");
            var instance = new EmptyClass();

            var fixture = factory.New(instance);

            Assert.That(fixture.Locale, Is.EqualTo("de"));
        }

        [Test]
        public void NullInstance_ThrowsArgumentNullException()
        {
            var factory = FixtureFactory.WithBogus();

            Assert.Throws<ArgumentNullException>(() => factory.New<EmptyClass>(null!));
        }
    }
}
