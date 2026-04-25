using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories;
using FixtureBuilder.FixtureFactories.WithMatching;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching
{
    internal sealed class RootProviderBuilderTests
    {
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IFixtureContext>();
        }

        private RootProviderBuilder<string> CreateBuilder() => new(_contextMock.Object);

        [Test]
        public void With_ReturnsBuilderForChaining()
        {
            var builder = CreateBuilder();

            var result = builder.With(42);

            Assert.That(result, Is.SameAs(builder));
        }

        [Test]
        public void With_AddsProviderToList()
        {
            var builder = CreateBuilder();

            builder.With(42);

            Assert.That(builder.Providers, Has.Count.EqualTo(1));
        }

        [Test]
        public void With_MultipleCallsAddsMultipleProviders()
        {
            var builder = CreateBuilder();

            builder.With(42).With("hello");

            Assert.That(builder.Providers, Has.Count.EqualTo(2));
        }

        [Test]
        public void With_ProviderIncludesRootTypeRule_MatchingRootType_ReturnsValue()
        {
            var builder = CreateBuilder();
            builder.With(42);

            var request = new FixtureRequest(typeof(int), "source", typeof(string), null);
            var result = builder.Providers[0].ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void With_ProviderIncludesRootTypeRule_NonMatchingRootType_ReturnsNoResult()
        {
            var builder = CreateBuilder();
            builder.With(42);

            var request = new FixtureRequest(typeof(int), "source", typeof(double), null);
            var result = builder.Providers[0].ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        // Options setter

        [Test]
        public void OptionsSetter_CallsAddRootOptionsOnContext()
        {
            var builder = CreateBuilder();
            var options = new FixtureOptions();

            builder.Options = options;

            _contextMock.Verify(c => c.AddRootOptions(typeof(string), options), Times.Once);
        }

        // SetOptions

        [Test]
        public void SetOptions_ClonesBaseOptionsAndAppliesAction()
        {
            var baseOptions = new FixtureOptions { AllowPrivateConstructors = true };
            _contextMock.Setup(c => c.GetBaseOptions()).Returns(baseOptions);
            var builder = CreateBuilder();

            builder.SetOptions(o => o.AllowPrivateConstructors = false);

            _contextMock.Verify(c => c.AddRootOptions(typeof(string),
                It.Is<FixtureOptions>(o => o.AllowPrivateConstructors == false)), Times.Once);
        }

        [Test]
        public void SetOptions_DoesNotMutateBaseOptions()
        {
            var baseOptions = new FixtureOptions { AllowPrivateConstructors = true };
            _contextMock.Setup(c => c.GetBaseOptions()).Returns(baseOptions);
            var builder = CreateBuilder();

            builder.SetOptions(o => o.AllowPrivateConstructors = false);

            Assert.That(baseOptions.AllowPrivateConstructors, Is.True);
        }

        [Test]
        public void AddProvider_NullProvider_Throws()
        {
            var builder = CreateBuilder();

            Assert.Throws<ArgumentNullException>(() => builder.AddProvider(null!));
        }

        [Test]
        public void AddProvider_AddsAdaptedProviderToContext()
        {
            var valueProviderMock = new Mock<ICompositeValueProvider>();
            _contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            var builder = CreateBuilder();
            var providerMock = new Mock<ICustomProvider>();

            builder.AddProvider(providerMock.Object);

            valueProviderMock.Verify(v => v.AddProvider(It.IsAny<CustomProviderAdapter>()), Times.Once);
        }
    }
}
