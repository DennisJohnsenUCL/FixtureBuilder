using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Creation.UninitializedProviders
{
    internal class RootUninitializedProviderTests
    {
        private Mock<IMemberInitializer> _memberInitializer;
        private RootUninitializedProvider _sut;

        public class SimpleClass
        {
            public string? Name { get; set; }
        }

        public class Parent
        {
            public SimpleClass? Child { get; set; }
        }

        public class ClassWithConstructorSideEffect
        {
            public bool ConstructorWasCalled { get; }

            public ClassWithConstructorSideEffect()
            {
                ConstructorWasCalled = true;
            }
        }

        public interface ISimpleInterface { }

        public abstract class AbstractClass { }

        [SetUp]
        public void SetUp()
        {
            _memberInitializer = new Mock<IMemberInitializer>();
            _sut = new RootUninitializedProvider(_memberInitializer.Object);
        }

        #region Constructor

        [Test]
        public void Constructor_ValidArguments_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new RootUninitializedProvider(_memberInitializer.Object));
        }

        [Test]
        public void Constructor_NullMemberInitializer_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new RootUninitializedProvider(null!));

            Assert.That(ex!.ParamName, Is.EqualTo("memberInitializer"));
        }

        #endregion

        #region ResolveUninitialized

        [Test]
        public void ResolveUninitialized_NullRequest_ThrowsArgumentNullException()
        {
            var context = Mock.Of<IFixtureContext>();

            var ex = Assert.Throws<ArgumentNullException>(() =>
                _sut.ResolveUninitialized(null!, InitializeMembers.None, context));

            Assert.That(ex!.ParamName, Is.EqualTo("request"));
        }

        [Test]
        public void ResolveUninitialized_NullContext_ThrowsArgumentNullException()
        {
            var request = new FixtureRequest(typeof(SimpleClass));

            var ex = Assert.Throws<ArgumentNullException>(() =>
                _sut.ResolveUninitialized(request, InitializeMembers.None, null!));

            Assert.That(ex!.ParamName, Is.EqualTo("context"));
        }

        [Test]
        public void ResolveUninitialized_SimpleClass_ReturnsUninitializedInstance()
        {
            var request = new FixtureRequest(typeof(ClassWithConstructorSideEffect));
            var context = Mock.Of<IFixtureContext>();

            var result = _sut.ResolveUninitialized(request, InitializeMembers.None, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(((ClassWithConstructorSideEffect)result!).ConstructorWasCalled, Is.False);
            }
        }

        [Test]
        public void ResolveUninitialized_InitializeMembersNone_DoesNotCallMemberInitializer()
        {
            var request = new FixtureRequest(typeof(SimpleClass));
            var context = Mock.Of<IFixtureContext>();

            _sut.ResolveUninitialized(request, InitializeMembers.None, context);

            _memberInitializer.Verify(
                m => m.InitializeMembers(It.IsAny<object>(), It.IsAny<InitializeMembers>(), It.IsAny<Type>(), It.IsAny<IFixtureContext>(), It.IsAny<RecursiveResolveContext>()),
                Times.Never);
        }

        [Test]
        public void ResolveUninitialized_InitializeMembersAll_CallsMemberInitializer()
        {
            var request = new FixtureRequest(typeof(SimpleClass));
            var context = Mock.Of<IFixtureContext>();

            _sut.ResolveUninitialized(request, InitializeMembers.All, context);

            _memberInitializer.Verify(
                m => m.InitializeMembers(It.IsAny<SimpleClass>(), InitializeMembers.All, typeof(SimpleClass), context, It.IsAny<RecursiveResolveContext>()),
                Times.Once);
        }

        [Test]
        public void ResolveUninitialized_TypeThatCannotBeCreatedUninitialized_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(string));
            var context = Mock.Of<IFixtureContext>();

            var result = _sut.ResolveUninitialized(request, InitializeMembers.None, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void ResolveUninitialized_InterfaceType_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(ISimpleInterface));
            var context = Mock.Of<IFixtureContext>();

            var result = _sut.ResolveUninitialized(request, InitializeMembers.None, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void ResolveUninitialized_AbstractType_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(AbstractClass));
            var context = Mock.Of<IFixtureContext>();

            var result = _sut.ResolveUninitialized(request, InitializeMembers.None, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
        #endregion

        [Test]
        public void ResolveUninitialized_PassesRootTypeToMemberInitializer()
        {
            var rootType = typeof(Parent);
            var request = new FixtureRequest(typeof(SimpleClass), typeof(Parent).GetProperty(nameof(Parent.Child))!, rootType);
            var context = Mock.Of<IFixtureContext>();

            _sut.ResolveUninitialized(request, InitializeMembers.All, context);

            _memberInitializer.Verify(
                m => m.InitializeMembers(It.IsAny<SimpleClass>(), InitializeMembers.All, rootType, context, It.IsAny<RecursiveResolveContext>()),
                Times.Once);
        }
    }
}
