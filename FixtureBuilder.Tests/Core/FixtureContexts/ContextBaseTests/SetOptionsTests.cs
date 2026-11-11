using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.ContextBaseTests
{
    internal sealed class SetOptionsTests
    {
        [Test]
        public void NullAction_ThrowsArgumentNullException()
        {
            var context = new EagerContext(
                new FixtureOptions(),
                Mock.Of<ICompositeConverter>(),
                Mock.Of<ICompositeTypeLink>(),
                Mock.Of<IRootUninitializedProvider>(),
                Mock.Of<ICompositeValueProvider>(),
                Mock.Of<IAutoConstructingProvider>(),
                Mock.Of<IConstructingProvider>());

            Assert.Throws<ArgumentNullException>(() => context.SetOptions(null!));
        }
    }
}
