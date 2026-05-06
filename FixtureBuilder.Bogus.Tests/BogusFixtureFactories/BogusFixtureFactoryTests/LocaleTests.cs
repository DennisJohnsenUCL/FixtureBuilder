namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusFixtureFactoryTests
{
    internal sealed class LocaleTests
    {
        [Test]
        public void Locale_CanBeChanged()
        {
            var factory = FixtureFactory.WithBogus();
            factory.Locale = "de";

            Assert.That(factory.Locale, Is.EqualTo("de"));
        }
    }
}
