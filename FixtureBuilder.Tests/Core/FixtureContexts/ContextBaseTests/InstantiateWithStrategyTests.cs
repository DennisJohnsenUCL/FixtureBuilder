using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.ContextBaseTests
{
    internal class InstantiateWithStrategyTests
    {
        private Mock<IAutoConstructingProvider> _autoConstructingMock;
        private Mock<IConstructingProvider> _constructingMock;
        private Mock<IUninitializedProvider> _uninitializedMock;

        private TestableContext CreateContext()
        {
            return new TestableContext(
                new FixtureOptions(),
                new ConverterGraph(new Mock<IValueConverter>().Object, new Mock<ICompositeConverter>().Object),
                new Mock<ICompositeTypeLink>().Object,
                _uninitializedMock.Object,
                new Mock<ICompositeValueProvider>().Object,
                _autoConstructingMock.Object,
                _constructingMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _autoConstructingMock = new Mock<IAutoConstructingProvider>();
            _constructingMock = new Mock<IConstructingProvider>();
            _uninitializedMock = new Mock<IUninitializedProvider>();
        }

        [Test]
        public void InstantiateWithStrategy_UseAutoConstructor_ResolvesWithAutoConstructor()
        {
            var expected = "Test string value";
            var request = new FixtureRequest(typeof(string));
            _autoConstructingMock.Setup(r => r.AutoResolve(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()))
                .Returns(expected);
            var context = CreateContext();

            var result = context.InstantiateWithStrategy(request, InstantiationMethod.UseAutoConstructor, InitializeMembers.None);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void InstantiateWithStrategy_UseDefaultConstructor_ResolvesWithDefaultConstructor()
        {
            var expected = "Test string value";
            var request = new FixtureRequest(typeof(string));
            _constructingMock
                .Setup(r => r.ResolveWithArguments(It.IsAny<FixtureRequest>(), It.IsAny<object?[]>()))
                .Returns(expected);
            var context = CreateContext();

            var result = context.InstantiateWithStrategy(request, InstantiationMethod.UseDefaultConstructor, InitializeMembers.None);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void InstantiateWithStrategy_CreateUninitialized_ResolvesWithUninitializedProvider()
        {
            var expected = "Test string value";
            var request = new FixtureRequest(typeof(string));
            var capturedInitMembers = InitializeMembers.None;
            _uninitializedMock.Setup(r => r.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), It.IsAny<IFixtureContext>()))
                .Callback<FixtureRequest, InitializeMembers, IFixtureContext, RecursiveResolveContext>((_, init, _, _) => capturedInitMembers = init)
                .Returns(expected);
            var context = CreateContext();

            var result = context.InstantiateWithStrategy(request, InstantiationMethod.CreateUninitialized, InitializeMembers.All);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.EqualTo(expected));
                Assert.That(capturedInitMembers, Is.EqualTo(InitializeMembers.All));
            }
        }
    }
}
