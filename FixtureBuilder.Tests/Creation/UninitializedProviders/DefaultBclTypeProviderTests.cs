using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Creation.UninitializedProviders
{
    internal sealed class DefaultBclTypeProviderTests
    {
        [Test]
        public void ResolveValue_ValueTypeInSystemNamespace_ReturnsDefaultInstance()
        {
            var provider = new DefaultBclTypeProvider();
            var request = TestHelper.MakeRequest(typeof(Guid));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(Guid.Empty));
        }

        [Test]
        public void ResolveValue_ClassInSystemChildNamespace_ReturnsInstanceFromParameterlessConstructor()
        {
            var provider = new DefaultBclTypeProvider();
            var request = TestHelper.MakeRequest(typeof(List<int>));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<List<int>>().And.Empty);
        }

        [Test]
        public void ResolveValue_SystemTypeWithoutParameterlessConstructor_ReturnsNoResult()
        {
            var provider = new DefaultBclTypeProvider();
            var request = TestHelper.MakeRequest(typeof(string));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [TestCase(typeof(PropertyHolder<int>))]
        [TestCase(typeof(SystemUnderTest.Invoice))]
        [TestCase(typeof(GlobalNamespaceClass))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new DefaultBclTypeProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
