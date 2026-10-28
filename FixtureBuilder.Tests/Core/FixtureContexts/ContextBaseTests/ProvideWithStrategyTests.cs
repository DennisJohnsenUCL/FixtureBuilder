using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.ContextBaseTests
{
    internal sealed class ProvideWithStrategyTests
    {
        private Mock<ICompositeValueProvider> _valueProviderMock;
        private Mock<IAutoConstructingProvider> _autoConstructingMock;

        private TestableContext CreateContext()
        {
            return new TestableContext(
                new FixtureOptions(),
                new ConverterGraph(new Mock<IValueConverter>().Object, new Mock<ICompositeConverter>().Object),
                new Mock<ICompositeTypeLink>().Object,
                new Mock<IUninitializedProvider>().Object,
                _valueProviderMock.Object,
                _autoConstructingMock.Object,
                new Mock<IConstructingProvider>().Object);
        }

        [SetUp]
        public void SetUp()
        {
            _valueProviderMock = new Mock<ICompositeValueProvider>();
            _autoConstructingMock = new Mock<IAutoConstructingProvider>();
        }

        [Test]
        public void ProvideWithStrategy_ProviderHasValue_ReturnsProvidedValue()
        {
            var expected = "Provided value";
            var request = new FixtureRequest(typeof(string));
            _valueProviderMock.Setup(r => r.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(expected);
            var context = CreateContext();

            var result = context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ProvideWithStrategy_ProviderHasValue_DoesNotCallInstantiate()
        {
            var request = new FixtureRequest(typeof(string));
            _valueProviderMock.Setup(r => r.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns("Provided value");
            var context = CreateContext();

            context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            _autoConstructingMock.Verify(r => r.AutoResolve(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void ProvideWithStrategy_ProviderReturnsNull_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(string));
            _valueProviderMock.Setup(r => r.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(null!);
            var context = CreateContext();

            var result = context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ProvideWithStrategy_NoProvider_FallsBackToInstantiate()
        {
            var expected = "Instantiated value";
            var request = new FixtureRequest(typeof(string));
            _valueProviderMock.Setup(r => r.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(new NoResult());
            _autoConstructingMock.Setup(r => r.AutoResolve(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(expected);
            var context = CreateContext();

            var result = context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
