namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusFixtureFactoryTests
{
    internal sealed class FakerConfigurationBuilderTests
    {
        [Test]
        public void AddBogusProvider_ProviderResolvesWithFactoryFaker()
        {
            var factory = FixtureFactory.WithBogus("de").AddBogusProvider(new FakerLocaleProvider());

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        [Test]
        public void AddBogusProvider_NullProvider_ThrowsArgumentNullException()
        {
            var factory = FixtureFactory.WithBogus();

            Assert.Throws<ArgumentNullException>(() => factory.AddBogusProvider(null!));
        }
    }
}
