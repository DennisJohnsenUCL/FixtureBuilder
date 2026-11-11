namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class FakerConstructionTests
    {
        #region UseConstructor

        [Test]
        public void UseConstructor_Faker_PassesArgumentsFromFixtureFaker()
        {
            var fixture = Fixture.WithBogus<StringConstructorClass>("de");

            var result = fixture.UseConstructor(faker => [faker.Locale]).Build();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        [Test]
        public void UseConstructor_Faker_IsInvokedForEachBuild()
        {
            var calls = 0;
            var fixture = Fixture.WithBogus<StringConstructorClass>().UseConstructor(_ => [$"value {++calls}"]);

            var result = fixture.Build(2);

            var values = result.Select(instance => instance.Value);
            Assert.That(values, Is.EqualTo(["value 1", "value 2"]));
        }

        #endregion

        #region UseCustomInstantiator

        [Test]
        public void UseCustomInstantiator_Faker_UsesInstanceCreatedWithFixtureFaker()
        {
            var fixture = Fixture.WithBogus<StringConstructorClass>("de");

            var result = fixture.UseCustomInstantiator(faker => new StringConstructorClass(faker.Locale)).Build();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        [Test]
        public void UseCustomInstantiator_Faker_IsInvokedForEachBuild()
        {
            var calls = 0;
            var fixture = Fixture.WithBogus<StringConstructorClass>()
                .UseCustomInstantiator(_ => new StringConstructorClass($"value {++calls}"));

            var result = fixture.Build(2);

            var values = result.Select(instance => instance.Value);
            Assert.That(values, Is.EqualTo(["value 1", "value 2"]));
        }

        #endregion
    }
}
