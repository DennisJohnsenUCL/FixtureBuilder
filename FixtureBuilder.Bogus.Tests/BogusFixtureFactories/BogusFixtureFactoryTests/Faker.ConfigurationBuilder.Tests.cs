using Bogus;
using FixtureBuilder.Core;
using Moq;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusFixtureFactoryTests
{
    internal sealed class AddBogusProviderTests
    {
        public class ConstructorClass(string name)
        {
            public string Name { get; } = name;
        }

        [Test]
        public void AddBogusProvider_ProviderIsUsedByFixture()
        {
            var factory = FixtureFactory.WithBogus();

            var provider = new Mock<IBogusCustomProvider>();
            provider.Setup(p => p.ResolveValue(It.Is<FixtureRequest>(r => r.Type == typeof(string)), It.IsAny<Faker>()))
                .Returns("from-bogus-provider");

            factory.AddBogusProvider(provider.Object);

            var result = factory.Build<ConstructorClass>();

            Assert.That(result.Name, Is.EqualTo("from-bogus-provider"));
        }
    }
}
