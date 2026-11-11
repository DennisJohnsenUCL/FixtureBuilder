namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class BuildTests
    {
        [Test]
        public void ConstructsInstance()
        {
            var factory = new FixtureFactory();

            var result = factory.Build<ConstructorSetsValueClass>();

            Assert.That(result.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void UsesFactoryConfiguration()
        {
            var factory = new FixtureFactory().With("injected");

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        [Test]
        public void InterfaceType_ThrowsInvalidOperationException()
        {
            var factory = new FixtureFactory();

            Assert.Throws<InvalidOperationException>(() => factory.Build<IMarkerInterface>());
        }

        [Test]
        public void AbstractType_ThrowsInvalidOperationException()
        {
            var factory = new FixtureFactory();

            Assert.Throws<InvalidOperationException>(() => factory.Build<AbstractClass>());
        }
    }
}
