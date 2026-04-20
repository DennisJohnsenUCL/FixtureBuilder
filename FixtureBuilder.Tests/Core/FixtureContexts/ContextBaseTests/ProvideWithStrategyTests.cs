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
        private Mock<ICompositeTypeLink> _typeLinkMock;
        private Mock<ICompositeValueProvider> _valueProviderMock;
        private Mock<IAutoConstructingProvider> _autoConstructingMock;

        private TestableContext CreateContext()
        {
            return new TestableContext(
                new FixtureOptions(),
                new ConverterGraph(new Mock<IValueConverter>().Object, new Mock<ICompositeConverter>().Object),
                _typeLinkMock.Object,
                new Mock<IUninitializedProvider>().Object,
                _valueProviderMock.Object,
                _autoConstructingMock.Object,
                new Mock<IConstructingProvider>().Object);
        }

        [SetUp]
        public void SetUp()
        {
            _typeLinkMock = new Mock<ICompositeTypeLink>();
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
            _typeLinkMock.Setup(r => r.Link(It.IsAny<Type>()))
                .Returns((Type?)null);
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
            _typeLinkMock.Setup(r => r.Link(It.IsAny<Type>()))
                .Returns((Type?)null);
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
            _typeLinkMock.Setup(r => r.Link(It.IsAny<Type>()))
                .Returns((Type?)null);
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
            _typeLinkMock.Setup(r => r.Link(It.IsAny<Type>()))
                .Returns((Type?)null);
            var context = CreateContext();

            var result = context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ProvideWithStrategy_LinksType_BeforeResolving()
        {
            var request = new FixtureRequest(typeof(IDisposable));
            FixtureRequest? capturedRequest = null;
            _valueProviderMock.Setup(r => r.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Callback<FixtureRequest, IFixtureContext>((req, _) => capturedRequest = req)
                .Returns(new NoResult());
            _typeLinkMock.Setup(r => r.Link(typeof(IDisposable)))
                .Returns(typeof(MemoryStream));
            _autoConstructingMock.Setup(r => r.AutoResolve(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(new MemoryStream());
            var context = CreateContext();

            context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(capturedRequest!.Type, Is.EqualTo(typeof(MemoryStream)));
        }

        [Test]
        public void ProvideWithStrategy_UnwrapsNullableValueType_BeforeResolving()
        {
            var request = new FixtureRequest(typeof(int?));
            FixtureRequest? capturedRequest = null;
            _valueProviderMock.Setup(r => r.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Callback<FixtureRequest, IFixtureContext>((req, _) => capturedRequest = req)
                .Returns(42);
            _typeLinkMock.Setup(r => r.Link(typeof(int)))
                .Returns((Type?)null);
            var context = CreateContext();

            context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(capturedRequest!.Type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void ProvideWithStrategy_UnwrapsNullable_ThenLinks()
        {
            var request = new FixtureRequest(typeof(int?));
            FixtureRequest? capturedRequest = null;
            _valueProviderMock.Setup(r => r.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Callback<FixtureRequest, IFixtureContext>((req, _) => capturedRequest = req)
                .Returns(100L);
            _typeLinkMock.Setup(r => r.Link(typeof(int)))
                .Returns(typeof(long));
            var context = CreateContext();

            context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(capturedRequest!.Type, Is.EqualTo(typeof(long)));
        }
    }
}
