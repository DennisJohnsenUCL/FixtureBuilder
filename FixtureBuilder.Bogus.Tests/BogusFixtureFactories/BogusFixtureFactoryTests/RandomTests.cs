using Bogus;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusFixtureFactoryTests
{
    internal sealed class RandomTests
    {
        [Test]
        public void Random_CanBeSet()
        {
            var factory = FixtureFactory.WithBogus();
            var randomizer = new Randomizer(42);

            factory.Random = randomizer;

            Assert.That(factory.Random, Is.SameAs(randomizer));
        }
    }
}
