namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    public class BogusFixtureBuildTests
    {
        private class SimpleClass;

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
        public void Build_CalledMultipleTimes_ReturnsDistinctInstances()
        {
            var bogus = Fixture.WithBogus<SimpleClass>();

            var first = bogus.Build();
            var second = bogus.Build();

            Assert.That(first, Is.Not.SameAs(second));
        }
    }
}
