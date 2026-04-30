using Bogus;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Bogus.Tests
{
    public class BogusMemberInstantiatorTests
    {
        public class SimpleClass;

        private Mock<IConstructor<SimpleClass>> _mockConstructor = null!;
        private Faker _faker = null!;
        private BogusMemberInstantiator<SimpleClass> _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _mockConstructor = new Mock<IConstructor<SimpleClass>>();
            _faker = new Faker();
            _sut = new BogusMemberInstantiator<SimpleClass>(_mockConstructor.Object, _faker);
        }

        [Test]
        public void UseAutoConstructor_DelegatesToInnerConstructor()
        {
            var expected = new SimpleClass();
            _mockConstructor.Setup(c => c.UseAutoConstructor()).Returns(expected);

            var result = _sut.UseAutoConstructor();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void UseConstructor_WithArgs_DelegatesToInnerConstructor()
        {
            var expected = new SimpleClass();
            var args = new object[] { "Alice", 42 };
            _mockConstructor.Setup(c => c.UseConstructor(args)).Returns(expected);

            var result = _sut.UseConstructor(args);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void UseConstructor_WithFaker_ResolvesAndDelegates()
        {
            var expected = new SimpleClass();
            _mockConstructor.Setup(c => c.UseConstructor(It.IsAny<object[]>())).Returns(expected);

            var result = _sut.UseConstructor(f => [f.Name.FirstName()]);

            _mockConstructor.Verify(c => c.UseConstructor(It.Is<object[]>(a => a.Length == 1 && a[0] is string)), Times.Once);
            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void CreateUninitialized_DelegatesToInnerConstructor()
        {
            var expected = new SimpleClass();
            _mockConstructor.Setup(c => c.CreateUninitialized()).Returns(expected);

            var result = _sut.CreateUninitialized();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void CreateUninitialized_WithInitializeMembers_DelegatesToInnerConstructor()
        {
            var expected = new SimpleClass();
            _mockConstructor.Setup(c => c.CreateUninitialized(InitializeMembers.None)).Returns(expected);

            var result = _sut.CreateUninitialized(InitializeMembers.None);

            Assert.That(result, Is.SameAs(expected));
        }
    }
}
