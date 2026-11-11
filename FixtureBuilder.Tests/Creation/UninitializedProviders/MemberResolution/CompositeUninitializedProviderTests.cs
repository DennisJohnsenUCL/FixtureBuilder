using FixtureBuilder.Creation.UninitializedProviders.MemberResolution;

namespace FixtureBuilder.Tests.Creation.UninitializedProviders.MemberResolution
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
