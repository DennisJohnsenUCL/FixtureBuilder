using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class NewTests
    {
        private FixtureFactory _factory;

        public class TestClass { }
        public interface ITestInterface { }
        public abstract class AbstractTestClass { }

        [SetUp]
        public void SetUp()
        {
            _factory = new FixtureFactory();
        }

        [Test]
        public void New_ReturnsFixtureConstructor()
        {
            var result = _factory.New<TestClass>();
            Assert.That(result, Is.InstanceOf<IFixtureConstructor<TestClass>>());
        }

        [Test]
        public void New_WithInstance_ReturnsFixtureConstructor()
        {
            var result = _factory.New(new TestClass());
            Assert.That(result, Is.InstanceOf<IFixtureConstructor<TestClass>>());
        }

        [Test]
        public void New_WithNullInstance_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _factory.New<TestClass>(null!));
        }

        [Test]
        public void New_InterfaceType_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => _factory.New<ITestInterface>());
        }

        [Test]
        public void New_AbstractType_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => _factory.New<AbstractTestClass>());
        }

        [Test]
        public void New_WithOptions_ReturnsFixtureConstructor()
        {
            var factory = new FixtureFactory(FixtureOptions.Default);
            var result = factory.New<TestClass>();
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void New_FixtureUsesFactoryContext()
        {
            var fixtureA = _factory.New<TestClass>();
            var fixtureB = _factory.New<TestClass>();

            Assert.That(TestHelper.GetContext(fixtureA), Is.SameAs(TestHelper.GetContext(fixtureB)));
        }

        [Test]
        public void New_WithInstance_FixtureUsesFactoryContext()
        {
            var fixtureA = _factory.New(new TestClass());
            var fixtureB = _factory.New(new TestClass());

            Assert.That(TestHelper.GetContext(fixtureA), Is.SameAs(TestHelper.GetContext(fixtureB)));
        }
    }
}
