using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class AddProviderTests
    {
        [Test]
        public void AddedProvider_SuppliesValues()
        {
            var factory = new FixtureFactory().AddProvider(new FixedStringProvider("provided"));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void LaterProvider_WinsOverEarlierProvider()
        {
            var factory = new FixtureFactory()
                .AddProvider(new FixedStringProvider("first"))
                .AddProvider(new FixedStringProvider("second"));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("second"));
        }

        private class NullForIntProvider : ICustomProvider
        {
            public object? ResolveValue(FixtureRequest request) => request.Type == typeof(int) ? null : NoResult.Instance;
        }

        [Test]
        public void ProviderReturnsNullForNonNullableValueType_ThrowsInvalidOperationException()
        {
            var factory = new FixtureFactory().AddProvider(new NullForIntProvider());
            var fixture = factory.New<StringAndIntConstructorClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.Build());
        }
    }
}
