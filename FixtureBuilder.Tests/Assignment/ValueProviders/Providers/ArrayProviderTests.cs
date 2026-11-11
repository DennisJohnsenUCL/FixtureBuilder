using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class ArrayProviderTests
    {
        [Test]
        public void ResolveValue_ValueTypeArray_ReturnsTenDefaultElements()
        {
            var provider = new ArrayProvider();
            var request = TestHelper.MakeRequest(typeof(int[]));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<int[]>());
                Assert.That(result, Is.EqualTo(new int[10]));
            }
        }

        [Test]
        public void ResolveValue_ReferenceTypeArray_ReturnsTenNullElements()
        {
            var provider = new ArrayProvider();
            var request = TestHelper.MakeRequest(typeof(string[]));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<string[]>());
                Assert.That(result, Is.EqualTo(new string?[10]));
            }
        }

        [Test]
        public void ResolveValue_JaggedArray_ReturnsTenNullInnerArrays()
        {
            var provider = new ArrayProvider();
            var request = TestHelper.MakeRequest(typeof(int[][]));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<int[][]>());
                Assert.That(result, Is.EqualTo(new int[10][]));
            }
        }

        [TestCase(typeof(int[,]))]
        [TestCase(typeof(int))]
        [TestCase(typeof(string))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new ArrayProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
