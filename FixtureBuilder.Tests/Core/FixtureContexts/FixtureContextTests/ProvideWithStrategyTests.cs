using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Core.FixtureContexts.ContextResolvers;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.FixtureContextTests
{
    internal sealed class ProvideWithStrategyTests
    {
        [Test]
        public void ProvideWithStrategy_ProviderHasValue_ReturnsProvidedValue()
        {
            var expected = "Provided value";
            var request = new FixtureRequest(typeof(string));
            var resolverMock = new Mock<IContextResolver>();
            resolverMock.Setup(r => r.ValueProvider.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(expected);
            resolverMock.Setup(r => r.TypeLink.Link(It.IsAny<Type>()))
                .Returns((Type?)null);
            var options = new FixtureOptions();
            var context = new FixtureContext(resolverMock.Object, options);

            var result = context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ProvideWithStrategy_ProviderHasValue_DoesNotCallInstantiate()
        {
            var request = new FixtureRequest(typeof(string));
            var resolverMock = new Mock<IContextResolver>();
            resolverMock.Setup(r => r.ValueProvider.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns("Provided value");
            resolverMock.Setup(r => r.TypeLink.Link(It.IsAny<Type>()))
                .Returns((Type?)null);
            var options = new FixtureOptions();
            var context = new FixtureContext(resolverMock.Object, options);

            context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            resolverMock.Verify(r => r.AutoConstructingProvider.AutoResolve(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void ProvideWithStrategy_ProviderReturnsNull_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(string));
            var resolverMock = new Mock<IContextResolver>();
            resolverMock.Setup(r => r.ValueProvider.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(null!);
            resolverMock.Setup(r => r.TypeLink.Link(It.IsAny<Type>()))
                .Returns((Type?)null);
            var options = new FixtureOptions();
            var context = new FixtureContext(resolverMock.Object, options);

            var result = context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ProvideWithStrategy_NoProvider_FallsBackToInstantiate()
        {
            var expected = "Instantiated value";
            var request = new FixtureRequest(typeof(string));
            var resolverMock = new Mock<IContextResolver>();
            resolverMock.Setup(r => r.ValueProvider.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(new NoResult());
            resolverMock.Setup(r => r.AutoConstructingProvider.AutoResolve(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(expected);
            resolverMock.Setup(r => r.TypeLink.Link(It.IsAny<Type>()))
                .Returns((Type?)null);
            var options = new FixtureOptions();
            var context = new FixtureContext(resolverMock.Object, options);

            var result = context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ProvideWithStrategy_LinksType_BeforeResolving()
        {
            var request = new FixtureRequest(typeof(IDisposable));
            var resolverMock = new Mock<IContextResolver>();
            FixtureRequest? capturedRequest = null;
            resolverMock.Setup(r => r.ValueProvider.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Callback<FixtureRequest, IFixtureContext>((req, _) => capturedRequest = req)
                .Returns(new NoResult());
            resolverMock.Setup(r => r.TypeLink.Link(typeof(IDisposable)))
                .Returns(typeof(MemoryStream));
            resolverMock.Setup(r => r.AutoConstructingProvider.AutoResolve(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(new MemoryStream());
            var options = new FixtureOptions();
            var context = new FixtureContext(resolverMock.Object, options);

            context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(capturedRequest!.Type, Is.EqualTo(typeof(MemoryStream)));
        }

        [Test]
        public void ProvideWithStrategy_UnwrapsNullableValueType_BeforeResolving()
        {
            var request = new FixtureRequest(typeof(int?));
            var resolverMock = new Mock<IContextResolver>();
            FixtureRequest? capturedRequest = null;
            resolverMock.Setup(r => r.ValueProvider.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Callback<FixtureRequest, IFixtureContext>((req, _) => capturedRequest = req)
                .Returns(42);
            resolverMock.Setup(r => r.TypeLink.Link(typeof(int)))
                .Returns((Type?)null);
            var options = new FixtureOptions();
            var context = new FixtureContext(resolverMock.Object, options);

            context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(capturedRequest!.Type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void ProvideWithStrategy_UnwrapsNullable_ThenLinks()
        {
            var request = new FixtureRequest(typeof(int?));
            var resolverMock = new Mock<IContextResolver>();
            FixtureRequest? capturedRequest = null;
            resolverMock.Setup(r => r.ValueProvider.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Callback<FixtureRequest, IFixtureContext>((req, _) => capturedRequest = req)
                .Returns(100L);
            resolverMock.Setup(r => r.TypeLink.Link(typeof(int)))
                .Returns(typeof(long));
            var options = new FixtureOptions();
            var context = new FixtureContext(resolverMock.Object, options);

            context.ProvideWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(capturedRequest!.Type, Is.EqualTo(typeof(long)));
        }
    }
}
