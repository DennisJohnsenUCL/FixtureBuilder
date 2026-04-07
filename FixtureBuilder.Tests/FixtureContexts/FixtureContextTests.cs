using FixtureBuilder.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.FixtureContexts
{
    internal sealed class FixtureContextTests
    {
        private Mock<IContextResolver> _resolver;
        private FixtureOptions _options;
        private FixtureContext _sut;

        [SetUp]
        public void SetUp()
        {
            _resolver = new Mock<IContextResolver>(MockBehavior.Strict);
            _options = FixtureOptions.Default;
            _sut = new FixtureContext(_resolver.Object, _options);
        }

        [Test]
        public void Constructor_NullResolver_Throws()
        {
            Assert.That(() => new FixtureContext(null!, _options), Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_NullOptions_Throws()
        {
            Assert.That(() => new FixtureContext(_resolver.Object, null!), Throws.ArgumentNullException);
        }

        [Test]
        public void SetOptions_SetsOptions()
        {
            var options = FixtureOptions.Default;

            _sut.SetOptions(options);

            Assert.That(_sut.Options, Is.SameAs(options));
        }

        [Test]
        public void SetOptions_NullOptions_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.SetOptions((FixtureOptions)null!));
        }

        //TODO: This test needs to be updated as soon as options object has any members
        [Test]
        public void SetOptions_Action_SetsOptions()
        {
            _sut.SetOptions(o => o.ToString());

            Assert.That(_sut.Options, Is.SameAs(_sut.Options));
        }

        [Test]
        public void SetOptions_Action_NullAction_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.SetOptions((Action<FixtureOptions>)null!));
        }
    }
}
