using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Creation.UninitializedProviders
{
    internal class RootUninitializedProviderTests
    {
        private readonly Type _rootType = typeof(object);
        private Mock<IMemberInitializer> _memberInitializer;
        private RootUninitializedProvider _sut;

        public class SimpleClass
        {
            public string? Name { get; set; }
        }

        public class ClassWithConstructorSideEffect
        {
            public bool ConstructorWasCalled { get; }

            public ClassWithConstructorSideEffect()
            {
                ConstructorWasCalled = true;
            }
        }

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
            var request = new FixtureRequest(typeof(SimpleClass), this, _rootType, null);

            var ex = Assert.Throws<ArgumentNullException>(() =>
                _sut.ResolveUninitialized(request, InitializeMembers.None, null!));

            Assert.That(ex!.ParamName, Is.EqualTo("context"));
        }

        [Test]
        public void ResolveUninitialized_SimpleClass_ReturnsUninitializedInstance()
        {
            var request = new FixtureRequest(typeof(ClassWithConstructorSideEffect), this, _rootType, null);
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
            var request = new FixtureRequest(typeof(SimpleClass), this, _rootType, null);
            var context = Mock.Of<IFixtureContext>();

            _sut.ResolveUninitialized(request, InitializeMembers.None, context);

            _memberInitializer.Verify(
                m => m.InitializeMembers(It.IsAny<object>(), It.IsAny<InitializeMembers>(), _rootType, It.IsAny<IFixtureContext>(), It.IsAny<RecursiveResolveContext>()),
                Times.Never);
        }

        [Test]
        public void ResolveUninitialized_InitializeMembersAll_CallsMemberInitializer()
        {
            var request = new FixtureRequest(typeof(SimpleClass), this, _rootType, null);
            var context = Mock.Of<IFixtureContext>();

            _sut.ResolveUninitialized(request, InitializeMembers.All, context);

            _memberInitializer.Verify(
                m => m.InitializeMembers(It.IsAny<SimpleClass>(), InitializeMembers.All, _rootType, context, It.IsAny<RecursiveResolveContext>()),
                Times.Once);
        }

        [Test]
        public void ResolveUninitialized_TypeThatCannotBeCreatedUninitialized_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(string), this, _rootType, null);
            var context = Mock.Of<IFixtureContext>();

            var result = _sut.ResolveUninitialized(request, InitializeMembers.None, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
        #endregion

        [Test]
        public void ResolveUninitialized_PassesRootTypeToMemberInitializer()
        {
            var rootType = typeof(string);
            var request = new FixtureRequest(typeof(SimpleClass), this, rootType, null);
            var context = Mock.Of<IFixtureContext>();

            _sut.ResolveUninitialized(request, InitializeMembers.All, context);

            _memberInitializer.Verify(
                m => m.InitializeMembers(It.IsAny<SimpleClass>(), InitializeMembers.All, rootType, context, It.IsAny<RecursiveResolveContext>()),
                Times.Once);
        }
    }
}
