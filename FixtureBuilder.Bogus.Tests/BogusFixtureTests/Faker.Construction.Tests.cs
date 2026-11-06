using Bogus;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class FakerConstructionTests
    {
        private class ClassWithParameterizedConstructor(string name)
        {
            public string Name { get; } = name;
        }

        [Test]
        public void UseConstructor_Faker_PassesGeneratedArgs()
        {
            var result = Fixture.WithBogus<ClassWithParameterizedConstructor>()
                .UseConstructor(f => [f.Name.FirstName()])
                .Build();

            Assert.That(result.Name, Is.Not.Empty);
        }

        [Test]
        public void UseConstructor_Faker_BuildMultiple_ProducesDifferentValues()
        {
            var factory = FixtureFactory.WithBogus();
            factory.Random = new Randomizer(42);

            var results = factory.New<ClassWithParameterizedConstructor>()
                .UseConstructor(f => [f.Name.FirstName()])
                .Build(2)
                .ToList();

            Assert.That(results[0].Name, Is.Not.EqualTo(results[1].Name));
        }
    }
}
