using FixtureBuilder.Configuration;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Configuration
{
    internal class MemberInstantiatorTests
    {
        private Mock<IFixtureContext> _contextMock = null!;
        private Mock<IAutoConstructingProvider> _autoConstructingMock = null!;
        private Mock<IConstructingProvider> _constructingMock = null!;
        private Mock<IRootUninitializedProvider> _uninitializedMock = null!;
        private MemberInstantiator<TestClass> _sut = null!;

        class TestClass
        {
            public string Value { get; set; } = null!;
        }

        class Root;

        [SetUp]
        public void SetUp()
        {
            _autoConstructingMock = new Mock<IAutoConstructingProvider>();
            _constructingMock = new Mock<IConstructingProvider>();
            _uninitializedMock = new Mock<IRootUninitializedProvider>();

            _contextMock = new Mock<IFixtureContext>();
            _contextMock.Setup(c => c.AutoConstructingProvider).Returns(_autoConstructingMock.Object);
            _contextMock.Setup(c => c.ConstructingProvider).Returns(_constructingMock.Object);
            _contextMock.Setup(c => c.UninitializedProvider).Returns(_uninitializedMock.Object);
            _contextMock.Setup(c => c.UnwrapAndLink(It.IsAny<Type>(), It.IsAny<Type>())).Returns((Type type, Type _) => type);

            _sut = new MemberInstantiator<TestClass>(typeof(Root), _contextMock.Object);
        }

        [Test]
        public void UseAutoConstructor_CallsAutoResolve()
        {
            var expected = new TestClass();
            _autoConstructingMock
                .Setup(c => c.AutoResolve(It.Is<TypeRequest>(r => r.Type == typeof(TestClass)), _contextMock.Object))
                .Returns(expected);

            var result = _sut.UseAutoConstructor();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void UseAutoConstructor_PassesContextAsDependency()
        {
            _autoConstructingMock
                .Setup(c => c.AutoResolve(It.IsAny<TypeRequest>(), _contextMock.Object))
                .Returns(new TestClass());

            _sut.UseAutoConstructor();

            _autoConstructingMock.Verify(c => c.AutoResolve(It.IsAny<TypeRequest>(), _contextMock.Object), Times.Once);
        }

        [Test]
        public void UseAutoConstructor_RequestCarriesRootType()
        {
            _sut.UseAutoConstructor();

            _autoConstructingMock.Verify(c => c.AutoResolve(
                It.Is<TypeRequest>(r => r.RootType == typeof(Root)),
                _contextMock.Object), Times.Once);
        }

        [Test]
        public void UseConstructor_ReturnsInstance()
        {
            var expected = new TestClass();
            _constructingMock
                .Setup(c => c.ResolveWithArguments(It.IsAny<Type>(), It.IsAny<object?[]>()))
                .Returns(expected);

            var result = _sut.UseConstructor();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void UseConstructor_PassesArgumentsToContext()
        {
            var args = new object?[] { "arg1", 42 };
            var expected = new TestClass();
            _constructingMock
                .Setup(c => c.ResolveWithArguments(It.IsAny<Type>(), args))
                .Returns(expected);

            var result = _sut.UseConstructor(args);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void CreateUninitialized_CallsResolveUninitialized()
        {
            var expected = new TestClass();
            _uninitializedMock
                .Setup(c => c.ResolveUninitialized(
                    It.Is<TypeRequest>(r => r.Type == typeof(TestClass)),
                    InitializeMembers.None,
                    _contextMock.Object))
                .Returns(expected);

            var result = _sut.CreateUninitialized();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void CreateUninitialized_RequestCarriesRootType()
        {
            _uninitializedMock
                .Setup(c => c.ResolveUninitialized(It.IsAny<TypeRequest>(), It.IsAny<InitializeMembers>(), _contextMock.Object))
                .Returns(new TestClass());

            _sut.CreateUninitialized();

            _uninitializedMock.Verify(c => c.ResolveUninitialized(
                It.Is<TypeRequest>(r => r.RootType == typeof(Root)),
                It.IsAny<InitializeMembers>(),
                _contextMock.Object), Times.Once);
        }

        class LinkedTestClass : TestClass;

        [Test]
        public void UseAutoConstructor_UsesLinkedType()
        {
            var expected = new LinkedTestClass();
            _contextMock
                .Setup(c => c.UnwrapAndLink(typeof(TestClass), typeof(Root)))
                .Returns(typeof(LinkedTestClass));

            _autoConstructingMock
                .Setup(c => c.AutoResolve(It.Is<TypeRequest>(r => r.Type == typeof(LinkedTestClass)), _contextMock.Object))
                .Returns(expected);

            var result = _sut.UseAutoConstructor();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void UseConstructor_DoesNotLinkType()
        {
            var expected = new TestClass();
            _contextMock
                .Setup(c => c.UnwrapAndLink(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(typeof(LinkedTestClass));

            _constructingMock
                .Setup(c => c.ResolveWithArguments(typeof(TestClass), It.IsAny<object?[]>()))
                .Returns(expected);

            var result = _sut.UseConstructor();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void CreateUninitialized_UsesLinkedType()
        {
            var expected = new LinkedTestClass();
            _contextMock
                .Setup(c => c.UnwrapAndLink(typeof(TestClass), typeof(Root)))
                .Returns(typeof(LinkedTestClass));

            _uninitializedMock
                .Setup(c => c.ResolveUninitialized(
                    It.Is<TypeRequest>(r => r.Type == typeof(LinkedTestClass)),
                    InitializeMembers.None,
                    _contextMock.Object))
                .Returns(expected);

            var result = _sut.CreateUninitialized();

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void CreateUninitialized_DefaultsToNone()
        {
            _uninitializedMock
                .Setup(c => c.ResolveUninitialized(It.IsAny<TypeRequest>(), InitializeMembers.None, _contextMock.Object))
                .Returns(new TestClass());

            _sut.CreateUninitialized();

            _uninitializedMock.Verify(c => c.ResolveUninitialized(
                It.IsAny<TypeRequest>(),
                InitializeMembers.None,
                _contextMock.Object), Times.Once);
        }

        [Test]
        public void CreateUninitialized_WithInitializeMembers_PassesValue()
        {
            var expected = new TestClass();
            _uninitializedMock
                .Setup(c => c.ResolveUninitialized(
                    It.IsAny<TypeRequest>(),
                    InitializeMembers.All,
                    _contextMock.Object))
                .Returns(expected);

            var result = _sut.CreateUninitialized(InitializeMembers.All);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void CreateUninitialized_NoResult_Throws()
        {
            _uninitializedMock
                .Setup(c => c.ResolveUninitialized(It.IsAny<TypeRequest>(), It.IsAny<InitializeMembers>(), _contextMock.Object))
                .Returns(NoResult.Instance);

            Assert.Throws<InvalidOperationException>(() => _sut.CreateUninitialized());
        }

        [Test]
        public void UseCustomInstantiator_ReturnsFactoryResult()
        {
            var expected = new TestClass { Value = "custom" };

            var result = _sut.UseCustomInstantiator(() => expected);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void UseCustomInstantiator_DoesNotCallAnyProvider()
        {
            _sut.UseCustomInstantiator(() => new TestClass());

            _autoConstructingMock.VerifyNoOtherCalls();
            _constructingMock.VerifyNoOtherCalls();
            _uninitializedMock.VerifyNoOtherCalls();
        }

        [Test]
        public void UseCustomInstantiator_ResultNull_ThrowsException()
        {
            var expected = new TestClass { Value = "custom" };

            Assert.Throws<InvalidOperationException>(() => _sut.UseCustomInstantiator(() => null!));
        }
    }
}
