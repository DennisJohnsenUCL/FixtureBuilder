using FixtureBuilder.FixtureContexts;
using FixtureBuilder.MemberInstantiation;
using FixtureBuilder.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.MemberInstantiators
{
    internal class MemberInstantiatorTests
    {
        private Mock<IFixtureContext> _contextMock = null!;
        private MemberInstantiator<TestClass> _sut = null!;

        class TestClass
        {
            public string Value { get; set; } = null!;
        }

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IFixtureContext>();
            _sut = new MemberInstantiator<TestClass>(_contextMock.Object);
        }

        [Test]
        public void UseAutoConstructor_CallsAutoResolve()
        {
            var expected = new TestClass();
            _contextMock
                .Setup(c => c.AutoResolve(It.Is<FixtureRequest>(r => r.Type == typeof(TestClass)), _contextMock.Object))
                .Returns(expected);

            var result = _sut.UseAutoConstructor();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void UseAutoConstructor_PassesContextAsDependency()
        {
            _contextMock
                .Setup(c => c.AutoResolve(It.IsAny<FixtureRequest>(), _contextMock.Object))
                .Returns(new TestClass());

            _sut.UseAutoConstructor();

            _contextMock.Verify(c => c.AutoResolve(It.IsAny<FixtureRequest>(), _contextMock.Object), Times.Once);
        }

        [Test]
        public void UseConstructor_ReturnsInstance()
        {
            var result = _sut.UseConstructor();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CreateUninitialized_CallsResolveUninitialized()
        {
            var expected = new TestClass();
            _contextMock
                .Setup(c => c.ResolveUninitialized(
                    It.Is<FixtureRequest>(r => r.Type == typeof(TestClass)),
                    InitializeMembers.None,
                    _contextMock.Object))
                .Returns(expected);

            var result = _sut.CreateUninitialized();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void CreateUninitialized_DefaultsToNone()
        {
            _contextMock
                .Setup(c => c.ResolveUninitialized(It.IsAny<FixtureRequest>(), InitializeMembers.None, _contextMock.Object))
                .Returns(new TestClass());

            _sut.CreateUninitialized();

            _contextMock.Verify(c => c.ResolveUninitialized(
                It.IsAny<FixtureRequest>(),
                InitializeMembers.None,
                _contextMock.Object), Times.Once);
        }

        [Test]
        public void CreateUninitialized_WithInitializeMembers_PassesValue()
        {
            var expected = new TestClass();
            _contextMock
                .Setup(c => c.ResolveUninitialized(
                    It.IsAny<FixtureRequest>(),
                    InitializeMembers.All,
                    _contextMock.Object))
                .Returns(expected);

            var result = _sut.CreateUninitialized(InitializeMembers.All);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void CreateUninitialized_NullResult_Throws()
        {
            _contextMock
                .Setup(c => c.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), _contextMock.Object))
                .Returns((object?)null);

            Assert.Throws<InvalidOperationException>(() => _sut.CreateUninitialized());
        }
    }
}
