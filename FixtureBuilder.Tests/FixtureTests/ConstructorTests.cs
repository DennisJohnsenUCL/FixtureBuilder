using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class ConstructorTests
    {
        [Test]
        public void NullContext_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Fixture<EmptyClass>((IFixtureContext)null!));
        }

        [Test]
        public void InstanceAndNullContext_ThrowsArgumentNullException()
        {
            var instance = new EmptyClass();

            Assert.Throws<ArgumentNullException>(() => new Fixture<EmptyClass>(instance, null!));
        }
    }
}
