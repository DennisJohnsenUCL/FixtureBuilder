using Bogus;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusFixtureFactoryTests
{
    internal sealed class RandomTests
    {
        [Test]
        public void Set_GetReturnsSetRandomizer()
        {
            var factory = FixtureFactory.WithBogus();
            var randomizer = new Randomizer(42);

            factory.Random = randomizer;

            Assert.That(factory.Random, Is.SameAs(randomizer));
        }

        [Test]
        public void SameSeed_RepeatsFakerValues()
        {
            var firstFactory = FixtureFactory.WithBogus();
            firstFactory.Random = new Randomizer(42);
            var secondFactory = FixtureFactory.WithBogus();
            secondFactory.Random = new Randomizer(42);

            var first = firstFactory.New<PropertyHolder<string>>().With(x => x.Value, faker => faker.Name.FirstName()).Build();
            var second = secondFactory.New<PropertyHolder<string>>().With(x => x.Value, faker => faker.Name.FirstName()).Build();

            Assert.That(second.Value, Is.EqualTo(first.Value));
        }
    }
}
