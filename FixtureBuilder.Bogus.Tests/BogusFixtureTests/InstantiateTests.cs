namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class InstantiateTests
    {
        #region Without constructor function

        [Test]
        public void NoConstructorFunction_SetsAutoConstructedInstance()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value).Build();

            Assert.That(result.Value.Value, Is.EqualTo("constructor"));
        }

        #endregion

        #region Faker constructor functions

        [Test]
        public void UseConstructorWithFaker_PassesArgumentsFromFixtureFaker()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<StringConstructorClass>>("de");

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseConstructor(faker => [faker.Locale])).Build();

            Assert.That(result.Value.Value, Is.EqualTo("de"));
        }

        [Test]
        public void UseConstructorWithFaker_IsInvokedForEachBuild()
        {
            var calls = 0;
            var fixture = Fixture.WithBogus<PropertyHolder<StringConstructorClass>>()
                .Instantiate(x => x.Value, constructor => constructor.UseConstructor(_ => [$"value {++calls}"]));

            var result = fixture.Build(2);

            var values = result.Select(holder => holder.Value.Value);
            Assert.That(values, Is.EqualTo(["value 1", "value 2"]));
        }

        [Test]
        public void UseCustomInstantiatorWithFaker_SetsInstanceCreatedWithFixtureFaker()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<StringConstructorClass>>("de");

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseCustomInstantiator(faker => new StringConstructorClass(faker.Locale))).Build();

            Assert.That(result.Value.Value, Is.EqualTo("de"));
        }

        #endregion

        #region Passthrough constructor functions

        [Test]
        public void UseAutoConstructor_SetsAutoConstructedInstance()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseAutoConstructor()).Build();

            Assert.That(result.Value.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void UseConstructor_SetsInstanceConstructedWithArguments()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<StringConstructorClass>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseConstructor("provided")).Build();

            Assert.That(result.Value.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void CreateUninitialized_SetsUninitializedInstance()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.CreateUninitialized()).Build();

            Assert.That(result.Value.Value, Is.Null);
        }

        [Test]
        public void CreateUninitializedWithMembers_SetsInstanceWithInitializedMembers()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.CreateUninitialized(InitializeMembers.All)).Build();

            Assert.That(result.Value.Value, Is.EqualTo("Value"));
        }

        [Test]
        public void UseCustomInstantiator_SetsCreatedInstance()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<StringConstructorClass>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseCustomInstantiator(() => new StringConstructorClass("provided"))).Build();

            Assert.That(result.Value.Value, Is.EqualTo("provided"));
        }

        #endregion
    }
}
