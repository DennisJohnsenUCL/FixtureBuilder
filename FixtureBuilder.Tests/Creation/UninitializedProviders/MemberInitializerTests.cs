using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Creation.UninitializedProviders
{
    internal sealed class MemberInitializerTests
    {
        [Test]
        public void Constructor_NullSkipFilter_ThrowsArgumentNullException()
        {
            var uninitializedProvider = Mock.Of<ICompositeUninitializedProvider>();

            var ex = Assert.Throws<ArgumentNullException>(() => new MemberInitializer(null!, uninitializedProvider));

            Assert.That(ex.ParamName, Is.EqualTo("skipFilter"));
        }

        [Test]
        public void Constructor_NullUninitializedProvider_ThrowsArgumentNullException()
        {
            var skipFilter = Mock.Of<IDataMemberSkipFilter>();

            var ex = Assert.Throws<ArgumentNullException>(() => new MemberInitializer(skipFilter, null!));

            Assert.That(ex.ParamName, Is.EqualTo("uninitializedProvider"));
        }
    }
}
