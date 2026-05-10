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

        [Test]
        public void WithBogus_Instance_ReturnsConfigurator()
        {
            var instance = new SimpleClass();

            var bogus = Fixture.WithBogus(instance);

            Assert.That(bogus, Is.InstanceOf<IBogusFixtureConfigurator<SimpleClass>>());
        }

        [Test]
        public void WithBogus_Instance_BuildReturnsSameInstance()
        {
            var instance = new SimpleClass();

            var result = Fixture.WithBogus(instance).Build();

            Assert.That(result, Is.SameAs(instance));
        }

        [Test]
        public void WithBogus_InstanceWithLocale_UsesLocale()
        {
            var instance = new SimpleClass();

            var bogus = Fixture.WithBogus(instance, "de");

            Assert.That(bogus.Locale, Is.EqualTo("de"));
        }
    }
}
