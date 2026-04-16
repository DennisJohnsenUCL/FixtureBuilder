using FixtureBuilder.Core;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class FixtureFactoryAddProviderTests
    {
        public class TestClass { }

        [Test]
        public void AddProvider_FixtureUsesAddedProvider()
        {
            var factory = new FixtureFactory();
            var request = new FixtureRequest(typeof(string));
            var expected = "from-added";

            var provider = new Mock<ICustomProvider>();
            provider.Setup(p => p.ResolveValue(It.Is<FixtureRequest>(r => r.Type == request.Type))).Returns(expected);

            factory.AddProvider(provider.Object);

            var fixture = factory.New<TestClass>();
            var context = TestHelper.GetContext(fixture);
            var result = context.ResolveValue(request, context);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void AddProvider_NullProvider_Throws()
        {
            var factory = new FixtureFactory();

            Assert.Throws<ArgumentNullException>(() => factory.AddProvider(null!));
        }
    }
}
