using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.ContextBaseTests
{
    internal sealed class SetOptionsTests
    {
        private FixtureOptions _options;
        private TestableContext _sut;

        [SetUp]
        public void SetUp()
        {
            _options = FixtureOptions.Default;
            _sut = new TestableContext(
                _options,
                new ConverterGraph(new Mock<IValueConverter>().Object, new Mock<ICompositeConverter>().Object),
                new Mock<ICompositeTypeLink>().Object,
                new Mock<IUninitializedProvider>().Object,
                new Mock<ICompositeValueProvider>().Object,
                new Mock<IAutoConstructingProvider>().Object,
                new Mock<IConstructingProvider>().Object);
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
