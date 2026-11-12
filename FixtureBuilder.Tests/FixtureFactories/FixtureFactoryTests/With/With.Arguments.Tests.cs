namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests.With
{
    internal sealed class WithArgumentsTests
    {
        #region With

        [Test]
        public void WithFunc_NullFunc_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();
            Func<string> func = null!;

            var ex = Assert.Throws<ArgumentNullException>(() => factory.With(func));

            Assert.That(ex.ParamName, Is.EqualTo("func"));
        }

        [Test]
        public void WithNamed_NullName_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.With("injected", null!));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        [Test]
        public void WithNamedFunc_NullFunc_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();
            Func<string> func = null!;

            var ex = Assert.Throws<ArgumentNullException>(() => factory.With(func, "value"));

            Assert.That(ex.ParamName, Is.EqualTo("func"));
        }

        [Test]
        public void WithNamedFunc_NullName_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.With(() => "injected", null!));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        #endregion

        #region WithParameter

        [Test]
        public void WithParameterFunc_NullFunc_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();
            Func<string> func = null!;

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WithParameter(func));

            Assert.That(ex.ParamName, Is.EqualTo("func"));
        }

        [Test]
        public void WithParameterNamed_NullName_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WithParameter("injected", null!));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        [Test]
        public void WithParameterNamedFunc_NullFunc_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();
            Func<string> func = null!;

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WithParameter(func, "value"));

            Assert.That(ex.ParamName, Is.EqualTo("func"));
        }

        [Test]
        public void WithParameterNamedFunc_NullName_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WithParameter(() => "injected", null!));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        #endregion

        #region WithPropertyOrField

        [Test]
        public void WithPropertyOrFieldFunc_NullFunc_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();
            Func<string> func = null!;

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WithPropertyOrField(func));

            Assert.That(ex.ParamName, Is.EqualTo("func"));
        }

        [Test]
        public void WithPropertyOrFieldNamed_NullName_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WithPropertyOrField("injected", null!));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        [Test]
        public void WithPropertyOrFieldNamedFunc_NullFunc_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();
            Func<string> func = null!;

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WithPropertyOrField(func, "Value"));

            Assert.That(ex.ParamName, Is.EqualTo("func"));
        }

        [Test]
        public void WithPropertyOrFieldNamedFunc_NullName_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WithPropertyOrField(() => "injected", null!));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        #endregion
    }
}
