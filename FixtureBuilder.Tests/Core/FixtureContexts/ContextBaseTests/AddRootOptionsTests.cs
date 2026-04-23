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
    internal sealed class AddRootOptionsTests
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
        public void AddRootOptions_OptionsForReturnsAddedOptions()
        {
            var context = CreateContext();
            var rootOptions = new FixtureOptions { AllowPrivateConstructors = false };

            context.AddRootOptions(typeof(string), rootOptions);

            Assert.That(context.OptionsFor(typeof(string)), Is.SameAs(rootOptions));
        }

        [Test]
        public void AddRootOptions_DuplicateType_Throws()
        {
            var context = CreateContext();
            context.AddRootOptions(typeof(string), new FixtureOptions());

            Assert.Throws<ArgumentException>(() => context.AddRootOptions(typeof(string), new FixtureOptions()));
        }

        [Test]
        public void AddRootOptions_MultipleTypes_EachResolvesCorrectly()
        {
            var context = CreateContext();
            var stringOptions = new FixtureOptions { AllowPrivateConstructors = false };
            var intOptions = new FixtureOptions { PreferSimplestConstructor = true };

            context.AddRootOptions(typeof(string), stringOptions);
            context.AddRootOptions(typeof(int), intOptions);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(context.OptionsFor(typeof(string)), Is.SameAs(stringOptions));
                Assert.That(context.OptionsFor(typeof(int)), Is.SameAs(intOptions));
            }
        }
    }
}
