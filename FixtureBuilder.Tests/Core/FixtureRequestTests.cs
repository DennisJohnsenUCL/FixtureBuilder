using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.Core
{
    internal sealed class FixtureRequestTests
    {
        [Test]
        public void Constructor_WithRootType_SetsRootType()
        {
            var request = new FixtureRequest(typeof(string), "source", typeof(double), "name");

            Assert.That(request.RootType, Is.EqualTo(typeof(double)));
        }

        [Test]
        public void Constructor_NoRootType_DefaultsToType()
        {
            var request = new FixtureRequest(typeof(string));

            Assert.That(request.RootType, Is.EqualTo(typeof(string)));
        }
    }
}
