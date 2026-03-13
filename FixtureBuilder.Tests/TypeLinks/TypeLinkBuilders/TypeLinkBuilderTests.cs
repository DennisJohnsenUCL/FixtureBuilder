using FixtureBuilder.TypeLinks;
using FixtureBuilder.TypeLinks.TypeLinkBuilders;
using Moq;

namespace FixtureBuilder.Tests.TypeLinks.TypeLinkBuilders
{
    internal class TypeLinkBuilderTests
    {
        private TypeLinkBuilder _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new TypeLinkBuilder();
        }

        #region Constructor

        [Test]
        public void Constructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new TypeLinkBuilder());
        }

        #endregion

        #region WithStrategies

        [Test]
        public void WithStrategies_ReturnsCompositeTypeLinkBuilder()
        {
            var result = _sut.WithStrategies();

            Assert.That(result, Is.InstanceOf<CompositeTypeLinkBuilder>());
        }

        #endregion

        #region WithDecorator

        [Test]
        public void WithDecorator_NullTypeLink_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _sut.WithDecorator(null!));

            Assert.That(ex!.ParamName, Is.EqualTo("typeLink"));
        }

        [Test]
        public void WithDecorator_ValidTypeLink_ReturnsSameBuilder()
        {
            var typeLink = Mock.Of<ITypeLink>();

            var result = _sut.WithDecorator(typeLink);

            Assert.That(result, Is.SameAs(_sut));
        }

        #endregion

        #region WithValidation

        [Test]
        public void WithValidation_NoInnerTypeLink_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.WithValidation());
        }

        [Test]
        public void WithValidation_WithInnerTypeLink_ReturnsSameBuilder()
        {
            _sut.WithDecorator(Mock.Of<ITypeLink>());

            var result = _sut.WithValidation();

            Assert.That(result, Is.SameAs(_sut));
        }

        #endregion

        #region Build

        [Test]
        public void Build_NoInnerTypeLink_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.Build());
        }

        [Test]
        public void Build_AfterWithDecorator_ReturnsProvidedTypeLink()
        {
            var typeLink = Mock.Of<ITypeLink>();
            _sut.WithDecorator(typeLink);

            var result = _sut.Build();

            Assert.That(result, Is.SameAs(typeLink));
        }

        [Test]
        public void Build_AfterWithValidation_ReturnsValidatingTypeLink()
        {
            _sut.WithDecorator(Mock.Of<ITypeLink>());
            _sut.WithValidation();

            var result = _sut.Build();

            Assert.That(result, Is.InstanceOf<ValidatingTypeLink>());
        }

        [Test]
        public void Build_FullFluentChain_ReturnsValidatingTypeLink()
        {
            var result = _sut
                .WithStrategies()
                    .AddEnumerableTypeLinks()
                    .And()
                .WithValidation()
                .Build();

            Assert.That(result, Is.InstanceOf<ValidatingTypeLink>());
        }

        #endregion
    }
}
