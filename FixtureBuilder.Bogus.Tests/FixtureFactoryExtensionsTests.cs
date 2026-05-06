namespace FixtureBuilder.Bogus.Tests
{
    internal sealed class FixtureFactoryExtensionsTests
    {
        [Test]
        public void WithBogus_DefaultLocale_IsEnglish()
        {
            var factory = FixtureFactory.WithBogus();

            Assert.That(factory.Locale, Is.EqualTo("en"));
        }

        [Test]
        public void WithBogus_WithLocale_UsesLocale()
        {
            var factory = FixtureFactory.WithBogus("de");

            Assert.That(factory.Locale, Is.EqualTo("de"));
        }
    }

}
