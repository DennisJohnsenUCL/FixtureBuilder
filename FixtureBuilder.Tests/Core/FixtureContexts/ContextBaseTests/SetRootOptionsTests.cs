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
    internal sealed class SetRootOptionsTests
    {
        private static TestableContext CreateContext()
        {
            return new TestableContext(
                new FixtureOptions(),
                new ConverterGraph(new Mock<IValueConverter>().Object, new Mock<ICompositeConverter>().Object),
                new Mock<ICompositeTypeLink>().Object,
                new Mock<IUninitializedProvider>().Object,
                new Mock<ICompositeValueProvider>().Object,
                new Mock<IAutoConstructingProvider>().Object,
                new Mock<IConstructingProvider>().Object);
        }

        [Test]
        public void SetRootOptions_OptionsForReturnsAddedOptions()
        {
            var context = CreateContext();
            var rootOptions = new FixtureOptions { AllowPrivateConstructors = false };

            context.SetRootOptions(typeof(string), rootOptions);

            Assert.That(context.OptionsFor(typeof(string)), Is.SameAs(rootOptions));
        }

        [Test]
        public void SetRootOptions_DuplicateType_ReplacesOptions()
        {
            var context = CreateContext();
            var secondOptions = new FixtureOptions();
            context.SetRootOptions(typeof(string), new FixtureOptions());

            context.SetRootOptions(typeof(string), secondOptions);

            Assert.That(context.OptionsFor(typeof(string)), Is.SameAs(secondOptions));
        }

        [Test]
        public void SetRootOptions_MultipleTypes_EachResolvesCorrectly()
        {
            var context = CreateContext();
            var stringOptions = new FixtureOptions { AllowPrivateConstructors = false };
            var intOptions = new FixtureOptions { PreferSimplestConstructor = true };

            context.SetRootOptions(typeof(string), stringOptions);
            context.SetRootOptions(typeof(int), intOptions);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(context.OptionsFor(typeof(string)), Is.SameAs(stringOptions));
                Assert.That(context.OptionsFor(typeof(int)), Is.SameAs(intOptions));
            }
        }
    }
}
