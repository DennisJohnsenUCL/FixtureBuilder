using Bogus;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class BogusFixtureCustomInstantiatorTests
    {
        private class Person(string name, int age)
        {
            public string Name { get; } = name;
            public int Age { get; } = age;
        }

        [Test]
        public void UseCustomInstantiator_CreatesInstanceWithFakerData()
        {
            var result = Fixture.WithBogus<Person>()
                .UseCustomInstantiator(f => new Person(f.Name.FirstName(), f.Random.Int(18, 65)))
                .Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.Not.Empty);
                Assert.That(result.Age, Is.InRange(18, 65));
            }
        }

        [Test]
        public void UseCustomInstantiator_BuildMultiple_ProducesDifferentValues()
        {
            var results = Fixture.WithBogus<Person>()
                .UseCustomInstantiator(f => new Person(f.Name.FirstName(), f.Random.Int(18, 65)))
                .Build(2)
                .ToList();

            Assert.That(results[0].Name, Is.Not.EqualTo(results[1].Name));
        }

        private class SimpleClass
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }
        [Test]
        public void UseCustomInstantiator_FollowedByConfiguration_AppliesBoth()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .UseCustomInstantiator(f => new SimpleClass { Name = f.Name.FirstName() })
                .With(x => x.Value, 42)
                .Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.Not.Empty);
                Assert.That(result.Value, Is.EqualTo(42));
            }
        }

        [Test]
        public void UseCustomInstantiator_Seeded_ProducesRepeatableResults()
        {
            var first = Fixture.WithBogus<Person>();
            first.Random = new Randomizer(42);

            var second = Fixture.WithBogus<Person>();
            second.Random = new Randomizer(42);

            var result1 = first.UseCustomInstantiator(f => new Person(f.Name.FirstName(), f.Random.Int(18, 65))).Build();
            var result2 = second.UseCustomInstantiator(f => new Person(f.Name.FirstName(), f.Random.Int(18, 65))).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result1.Name, Is.EqualTo(result2.Name));
                Assert.That(result1.Age, Is.EqualTo(result2.Age));
            }
        }
    }
}
