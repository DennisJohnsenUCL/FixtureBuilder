namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class BogusFixtureBuildTests
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

        [Test]
        public void Build_WithZero_Throws()
        {
            var bogus = Fixture.WithBogus<SimpleClass>();

            Assert.That(() => bogus.Build(0), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Build_WithNegative_Throws()
        {
            var bogus = Fixture.WithBogus<SimpleClass>();

            Assert.That(() => bogus.Build(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Build_WithInstance_ReturnsSameInstance()
        {
            var instance = new SimpleClass();

            var result = Fixture.WithBogus(instance).Build();

            Assert.That(result, Is.SameAs(instance));
        }

        [Test]
        public void Build_WithInstance_CountOne_ReturnsSameInstance()
        {
            var instance = new SimpleClass();

            var results = Fixture.WithBogus(instance).Build(1).ToList();

            Assert.That(results[0], Is.SameAs(instance));
        }

        [Test]
        public void Build_WithInstance_CountGreaterThanOne_Throws()
        {
            var instance = new SimpleClass();
            var bogus = Fixture.WithBogus(instance);

            Assert.That(() => bogus.Build(2), Throws.TypeOf<InvalidOperationException>());
        }
    }
}
