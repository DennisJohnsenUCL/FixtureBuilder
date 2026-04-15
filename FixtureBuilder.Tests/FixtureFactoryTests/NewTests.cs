using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Tests.FixtureFactoryTests
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
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void New_WithInstance_ReturnsFixtureConstructor()
        {
            var instance = new TestClass();
            var result = _factory.New(instance);
            Assert.That(result, Is.Not.Null);
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
    }
}
