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
    internal sealed class OptionsForTests
    {
        private FixtureOptions _baseOptions;

        private TestableContext CreateContext()
        {
            return new TestableContext(
                _baseOptions,
                new ConverterGraph(new Mock<IValueConverter>().Object, new Mock<ICompositeConverter>().Object),
                new Mock<ICompositeTypeLink>().Object,
                new Mock<IUninitializedProvider>().Object,
                new Mock<ICompositeValueProvider>().Object,
                new Mock<IAutoConstructingProvider>().Object,
                new Mock<IConstructingProvider>().Object);
        }

        [SetUp]
        public void SetUp()
        {
            _baseOptions = new FixtureOptions();
        }

        [Test]
        public void OptionsFor_NoRootOptionsRegistered_ReturnsBaseOptions()
        {
            var context = CreateContext();

            var result = context.OptionsFor(typeof(string));

            Assert.That(result, Is.SameAs(_baseOptions));
        }

        [Test]
        public void OptionsFor_RootOptionsRegistered_ReturnsRootOptions()
        {
            var context = CreateContext();
            var rootOptions = new FixtureOptions();
            context.AddRootOptions(typeof(string), rootOptions);

            var result = context.OptionsFor(typeof(string));

            Assert.That(result, Is.SameAs(rootOptions));
        }

        [Test]
        public void OptionsFor_DifferentTypeRegistered_ReturnsBaseOptions()
        {
            var context = CreateContext();
            context.AddRootOptions(typeof(int), new FixtureOptions());

            var result = context.OptionsFor(typeof(string));

            Assert.That(result, Is.SameAs(_baseOptions));
        }
    }
}
