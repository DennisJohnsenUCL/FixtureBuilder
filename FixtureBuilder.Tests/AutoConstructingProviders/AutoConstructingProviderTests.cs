#pragma warning disable IDE0060
#pragma warning disable CS9113

using FixtureBuilder.AutoConstructingProviders;
using FixtureBuilder.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.AutoConstructingProviders
{
    [TestFixture]
    internal class AutoConstructingProviderTests
    {
        private AutoConstructingProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new AutoConstructingProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        public class SingleParam(string value)
        {
            public string Value { get; } = value;
        }

        #region Null guard tests

        [Test]
        public void AutoResolve_NullRequest_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.AutoResolve(null!, _contextMock.Object));
        }

        public class Parameterless
        {
            public bool Constructed { get; } = true;
        }
        [Test]
        public void AutoResolve_NullContext_ThrowsArgumentNullException()
        {
            var request = new FixtureRequest(typeof(Parameterless));

            Assert.Throws<ArgumentNullException>(() =>
                _sut.AutoResolve(request, null!));
        }

        #endregion

        #region Interface and abstract type tests

        public interface IInterfaceType { }
        [Test]
        public void AutoResolve_InterfaceType_ThrowsInvalidOperationException()
        {
            var request = new FixtureRequest(typeof(IInterfaceType));

            var ex = Assert.Throws<InvalidOperationException>(() =>
                _sut.AutoResolve(request, _contextMock.Object));

            Assert.That(ex!.Message, Does.Contain(nameof(IInterfaceType)));
        }

        public abstract class AbstractType { }
        [Test]
        public void AutoResolve_AbstractType_ThrowsInvalidOperationException()
        {
            var request = new FixtureRequest(typeof(AbstractType));

            var ex = Assert.Throws<InvalidOperationException>(() =>
                _sut.AutoResolve(request, _contextMock.Object));

            Assert.That(ex!.Message, Does.Contain(nameof(AbstractType)));
        }

        #endregion

        #region Parameter resolution tests

        [Test]
        public void AutoResolve_SingleParameter_ResolvesViaContext()
        {
            var request = new FixtureRequest(typeof(SingleParam));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(string))).Returns(typeof(string));
            _contextMock
                .Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(),
                    It.IsAny<IFixtureContext>()))
                .Returns("test-value");

            var result = (SingleParam)_sut.AutoResolve(request, _contextMock.Object);

            Assert.That(result.Value, Is.EqualTo("test-value"));
        }

        public class TwoParams(string first, int second)
        {
            public string First { get; } = first;
            public int Second { get; } = second;
        }
        [Test]
        public void AutoResolve_MultipleParameters_ResolvesEachViaContext()
        {
            var request = new FixtureRequest(typeof(TwoParams));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(string))).Returns(typeof(string));
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(int))).Returns(typeof(int));
            _contextMock
                .Setup(c => c.ResolveValue(It.Is<FixtureRequest>(p => p.Type == typeof(string)),
                    It.IsAny<IFixtureContext>()))
                .Returns("hello");
            _contextMock
                .Setup(c => c.ResolveValue(It.Is<FixtureRequest>(p => p.Type == typeof(int)),
                    It.IsAny<IFixtureContext>()))
                .Returns(42);

            var result = (TwoParams)_sut.AutoResolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.First, Is.EqualTo("hello"));
                Assert.That(result.Second, Is.EqualTo(42));
            }
        }

        #endregion

        public class MultipleConstructors
        {
            public int CtorParamCount { get; }

            public MultipleConstructors() => CtorParamCount = 0;
            public MultipleConstructors(string a) => CtorParamCount = 1;
            public MultipleConstructors(string a, int b) => CtorParamCount = 2;
        }
        [Test]
        public void AutoResolve_MultiplePublicConstructors_SelectsSimplest()
        {
            var request = new FixtureRequest(typeof(MultipleConstructors));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = (MultipleConstructors)_sut.AutoResolve(request, _contextMock.Object);

            Assert.That(result.CtorParamCount, Is.Zero);
        }

        public class PublicAndPrivateCtors
        {
            public bool PublicCtor = false;
            public bool PrivateCtor = false;

            private PublicAndPrivateCtors() => PrivateCtor = true;
            public PublicAndPrivateCtors(string a) => PublicCtor = true;
        }
        [Test]
        public void AutoResolve_AllowPrivate_PreferSimplest_SelectsPrivateCtor()
        {
            var request = new FixtureRequest(typeof(PublicAndPrivateCtors));
            var options = new FixtureOptions { AllowPrivateConstructors = true, PreferSimplestConstructor = true };
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = (PublicAndPrivateCtors)_sut.AutoResolve(request, _contextMock.Object);

            Assert.That(result.PrivateCtor, Is.True);
        }

        [Test]
        public void AutoResolve_AllowPrivate_DoNotPreferSimplest_SelectsPublicCtor()
        {
            var request = new FixtureRequest(typeof(PublicAndPrivateCtors));
            var options = new FixtureOptions { AllowPrivateConstructors = true, PreferSimplestConstructor = false };
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(string))).Returns(typeof(string));
            _contextMock
                .Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns("dummy");

            var result = (PublicAndPrivateCtors)_sut.AutoResolve(request, _contextMock.Object);

            Assert.That(result.PublicCtor, Is.True);
        }

        [Test]
        public void AutoResolve_DoNotAllowPrivate_PreferSimplest_SelectsPublicCtor()
        {
            var request = new FixtureRequest(typeof(PublicAndPrivateCtors));
            var options = new FixtureOptions { AllowPrivateConstructors = false, PreferSimplestConstructor = true };
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(string))).Returns(typeof(string));
            _contextMock
                .Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns("dummy");

            var result = (PublicAndPrivateCtors)_sut.AutoResolve(request, _contextMock.Object);

            Assert.That(result.PublicCtor, Is.True);
        }

        [Test]
        public void AutoResolve_DoNotAllowPrivate_DoNotPreferSimplest_SelectsPublicCtor()
        {
            var request = new FixtureRequest(typeof(PublicAndPrivateCtors));
            var options = new FixtureOptions { AllowPrivateConstructors = false, PreferSimplestConstructor = false };
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(string))).Returns(typeof(string));
            _contextMock
                .Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns("dummy");

            var result = (PublicAndPrivateCtors)_sut.AutoResolve(request, _contextMock.Object);

            Assert.That(result.PublicCtor, Is.True);
        }

        public class OnlyPrivateCtor
        {
            public bool PrivateCtor = false;

            private OnlyPrivateCtor() => PrivateCtor = true;
        }
        [Test]
        public void AutoResolve_OnlyPrivate_DoNotAllowPrivate_ThrowsException()
        {
            var request = new FixtureRequest(typeof(OnlyPrivateCtor));
            var options = new FixtureOptions { AllowPrivateConstructors = false };
            _contextMock.Setup(c => c.Options).Returns(options);

            Assert.Throws<InvalidOperationException>(() => _sut.AutoResolve(request, _contextMock.Object));
        }

        [Test]
        public void AutoResolve_OnlyPrivate_AllowPrivate_SelectsPrivateCtor()
        {
            var request = new FixtureRequest(typeof(OnlyPrivateCtor));
            var options = new FixtureOptions { AllowPrivateConstructors = true };
            _contextMock.Setup(c => c.Options).Returns(options);

            var result = (OnlyPrivateCtor)_sut.AutoResolve(request, _contextMock.Object);

            Assert.That(result.PrivateCtor, Is.True);
        }

        #region NoResult fallback and recursive resolution tests

        public class Inner
        {
            public bool Constructed { get; } = true;
        }
        public class Outer(Inner inner)
        {
            public Inner Inner { get; } = inner;
        }
        [Test]
        public void AutoResolve_ParameterReturnsNoResult_FallsBackToAutoResolve()
        {
            var request = new FixtureRequest(typeof(Outer));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(Inner))).Returns(typeof(Inner));
            _contextMock
                .Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(new NoResult());

            var result = (Outer)_sut.AutoResolve(request, _contextMock.Object);

            Assert.That(result.Inner.Constructed, Is.True);
        }

        [Test]
        public void AutoResolve_ParameterReturnsValue_DoesNotAutoResolveParameter()
        {
            var expected = new Inner();
            var request = new FixtureRequest(typeof(Outer));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(Inner))).Returns(typeof(Inner));
            _contextMock
                .Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(expected);

            var result = (Outer)_sut.AutoResolve(request, _contextMock.Object);

            Assert.That(result.Inner, Is.SameAs(expected));
        }

        #endregion

        #region UnwrapAndLink resolution tests

        public class LinkedParam(Inner inner)
        {
            public Inner Inner { get; } = inner;
        }
        [Test]
        public void AutoResolve_UnwrapAndLinkReturnsType_UsesLinkedTypeForRequest()
        {
            var request = new FixtureRequest(typeof(LinkedParam));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(Inner))).Returns(typeof(Inner));
            _contextMock
                .Setup(c => c.ResolveValue(It.Is<FixtureRequest>(r => r.Type == typeof(Inner)),
                    It.IsAny<IFixtureContext>()))
                .Returns(new Inner());

            _sut.AutoResolve(request, _contextMock.Object);

            _contextMock.Verify(c => c.UnwrapAndLink(typeof(Inner)), Times.Once);
        }

        [Test]
        public void AutoResolve_UnwrapAndLinkReturnsOriginalType_UsesOriginalParameterType()
        {
            var request = new FixtureRequest(typeof(LinkedParam));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(Inner))).Returns(typeof(Inner));
            _contextMock
                .Setup(c => c.ResolveValue(It.Is<FixtureRequest>(r => r.Type == typeof(Inner)),
                    It.IsAny<IFixtureContext>()))
                .Returns(new Inner());

            var result = (LinkedParam)_sut.AutoResolve(request, _contextMock.Object);

            Assert.That(result.Inner, Is.Not.Null);
        }

        #endregion

        #region Circular dependency tests

        public class CircularA(CircularB b) { }
        public class CircularB(CircularA a) { }

        [Test]
        public void AutoResolve_CircularDependency_ThrowsInvalidOperationException()
        {
            var request = new FixtureRequest(typeof(CircularA));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(It.IsAny<Type>())).Returns((Type t) => t);
            _contextMock
                .Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(new NoResult());

            Assert.Throws<InvalidOperationException>(() =>
                _sut.AutoResolve(request, _contextMock.Object));
        }

        public class SelfReferencing(SelfReferencing self) { }
        [Test]
        public void AutoResolve_SelfReferencingType_ThrowsInvalidOperationException()
        {
            var request = new FixtureRequest(typeof(SelfReferencing));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(It.IsAny<Type>())).Returns((Type t) => t);
            _contextMock
                .Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(new NoResult());

            Assert.Throws<InvalidOperationException>(() =>
                _sut.AutoResolve(request, _contextMock.Object));
        }

        #endregion
    }
}
