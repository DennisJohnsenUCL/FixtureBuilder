namespace FixtureBuilder.Bogus.Tests
{
    internal sealed class FixtureExtensionsTests
    {
        [Test]
        public void WithBogus_DefaultLocale_IsEnglish()
        {
            var fixture = Fixture.WithBogus<EmptyClass>();

            Assert.That(fixture.Locale, Is.EqualTo("en"));
        }

        [Test]
        public void WithBogus_Locale_UsesLocale()
        {
            var fixture = Fixture.WithBogus<EmptyClass>("de");

            Assert.That(fixture.Locale, Is.EqualTo("de"));
        }

        [Test]
        public void WithBogus_Instance_BuildReturnsInstance()
        {
            var instance = new EmptyClass();

            var result = Fixture.WithBogus(instance).Build();

            Assert.That(result, Is.SameAs(instance));
        }

        [Test]
        public void WithBogus_InstanceDefaultLocale_IsEnglish()
        {
            var instance = new EmptyClass();

            var fixture = Fixture.WithBogus(instance);

            Assert.That(fixture.Locale, Is.EqualTo("en"));
        }

        [Test]
        public void WithBogus_InstanceAndLocale_UsesLocale()
        {
            var instance = new EmptyClass();

            var fixture = Fixture.WithBogus(instance, "de");

            Assert.That(fixture.Locale, Is.EqualTo("de"));
        }
    }
}
