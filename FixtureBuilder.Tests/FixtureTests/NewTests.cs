namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class NewTests
    {
        [Test]
        public void WithoutInstance_ReturnsFixtureConstructor()
        {
            var method = typeof(Fixture).GetMethods().Single(candidate => candidate.Name == nameof(Fixture.New) && candidate.GetParameters().Length == 0);

            var returnType = method.ReturnType.GetGenericTypeDefinition();

            Assert.That(returnType, Is.EqualTo(typeof(IFixtureConstructor<>)));
        }

        [Test]
        public void WithInstance_ReturnsFixtureConfigurator()
        {
            var method = typeof(Fixture).GetMethods().Single(candidate => candidate.Name == nameof(Fixture.New) && candidate.GetParameters().Length == 1);

            var returnType = method.ReturnType.GetGenericTypeDefinition();

            Assert.That(returnType, Is.EqualTo(typeof(IFixtureConfigurator<>)));
        }

        [Test]
        public void WithInstance_BuildsThatInstance()
        {
            var instance = new EmptyClass();

            var result = Fixture.New(instance).Build();

            Assert.That(result, Is.SameAs(instance));
        }

        [Test]
        public void InterfaceType_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<IMarkerInterface>());
        }

        [Test]
        public void AbstractType_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<AbstractClass>());
        }

        [Test]
        public void NullInstance_ThrowsArgumentNullException()
        {
            EmptyClass instance = null!;

            Assert.Throws<ArgumentNullException>(() => Fixture.New(instance));
        }

        [Test]
        public void FixtureInstance_ThrowsInvalidOperationException()
        {
            var instance = Fixture.New<EmptyClass>();

            Assert.Throws<InvalidOperationException>(() => Fixture.New(instance));
        }
    }
}
