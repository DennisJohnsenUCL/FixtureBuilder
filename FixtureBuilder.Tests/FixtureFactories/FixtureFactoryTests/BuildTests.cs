namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class BuildTests
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
        public void Build_ReturnsInstance()
        {
            var result = _factory.Build<TestClass>();
            Assert.That(result, Is.InstanceOf<TestClass>());
        }

        [Test]
        public void Build_InterfaceType_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => _factory.Build<ITestInterface>());
        }

        [Test]
        public void Build_AbstractType_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => _factory.Build<AbstractTestClass>());
        }
    }
}
