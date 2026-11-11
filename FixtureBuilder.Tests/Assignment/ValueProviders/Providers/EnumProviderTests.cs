using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class EnumProviderTests
    {
        private enum Color
        {
            Red,
            Green,
            Blue
        }

        [Test]
        public void ResolveValue_Enum_ReturnsFirstDefinedValue()
        {
            var provider = new EnumProvider();
            var request = TestHelper.MakeRequest(typeof(Color));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(Color.Red));
        }

        private enum Priority
        {
            Low = 10,
            Medium = 20,
            High = 30
        }

        [Test]
        public void ResolveValue_EnumWithCustomValues_ReturnsFirstDefinedValue()
        {
            var provider = new EnumProvider();
            var request = TestHelper.MakeRequest(typeof(Priority));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(Priority.Low));
        }

        [Flags]
        private enum Permissions
        {
            None = 0,
            Read = 1,
            Write = 2
        }

        [Test]
        public void ResolveValue_FlagsEnum_ReturnsFirstDefinedValue()
        {
            var provider = new EnumProvider();
            var request = TestHelper.MakeRequest(typeof(Permissions));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(Permissions.None));
        }

        private enum EmptyEnum;

        [Test]
        public void ResolveValue_EnumWithoutMembers_ReturnsZeroValue()
        {
            var provider = new EnumProvider();
            var request = TestHelper.MakeRequest(typeof(EmptyEnum));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(default(EmptyEnum)));
        }

        [TestCase(typeof(string))]
        [TestCase(typeof(int))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new EnumProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
