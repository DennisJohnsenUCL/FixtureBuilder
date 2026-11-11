using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Tests.Creation.UninitializedProviders
{
    internal sealed class RootUninitializedProviderTests
    {
        [Test]
        public void Constructor_NullMemberInitializer_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new RootUninitializedProvider(null!));

            Assert.That(ex.ParamName, Is.EqualTo("memberInitializer"));
        }
    }
}
