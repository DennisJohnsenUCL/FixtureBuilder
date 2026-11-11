using FixtureBuilder.Assignment.TypeLinks;

namespace FixtureBuilder.Tests.Assignment.TypeLinks
{
    internal sealed class CompositeTypeLinkTests
    {
        [Test]
        public void Constructor_NullTypeLinks_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CompositeTypeLink(null!));
        }
    }
}
