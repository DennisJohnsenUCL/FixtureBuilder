namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class NewTests
    {
        [Test]
        public void WithoutInstance_ReturnsFixtureConstructor()
        {
            var method = typeof(FixtureFactory).GetMethods().Single(candidate => candidate.Name == nameof(FixtureFactory.New) && candidate.GetParameters().Length == 0);

            var returnType = method.ReturnType.GetGenericTypeDefinition();

            Assert.That(returnType, Is.EqualTo(typeof(IFixtureConstructor<>)));
        }

        [Test]
        public void WithInstance_ReturnsFixtureConfigurator()
        {
            var method = typeof(FixtureFactory).GetMethods().Single(candidate => candidate.Name == nameof(FixtureFactory.New) && candidate.GetParameters().Length == 1);

            var returnType = method.ReturnType.GetGenericTypeDefinition();

            Assert.That(returnType, Is.EqualTo(typeof(IFixtureConfigurator<>)));
        }

        [Test]
        public void WithoutInstance_UsesFactoryConfiguration()
        {
            var factory = new FixtureFactory().With("injected");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        [Test]
        public void WithInstance_BuildsThatInstance()
        {
            var factory = new FixtureFactory();
            var instance = new ConstructorSetsValueClass();

            var result = factory.New(instance).Build();

            Assert.That(result, Is.SameAs(instance));
        }

        [Test]
        public void WithInstance_UsesFactoryConfiguration()
        {
            var factory = new FixtureFactory().With("injected");
            var instance = new PropertyHolder<StringConstructorClass>();

            var result = factory.New(instance).Instantiate(x => x.Value).Build();

            Assert.That(result.Value.Value, Is.EqualTo("injected"));
        }

        [Test]
        public void InterfaceType_ThrowsInvalidOperationException()
        {
            var factory = new FixtureFactory();

            Assert.Throws<InvalidOperationException>(() => factory.New<IMarkerInterface>());
        }

        [Test]
        public void AbstractType_ThrowsInvalidOperationException()
        {
            var factory = new FixtureFactory();

            Assert.Throws<InvalidOperationException>(() => factory.New<AbstractClass>());
        }

        [Test]
        public void NullInstance_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();
            ConstructorSetsValueClass instance = null!;

            var ex = Assert.Throws<ArgumentNullException>(() => factory.New(instance));

            Assert.That(ex.ParamName, Is.EqualTo("instance"));
        }
    }
}
