using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Core.FixtureContexts.ContextResolvers;
using FixtureBuilder.Creation;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.FixtureContextTests
{
    internal class InstantiateWithStrategyTests
    {
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

        [Test]
        public void InstantiateWithStrategy_UseDefaultConstructor_ResolvesWithDefaultConstructor()
        {
            var expected = "Test string value";
            var request = new FixtureRequest(typeof(string));
            var resolverMock = new Mock<IContextResolver>();
            resolverMock
                .Setup(r => r.ConstructingProvider.ResolveWithArguments(It.IsAny<FixtureRequest>(), It.IsAny<object?[]>()))
                .Returns(expected);
            var options = new FixtureOptions();
            var context = new FixtureContext(resolverMock.Object, options);

            var result = context.InstantiateWithStrategy(request, InstantiationMethod.UseDefaultConstructor, InitializeMembers.None);

            Assert.That(result, Is.EqualTo(expected));
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
