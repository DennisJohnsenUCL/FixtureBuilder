namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusRootConfigurationBuilderTests
{
    internal sealed class FakerProviderBuilderTests
    {
        #region With

        [Test]
        public void With_Faker_AppliesValueFromFactoryFaker()
        {
            var factory = FixtureFactory.WithBogus("de")
                .WhenBuilding<StringConstructorClass>(builder => builder.With<string>(faker => faker.Locale));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        [Test]
        public void With_Faker_IsInvokedForEachValue()
        {
            var calls = 0;
            var factory = FixtureFactory.WithBogus()
                .WhenBuilding<StringConstructorClass>(builder => builder.With<string>(_ => $"value {++calls}"));

            var first = factory.Build<StringConstructorClass>();
            var second = factory.Build<StringConstructorClass>();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(first.Value, Is.EqualTo("value 1"));
                Assert.That(second.Value, Is.EqualTo("value 2"));
            }
        }

        [Test]
        public void With_FakerAndName_AppliesValueFromFactoryFaker()
        {
            var factory = FixtureFactory.WithBogus("de")
                .WhenBuilding<StringConstructorClass>(builder => builder.With<string>(faker => faker.Locale, "value"));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        #endregion

        #region WithParameter

        [Test]
        public void WithParameter_Faker_AppliesValueFromFactoryFaker()
        {
            var factory = FixtureFactory.WithBogus("de")
                .WhenBuilding<StringConstructorClass>(builder => builder.WithParameter<string>(faker => faker.Locale));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        [Test]
        public void WithParameter_FakerAndName_AppliesValueFromFactoryFaker()
        {
            var factory = FixtureFactory.WithBogus("de")
                .WhenBuilding<StringConstructorClass>(builder => builder.WithParameter<string>(faker => faker.Locale, "value"));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        #endregion

        #region WithPropertyOrField

        [Test]
        public void WithPropertyOrField_Faker_AppliesValueFromFactoryFaker()
        {
            var factory = FixtureFactory.WithBogus("de")
                .WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField<string>(faker => faker.Locale));

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        [Test]
        public void WithPropertyOrField_Faker_IsInvokedForEachValue()
        {
            var calls = 0;
            var factory = FixtureFactory.WithBogus()
                .WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField<string>(_ => $"value {++calls}"));

            var first = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();
            var second = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(first.Value, Is.EqualTo("value 1"));
                Assert.That(second.Value, Is.EqualTo("value 2"));
            }
        }

        [Test]
        public void WithPropertyOrField_FakerAndName_AppliesValueFromFactoryFaker()
        {
            var factory = FixtureFactory.WithBogus("de")
                .WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField<string>(faker => faker.Locale, "Value"));

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        #endregion
    }
}
