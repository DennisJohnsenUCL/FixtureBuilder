namespace FixtureBuilder.Bogus.Tests
{
    internal sealed class FixtureFactoryExtensionsTests
    {
        //TODO: Use .Locale to test against
        [Test]
        public void WithBogus_ReturnsBogusFixtureFactory()
        {
            var factory = FixtureFactory.WithBogus();

            Assert.That(factory, Is.InstanceOf<BogusFixtureFactory>());
        }

        //TODO: Use .Locale to test against
        [Test]
        public void WithBogus_WithLocale_ReturnsBogusFixtureFactory()
        {
            var factory = FixtureFactory.WithBogus("fr");

            Assert.That(factory, Is.InstanceOf<BogusFixtureFactory>());
        }
    }

}
