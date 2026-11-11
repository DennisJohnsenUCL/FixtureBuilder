namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusRootConfigurationBuilderTests
{
    internal sealed class PassthroughProviderBuilderTests
    {
        #region With

        [Test]
        public void With_Value_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WhenBuilding<StringConstructorClass>(builder => builder.With("scoped"));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void With_Func_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WhenBuilding<StringConstructorClass>(builder => builder.With(() => "scoped"));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void With_ValueAndName_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WhenBuilding<StringConstructorClass>(builder => builder.With("scoped", "value"));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void With_FuncAndName_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WhenBuilding<StringConstructorClass>(builder => builder.With(() => "scoped", "value"));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        #endregion

        #region WithParameter

        [Test]
        public void WithParameter_Value_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WhenBuilding<StringConstructorClass>(builder => builder.WithParameter("scoped"));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithParameter_Func_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WhenBuilding<StringConstructorClass>(builder => builder.WithParameter(() => "scoped"));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithParameter_ValueAndName_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WhenBuilding<StringConstructorClass>(builder => builder.WithParameter("scoped", "value"));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithParameter_FuncAndName_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WhenBuilding<StringConstructorClass>(builder => builder.WithParameter(() => "scoped", "value"));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        #endregion

        #region WithPropertyOrField

        [Test]
        public void WithPropertyOrField_Value_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField("scoped"));

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithPropertyOrField_Func_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField(() => "scoped"));

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithPropertyOrField_ValueAndName_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField("scoped", "Value"));

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithPropertyOrField_FuncAndName_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField(() => "scoped", "Value"));

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        #endregion
    }
}
