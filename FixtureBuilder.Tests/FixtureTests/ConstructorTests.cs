using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class ConstructorTests
    {
        private IFixtureContext _context;

        public class TestClass { }
        public interface ITestInterface { }
        public abstract class AbstractTestClass { }

        [SetUp]
        public void SetUp()
        {
            _context = new Mock<IFixtureContext>().Object;
        }

        [Test]
        public void Constructor_NullContext_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Fixture<TestClass>((IFixtureContext)null!));
        }

        [Test]
        public void Constructor_InterfaceType_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => new Fixture<ITestInterface>(_context));
        }

        [Test]
        public void Constructor_AbstractType_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => new Fixture<AbstractTestClass>(_context));
        }

        [Test]
        public void Constructor_FixtureType_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => new Fixture<Fixture<TestClass>>(_context));
        }

        [Test]
        public void Constructor_ValidConcreteType_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new Fixture<TestClass>(_context));
        }

        [Test]
        public void InstanceConstructor_NullInstance_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Fixture<TestClass>(null!, _context));
        }

        [Test]
        public void InstanceConstructor_NullContext_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Fixture<TestClass>(new TestClass(), null!));
        }

        [Test]
        public void InstanceConstructor_FixtureInstance_Throws()
        {
            var fixtureInstance = new Fixture<TestClass>(_context);
            Assert.Throws<InvalidOperationException>(() => new Fixture<Fixture<TestClass>>(fixtureInstance, _context));
        }

        [Test]
        public void InstanceConstructor_ValidInstanceAndContext_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new Fixture<TestClass>(new TestClass(), _context));
        }
    }
}
