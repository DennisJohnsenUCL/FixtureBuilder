using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Core.FixtureContexts.ContextResolvers;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.FixtureContextTests
{
    internal sealed class SetOptionsTests
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
        public void SetOptions_SetsOptions()
        {
            var options = FixtureOptions.Default;

            _sut.Options = options;

            Assert.That(_sut.Options, Is.SameAs(options));
        }

        [Test]
        public void SetOptions_Action_SetsOptions()
        {
            _sut.SetOptions(o => o.AllowPrivateConstructors = false);

            Assert.That(_sut.Options.AllowPrivateConstructors, Is.False);
        }

        [Test]
        public void SetOptions_Action_NullAction_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.SetOptions(null!));
        }
    }
}
