using Bogus;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class InstantiateTests
    {
        private class Parent
        {
            public Child Child { get; set; } = null!;
        }

        private class Child
        {
            public string Name { get; set; } = string.Empty;

            public Child() { }
            public Child(string name) => Name = name;
        }

        [Test]
        public void Instantiate_Passthrough_CreatesInstance()
        {
            var result = Fixture.WithBogus<Parent>()
                .Instantiate(x => x.Child)
                .Build();

            Assert.That(result.Child, Is.Not.Null);
        }

        [Test]
        public void Instantiate_WithConstructor_PassesArgs()
        {
            var result = Fixture.WithBogus<Parent>()
                .Instantiate(x => x.Child, c => c.UseConstructor("Alice"))
                .Build();

            Assert.That(result.Child.Name, Is.EqualTo("Alice"));
        }

        [Test]
        public void Instantiate_WithBogusConstructor_UsesFaker()
        {
            var result = Fixture.WithBogus<Parent>()
                .Instantiate(x => x.Child, c => c.UseConstructor(f => [f.Name.FirstName()]))
                .Build();

            Assert.That(result.Child.Name, Is.Not.Empty);
        }

        [Test]
        public void Instantiate_WithBogusConstructor_BuildMultiple_ProducesDifferentValues()
        {
            var results = Fixture.WithBogus<Parent>()
                .Instantiate(x => x.Child, c => c.UseConstructor(f => [f.Name.FirstName()]))
                .Build(2)
                .ToList();

            Assert.That(results[0].Child.Name, Is.Not.EqualTo(results[1].Child.Name));
        }

        [Test]
        public void Instantiate_WithBogusConstructor_Seeded_ProducesRepeatableResults()
        {
            var first = Fixture.WithBogus<Parent>();
            first.Random = new Randomizer(42);

            var second = Fixture.WithBogus<Parent>();
            second.Random = new Randomizer(42);

            var result1 = first.Instantiate(x => x.Child, c => c.UseConstructor(f => [f.Name.FirstName()])).Build();
            var result2 = second.Instantiate(x => x.Child, c => c.UseConstructor(f => [f.Name.FirstName()])).Build();

            Assert.That(result1.Child.Name, Is.EqualTo(result2.Child.Name));
        }
    }
}
