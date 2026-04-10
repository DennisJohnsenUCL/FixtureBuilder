using FixtureBuilder.FixtureContexts;
using FixtureBuilder.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.FixtureContexts
{
    internal sealed class FixtureContextTests
    {
        private Mock<IContextResolver> _resolver;
        private FixtureOptions _options;
        private FixtureContext _sut;

        [SetUp]
        public void SetUp()
        {
            _resolver = new Mock<IContextResolver>(MockBehavior.Strict);
            _options = FixtureOptions.Default;
            _sut = new FixtureContext(_resolver.Object, _options);
        }

        [Test]
        public void Constructor_NullResolver_Throws()
        {
            Assert.That(() => new FixtureContext(null!, _options), Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_NullOptions_Throws()
        {
            Assert.That(() => new FixtureContext(_resolver.Object, null!), Throws.ArgumentNullException);
        }

        [Test]
        public void SetOptions_SetsOptions()
        {
            var options = FixtureOptions.Default;

            _sut.Options = options;

            Assert.That(_sut.Options, Is.SameAs(options));
        }

        [Test]
        public void SetOptions_Action_SetsOptions()
        {
            _sut.SetOptions(o => o.AllowPrivateConstructors = false);

            Assert.That(_sut.Options.AllowPrivateConstructors, Is.False);
        }

        [Test]
        public void SetOptions_Action_NullAction_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.SetOptions((Action<FixtureOptions>)null!));
        }

        [Test]
        public void InstantiateWithStrategy_UseAutoConstructor_ResolvesWithAutoConstructor()
        {
            var expected = "Test string value";
            var request = new FixtureRequest(typeof(string));
            var resolverMock = new Mock<IContextResolver>();
            resolverMock.Setup(r => r.AutoConstructingProvider.AutoResolve(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(expected);
            var options = new FixtureOptions();
            var context = new FixtureContext(resolverMock.Object, options);

            var result = context.InstantiateWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(result, Is.EqualTo(expected));
        }

        class DefaultCtorClass
        {
            public bool DefaultCtorCalled;
            public DefaultCtorClass() => DefaultCtorCalled = true;
        }
        [Test]
        public void InstantiateWithStrategy_UseDefaultConstructor_ResolvesWithDefaultConstructor()
        {
            var request = new FixtureRequest(typeof(DefaultCtorClass));
            var resolverMock = new Mock<IContextResolver>();
            var options = new FixtureOptions();
            var context = new FixtureContext(resolverMock.Object, options);

            var result = context.InstantiateWithStrategy(request, InstantiationMethod.UseDefaultConstructor, InitializeMembers.None);

            Assert.That(((DefaultCtorClass)result).DefaultCtorCalled, Is.True);
        }

        [Test]
        public void InstantiateWithStrategy_CreateUninitialized_ResolvesWithUninitializedProvider()
        {
            var expected = "Test string value";
            var request = new FixtureRequest(typeof(string));
            var capturedInitMembers = InitializeMembers.None;
            var resolverMock = new Mock<IContextResolver>();
            resolverMock.Setup(r => r.UninitializedProvider.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), It.IsAny<IFixtureContext>()))
                .Callback<FixtureRequest, InitializeMembers, IFixtureContext, RecursiveResolveContext>((_, init, _, _) => capturedInitMembers = init)
                .Returns(expected);
            var options = new FixtureOptions();
            var context = new FixtureContext(resolverMock.Object, options);

            var result = context.InstantiateWithStrategy(request, InstantiationMethod.CreateUninitialized, InitializeMembers.All);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.EqualTo(expected));
                Assert.That(capturedInitMembers, Is.EqualTo(InitializeMembers.All));
            }
        }
    }
}
