namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    public class BogusFixtureBuildTests
    {
        private class SimpleClass
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        [Test]
        public void Build_ReturnsNonNullInstance()
        {
            var result = Fixture.WithBogus<SimpleClass>().Build();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Build_ReturnsInstanceOfCorrectType()
        {
            var result = Fixture.WithBogus<SimpleClass>().Build();

            Assert.That(result, Is.InstanceOf<SimpleClass>());
        }

        [Test]
        public void Build_WithCount_ReturnsCorrectNumberOfInstances()
        {
            var results = Fixture.WithBogus<SimpleClass>().Build(3);

            Assert.That(results.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Build_WithCount_ReturnsDistinctInstances()
        {
            var results = Fixture.WithBogus<SimpleClass>().Build(3).ToList();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(results[0], Is.Not.SameAs(results[1]));
                Assert.That(results[1], Is.Not.SameAs(results[2]));
            }
        }

        [Test]
        public void Build_WithCount_ReturnsStableResults()
        {
            var bogus = Fixture.WithBogus<SimpleClass>();
            var results = bogus.Build(2);

            var first = results.ToList();
            var second = results.ToList();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(first[0], Is.SameAs(second[0]));
                Assert.That(first[1], Is.SameAs(second[1]));
            }
        }

        [Test]
        public void Build_WithCount_AppliesConfigurationToAll()
        {
            var results = Fixture.WithBogus<SimpleClass>()
                .With(x => x.Name, "Alice")
                .Build(2)
                .ToList();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(results[0].Name, Is.EqualTo("Alice"));
                Assert.That(results[1].Name, Is.EqualTo("Alice"));
            }
        }
    }
}
