namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class SetOptionsTests
    {
        [Test]
        public void ChangesOptions()
        {
            var factory = new FixtureFactory();

            factory.SetOptions(options => options.AllowPrivateConstructors = false);

            Assert.That(factory.Options.AllowPrivateConstructors, Is.False);
        }

        [Test]
        public void FixturesUseChangedOptions()
        {
            var factory = new FixtureFactory();
            factory.SetOptions(options => options.AllowPrivateConstructors = false);
            var fixture = factory.New<PrivateConstructorClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.Build());
        }

        [Test]
        public void NullAction_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.SetOptions(null!));

            Assert.That(ex.ParamName, Is.EqualTo("action"));
        }
    }
}
