namespace FixtureBuilder.Bogus.Tests
{
    internal sealed class FixtureExtensionsTests
    {
        class SimpleClass;

        [Test]
        public void WithBogus_WithLocale_UsesLocale()
        {
            var bogus = Fixture.WithBogus<SimpleClass>("de");

            Assert.That(bogus.Locale, Is.EqualTo("de"));
        }

        [Test]
        public void WithBogus_DefaultLocale_IsEnglish()
        {
            var bogus = Fixture.WithBogus<SimpleClass>();

            Assert.That(bogus.Locale, Is.EqualTo("en"));
        }
    }
}
