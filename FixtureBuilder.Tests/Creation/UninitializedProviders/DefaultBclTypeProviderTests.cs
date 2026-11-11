using System.Text;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Creation.UninitializedProviders
{
    internal sealed class DefaultBclTypeProviderTests
    {
        [Test]
        public void ResolveValue_SystemTypeWithParameterlessConstructor_ReturnsInstance()
        {
            var provider = new DefaultBclTypeProvider();
            var request = TestHelper.MakeRequest(typeof(StringBuilder));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<StringBuilder>());
        }

        [Test]
        public void ResolveValue_SystemTypeWithoutParameterlessConstructor_ReturnsNoResult()
        {
            var provider = new DefaultBclTypeProvider();
            var request = TestHelper.MakeRequest(typeof(Uri));

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
