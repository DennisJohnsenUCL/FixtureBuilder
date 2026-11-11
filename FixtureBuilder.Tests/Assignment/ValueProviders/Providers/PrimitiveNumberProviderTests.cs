using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class PrimitiveNumberProviderTests
    {
        [TestCase(typeof(byte))]
        [TestCase(typeof(sbyte))]
        [TestCase(typeof(short))]
        [TestCase(typeof(ushort))]
        [TestCase(typeof(int))]
        [TestCase(typeof(uint))]
        [TestCase(typeof(long))]
        [TestCase(typeof(ulong))]
        [TestCase(typeof(float))]
        [TestCase(typeof(double))]
        [TestCase(typeof(decimal))]
        public void ResolveValue_NumberType_ReturnsNumberFromOneToTen(Type numberType)
        {
            var provider = new PrimitiveNumberProvider();
            var request = TestHelper.MakeRequest(numberType);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf(numberType));
                Assert.That(result, Is.InRange(1, 10));
            }
        }

        [TestCase(typeof(int?))]
        [TestCase(typeof(string))]
        [TestCase(typeof(bool))]
        [TestCase(typeof(char))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new PrimitiveNumberProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
