namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusFixtureFactoryTests
{
    internal sealed class PassthroughProviderBuilderTests
    {
        #region With

        [Test]
        public void With_Value_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().With("provided");

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void With_Func_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().With(() => "provided");

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void With_ValueAndName_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().With("provided", "value");

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void With_FuncAndName_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().With(() => "provided", "value");

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void With_ReturnsSameFactory()
        {
            var factory = FixtureFactory.WithBogus();

            var result = factory.With("value");

            Assert.That(result, Is.SameAs(factory));
        }

        #endregion

        #region WithParameter

        [Test]
        public void WithParameter_Value_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WithParameter("provided");

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void WithParameter_Func_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WithParameter(() => "provided");

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void WithParameter_ValueAndName_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WithParameter("provided", "value");

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void WithParameter_FuncAndName_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WithParameter(() => "provided", "value");

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        #endregion

        #region WithPropertyOrField

        [Test]
        public void WithPropertyOrField_Value_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WithPropertyOrField("provided");

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void WithPropertyOrField_Func_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WithPropertyOrField(() => "provided");

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void WithPropertyOrField_ValueAndName_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WithPropertyOrField("provided", "Value");

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void WithPropertyOrField_FuncAndName_AppliesValue()
        {
            var factory = FixtureFactory.WithBogus().WithPropertyOrField(() => "provided", "Value");

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        #endregion
    }
}
