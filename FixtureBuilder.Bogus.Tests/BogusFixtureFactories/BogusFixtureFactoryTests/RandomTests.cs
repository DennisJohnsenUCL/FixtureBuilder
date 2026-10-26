using Bogus;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusFixtureFactoryTests
{
    internal sealed class RandomTests
    {
        private class SimpleClass
        {
            public string Name { get; set; } = string.Empty;
        }

        [Test]
        public void Random_CanBeSet()
        {
            var factory = FixtureFactory.WithBogus();
            var randomizer = new Randomizer(42);

            factory.Random = randomizer;

            Assert.That(factory.Random, Is.SameAs(randomizer));
        }

        [Test]
        public void Random_Seeded_ProducesRepeatableResults()
        {
            var firstFactory = FixtureFactory.WithBogus();
            firstFactory.Random = new Randomizer(42);
            var first = firstFactory.New<SimpleClass>();

            var secondFactory = FixtureFactory.WithBogus();
            secondFactory.Random = new Randomizer(42);
            var second = secondFactory.New<SimpleClass>();

            var result1 = first.With(x => x.Name, f => f.Name.FirstName()).Build();
            var result2 = second.With(x => x.Name, f => f.Name.FirstName()).Build();

            Assert.That(result1.Name, Is.EqualTo(result2.Name));
        }
    }
}
