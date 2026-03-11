using FixtureBuilder.FixtureContexts;
using FixtureBuilder.TypeLinks;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.ValueConverters;
using Moq;

namespace FixtureBuilder.Tests.FixtureContexts
{
    internal sealed class FixtureContextTests
    {
        private Mock<IContextResolver> _resolver;
        private FixtureContext _sut;

        [SetUp]
        public void SetUp()
        {
            _resolver = new Mock<IContextResolver>(MockBehavior.Strict);
            _sut = new FixtureContext(_resolver.Object);
        }

        [Test]
        public void Constructor_NullResolver_Throws()
        {
            Assert.That(() => new FixtureContext(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Convert_DelegatesToResolverConverter()
        {
            var target = typeof(string);
            var value = new object();
            var context = Mock.Of<IFixtureContext>();
            var expected = "converted";

            var converter = new Mock<IValueConverter>(MockBehavior.Strict);
            converter.Setup(c => c.Convert(target, value, context)).Returns(expected);
            _resolver.Setup(r => r.GetConverter()).Returns(converter.Object);

            var result = _sut.Convert(target, value, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.EqualTo(expected));
                converter.Verify(c => c.Convert(target, value, context), Times.Once);
            }
        }

        [Test]
        public void Convert_ConverterReturnsNull_ReturnsNull()
        {
            var converter = new Mock<IValueConverter>(MockBehavior.Strict);
            converter.Setup(c => c.Convert(It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<IFixtureContext>())).Returns((object?)null);
            _resolver.Setup(r => r.GetConverter()).Returns(converter.Object);

            var result = _sut.Convert(typeof(int), new object(), Mock.Of<IFixtureContext>());

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Link_DelegatesToResolverTypeLink()
        {
            var target = typeof(IList<string>);
            var expected = typeof(List<string>);

            var typeLink = new Mock<ITypeLink>(MockBehavior.Strict);
            typeLink.Setup(t => t.Link(target)).Returns(expected);
            _resolver.Setup(r => r.GetTypeLink()).Returns(typeLink.Object);

            var result = _sut.Link(target);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.EqualTo(expected));
                typeLink.Verify(t => t.Link(target), Times.Once);
            }
        }

        [Test]
        public void Link_TypeLinkReturnsNull_ReturnsNull()
        {
            var typeLink = new Mock<ITypeLink>(MockBehavior.Strict);
            typeLink.Setup(t => t.Link(It.IsAny<Type>())).Returns((Type?)null);
            _resolver.Setup(r => r.GetTypeLink()).Returns(typeLink.Object);

            var result = _sut.Link(typeof(object));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveUninitialized_DelegatesToResolverUninitializedProvider()
        {
            var request = new FixtureRequest(typeof(string));
            var initializeMembers = InitializeMembers.All;
            var context = Mock.Of<IFixtureContext>();
            var expected = "resolved";

            var provider = new Mock<IFixtureUninitializedProvider>(MockBehavior.Strict);
            provider.Setup(p => p.ResolveUninitialized(request, initializeMembers, context)).Returns(expected);
            _resolver.Setup(r => r.GetUninitializedProvider()).Returns(provider.Object);

            var result = _sut.ResolveUninitialized(request, initializeMembers, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.EqualTo(expected));
                provider.Verify(p => p.ResolveUninitialized(request, initializeMembers, context), Times.Once);
            }
        }

        [Test]
        public void ResolveUninitialized_ProviderReturnsNull_ReturnsNull()
        {
            var provider = new Mock<IFixtureUninitializedProvider>(MockBehavior.Strict);
            provider.Setup(p => p.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), It.IsAny<IFixtureContext>())).Returns((object?)null);
            _resolver.Setup(r => r.GetUninitializedProvider()).Returns(provider.Object);

            var result = _sut.ResolveUninitialized(new FixtureRequest(typeof(int)), InitializeMembers.None, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.Null);
        }
    }
}
