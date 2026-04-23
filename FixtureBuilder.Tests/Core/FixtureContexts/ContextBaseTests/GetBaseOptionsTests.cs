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
    internal sealed class GetBaseOptionsTests
    {
        private static TestableContext CreateContext(FixtureOptions options)
        {
            return new TestableContext(
                options,
                new ConverterGraph(new Mock<IValueConverter>().Object, new Mock<ICompositeConverter>().Object),
                new Mock<ICompositeTypeLink>().Object,
                new Mock<IUninitializedProvider>().Object,
                new Mock<ICompositeValueProvider>().Object,
                new Mock<IAutoConstructingProvider>().Object,
                new Mock<IConstructingProvider>().Object);
        }

        [Test]
        public void GetBaseOptions_ReturnsOptionsPassedToContext()
        {
            var options = new FixtureOptions();
            var context = CreateContext(options);

            var result = context.GetBaseOptions();

            Assert.That(result, Is.SameAs(options));
        }

        [Test]
        public void GetBaseOptions_AfterSetOptions_ReturnsSameInstance()
        {
            var options = new FixtureOptions();
            var context = CreateContext(options);

            context.SetOptions(o => o.AllowPrivateConstructors = false);

            Assert.That(context.GetBaseOptions(), Is.SameAs(options));
        }
    }
}
