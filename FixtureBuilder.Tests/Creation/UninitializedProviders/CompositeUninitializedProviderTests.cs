using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Tests.Creation.UninitializedProviders
{
    internal sealed class CompositeUninitializedProviderTests
    {
        [Test]
        public void Constructor_NullDefaultBclTypeProvider_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CompositeUninitializedProvider(null!));
        }
    }
}
