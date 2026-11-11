namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusFixtureFactoryTests
{
    internal sealed class BuildTests
    {
        [Test]
        public void ConstructsFixtureImplicitly()
        {
            var factory = FixtureFactory.WithBogus();

            var result = factory.Build<ConstructorSetsValueClass>();

            Assert.That(result.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void Interface_ThrowsInvalidOperationException()
        {
            var factory = FixtureFactory.WithBogus();

            Assert.Throws<InvalidOperationException>(() => factory.Build<IMarkerInterface>());
        }

        private abstract class AbstractClass;

        [Test]
        public void AbstractClass_ThrowsInvalidOperationException()
        {
            var factory = FixtureFactory.WithBogus();

            Assert.Throws<InvalidOperationException>(() => factory.Build<AbstractClass>());
        }
    }
}
