using Bogus;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactoryTests
{
    internal sealed class NewTests
    {
        private class SimpleClass
        {
            public string Name { get; set; } = string.Empty;
        }

        private BogusFixtureFactory _factory = null!;

        [SetUp]
        public void SetUp()
        {
            _factory = FixtureFactory.WithBogus();
        }

        [Test]
        public void New_ReturnsConstructor()
        {
            var constructor = _factory.New<SimpleClass>();

            Assert.That(constructor, Is.InstanceOf<IBogusFixtureConstructor<SimpleClass>>());
        }

        [Test]
        public void New_MultipleCallsProduceIndependentFixtures()
        {
            var result1 = _factory.New<SimpleClass>()
                .With(x => x.Name, "First")
                .Build();

            var result2 = _factory.New<SimpleClass>()
                .With(x => x.Name, "Second")
                .Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result1.Name, Is.EqualTo("First"));
                Assert.That(result2.Name, Is.EqualTo("Second"));
            }
        }

        [Test]
        public void New_SharesLocaleWithFactory()
        {
            var factory = FixtureFactory.WithBogus("de");

            var constructor = factory.New<SimpleClass>();

            Assert.That(constructor.Locale, Is.EqualTo(factory.Locale));
        }

        [Test]
        public void New_SharesRandomWithFactory()
        {
            var factory = FixtureFactory.WithBogus();
            var randomizer = new Randomizer(42);
            factory.Random = randomizer;

            var constructor = factory.New<SimpleClass>();

            Assert.That(constructor.Random, Is.SameAs(factory.Random));
        }
    }
}
