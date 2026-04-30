using Bogus;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class RandomTests
    {
        private class SimpleClass
        {
            public string Name { get; set; } = string.Empty;
        }

        [Test]
        public void Random_SetSeed_ProducesRepeatableResults()
        {
            var first = Fixture.WithBogus<SimpleClass>();
            first.Random = new Randomizer(42);

            var second = Fixture.WithBogus<SimpleClass>();
            second.Random = new Randomizer(42);

            var result1 = first.With(x => x.Name, f => f.Name.FirstName()).Build();
            var result2 = second.With(x => x.Name, f => f.Name.FirstName()).Build();

            Assert.That(result1.Name, Is.EqualTo(result2.Name));
        }
    }
}
